using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinZone : MonoBehaviour
{/**
     * If the player enters the collider, they win
     */
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            PuzzleManager.Instance.OnLevelFinish();
    }
}
