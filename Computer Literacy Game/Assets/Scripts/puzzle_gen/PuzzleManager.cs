using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleManager : MonoBehaviour
{
    private enum WaitingState { NONE, TOSTART, TOFAIL }
    public static PuzzleManager Instance { get; private set; } = null;
    private float playerScore = 0;

    //Debug thing
    [SerializeField]
    private FakeFile fileOverride = null;

    //Things it needs access to
    public FileStructure fileStruct;
    [SerializeField]
    private Transform gameRoot;
    [SerializeField]
    private PlayerFollower mainCamera;
    [SerializeField]
    private Text scoreText;
    [SerializeField]
    private Text levelText;
    [SerializeField]
    private Button[] buttons;
    [SerializeField]
    private AudioSource oopsAudio;
    [SerializeField]
    private AudioSource failAudio;
    [SerializeField]
    private AudioSource winAudio;

    //State information
    private WaitingState curState = WaitingState.NONE;
    private bool hooked = false;
    private PuzzleVector currentPuzzleVector = null;
    public PuzzleLevel currentLevel = null;
    private FakeFile puzzleFile = null;
    [NonSerialized]
    public DialogueChain currentIntroDialogue = null;
    private int mistakes = 0;
    private int maxLevels = 10;
    private int levelCount = 0;
    private int fails = 0;
    private float startTime = 0;
    private float difficulty = 1;

    //Arrays for the different files that can be used as puzzle solutions
    public FakeFile[] deleteFiles;
    public FakeFile[] createFiles;
    private int deleteIndex = 0;
    private int createIndex = 0;

    //Array of the actual puzzle levels that can be used
    [SerializeField]
    private PuzzleLevel[] levels;

    /**
     * On awake, set as the instance
     */
    private void Start()
    {
        Instance = this;
        RequeueDelete();
        RequeueCreate();
        mainCamera.SetFollow(true);
        SetPlayerScore(0);

        //Check for the save
        if (DBManager.saveFound)
        {
            ReadSave();
            DialogueManager.Instance.Skip();
            GenPuzzle(difficulty);
            StartPuzzle();
        }
        else
        {
            GenPuzzle(difficulty);

            //Wait to start the puzzle
            curState = WaitingState.TOSTART;
        }
    }

    /**
     * Change the player score
     */
    public void UpdatePlayerScore(float scoreAddition)
    {
        SetPlayerScore(playerScore + scoreAddition);
    }

    /**
     * Sets the player's score
     */
    public void SetPlayerScore(float score)
    {
        playerScore = score;
        scoreText.text = playerScore.ToString("N0");
    }

    /**
     * When dialogue finishes, do whatever we need to
     */
    private void OnDialogueFinish()
    {
        if (curState == WaitingState.TOSTART)
        {
            StartPuzzle();
            curState = WaitingState.NONE;
        }
        else if(curState == WaitingState.TOFAIL)
        {
            GenNextPuzzle();
            StartPuzzle();
            curState= WaitingState.NONE;
            mainCamera.SetFollow(false);
        }
    }

    /**
     * Just replays the current dialogue
     */
    public void ReplayDialogue()
    {
        if(currentIntroDialogue != null)
            DialogueManager.Instance.PlayDialogue(currentIntroDialogue);
    }

    /**
     * When the level is beaten, do things
     */
    public void OnLevelFinish()
    {
        //If the player just won, make the camera follow them and say stuff
        if (levelCount >= maxLevels || currentPuzzleVector.GetDifficulty() >= PuzzleVector.GetMaxDifficulty())
        {
            TriggerEndSequence();
        }

        //Otherwise, generate the next level and start it
        else
        {
            GenNextPuzzle();
            StartPuzzle();
        }
    }

    /**
     * Triggers the end sequence of the game
     */
    public void TriggerEndSequence()
    {
        if (currentLevel != null)
            Destroy(currentLevel.gameObject);
        mainCamera.SetFollow(true);
        DialogueManager.Instance.PlayDialogue(EndGameDialogueBuilder.MakeEndDialogue(playerScore, levelCount, fails));
        StartCoroutine(DBManager.SendCompletion());
    }

    /**
     * Checks whether or not the player has made a correct file system
     */
    public void CheckAnswer()
    {
        //Error check
        if (currentPuzzleVector == null)
            return;

        //Query the file system for its current state
        FileStructureState state = fileStruct.CheckIntegrity();
        string causeString;
        switch(state.Cause)
        {
            case FileStateCause.PRESENT:
                causeString = "was intact";
                break;
            case FileStateCause.GONE:
                causeString = "was deleted";
                break;
            case FileStateCause.BADCONTENT:
                causeString = "had the wrong contents";
                break;
            default:
                causeString = "###ERROR###";
                break;
        }
        string typeString;
        if (state.TriggeredByFolder)
            typeString = "folder";
        else
            typeString = "file";

        Debug.Log("Checking file structure state:\n" + state.ToString());

        //If its a bad state, inc mistakes and check number
        if(state.State == FileState.BROKEN)
        {
            mistakes++;

            //<3 mistakes, give an error, reset the file system, and let the user try again
            if(mistakes < 3)
            {
                oopsAudio.Play();
                DialogueChain chain = DialogueManager.Instance.MakeDialogue(new string[]
                {
                    "Yikes! Okay, looks like you triggered the system by doing something it didn't want. Let me check something here...",
                    "Looks like it got mad because the " + typeString + " at " + state.TriggeringPath + " " + causeString + ", which it wasn't expecting.",
                    "You've made " + mistakes + " errors so far, so lets keep going. Please try again."
                });
                DialogueManager.Instance.PlayDialogue(chain);
                fileStruct.MakeStructure();
            }

            //Otherwise, scold the player and GenNextPuzzle
            else
            {
                //Send info to the DB
                UpdatePlayerScore(-2);
                StartCoroutine(DBManager.SendEntry(levelCount, playerScore, currentPuzzleVector.GetDifficulty(),
                    (int)currentPuzzleVector.GetPuzzleType(), currentPuzzleVector.GetDepth(), currentPuzzleVector.GetUnknownLen(), 
                    mistakes, Time.time - startTime));

                //Handle failure on a gameplay level
                failAudio.Play();
                DialogueManager.Instance.PlayFailChain();
                currentLevel.gameObject.SetActive(false);
                mainCamera.SetFollow(true);
                curState = WaitingState.TOFAIL;
                fails++;

                //Disable the buttons
                foreach (Button button in buttons)
                    button.interactable = false;
            }
        }


        //If it's an intact state, tell the player that nothing happened
        else if(state.State == FileState.INTACT)
        {
            DialogueManager.Instance.PlayIntactChain();
        }

        //If it's a win state, congratulate the player and generate another puzzle
        else
        {
            //Update DB
            UpdatePlayerScore(Mathf.Pow(2, 2 - mistakes) * Mathf.Sqrt(currentPuzzleVector.GetDifficulty()));
            StartCoroutine(DBManager.SendEntry(levelCount, playerScore, currentPuzzleVector.GetDifficulty(),
                (int)currentPuzzleVector.GetPuzzleType(), currentPuzzleVector.GetDepth(), currentPuzzleVector.GetUnknownLen(), 
                mistakes, Time.time - startTime));

            //Gameplay things
            winAudio.Play();
            currentLevel.TriggerWin();
            DialogueManager.Instance.PlaySuccessChain();

            //Disable the buttons
            foreach (Button button in buttons)
                button.interactable = false;
        }
    }

    /**
     * Generates the next puzzle based on the saved number of mistakes
     */
    public void GenNextPuzzle()
    {
        //Check if the game is ending
        if(levelCount >= maxLevels)
        {
            TriggerEndSequence();
            return;
        }

        levelCount++;

        //Query PuzzleVector for the next puzzle vector
        currentPuzzleVector = currentPuzzleVector.GenNextPuzzle(mistakes);

        //Disable all of the different files
        foreach (FakeFile f in deleteFiles)
            f.SetActiveInPuzzle(false);
        foreach (FakeFile f in createFiles)
            f.SetActiveInPuzzle(false);

        //Find a file for the puzzle and enable it
        puzzleFile = null;
        switch (currentPuzzleVector.GetPuzzleType())
        {
            case PuzzleType.DELETE:
                Debug.Log("Picking delete file");
                puzzleFile = SelectDeleteFile();
                puzzleFile.SetActiveInPuzzle(true);
                break;
            case PuzzleType.CREATE:
            case PuzzleType.MOVE:
                Debug.Log("Picking create/move file");
                puzzleFile = SelectCreateFile();
                puzzleFile.SetActiveInPuzzle(true);
                if(currentPuzzleVector.GetPuzzleType() == PuzzleType.MOVE)
                    puzzleFile.shouldCheckContent = true;
                break;
        }

        //Make the intro dialogue
        currentIntroDialogue = PuzzleIntroBuilder.MakeIntroDialogue(currentPuzzleVector, puzzleFile.GetFullPath(),
            puzzleFile.insertDict);

        //Destroy the previous level
        Destroy(currentLevel.gameObject);

        //Get a list of valid levels
        List<PuzzleLevel> options = new List<PuzzleLevel>();
        foreach(PuzzleLevel level in levels)
        {
            if (puzzleFile.fileName.Contains(level.acceptableFileName))
                options.Add(level);
        }

        //Pick a random one
        currentLevel = options[UnityEngine.Random.Range(0, options.Count)];
    }

    /**
     * Generates a puzzle of the given difficulty
     */
    public void GenPuzzle(float dif)
    {
        //Check if the game is ending
        if (levelCount >= maxLevels)
        {
            TriggerEndSequence();
            return;
        }

        levelCount++;
        InputStateTracker.SetState(InputStateTracker.InputState.GAME);

        //Create the puzzle vector
        currentPuzzleVector = PuzzleVector.CreatePuzzleVector(dif);

        //Disable all of the files
        foreach (FakeFile f in deleteFiles)
            f.SetActiveInPuzzle(false);
        foreach (FakeFile f in createFiles)
            f.SetActiveInPuzzle(false);

        //Find a file for the puzzle and enable it
        puzzleFile = null;
        if (fileOverride == null)
        {
            switch (currentPuzzleVector.GetPuzzleType())
            {
                case PuzzleType.DELETE:
                    Debug.Log("Picking delete file");
                    puzzleFile = SelectDeleteFile();
                    puzzleFile.SetActiveInPuzzle(true);
                    break;
                case PuzzleType.CREATE:
                case PuzzleType.MOVE:
                    Debug.Log("Picking create/move file");
                    puzzleFile = SelectCreateFile();
                    puzzleFile.SetActiveInPuzzle(true);
                    if (currentPuzzleVector.GetPuzzleType() == PuzzleType.MOVE)
                        puzzleFile.shouldCheckContent = true;
                    break;
            }
        }
        else
        {
            puzzleFile = fileOverride;
            fileOverride.SetActiveInPuzzle(true);
        }

        //Make the intro dialogue
        currentIntroDialogue = PuzzleIntroBuilder.MakeIntroDialogue(currentPuzzleVector, puzzleFile.GetFullPath(), 
            puzzleFile.insertDict);

        //Hook into the dialogue manager, if we haven't already
        if(!hooked)
        {
            hooked = true;
            DialogueManager.Instance.OnDialogueChainComplete += OnDialogueFinish;
        }

        //Get a list of valid levels
        List<PuzzleLevel> options = new List<PuzzleLevel>();
        foreach (PuzzleLevel level in levels)
        {
            if (puzzleFile.fileName.Contains(level.acceptableFileName))
                options.Add(level);
        }

        //Pick a random one
        currentLevel = options[UnityEngine.Random.Range(0, options.Count)];
    }

    /**
     * Starts the current puzzle
     */
    public void StartPuzzle()
    {
        //Error check
        if (currentPuzzleVector == null || currentLevel == null)
            return;

        Debug.Log("Starting puzzle:\n" + currentPuzzleVector.GetFancyString());

        //Regen the file structure
        fileStruct.MakeStructure();

        //Find the starting location
        string[] pathParts = puzzleFile.GetFullPath().Split('\\');
        string startPath = pathParts[0];
        for (int i = 1; i <= PuzzleVector.maxDepth - currentPuzzleVector.GetDepth(); i++)
        {
            startPath += "\\" + pathParts[i];
        }
        FolderOpener.Instance.challengeStart = startPath;

        //Play the dialogue for the puzzle
        DialogueManager.Instance.PlayDialogue(currentIntroDialogue);

        //Load the actual level
        currentLevel = Instantiate(currentLevel, gameRoot);
        currentLevel.StartLevel();

        //Reset number of mistakes
        mistakes = 0;

        //Unlock the camera
        mainCamera.SetFollow(false);

        //Enable the buttons
        foreach (Button button in buttons)
            button.interactable = true;

        //Set the level text
        difficulty = currentPuzzleVector.GetDifficulty();
        levelText.text = difficulty.ToString("N0") + "/" + PuzzleVector.GetMaxDifficulty().ToString("N0");

        //Mark the start time
        startTime = Time.time;

        //Make the save
        WriteSave();
    }

    /**
     * Rebuild the delete queue
     */
    private void RequeueDelete()
    {
        deleteIndex = 0;

        //Shuffle the files
        for(int i = 0; i < deleteFiles.Length; i++)
        {
            int rand = UnityEngine.Random.Range(i, deleteFiles.Length);
            FakeFile tmp = deleteFiles[i];
            deleteFiles[i] = deleteFiles[rand];
            deleteFiles[rand] = tmp;
        }
    }

    /**
     * Rebuild the create queue
     */
    private void RequeueCreate()
    {
        createIndex = 0;

        //Shuffle the files
        for (int i = 0; i < createFiles.Length; i++)
        {
            int rand = UnityEngine.Random.Range(i, createFiles.Length);
            FakeFile tmp = createFiles[i];
            createFiles[i] = createFiles[rand];
            createFiles[rand] = tmp;
        }
    }

    /**
     * Selects the next deletion puzzle
     */
    private FakeFile SelectDeleteFile()
    {
        //If needed, remake the delete queue
        if(deleteIndex >= deleteFiles.Length)
            RequeueDelete();

        FakeFile selection = deleteFiles[deleteIndex];
        deleteIndex++;
        return selection;
    }

    /**
     * Selects the next creation puzzle
     */
    private FakeFile SelectCreateFile()
    {
        //If needed, remake the delete queue
        if (createIndex >= createFiles.Length)
            RequeueCreate();

        FakeFile selection = createFiles[createIndex];
        createIndex++;
        return selection;
    }

    /**
     * Read from the save file
     */
    private void ReadSave()
    {
        StreamReader reader = new StreamReader(DBManager.savePath);
        levelCount = int.Parse(reader.ReadLine()) - 1;
        SetPlayerScore(float.Parse(reader.ReadLine()));
        difficulty = float.Parse(reader.ReadLine());
        reader.Close();
    }

    /**
     * Write to the save file
     */
    private void WriteSave()
    {
        if (DBManager.savePath == null)
            return;

        StreamWriter writer = new StreamWriter(DBManager.savePath);
        writer.WriteLine(levelCount.ToString());
        writer.WriteLine(playerScore.ToString());
        writer.WriteLine(difficulty.ToString());
        writer.Close();
    }

    /**
     * If needed, unhook on destroy
     */
    private void OnDestroy()
    {
        if(hooked)
            DialogueManager.Instance.OnDialogueChainComplete -= OnDialogueFinish;
    }
}
