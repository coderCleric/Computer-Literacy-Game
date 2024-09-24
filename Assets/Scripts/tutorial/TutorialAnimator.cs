using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialAnimator : Animateable
{
    private Animator animator;

    /**
     * On awake, grab the animator
     */
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    /**
     * Animation just needs to trigger the animator
     */
    public override void StartAnimation()
    {
        animator.enabled = true;
        animator.SetTrigger("play");
    }

    /**
     * Disables the animator. Useful if other actions need to take place
     */
    public void DisableAnimator()
    {
        animator.enabled = false;
    }
}
