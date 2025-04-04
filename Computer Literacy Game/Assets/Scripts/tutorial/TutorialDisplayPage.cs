using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialDisplayPage : TutorialPage
{
    /**
     * No animation, just turn on display
     */
    public override void Enable()
    {
        gameObject.SetActive(true);
    }

    /**
     * No animation, so just do nothing
     */
    public override void PlayAnim(){}

    /**
     * This will never have an animation
     */
    public override bool HasAnim()
    {
        return false;
    }
}
