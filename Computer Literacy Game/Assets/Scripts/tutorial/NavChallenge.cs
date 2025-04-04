using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavChallenge : TutorialSuccessTrigger
{
    [SerializeField]
    private TreeNode target;
    [SerializeField]
    private TreeAnt ant;

    /**
     * On awake, listen to arrivals of the ant
     */
    private void Awake()
    {
        ant.OnArriveAtNode += CheckState;
    }

    /**
     * When the ant arrives at a location, need to check the challenge state
     */
    private void CheckState()
    {
        if (target == ant.GetCurrentNode())
            InvokeOnSuccess();
    }

    /**
     * If destroyed, unlink from the ant
     */
    private void OnDestroy()
    {
        ant.OnArriveAtNode -= CheckState;
    }
}
