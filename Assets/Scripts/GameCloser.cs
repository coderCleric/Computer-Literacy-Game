using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCloser : MonoBehaviour
{
    [SerializeField]
    private GameObject closePrompt;

    /**
     * Pop up the close prompt
     */
    public void PromptClose()
    {
        closePrompt.SetActive(true);
    }

    /**
     * Hide the close prompt
     */
    public void CancelClose()
    {
        closePrompt.SetActive(false);
    }

    /**
     * Actually close the game (only works in build)
     */
    public void CompleteClose()
    {
        //Gets rid of all of the file explorer windows, which is nice
        if(PuzzleManager.Instance != null)
        {
            PuzzleManager.Instance.fileStruct.ClearStructure();
        }

        Application.Quit();
    }
}
