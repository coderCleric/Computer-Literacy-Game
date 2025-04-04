using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPage : MonoBehaviour
{
    [SerializeField]
    protected TreeAnt ant;
    [SerializeField]
    private string antPath;
    public string AntPath => antPath;
    [SerializeField]
    private Animateable[] animateables;

    /**
     * Disable this page
     */
    public virtual void Disable()
    {
        gameObject.SetActive(false);
    }

    /**
     * Enable this page
     */
    public virtual void Enable()
    {
        gameObject.SetActive(true);
        PlayAnim();
    }

    /**
     * Play the animation for this page
     */
    public virtual void PlayAnim()
    {
        if(ant != null)
            ant.MoveAnt(antPath);
        foreach(Animateable animateable in animateables)
        {
            animateable.StartAnimation();
        }
    }

    /**
     * Tells whether or not this page has an animation
     */
    public virtual bool HasAnim()
    {
        return ant != null || animateables != null;
    }
}
