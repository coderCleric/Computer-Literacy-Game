using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class FakeFileScreen : MonoBehaviour
{
    [SerializeField]
    private Text pathText;
    [SerializeField]
    private Text[] fileSlots;
    [SerializeField]
    private TreeAnt ant;

    /**
     * On awake, do some setup
     */
    private void Start()
    {
        //Link to the ant
        ant.OnArriveAtNode += UpdateDisplay;

        //Initialize the display
        UpdateDisplay(); 
    }

    /**
     * When the ant gets to a new node, need to update the display
     */
    private void UpdateDisplay()
    {
        Debug.Log("Updating display");

        //Update the path
        TreeNode curNode = ant.GetCurrentNode();
        pathText.text = curNode.GetPath();

        //Clear the list
        foreach (Text t in fileSlots)
            t.text = "";

        //Update the list
        int len = curNode.children.Length;
        for(int i = 0; i < len; i++)
        {
            fileSlots[i].text = curNode.children[i].nodeName;
        }
    }

    /**
     * If destroyed, unlink
     */
    private void OnDestroy()
    {
        ant.OnArriveAtNode -= UpdateDisplay;
    }
}
