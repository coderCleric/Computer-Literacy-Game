using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InputStateTracker
{
    //Enums
    public enum InputState {GAME, TUTORIAL}
    public enum GameState {PLAY, DIALOGUE}

    //Actual states
    private static InputState overallState = InputState.GAME;
    private static GameState gameState = GameState.PLAY;

    //Check the given overall state
    public static bool CheckState(InputState state)
    {
        return state == overallState;
    }

    //Check the given game state
    public static bool CheckGameState(GameState state)
    {
        return overallState == InputState.GAME && gameState == state;
    }

    //Assign the given overall state
    public static void SetState(InputState state)
    {
        overallState = state;
    }

    //Assign the given game state
    public static void SetGameState(GameState state)
    {
        gameState = state;
    }

    //Gets the string representation of the state
    public static string GetStateString()
    {
        return "Game: " + gameState + "\tOverall: " + overallState;
    }
}
