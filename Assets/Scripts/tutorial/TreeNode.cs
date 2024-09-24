using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeNode : MonoBehaviour
{
    public string nodeName = "";
    public TreeNode parent = null;
    public TreeNode[] children = null;

    /**
     * On awake, need to link children to us
     */
    protected void Awake()
    {
        foreach(TreeNode node in children)
        {
            node.parent = this;
        }
    }

    /**
     * Gives the node path from the string path
     * 
     * @param path The path to evaluate
     * @return The list of treenodes, or null if the path is invalid
     */
    public List<TreeNode> ConvertToNodePath(string path)
    {
        //Split the query
        string[] parts = path.Split('/');

        //If the very first item is this, set it as the first
        List<TreeNode> result = new List<TreeNode>();
        bool skipFirst = false;
        if (parts[0].Equals(nodeName))
        {
            result.Add(this);
            skipFirst = true;
        }

        //Do a loop to find the path
        TreeNode curNode = this;
        foreach (string part in parts)
        {
            //Skip the first one if it's already been grabbed
            if (skipFirst) {
                skipFirst = false;
                continue;
            }

            //If it's a .., go to the parent
            if(part.Equals(".."))
            {
                if(curNode.parent == null) //Need to have a parent for that to work
                    return null;

                curNode = curNode.parent;
                result.Add(curNode);
            }

            //If it's a name, go to the child
            else
            {
                bool found = false;
                foreach (TreeNode child in curNode.children)
                {
                    //Found it
                    if (child.nodeName.Equals(part))
                    {
                        found = true;
                        curNode = child;
                        result.Add(curNode);
                        break;
                    }
                }
                if (!found) //No found child is an error
                    return null;
            }
        }

        return result;
    }

    /**
     * Finds the path to the given node from the current. Assumes that the target node is a descendant of the current.
     * 
     * @param target The name of the target node
     * @return A list of nodes to follow, or NULL if it wasn't found
     */
    public List<TreeNode> GetNodesToTarget(string targetName)
    {
        //If this is the node, return a new list with us in it
        if(nodeName.Equals(targetName))
            return new List<TreeNode> { this };

        //If we have no children, return null
        if (children == null)
            return null;

        //If we have children, check to see if one found something
        List<TreeNode> result = null;
        foreach(TreeNode node in children)
        {
            result = node.GetNodesToTarget(targetName);

            //If it did, add ourselves to it
            if(result != null)
            {
                result.Insert(0, this);
                break;
            }
        }

        //Result will now hold either null or the list
        return result;
    }

    /**
     * Gets the path of the node as a string
     * 
     * @return The string representation of the path
     */
    public string GetPath()
    {
        string result = nodeName;

        //Add the names of the parents
        TreeNode walker = parent;
        while (walker != null)
        {
            result = walker.nodeName + "/" + result;
            walker = walker.parent;
        }

        return result;
    }

    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        foreach(TreeNode node in children)
        {
            Gizmos.DrawLine(transform.position, node.transform.position);
        }
    }
    #endif
}
