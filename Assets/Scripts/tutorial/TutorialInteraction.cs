using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialInteraction : MonoBehaviour
{
    [SerializeField]
    private TutorialStateManager stateManager;
    [SerializeField]
    private GameObject eprompt;
    private bool playerInside;

    //When the player enters, set them as being inside
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerInside = true;
            eprompt.SetActive(true);
        }
    }

    //When the player exits, set them as being outside
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerInside = false;
            eprompt.SetActive(false);
        }
    }

    //If the player is in the zone, check for interaction
    private void Update()
    {
        if (InputStateTracker.CheckGameState(InputStateTracker.GameState.PLAY) && playerInside && Input.GetButtonDown("Interact"))
        {
            stateManager.EnterTutorial();
        }
    }
}
