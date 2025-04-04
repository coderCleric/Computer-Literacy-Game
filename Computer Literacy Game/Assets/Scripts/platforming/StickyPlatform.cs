//Causes the player to stick to the object when standing on it

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyPlatform : MonoBehaviour
{
    //Public
    public GameObject player;

    //Private
    private Transform playerParent;

    //On collision, stick the player to the platform
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject == player)
        {
            playerParent = player.transform.parent;
            player.transform.SetParent(transform);
        }
    }

    //On collision exit, detatch the player
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == player)
        {
            player.transform.SetParent(playerParent);
        }
    }
}
