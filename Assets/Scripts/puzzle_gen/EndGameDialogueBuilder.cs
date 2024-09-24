using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class EndGameDialogueBuilder
{
    /**
     * Actually makes the ending dialogue
     */
    public static DialogueChain MakeEndDialogue(float points, int levels, int fails) {
        //Static lines
        string[] lines = new string[4];
        lines[0] = "Nice! Looks like we made it all the way into their system! Thanks a ton for the help.";
        lines[1] = "I hope you don’t mind, but I was tallying a sort of points system for your performance throughout. It looks like you got #POINTS points in total over the course of #LEVELS levels.";
        lines[3] = "Anyway, I’m just going to fall until I reach the core of the system. It’s going to take a while, so just go ahead and close the game whenever. Thanks again for the help!";

        //Line depending on number of fails
        switch(fails)
        {
            case 0:
                lines[2] = "Great job! You managed to not fully trip the system even once, which is really impressive!";
                break;
            case 1:
            case 2:
                lines[2] = "You did a really good job overall! You only tripped the system occasionally, so it was a pretty clean infiltration.";
                break;
            case 3:
            case 4:
            case 5:
                lines[2] = "Good job! You tripped the system a few times, but still not a ton!";
                break;
            default:
                lines[2] = "You tripped the system a fair amount, but you were still able to get us through in the end! Nice work!";
                break;
        }

        //Actually insert parameters into the second line
        lines[1] = lines[1].Replace("#POINTS", points.ToString("N0"));
        lines[1] = lines[1].Replace("#LEVELS", levels.ToString());

        //Make the actual dialogue chain
        GameObject go = new GameObject("EndDialogue");
        go.transform.parent = DialogueManager.Instance.transform;
        DialogueChain chain = go.AddComponent<DialogueChain>();
        chain.lines = lines;

        return chain;
    }
}
