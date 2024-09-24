using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickableTree : MonoBehaviour
{
    [SerializeField]
    private TreeAnt ant;

    /**
     * Navigates the ant from the current node to the one with the target name
     * 
     * @param targetName The name of the target node
     */
    public void NavToNode(string targetName)
    {
        Debug.Log("calling navtonode");

        //If the ant is already moving, cancel
        if (ant.IsMoving)
            return;

        //If that's the current node, cancel
        if (targetName.Equals(ant.GetCurrentNode().nodeName))
            return;

        //First, need to nav back to the root
        TreeNode root = ant.GetCurrentNode();
        while(root.parent != null)
            root = root.parent;

        //Get the abs path of current pos
        List<TreeNode> listToCur = root.GetNodesToTarget(ant.GetCurrentNode().nodeName);

        //Abs path of target
        List<TreeNode> listToTarget = root.GetNodesToTarget(targetName);

        //Find where they stop matching
        int firstDifIndex = 0;
        int lenToCur = listToCur.Count;
        int lenToTarget = listToTarget.Count;
        for(; firstDifIndex < lenToCur && firstDifIndex < lenToTarget; firstDifIndex++)
        {
            if (listToCur[firstDifIndex] != listToTarget[firstDifIndex])
                break;
        }

        //For each unmatched node in the cur pos, step back
        List<TreeNode> result = new List<TreeNode>();
        TreeNode curParent = ant.GetCurrentNode().parent;
        for(int i = firstDifIndex; i < lenToCur; i++)
        {
            result.Add(curParent);
            curParent = curParent.parent;
        }

        //Append the unmatched nodes of the target pos
        for(int i = firstDifIndex; i < lenToTarget; i++)
        {
            result.Add(listToTarget[i]);
        }

        //Move the ant to the location
        ant.MoveAntWithoutReposition(result);
    }
}
