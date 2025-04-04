using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FileState { INTACT, WIN, BROKEN }
public enum FileStateCause { GONE, PRESENT, BADCONTENT }

public class FileStructureState
{
    public FileState State {  get; private set; } = FileState.INTACT;
    public FileStateCause Cause { get; private set; } = FileStateCause.PRESENT;
    public string TriggeringPath { get; private set; } = "";
    public bool TriggeredByFolder { get; private set; } = false;

    /**
     * Make a new file structure state from the given state, cause, and start of the triggering path
     */
    public FileStructureState(FileState state, FileStateCause cause, string triggeringFileName, bool isFolder)
    {
        State = state;
        Cause = cause;
        TriggeringPath = triggeringFileName;
        TriggeredByFolder = isFolder;
    }

    /**
     * Updates the triggering path to include the given name
     * Note that the name should be the parent of the previous file
     */
    public void UpdatePath(string name)
    {
        TriggeringPath = name + "\\" + TriggeringPath;
    }

    /**
     * Gets the string representation of the structure state
     */
    public override string ToString()
    {
        string retStr = "State: " + State + "\n";
        retStr += "Cause: " + Cause + "\n";
        retStr += "Triggering path: " + TriggeringPath + "\n";

        return retStr;
    }
}
