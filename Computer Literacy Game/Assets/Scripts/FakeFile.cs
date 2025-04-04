using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;

public enum ActionType { NONE, MAYDELETE, DELETE, CREATE }

public class FakeFile : FakeThing
{
    [SerializeField]
    private ActionType expectedAction = ActionType.NONE;
    public ActionType activateableAction = ActionType.DELETE;

    //Used for extra checks on move puzzles
    public bool shouldCheckContent = false;
    [SerializeField]
    private string expectedContent = "";
    [SerializeField]
    private string initialContent = "";

    //Used for puzzle dialogue
    public Dictionary<string, string> insertDict;
    [SerializeField]
    private string[] insertKeys;
    [SerializeField]
    private string[] insertValues;

    /**
     * On start, make the dictionary
     */
    public override void Start()
    {
        base.Start();

        //Manage the dictionary
        insertDict = new Dictionary<string, string>();

        //Error check
        if(insertKeys.Length != insertValues.Length)
        {
            Debug.LogError("Fake file " + fileName + " has an invalid dictionary!");
            return;
        }

        //Otherwise, add to the dict
        for(int i = 0; i < insertKeys.Length; i++)
        {
            insertDict.Add(insertKeys[i], insertValues[i]);
        }
    }

    /**
     * File may have an action that must occur
     * 
     * @return The expected action type
     */
    public ActionType GetExpectedAction()
    {
        return expectedAction;
    }

    /**
     * Set whether or not this file is active in the current puzzle
     */
    public void SetActiveInPuzzle(bool active)
    {
        if (active)
            expectedAction = activateableAction;
        else
        {
            expectedAction = ActionType.NONE;
            shouldCheckContent = false;
        }
    }

    /**
     * Checks the file state
     * 
     * @param path The path to check at
     * @return The state of the file
     */
    public override FileStructureState CheckState(string path)
    {
        //Check if the file exists
        string fullPath = path + "\\" + fileName;
        string[] matchingFiles = Directory.GetFiles(path, fileName + ".*");
        bool exists = File.Exists(fullPath) || matchingFiles.Length > 0;

        //Also need to figure out the proper name
        if(exists && matchingFiles.Length > 0) { 
            fullPath = matchingFiles[0];
        }

        //If it exists, different behaviors based on expected action
        if (exists)
        {
            //Check that the content is good
            if(shouldCheckContent)
            {
                string contents = File.ReadAllText(fullPath);
                if (!expectedContent.Equals(contents))
                    return new FileStructureState(FileState.BROKEN, FileStateCause.BADCONTENT, fileName, false);
            }

            //Check based on the expected action
            switch(expectedAction)
            {
                case ActionType.NONE: //No action expected, should be there
                    return new FileStructureState(FileState.INTACT, FileStateCause.PRESENT, fileName, false);

                case ActionType.DELETE: //If it may/should be removed, nothing happens if it stays
                case ActionType.MAYDELETE:
                    return new FileStructureState(FileState.INTACT, FileStateCause.PRESENT, fileName, false);

                case ActionType.CREATE: //If it's supposed to be created, finding it indicates a win
                    return new FileStructureState(FileState.WIN, FileStateCause.PRESENT, fileName, false);
            }
        }

        //If it's not found, determine behavior
        else
        {
            switch (expectedAction)
            {
                case ActionType.NONE: //If an expected file is removed, that's an error
                    return new FileStructureState(FileState.BROKEN, FileStateCause.GONE, fileName, false);

                case ActionType.MAYDELETE: //This file doesn't have to exist at all times, so we're stable
                case ActionType.CREATE:
                    return new FileStructureState(FileState.INTACT, FileStateCause.GONE, fileName, false);

                case ActionType.DELETE: //This file is meant to be removed, that's a win
                    return new FileStructureState(FileState.WIN, FileStateCause.GONE, fileName, false);
            }
        }

        Debug.LogError("FakeFile.CheckState reached end of function, should have stopped in a switch!");
        return new FileStructureState(FileState.INTACT, FileStateCause.PRESENT, fileName, false);
    }

    /**
     * Creates this file in the actual file system
     */
    public override void CreateInSystem(string path)
    {
        //Only make it if it should initially exist
        if (expectedAction != ActionType.CREATE)
        {
            File.Create(path + "\\" + fileName).Close();
            if (!initialContent.Equals(""))
            {
                StreamWriter writer = new StreamWriter(path + "\\" + fileName);
                writer.Write(initialContent);
                writer.Close();
            }
        }
    }
}
