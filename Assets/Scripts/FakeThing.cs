using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FakeThing : MonoBehaviour
{
    public string fileName = "";
    public FakeFolder parent = null;

    /**
     * On start, if out name is blank, grab the object name
     */
    public virtual void Start()
    {
        //Set a default name if given none
        if(fileName.Equals(""))
            fileName = gameObject.name;

        //Grab the parent
        if(transform.parent != null && transform.parent.gameObject.GetComponent<FakeFolder>() != null)
            parent = transform.parent.gameObject.GetComponent<FakeFolder>();

    }

    /**
     * Gets the full path to the fake thing
     */
    public virtual string GetFullPath()
    {
        if (parent == null)
            return fileName;
        else
            return parent.GetFullPath() + "\\" + fileName;
    }

    /**
     * Returns the state of the fake thing
     * 
     * @param The path to check at
     * @return The state
     */
    public abstract FileStructureState CheckState(string path);

    /**
     * Creates the actual file or folder
     * 
     * @param path The path to the directory to make it in
     */
    public abstract void CreateInSystem(string path);
}
