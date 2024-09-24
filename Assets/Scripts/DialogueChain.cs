using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueChain : MonoBehaviour
{
    [TextArea(1, 20)]
    public string[] lines;
    private int curIndex = 0;

    /**
     * Gets the next line of dialogue
     */
    public string GetNextLine()
    {
        //This line is valid, retrieve it and move on
        if(curIndex < lines.Length)
        {
            string line = lines[curIndex];
            curIndex++;
            return line;
        }

        //This line is invalid, reset and return null
        else
        {
            curIndex = 0;
            return null;
        }
    }
}
