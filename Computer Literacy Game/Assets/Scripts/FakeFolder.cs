using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class FakeFolder : FakeThing
{
    public bool preBuilt = false;
    public FakeFile[] childFiles;
    public FakeFolder[] childFolders;

    /**
     * On start, collect any children (assuming we're not prebuilt)
     */
    public override void Start()
    {
        //Grab name like before
        base.Start();

        //Exit early if prebuilt
        if (preBuilt)
            return;

        //Collect the child files
        //Count the number
        int fileCount = 0;
        foreach(Transform i in transform)
        {
            if(i.GetComponent<FakeFile>() != null)
                fileCount++;
        }
        //Build the array
        childFiles = new FakeFile[fileCount];
        int insertIndex = 0;
        foreach (Transform i in transform)
        {
            if (i.GetComponent<FakeFile>() != null)
            {
                childFiles[insertIndex] = i.GetComponent<FakeFile>();
                insertIndex++;
            }
        }

        //Collect the child folders
        //Count the number
        int folderCount = 0;
        foreach (Transform i in transform)
        {
            if (i.GetComponent<FakeFolder>() != null)
                folderCount++;
        }
        //Build the array
        childFolders = new FakeFolder[folderCount];
        insertIndex = 0;
        foreach (Transform i in transform)
        {
            if (i.GetComponent<FakeFolder>() != null)
            {
                childFolders[insertIndex] = i.GetComponent<FakeFolder>();
                insertIndex++;
            }
        }
    }

    /**
     * Checks the state of this folder, and then the ones of all children
     * 
     * @param path The path the folder should be found at
     * @return The state of the folder
     */
    public override FileStructureState CheckState(string path)
    {
        //First, check that this folder exists
        string fullPath = path + "\\" + fileName;
        bool exists = Directory.Exists(fullPath);

        //If it doesn't, return that it's broken
        if (!exists)
            return new FileStructureState(FileState.BROKEN, FileStateCause.GONE, fileName, true);

        //Collect lists of files and folders in this one
        string[] fileNames = Directory.GetFiles(fullPath).Select(Path.GetFileName).ToArray();
        string[] folderNames = Directory.GetDirectories(fullPath).Select(Path.GetFileName).ToArray();

        //Check to make sure that we don't have any unexpected files or folders
        foreach(string name in fileNames)
        {
            //May need to strip the extension
            int dotIndex = name.IndexOf('.');
            string withoutExtension = name;
            if (dotIndex > 0)
                withoutExtension = name.Substring(0, dotIndex);

            bool wasExpected = false;
            foreach (FakeFile child in childFiles)
            {
                if (child.fileName.Equals(withoutExtension))
                {
                    wasExpected = true;
                    break;
                }
            }
            if (!wasExpected)
                return new FileStructureState(FileState.BROKEN, FileStateCause.PRESENT, fileName + "\\" + name, false);
        }
        foreach (string name in folderNames)
        {
            bool wasExpected = false;
            foreach (FakeFolder child in childFolders)
            {
                if (child.fileName.Equals(name))
                {
                    wasExpected = true;
                    break;
                }
            }
            if (!wasExpected)
                return new FileStructureState(FileState.BROKEN, FileStateCause.PRESENT, fileName + "\\" + name, true);
        }

        //Ask child files and folders to figure out what to return
        FileStructureState returnState = null;

        //First, ask the files
        foreach (FakeFile child in childFiles)
        {
            FileStructureState thisState = child.CheckState(fullPath);
            if (returnState == null || thisState.State > returnState.State)
                returnState = thisState;
        }

        //Then the folders
        foreach (FakeFolder child in childFolders)
        {
            FileStructureState thisState = child.CheckState(fullPath);
            if (returnState == null || thisState.State > returnState.State)
                returnState = thisState;
        }

        //No answer means no files or folders, so we'll set it to be intact
        if (returnState == null)
            return new FileStructureState(FileState.INTACT, FileStateCause.PRESENT, fileName, true);

        returnState.UpdatePath(fileName); //Remember to add ourself to the path!
        return returnState;
    }

    /**
     * Creates this folder in the file system, and asks it's children to do the same
     */
    public override void CreateInSystem(string path)
    {
        //First, make this folder
        string fullPath = path + "\\" + fileName;
        Directory.CreateDirectory(fullPath);

        //Then, make all of the files in this folder
        foreach(FakeFile file in childFiles)
            file.CreateInSystem(fullPath);

        //Finally, recursive calls on directories
        foreach (FakeFolder folder in childFolders)
            folder.CreateInSystem(fullPath);
    }
}
