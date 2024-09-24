using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class TutorialStateManager : MonoBehaviour
{
    [SerializeField]
    private GameObject tutorialCanvas;
    [SerializeField]
    private TutorialController tutorialController;
    [SerializeField]
    private GameObject gameRoot;
    [SerializeField]
    private AudioSource enterAudio;
    [SerializeField]
    private AudioSource exitAudio;

    /**
     * On start, make sure appropriate things are disabled/enabled
     */
    private void Awake()
    {
        tutorialCanvas.SetActive(false);
        tutorialController.ExitTutorial();
        gameRoot.SetActive(true);
    }

    /**
     * Enter the game
     */
    public void EnterGame()
    {
        ExitTutorial();

        InputStateTracker.SetState(InputStateTracker.InputState.GAME);
        gameRoot.SetActive(true);
    }

    /**
     * Exit the game
     */
    public void ExitGame()
    {
        gameRoot.SetActive(false);
    }

    /**
     * Enter the tutorial
     */
    public void EnterTutorial()
    {
        ExitGame();

        InputStateTracker.SetState(InputStateTracker.InputState.TUTORIAL);
        tutorialCanvas.SetActive(true);
        tutorialController.StartTutorial();
        enterAudio.Play();
    }

    /**
     * Exit the tutorial
     */
    public void ExitTutorial()
    {
        tutorialCanvas.SetActive(false);
        tutorialController.ExitTutorial();
        exitAudio.Play();
    }
}
