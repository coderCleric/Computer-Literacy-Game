using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class FileAntNav : MonoBehaviour
{
    [SerializeField]
    private TreeAnt ant;
    private float lastClickTime = -9;
    private int lastClickIndex = -1;

    /**
     * Try and send the ant to the indexed node
     * 
     * @param index The index to go to
     */
    public void GoToIndex(int index)
    {
        //First, check click stats
        bool act = (Time.time < lastClickTime + 0.5f) && (index == lastClickIndex);

        //Next, update the stats
        lastClickIndex = index;
        lastClickTime = Time.time;

        //If we should act, send the ant to the correct node
        if(act && !ant.IsMoving && ant.GetCurrentNode().children.Length > index)
        {
            List<TreeNode> target = new List<TreeNode> {ant.GetCurrentNode().children[index]};
            ant.MoveAntWithoutReposition(target);
        }
    }

    /**
     * Try and move the ant up to the parent
     */
    public void GoToParent()
    {
        if (!ant.IsMoving && ant.GetCurrentNode().parent != null)
        {
            List<TreeNode> target = new List<TreeNode> {ant.GetCurrentNode().parent};
            ant.MoveAntWithoutReposition(target);
        }
    }
}
