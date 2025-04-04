//A simple "stick to target" script, meant for things like cameras

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sticky2D : MonoBehaviour
{
    public Transform target;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Late Update is called after all update calls
    //Set to target at the end of the frame
    private void LateUpdate()
    {
        Transform tf = GetComponent<Transform>();
        tf.position = new Vector3(target.position.x, target.position.y, tf.position.z);
    }
}
