using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialChallengePage : TutorialPage
{
    [SerializeField]
    private TutorialSuccessTrigger successTrigger;
    [SerializeField]
    private Button nextButton;
    [SerializeField]
    private AudioSource winAudio;
    private bool passed = false;

    /**
     * On awake, listen for success
     */
    private void Awake()
    {
        successTrigger.OnSuccess += OnSuccess;
    }

    /**
     * When activated, disable the next button
     */
    public override void PlayAnim()
    {
        base.PlayAnim();
        nextButton.interactable = passed;
    }

    /**
     * On success, continue allow player to continue
     */
    private void OnSuccess()
    {
        passed = true;
        nextButton.interactable = true;
        winAudio.Play();
    }

    /**
     * On destroy, remove listener
     */
    private void OnDestroy()
    {
        successTrigger.OnSuccess -= OnSuccess;
    }
}
