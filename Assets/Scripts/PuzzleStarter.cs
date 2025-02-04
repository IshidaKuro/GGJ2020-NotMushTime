﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleStarter : MonoBehaviour
{
    public delegate void PuzzleDelegate(string message);
    public event PuzzleDelegate OnStartError;

    public Vector3 PortalPosition;

    public Vector3 PortalSize;

    [SerializeField]
    private Puzzle puzzleToStart;

    [SerializeField]
    private bool isEnabled = true;

    List<Player> collidingPlayers = new List<Player>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player collidingPlayer = collision.gameObject.GetComponent<Player>();
        if(collidingPlayer)
        {
            collidingPlayer.OnInteract += CollidingPlayer_OnInteract;

            if(!collidingPlayers.Contains(collidingPlayer))
            {
                collidingPlayers.Add(collidingPlayer);
            }
        }
    }

    private void CollidingPlayer_OnInteract(Player interactingPlayer)
    {
        if(!isEnabled || puzzleToStart == null)
        {
            return;
        }

        if(puzzleToStart.PlayersRequired > collidingPlayers.Count)
        {
            if (OnStartError != null)
            {
                OnStartError("You require both players to start this puzzle!");
            }

            return;
        }

        Player[] players = new Player[puzzleToStart.PlayersRequired];
        players[0] = interactingPlayer;

        foreach (Player player in collidingPlayers)
        {
            if (player != interactingPlayer)
            {
                players[1] = player;
                break;
            }
        }

        isEnabled = false;

        GameManager.Instance.StartPuzzle(players, puzzleToStart, PortalPosition, PortalSize);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Player collidingPlayer = collision.gameObject.GetComponent<Player>();
        if (collidingPlayer)
        {
            collidingPlayer.OnInteract -= CollidingPlayer_OnInteract;

            if (collidingPlayers.Contains(collidingPlayer))
            {
                collidingPlayers.Remove(collidingPlayer);
            }
        }
    }
}
