using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFollower : MonoBehaviour
{
    private bool follow = true;

    /**
     * Sets whether or not the object should follow the player
     */
    public void SetFollow(bool follow)
    {
        this.follow = follow;
        if(!follow)
            transform.localPosition = new Vector3(0, 0, transform.localPosition.z);
    }

    /**
     * Snap to the player position
     */
    private void LateUpdate()
    {
        if (follow && PlayerController.Instance != null)
            transform.position = new Vector3(PlayerController.Instance.transform.position.x, 
                PlayerController.Instance.transform.position.y, transform.position.z);
    }
}
