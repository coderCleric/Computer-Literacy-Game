using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [SerializeField]
    private GameObject dialogueObject;
    [SerializeField]
    private Text dialogueText;
    [SerializeField]
    private DialogueChain startDialogue = null;

    private DialogueChain curChain = null;
    public static DialogueManager Instance { get; private set; } = null;
    public delegate void DialogueChainCompleteEvent();
    public event DialogueChainCompleteEvent OnDialogueChainComplete;

    //Arrays of the different dialogue chains that might be useful
    [SerializeField]
    private Transform intactChainRoot;
    [SerializeField]
    private Transform successChainRoot;
    [SerializeField]
    private Transform failChainRoot;

    private DialogueChain[] intactChains;
    private DialogueChain[] successChains;
    private DialogueChain[] failChains;

    /**
     * On start, play the starting dialogue
     */
    private void Awake()
    {
        //Play the start dialogue, if there is one
        Instance = this;
        dialogueObject.SetActive(false);
        if (startDialogue != null) 
            PlayDialogue(startDialogue);

        //Collect the various different dialogue chains
        if(intactChainRoot != null)
            intactChains = intactChainRoot.GetComponentsInChildren<DialogueChain>();
        if (successChainRoot != null)
            successChains = successChainRoot.GetComponentsInChildren<DialogueChain>();
        if (failChainRoot != null)
            failChains = failChainRoot.GetComponentsInChildren<DialogueChain>();
    }

    public void PrintAllPuzzles(int dif)
    {
        //Print all of the puzzles
        PuzzleVector.GenAllPuzzleVectors();
        StreamWriter writer = new StreamWriter("allvecs.txt");
        foreach(PuzzleVector vec in PuzzleVector.allVecs)
        {
            writer.WriteLine(vec.GetFancyString() + "\n");
        }

        //Collect some stats
        int firstDel = -1;
        int lastDel = -1;
        int firstMove = -1;
        int lastMove = -1;
        int firstCreate = -1;
        int lastCreate = -1;
        int firstEdit = -1;
        int lastEdit = -1;
        for(int i = 0; i < PuzzleVector.allVecs.Count; i++)
        {
            PuzzleVector vec = PuzzleVector.allVecs[i];

            //Delete
            if(vec.GetPuzzleType() == PuzzleType.DELETE)
            {
                if (firstDel == -1)
                    firstDel = i;
                lastDel = i;
            }

            //Create
            if (vec.GetPuzzleType() == PuzzleType.CREATE)
            {
                if (firstCreate == -1)
                    firstCreate = i;
                lastCreate = i;
            }

            //Move
            if (vec.GetPuzzleType() == PuzzleType.MOVE)
            {
                if (firstMove == -1)
                    firstMove = i;
                lastMove = i;
            }
        }

        //Print those stats
        writer.WriteLine("First delete at pos " + firstDel + ", dif " + PuzzleVector.allVecs[firstDel].GetDifficulty());
        writer.WriteLine("Last delete at pos " + lastDel + ", dif " + PuzzleVector.allVecs[lastDel].GetDifficulty());
        writer.WriteLine("First create at pos " + firstCreate + ", dif " + PuzzleVector.allVecs[firstCreate].GetDifficulty());
        writer.WriteLine("Last create at pos " + lastCreate + ", dif " + PuzzleVector.allVecs[lastCreate].GetDifficulty());
        writer.WriteLine("First move at pos " + firstMove + ", dif " + PuzzleVector.allVecs[firstMove].GetDifficulty());
        writer.WriteLine("Last move at pos " + lastMove + ", dif " + PuzzleVector.allVecs[lastMove].GetDifficulty());
        writer.WriteLine("First edit at pos " + firstEdit + ", dif " + PuzzleVector.allVecs[firstEdit].GetDifficulty());
        writer.WriteLine("Last edit at pos " + lastEdit + ", dif " + PuzzleVector.allVecs[lastEdit].GetDifficulty());
        writer.WriteLine(PuzzleVector.allVecs.Count + " entries in total.");

        writer.Close();
    }

    public void PrintPuzzleChain(int len)
    {
        StreamWriter writer = new StreamWriter("puzzlechain.txt");

        Dictionary<string, string> delDict = new Dictionary<string, string>
        {
            { "#OBSTACLE", "pile of rocks" }
        };
        Dictionary<string, string> createDict = new Dictionary<string, string>
        {
            { "#OBSTACLE", "pit" },
            { "#SPANNER", "bridge" },
            { "#FILENAME", "bridge.txt" }
        };
        Dictionary<string, string> editDict = new Dictionary<string, string>
        {
            { "#OBSTACLE", "cliff" },
            { "#SPANNER", "vine" }
        };

        //Make the initial puzzle
        PuzzleVector curPuzzle = PuzzleVector.CreatePuzzleVector(1);
        writer.WriteLine(curPuzzle.GetFancyString());
        writer.WriteLine("Dialogue:");
        Dictionary<string, string> curDict = delDict;
        switch(curPuzzle.GetPuzzleType())
        {
            case PuzzleType.DELETE:
                curDict = delDict;
                break;
            case PuzzleType.CREATE:
            case PuzzleType.MOVE:
                curDict = createDict;
                break;
        }
        DialogueChain dialogue = PuzzleIntroBuilder.MakeIntroDialogue(curPuzzle, "root/folder1/folder2/folder3", curDict);
        foreach(string line in dialogue.lines)
        {
            writer.WriteLine(line);
        }

        for (int i = 0; i < len - 1; i++)
        {
            int mistakes = Random.Range(0, 4);
            curPuzzle = curPuzzle.GenNextPuzzle(mistakes, true);
            switch (curPuzzle.GetPuzzleType())
            {
                case PuzzleType.DELETE:
                    curDict = delDict;
                    break;
                case PuzzleType.CREATE:
                case PuzzleType.MOVE:
                    curDict = createDict;
                    break;
            }
            writer.WriteLine("\nGenerating puzzle from " + mistakes + " mistakes...");
            writer.WriteLine(curPuzzle.GetFancyString());
            writer.WriteLine("Dialogue:");
            dialogue = PuzzleIntroBuilder.MakeIntroDialogue(curPuzzle, "root/folder1/folder2/folder3", curDict);
            foreach (string line in dialogue.lines)
            {
                writer.WriteLine(line);
            }
        }

        writer.Close();
    }

    /**
     * Creates a dialogue chain from the given array of strings
     */
    public DialogueChain MakeDialogue(string[] lines)
    {
        //Error check
        if(lines == null)
            return null;

        //Make the dialogue chain
        GameObject go = new GameObject("NewDialogue");
        go.transform.parent = transform;
        DialogueChain chain = go.AddComponent<DialogueChain>();
        chain.lines = lines.ToArray();

        return chain;
    }

    /**
     * Begins playing the given dialogue chain
     */
    public void PlayDialogue(DialogueChain dialogue)
    {
        //Don't want to play if one is already active
        if(curChain !=  null)
        {
            Debug.LogError("Dialogue manager was told to interrupt active dialogue!");
            return;
        }

        if (dialogue == null)
            return;

        //Do setup
        InputStateTracker.SetGameState(InputStateTracker.GameState.DIALOGUE);
        dialogueObject.SetActive(true);
        curChain = dialogue;
        ShowNextLine();
    }

    /**
     * Shows the next line of dialogue
     */
    private void ShowNextLine()
    {
        //Do nothing if no dialogue is active
        if (curChain == null)
            return;

        //First, retrieve the next line
        string line = curChain.GetNextLine();

        //If it's null, reset
        if(line == null)
        {
            InputStateTracker.SetGameState(InputStateTracker.GameState.PLAY);
            dialogueObject.SetActive(false);
            curChain = null;
            OnDialogueChainComplete?.Invoke();
        }

        //If it's not, update the text
        else
        {
            dialogueText.text = line;
        }
    }

    /**
     * Skip the current dialogue sequence
     */
    public void Skip()
    {
        if (curChain == null)
            return;

        InputStateTracker.SetGameState(InputStateTracker.GameState.PLAY);
        dialogueObject.SetActive(false);
        curChain = null;
        OnDialogueChainComplete?.Invoke();
    }

    /**
     * Play a dialogue chain when the player does nothing
     */
    public void PlayIntactChain()
    {
        int index = Random.Range(0, intactChains.Length);
        PlayDialogue(intactChains[index]);
    }

    /**
     * Play a dialogue chain to congratulate the player
     */
    public void PlaySuccessChain()
    {
        int index = Random.Range(0, successChains.Length);
        PlayDialogue(successChains[index]);
    }

    /**
     * Play a dialogue chain to scold the player
     */
    public void PlayFailChain()
    {
        int index = Random.Range(0, failChains.Length);
        PlayDialogue(failChains[index]);
    }

    /**
     * If the player presses interact, need to advance dialogue
     */
    private void Update()
    {
        if (InputStateTracker.CheckGameState(InputStateTracker.GameState.DIALOGUE) && curChain != null && Input.GetButtonDown("Interact"))
        {
            ShowNextLine();
        }
    }
}
