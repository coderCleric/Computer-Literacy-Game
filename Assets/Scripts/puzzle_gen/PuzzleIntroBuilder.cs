using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class PuzzleIntroBuilder
{
    //Comment on the puzzle
    private static string[] deleteLevelLines = {
        "Okay, so, we need to get through to the other side of the screen, but it looks like there's a big <color=red>#OBSTACLE</color> in the way.",
        "Hm, that's not good. We really need to get past the right side of the screen, but the path is blocked by that <color=red>#OBSTACLE</color>.",
        "Rats! We know where to go, but there's a giant <color=red>#OBSTACLE</color> in the way!",
        "Hm, okay, I think we should try and go to the right, but it seems like that <color=red>#OBSTACLE</color> is blocking the way."
    };
    private static string[] createLevelLines = { 
        "I know I'm a pretty good jumper, but even I can't get across that <color=red>#OBSTACLE</color>.",
        "Whoah, there's not way that I can jump across that <color=red>#OBSTACLE</color>.",
        "Okay, I think we're supposed to go to the right, but that <color=red>#OBSTACLE</color> is way too wide to just jump accross.",
        "Woah! You don't see a <color=red>#OBSTACLE</color> like that very often now do you! Unfortunately, it's right in the way of where we need to go.",
        "Oh boy, that's a pretty big <color=red>#OBSTACLE</color> right there, there's no way we can just jump across."
    };
    
    //Comment on the solution
    private static string[] deleteSolutionLines = {
        "Fortunately, I think that there might just be a way for us to get rid of that thing!",
        "There should be a way for us to make the <color=red>#OBSTACLE</color> disappear though.",
        "Hold on, I think I see something in the code. Aha! I know how to get rid of the <color=red>#OBSTACLE</color>!",
        "Fortunately for us, I know exactly just what to do, we just need you to do it!"
    };
    private static string[] createSolutionLines = {
        "Looks like this area is looking to make a <color=lime>#SPANNER</color> though, so there might be some way we can make it appear.",
        "I'm seeing traces of a <color=lime>#SPANNER</color> in this area, there might be a way that we can make it show up properly.",
        "Hmm, looks like there's room for a <color=lime>#SPANNER</color> in the code. If we do the right thing, the <color=lime>#SPANNER</color> should appear!",
        "Oh, hey! I think if we do the right thing, we should be able to make a <color=lime>#SPANNER</color> that will let us cross the <color=red>#OBSTACLE</color>!",
        "Fortunately, I'm seeing traces of a <color=lime>#SPANNER</color> in the system. If we can set up the right environment, it should appear."
    };
    
    //Comment on the expected action
    private static string[] deleteActionLines = { 
        "It seems like the <color=red>#OBSTACLE</color> is associated with a specific file in the system. If we can delete the file, then the <color=red>#OBSTACLE</color> should go away!",
        "Looks like the <color=red>#OBSTACLE</color> is linked to a certain file in the file system. Deleting that file should get rid of the <color=red>#OBSTACLE</color>.",
        "What I'm seeing is the the <color=red>#OBSTACLE</color> relies on a specific file. If we delete that file, the <color=red>#OBSTACLE</color> will go away!",
        "I think that there should be a file that the <color=red>#OBSTACLE</color> is associated with. If we get rid of that file, the obstacle should vanish!"
    };
    private static string[] createActionLines = { 
        "Security in this area is pretty <color=lime>lax</color>, so we should be able to fool the system pretty easily. If we make a file in the right location with the name <color=lime>#FILENAME</color>, it should make a <color=lime>#SPANNER</color> that will let us cross!",
        "I don't think the system is looking at the contents of any files here, so we should be able to <color=lime>make our own file</color> with the name <color=lime>#FILENAME</color> to make the <color=lime>#SPANNER</color> appear.",
        "The system isn't paying too much attention to the particulars of the file, so just making one with the name <color=lime>#FILENAME</color> in the right location should make the <color=lime>#SPANNER</color> appear.",
        "Fortunately for us, the system isn't paying very much attention to this area, so just making a file named <color=lime>#FILENAME</color> in the right location should cause the <color=lime>#SPANNER</color> to appear."
    };
    private static string[] moveActionLines = { 
        "Security in this area is pretty <color=red>tight</color> though, we won't be able to just make our own file. Fortunately though, we should be able to take a file from elsewhere in the system so we can make a <color=lime>#SPANNER</color> anyway!",
        "Looks like the system is paying <color=red>lots of attention</color> to this area though, so we can't just make our own file. If we can <color=lime>find a relevant file elsewhere in the system and bring it here</color>, that should work.",
        "Bad luck for us, the system is paying close attention to the contents of files in this area, so we'll need to find a suitable file somewhere in the system and bring it to the correct location for the <color=lime>#SPANNER</color> to appear.",
        "Doesn't look like this one's going to be as simple as just making a file though. The system expects certain contents, so we'll need to find the file somewhere in the file system and bring it to the correct location for the <color=lime>#SPANNER</color> to show up."
    };
    
    //Comment on the known path
    private static string[] knownPathLines = { 
        "I know that the file we're looking for should be somewhere in <color=lime>#KNOWNPATH</color>.",
        "Seems like the place we need to do things is somewhere in <color=lime>#KNOWNPATH</color>.",
        "<color=lime>#KNOWNPATH</color> should contain the file we're looking for somewhere.",
        "I'm pretty sure that the file we're looking for should be somewhere in <color=lime>#KNOWNPATH</color>.",
        "You should start looking for where we need to do that in <color=lime>#KNOWNPATH</color>."
    };

    //Comment on the unknown path
    private static string[] unknownPathLines = { 
        "Unfortunately, I'm not able to pinpoint the exact location. It should be <color=lime>#UNKNOWNNUM</color> folders deeper than the path that I told you.",
        "I can't figure out where exactly you need to go though, it seems like the actual location is <color=lime>#UNKNOWNNUM</color> folders deeper than that path.",
        "That location isn't the exact one though, sorry. I do know that it should be <color=lime>#UNKNOWNNUM</color> folders deeper than that.",
        "Ugh, I'm really not sure on the exact location, but it should be <color=lime>#UNKNOWNNUM</color> folders deeper than the path I just gave you."
    };

    //Special case: no known parts
    private static string[] noKnownLines = { 
        "Unfortunately, I have <color=red>no clue</color> where the file we're looking for is, I just know that it's <color=lime>#UNKNOWNNUM</color> folders deeper than where you'll start.",
        "Yikes, things are really obscured here. I don't have a clue what location we're looking for, except that its <color=lime>#UNKNOWNNUM</color> folders deeper than where you'll start.",
        "Rats! I have no idea where you should actually do the file operation, I just know that it's <color=lime>#UNKNOWNNUM</color> folders deeper than where you'll start."
    };

    //Comment on where the button will start them
    private static string[] startLocationLines = { 
        "When you click on the button to open the file explorer it should start you at <color=lime>#STARTPATH</color>. Good luck!",
        "Looks like the system will start you at <color=lime>#STARTPATH</color>. Best of luck!",
        "The button to open the file explorer should start you in <color=lime>#STARTPATH</color>. Good luck!",
        "Looks like you'll be starting at <color=lime>#STARTPATH</color>. Have fun!"
    };

    //Special case: no movement
    private static string[] noMovementLines = { 
        "It looks like the file you'll need to mess with is actually in the <color=lime>same folder you'll start in</color>, so that's nice!",
        "Fortunately for us, the system is going to start you at the <color=lime>same folder that you need to do things in</color>. Better get going!",
        "Hey, nice! Looks like the folder that you start in will actually be the <color=lime>same as</color> the one you need to do a file operation in!",
        "Oh, hey! That's cool! Looks like you'll be starting <color=lime>in the location</color> where you need to do the file operation!"
    };

    /**
     * Create the puzzle intro dialogue
     */
    public static DialogueChain MakeIntroDialogue(PuzzleVector puzzleVector, string pathToAction, Dictionary<string, string> insertions)
    {
        List<string> lines = new List<string>();

        //Start by creating the level and solution lines
        switch(puzzleVector.GetPuzzleType())
        {
            case PuzzleType.DELETE:
                lines.Add(GetRandomLine(deleteLevelLines));
                lines.Add(GetRandomLine(deleteSolutionLines));
                break;
            case PuzzleType.CREATE:
            case PuzzleType.MOVE:
                lines.Add(GetRandomLine(createLevelLines));
                lines.Add(GetRandomLine(createSolutionLines));
                break;
        }

        //Next, make the action comment
        switch (puzzleVector.GetPuzzleType())
        {
            case PuzzleType.DELETE:
                lines.Add(GetRandomLine(deleteActionLines));
                break;
            case PuzzleType.CREATE:
                lines.Add(GetRandomLine(createActionLines));
                break;
            case PuzzleType.MOVE:
                lines.Add(GetRandomLine(moveActionLines));
                break;
        }

        //Find out some key things about the path
        bool noDepth = puzzleVector.GetDepth() == 0;
        bool allUnknown = !noDepth && puzzleVector.GetDepth() == puzzleVector.GetUnknownLen();
        bool allKnown = puzzleVector.GetUnknownLen() == 0;

        //Find the starting location and known section of the path
        string[] pathParts = pathToAction.Split('\\');
        string startPath = pathParts[0];
        for(int i = 1; i <= PuzzleVector.maxDepth - puzzleVector.GetDepth(); i++)
        {
            startPath += "/" + pathParts[i];
        }
        string knownPath = pathParts[0];
        for (int i = 1; i <= (PuzzleVector.maxDepth - puzzleVector.GetDepth()) + (puzzleVector.GetDepth() - puzzleVector.GetUnknownLen()); i++)
        {
            knownPath += "/" + pathParts[i];
        }

        //Make the known path line
        if (!noDepth && !allUnknown)
            lines.Add(GetRandomLine(knownPathLines));

        //Make the unknown path line
        if (!allKnown && !allUnknown)
            lines.Add(GetRandomLine(unknownPathLines));
        else if (allUnknown)
            lines.Add(GetRandomLine(noKnownLines));

        //Make the start location line
        if(!noDepth)
            lines.Add(GetRandomLine(startLocationLines));
        else
            lines.Add(GetRandomLine(noMovementLines));

        //Replace path flags with actual paths
        for(int i = 0; i < lines.Count; i++)
        {
            lines[i] = lines[i].Replace("#STARTPATH", startPath);
            lines[i] = lines[i].Replace("#KNOWNPATH", knownPath);
            lines[i] = lines[i].Replace("#UNKNOWNNUM", puzzleVector.GetUnknownLen().ToString());
        }

        //Do replacements based on the dictionary
        foreach(string key in insertions.Keys)
        {
            for (int i = 0; i < lines.Count; i++)
            {
                lines[i] = lines[i].Replace(key, insertions[key]);
            }
        }

        //Make the dialogue chain
        GameObject go = new GameObject("NewIntroDialogue");
        go.transform.parent = DialogueManager.Instance.transform;
        DialogueChain chain = go.AddComponent<DialogueChain>();
        chain.lines = lines.ToArray();

        return chain;
    }

    /**
     * Gets a random line from the specified array
     */
    private static string GetRandomLine(string[] lineArray)
    {
        return lineArray[UnityEngine.Random.Range(0, lineArray.Length)];
    }
}
