using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public enum PuzzleType { DELETE, CREATE, MOVE }

public class PuzzleVector
{
    public static int maxDepth { get; private set; } = 3;
    public static List<PuzzleVector> allVecs = new List<PuzzleVector>();

    private PuzzleType type;
    private int depth;
    private int unknown;

    /**
     * Factory to generate puzzle vectors based on a desired difficulty
     */
    public static PuzzleVector CreatePuzzleVector(float difficulty, bool constrainType = false, PuzzleType t = PuzzleType.DELETE, int depth = -1, int unknown = -1, int favorDirection = 0)
    {
        //Loop through each possibility, calculating difficulty
        float closestDif = -9999;
        List<PuzzleVector> closestVecs = new List<PuzzleVector>();
        foreach(PuzzleType type in Enum.GetValues(typeof(PuzzleType)))
        {
            if (constrainType && t != type)
                continue;
            for(int d = 0; d <= maxDepth; d++)
            {
                if(depth != -1 && d != depth)
                    continue;
                for(int u = 0; u <= d; u++)
                {
                    if (depth != -1 && unknown != -1 && unknown != u)
                        continue;
                    float curDif = CalculateDifficulty(type, d, u);

                    //Based on the favored direction, might go on to next loop
                    if (favorDirection < 0 && curDif > difficulty)
                        continue;
                    if (favorDirection > 0 && curDif < difficulty)
                        continue;

                    //If we find a new closest, clear the list
                    if (curDif > 0 && Mathf.Abs(curDif - difficulty) < Mathf.Abs(closestDif - difficulty))
                    {
                        closestDif = curDif;
                        closestVecs.Clear();
                    }

                    //If this matches the difference of the closest, add it to the list
                    if (Mathf.Abs(curDif - difficulty) == Mathf.Abs(closestDif - difficulty))
                        closestVecs.Add(new PuzzleVector(type, d, u));
                }
            }
        }

        //If we ended up with no vectors, remove all constraints
        if (closestVecs.Count == 0)
            return CreatePuzzleVector(difficulty);

        //Return a random puzzle vector from the list
        return closestVecs[UnityEngine.Random.Range(0, closestVecs.Count)];
    }

    public static void GenAllPuzzleVectors()
    {
        allVecs.Clear();

        //Loop through each possibility, saving it
        foreach (PuzzleType type in Enum.GetValues(typeof(PuzzleType)))
        {
            for (int d = 0; d <= maxDepth; d++)
            {
                for (int u = 0; u <= d; u++)
                {
                    allVecs.Add(new PuzzleVector(type, d, u));
                }
            }
        }

        //Sort
        allVecs.Sort((a, b) => {
            float aDif = a.GetDifficulty();
            float bDif = b.GetDifficulty();
            if (aDif > bDif)
                return 1;
            else if(aDif < bDif)
                return -1;
            else
                return 0;
        }) ;
    }

    /**
     * Calculates the difficulty of a raw vector. -1 indicates a bad vector
     */
    private static float CalculateDifficulty(PuzzleType type, int depth, int unknown)
    {
        float sum = 0;

        //First, eliminate invalid input (unknown greater than depth)
        if (unknown > depth)
            return -1;

        //No errors, calculate the actual sum
        sum = TypeWeight(type) + (0.5f * depth) + (1.5f * unknown);

        return sum;
    }

    /**
     * Gives the weight of a given puzzle type
     */
    public static int TypeWeight(PuzzleType type)
    {
        switch(type)
        {
            case PuzzleType.DELETE:
                return 1;
            case PuzzleType.CREATE:
                return 4;
            default:
                return 8;
        }
    }

    /**
     * Gives the maximum difficulty of any puzzle
     */
    public static float GetMaxDifficulty()
    {
        return CalculateDifficulty(PuzzleType.MOVE, maxDepth, maxDepth);
    }

    /**
     * Constructor, makes a puzzle vector from the given array
     */
    private PuzzleVector(PuzzleType type, int depth, int unknown)
    {
        this.type = type;
        this.depth = depth;
        this.unknown = unknown;
    }

    /**
     * Generates the next puzzle based on performance and the current one
     */
    public PuzzleVector GenNextPuzzle(int errors, bool applyConstraint = false)
    {
        float stepSize = GetDifficulty() * 0.2f;
        float nextDif;

        //Need to calculate the new target difficulty
        //They made some legal number of errors
        if (errors < 3) {
            int steps = 3 - errors;
            nextDif = GetDifficulty() + (steps * stepSize);
        }

        //They failed the puzzle, take a step back
        else
            nextDif = GetDifficulty() - stepSize;

        //Actually generate the puzzle, constrain either the depth or the type
        int constrain = UnityEngine.Random.Range(0, 2);

        //Determine what direction to favor (lower if many mistakes, higher if few)
        int favorDirection;
        if (errors < 2)
            favorDirection = 1;
        else
            favorDirection = -1;

        //If we're not constraining, just return one
        if (!applyConstraint)
            return CreatePuzzleVector(nextDif, favorDirection: favorDirection);

        //Constraining the type
        if(constrain == 0)
        {
            return CreatePuzzleVector(nextDif, constrainType: true, t: GetPuzzleType(), favorDirection: favorDirection);
        }

        //Constraining the depth
        else
        {
            return CreatePuzzleVector(nextDif, depth: GetDepth(), favorDirection: favorDirection);
        }
    }

    /**
     * Gets the difficulty of the puzzle vector
     */
    public float GetDifficulty()
    {
        return CalculateDifficulty(type, depth, unknown);
    }

    /**
     * Gets the type of puzzle that this one represents
     */
    public PuzzleType GetPuzzleType()
    {
        return type;
    }

    /**
     * Gets the depth of the puzzle
     */
    public int GetDepth()
    {
        return depth;
    }

    /**
     * Gets the unknown path length
     */
    public int GetUnknownLen()
    {
        return unknown;
    }

    /**
     * Gets the string representation of the vector
     */
    public override string ToString()
    {
        return "Type: " + type + "\tD: " + depth + "\tU: " + unknown;
    }

    /**
     * Gets the vector as a fancier string
     */
    public string GetFancyString()
    {
        string retStr = "Difficulty: " + GetDifficulty() + "\n";
        retStr += "Depth: " + GetDepth() + "\n";
        retStr += "Unknown length: " + GetUnknownLen() + "\n";

        switch(GetPuzzleType())
        {
            case PuzzleType.DELETE:
                retStr += "Puzzle type: Deletion";
                break;
            case PuzzleType.CREATE:
                retStr += "Puzzle type: Creation";
                break;
            case PuzzleType.MOVE:
                retStr += "Puzzle type: Movement";
                break;
        }

        return retStr;
    }

    /**
     * Gets the vector in a CSV formatted string
     */
    public string GetCSVString()
    {
        return GetDifficulty().ToString() + "," + type.ToString() + "," + depth + "," + unknown;
    }
}
