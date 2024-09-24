using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportZone : MonoBehaviour
{
    [SerializeField]
    private Transform teleportPoint;

    /**
     * If the player enters the collider, teleport them
     */
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
            collision.transform.position = teleportPoint.position;
    }
}
