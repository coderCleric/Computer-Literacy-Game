using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideableTreeNode : TreeNode
{
    private SpriteRenderer cover;

    /**
     * On awake, grab the cover
     */
    protected new void Awake()
    {
        base.Awake();
        cover = GetComponent<SpriteRenderer>();
    }

    /**
     * Reveal this node, as well as adjacent nodes
     */
    public void Reveal()
    {
        //First, uncover this
        SetCovered(false);

        //Next, uncover all children
        foreach(HideableTreeNode node in children)
        {
            node.SetCovered(false);
        }

        //Finally, uncover the parents
        HideableTreeNode walker = this.parent as HideableTreeNode;
        while(walker != null)
        {
            walker.SetCovered(false);
            walker = walker.parent as HideableTreeNode;
        }
    }

    /**
     * Set whether or not this node should be covered
     * 
     * @param covered Whether or not the node should be covered
     */
    public void SetCovered(bool covered)
    {
        cover.enabled = covered;
    }

    /**
     * Hides this node and all children
     */
    public void HideAll()
    {
        SetCovered(true);
        foreach(HideableTreeNode node in children)
            node.HideAll();
    }
}
