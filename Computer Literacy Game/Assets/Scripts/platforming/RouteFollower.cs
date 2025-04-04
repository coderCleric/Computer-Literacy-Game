//Allows the object to follow the given set of waypoints

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RouteFollower : MonoBehaviour
{
    //Publics
    public Transform[] waypoints;
    public float speed;
    public bool loop;

    //Privates
    private int currentTargetI = 0;

    // Update is called once per frame
    void Update()
    {
        //Auto return if the target is invalid
        if (currentTargetI > waypoints.Length - 1)
            return;

        //Check if we made it to the waypoint
        if(Vector2.Distance(transform.position, waypoints[currentTargetI].transform.position) < 0.1f)
        {
            //Increment target
            currentTargetI++;


            if (loop)
            {
                currentTargetI = currentTargetI % waypoints.Length;
            }
        }

        //Move towards the selected waypoint
        Debug.Log("Test");
        transform.position = Vector2.MoveTowards(transform.position, waypoints[currentTargetI].transform.position, Time.deltaTime * speed);
    }
}
