using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleLevel : MonoBehaviour
{
    public Transform playerSpawn;
    public string acceptableFileName;
    [SerializeField]
    private GameObject victoryObject;
    [SerializeField]
    private bool startDisabled = false;

    /**
     * Start the level
     */
    public void StartLevel()
    {
        //Disable the victory object, if needed
        if (startDisabled)
            victoryObject.SetActive(false);

        //Move the player to the spawn
        PlayerController.Instance.transform.position = playerSpawn.position;
    }

    /**
     * Trigger the player winning, causing the victory object to be affected
     */
    public void TriggerWin()
    {
        victoryObject.SetActive(startDisabled);
    }
}
