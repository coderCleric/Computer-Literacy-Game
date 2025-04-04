using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TreeAnt : MonoBehaviour
{
    public TreeNode homeNode;
    public float speed = 1;
    private Vector3 startPos;

    //Needed to run the walking
    private TreeNode[] pathNodes = null;
    private int currentNodeIndex = -1;
    private bool isMoving = false;
    public bool IsMoving => isMoving;

    //Let's the ant listen do things for other objects when arriving at nodes
    public delegate void AntArriveAtNodeEvent();
    public event AntArriveAtNodeEvent OnArriveAtNode;
    protected void InvokeOnArriveAtNode()
    {
        if (OnArriveAtNode != null)
            OnArriveAtNode();
    }

    /**
     * On start, save the position
     */
    private void Awake()
    {
        startPos = transform.position;
    }

    /**
     * Moves the ant along the given path
     */
    public void MoveAnt(string path)
    {
        //Get the broken up path
        List<TreeNode> nodePath = homeNode.ConvertToNodePath(path);

        MoveAnt(nodePath);
    }

    /**
     * Moves the ant along the given list of nodes
     */
    public void MoveAnt(List<TreeNode> nodePath)
    {
        transform.position = startPos;
        MoveAntWithoutReposition(nodePath);
    }

    /**
     * Moves the ant along the path without the initial reposition
     */
    public void MoveAntWithoutReposition(List<TreeNode> nodePath)
    {
        //If it's an invalid path, scream and stop
        if (nodePath == null)
        {
            Debug.LogError("Ant was told to walk invalid path!");
            return;
        }

        //Set it up to start moving
        isMoving = true;
        currentNodeIndex = -1;
        pathNodes = nodePath.ToArray();
    }

    /**
     * Gets the current node the ant is at
     * 
     * @return The ant's current node
     */
    public TreeNode GetCurrentNode()
    {
        if (currentNodeIndex == -1)
            return homeNode;

        return pathNodes[currentNodeIndex];
    }

    /**
     * Move the ant every frame
     */
    private void Update()
    {
        if(isMoving)
        {
            //Preprocess the target position's z
            Vector3 target = pathNodes[currentNodeIndex + 1].transform.position;
            target.z = transform.position.z;

            //rotate to face
            transform.rotation = Quaternion.LookRotation(Vector3.forward, target - transform.position);
            //Take the step
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

            //If that step brought us to the target, need to do extra logic
            if((target - transform.position).magnitude < 0.001)
            {
                currentNodeIndex++;

                //That was the last node, need to stop
                if(currentNodeIndex == pathNodes.Length - 1)
                {
                    isMoving = false;
                }

                //Do what any listeners want
                InvokeOnArriveAtNode();
            }
        }
    }
}
