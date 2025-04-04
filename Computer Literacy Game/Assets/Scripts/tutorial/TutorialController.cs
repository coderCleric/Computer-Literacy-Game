using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialController : MonoBehaviour
{
    [SerializeField]
    private TutorialPage[] pages;
    [SerializeField]
    private TutorialPage folderTriggerPage;
    private int currentPageIndex = 0;
    [SerializeField]
    private Button nextButton;
    [SerializeField]
    private Button replayButton;
    [SerializeField]
    private Button prevButton;
    [SerializeField]
    private Button folderButton;
    [SerializeField]
    private FileStructure fileStructure;
    [SerializeField]
    private GameObject[] disableOnFinish;
    [SerializeField]
    private DialogueChain endDialogue;
    [SerializeField]
    private AudioSource flipAudio;

    /**
     * On start, do some setup
     */
    private void Start()
    {
        //Disable all but the first page
        bool isFirst = true;
        foreach(TutorialPage page in pages)
        {
            if(isFirst) //Don't disable the first one
            {
                isFirst = false;
                continue;
            }
            page.Disable();
        }

        //Have the first page do it's thing
        pages[0].Enable();
    }

    /**
     * Does the logic for entering the tutorial
     */
    public void StartTutorial()
    {
        gameObject.SetActive(true);
        pages[currentPageIndex].PlayAnim();
    }

    /**
     * Does the logic for exiting the tutorial
     */
    public void ExitTutorial()
    {
        gameObject.SetActive(false);
    }

    /**
     * Go to the next tutorial page
     */
    public void NextPage()
    {
        //Error check
        if(currentPageIndex >= pages.Length - 1)
        {
            Debug.LogError("TutorialController was told to go to next page at end of list!");
            return;
        }

        prevButton.interactable = true; //Enable the previous button

        //Turn current off and next on
        pages[currentPageIndex].Disable();
        currentPageIndex++;
        pages[currentPageIndex].Enable();

        //Check if the next button needs disabled
        if (currentPageIndex >= pages.Length - 1)
            nextButton.interactable = false;

        //Check if the replay button needs disabled
        if (!pages[currentPageIndex].HasAnim())
            replayButton.interactable = false;
        else
            replayButton.interactable = true;

        //Check if the folder button needs revealed
        if (pages[currentPageIndex] == folderTriggerPage)
        {
            fileStructure.MakeTutorialStructure();
            folderButton.gameObject.SetActive(true);
        }

        //Check if the door needs opened
        if (currentPageIndex == pages.Length - 1)
        {
            foreach(GameObject go in disableOnFinish)
                go.SetActive(false);
            DialogueManager.Instance.PlayDialogue(endDialogue);
        }

        //Play the audio
        flipAudio.Play();
    }

    /**
     * Go to the previous tutorial page
     */
    public void PrevPage()
    {
        //Error check
        if (currentPageIndex <= 0)
        {
            Debug.LogError("TutorialController was told to go to prev page at start of list!");
            return;
        }

        nextButton.interactable = true; //Enable the next button

        //Turn current off and next on
        pages[currentPageIndex].Disable();
        currentPageIndex--;
        pages[currentPageIndex].Enable();

        //Check if the prev button needs disabled
        if (currentPageIndex <= 0)
            prevButton.interactable = false;

        //Check if the replay button needs disabled
        if (!pages[currentPageIndex].HasAnim())
            replayButton.interactable = false;
        else
            replayButton.interactable = true;

        //Play the audio
        flipAudio.Play();
    }

    /**
     * Replay the animation of the current page
     */
    public void ReplayCurrentPage()
    {
        pages[currentPageIndex].PlayAnim();
    }
}
