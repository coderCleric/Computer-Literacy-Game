using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideableNodeManager : MonoBehaviour
{
    [SerializeField]
    private HideableTreeNode root;
    [SerializeField]
    private TreeAnt ant;

    /**
     * On awake, link to the ant
     */
    private void Start()
    {
        ant.OnArriveAtNode += UpdateNodes;
        (ant.GetCurrentNode() as HideableTreeNode).Reveal();
    }

    /**
     * Check the ant for what nodes should be active
     */
    private void UpdateNodes()
    {
        //First, hide them all
        root.HideAll();

        //Then, reveal from the current
        (ant.GetCurrentNode() as HideableTreeNode).Reveal();
    }

    /**
     * If destroyed, unlink from the ant
     */
    private void OnDestroy()
    {
        ant.OnArriveAtNode -= UpdateNodes;
    }
}
