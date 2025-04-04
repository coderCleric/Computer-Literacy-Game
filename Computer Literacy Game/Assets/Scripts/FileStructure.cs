using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileStructure : MonoBehaviour
{
    public string pathToFirstFolder;
    public FakeFolder firstFolder;

    /**
     * On start, do some setup
     */
    private void Start()
    {
        pathToFirstFolder = FolderOpener.GetToyFolderPath();

        //Make sure that the starting folder exists
        if(!Directory.Exists(FolderOpener.GetStartFolderPath()))
        {
            Directory.CreateDirectory(FolderOpener.GetStartFolderPath());
        }
    }

    /**
     * Verifies the integrity of the file system
     */
    public FileStructureState CheckIntegrity()
    {
        //Literally just store the result of the call on the root folder
        FileStructureState state = firstFolder.CheckState(pathToFirstFolder);

        return state;
    }

    /**
     * Clears everything from the starting folder
     */
    public void ClearStructure()
    {
        //Try and delete anything inside of pathtofirstfolder
        try
        {
            //Try and delete the files first
            foreach (string path in Directory.GetFiles(FolderOpener.GetStartFolderPath()))
                File.Delete(path);

            //Then, go for the folders, recursively deleting them
            foreach (string path in Directory.GetDirectories(FolderOpener.GetStartFolderPath()))
                Directory.Delete(path, true);
        }
        catch (IOException) //Make this better when it actually has stuff to interact with (might not need to, no errors yet)
        {
            Debug.LogError("Error: File structure was unable to clear folder! Is it viewed in an explorer?");
            return;
        }

        //Make the safety train of folders
        Directory.CreateDirectory(FolderOpener.GetStartFolderPath() + FolderOpener.AppendPath);
    }

    /**
     * Uses the in-game structure to make the required file structure
     */
    public void MakeStructure()
    {
        //Clear the folder
        ClearStructure();

        //Working in a clear folder now, let recursive calls to our structures handle it
        firstFolder.CreateInSystem(pathToFirstFolder);
    }

    /**
     * Makes the file structure for the tutorial
     */
    public void MakeTutorialStructure()
    {
        //Clear the folder
        ClearStructure();

        //Create the tutorial structure
        Directory.CreateDirectory(pathToFirstFolder + "\\pass\\word");
        Directory.CreateDirectory(pathToFirstFolder + "\\wrong\\way");
        File.Create(pathToFirstFolder + "\\pass\\word\\wendigo");
        File.Create(pathToFirstFolder + "\\wrong\\way\\nope");
    }
}
