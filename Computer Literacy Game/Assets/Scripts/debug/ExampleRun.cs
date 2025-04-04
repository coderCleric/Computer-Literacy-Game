using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ExampleRun : MonoBehaviour
{
    [SerializeField]
    private float baseConfidence = 0.8f;
    [SerializeField]
    private float confidenceGrowth = 0.05f;
    [SerializeField]
    private float typeChangeEffect = -0.3f;
    [SerializeField]
    private float depthEffect = -0.1f;
    [SerializeField]
    private float unknownEffect = -0.2f;
    [SerializeField]
    private int maxTests = 10;

    /**
     * Do the simulated test and return the result to the console
     */
    public void ConsoleTests()
    {
        List<string> testResults = new List<string>();

        //Generate the first vector
        PuzzleVector vec = PuzzleVector.CreatePuzzleVector(1);

        //Loop until we hit the max tests or dif
        List<PuzzleType> encounteredTypes = new List<PuzzleType>();
        encounteredTypes.Add(PuzzleType.DELETE);
        float startConfidence = baseConfidence;
        float prevSuccessfulDiff = 0;
        int tests = 0;
        while(prevSuccessfulDiff < PuzzleVector.GetMaxDifficulty() && tests < maxTests)
        {
            tests++;

            //Simulate attempts on the current puzzle
            int mistakes = 0;
            while(mistakes < 3)
            {
                float roll = Random.Range(0.0f, 1.0f);

                //Make the DC
                //Start with the positives
                float dc = startConfidence;
                dc += tests * confidenceGrowth;

                //If they haven't seen this type before, hit the base confidence
                if(!encounteredTypes.Contains(vec.GetPuzzleType()))
                {
                    Debug.Log("New type: " + vec.GetPuzzleType());
                    encounteredTypes.Add (vec.GetPuzzleType());
                    startConfidence += typeChangeEffect;
                }

                //Harder with more depth
                dc += vec.GetDepth() * depthEffect;

                //Even harder with more unknown
                dc += vec.GetUnknownLen() * unknownEffect;

                //If they rolled higher, that's a mistake
                if (roll > dc)
                    mistakes++;

                //Otherwise, they win
                else
                {
                    prevSuccessfulDiff = vec.GetDifficulty();
                    break;
                }
            }

            //Record the puzzle vector for later reporting
            testResults.Add("Dif: " + vec.GetDifficulty() + "\t" + vec.ToString() + "\tMis: " + mistakes);

            //Make the next puzzle vector
            vec = vec.GenNextPuzzle(mistakes);
        }

        //End of the loop, report to the console
        string ret = "";
        foreach(string str in testResults)
            ret += str + "\n";
        Debug.Log(ret);
    }

    /**
     * Do the simulated test and return the result to a file
     */
    public void CSVTests()
    {
        List<string> testResults = new List<string>();

        //Generate the first vector
        PuzzleVector vec = PuzzleVector.CreatePuzzleVector(1);

        //Loop until we hit the max tests or dif
        List<PuzzleType> encounteredTypes = new List<PuzzleType>();
        encounteredTypes.Add(PuzzleType.DELETE);
        float startConfidence = baseConfidence;
        float prevSuccessfulDiff = 0;
        int tests = 0;
        while (prevSuccessfulDiff < PuzzleVector.GetMaxDifficulty() && tests < maxTests)
        {
            tests++;

            //Simulate attempts on the current puzzle
            int mistakes = 0;
            while (mistakes < 3)
            {
                float roll = Random.Range(0.0f, 1.0f);

                //Make the DC
                //Start with the positives
                float dc = startConfidence;
                dc += tests * confidenceGrowth;

                //If they haven't seen this type before, hit the base confidence
                if (!encounteredTypes.Contains(vec.GetPuzzleType()))
                {
                    Debug.Log("New type: " + vec.GetPuzzleType());
                    encounteredTypes.Add(vec.GetPuzzleType());
                    startConfidence += typeChangeEffect;
                }

                //Harder with more depth
                dc += vec.GetDepth() * depthEffect;

                //Even harder with more unknown
                dc += vec.GetUnknownLen() * unknownEffect;

                //If they rolled higher, that's a mistake
                if (roll > dc)
                    mistakes++;

                //Otherwise, they win
                else
                {
                    prevSuccessfulDiff = vec.GetDifficulty();
                    break;
                }
            }

            //Record the puzzle vector for later reporting
            testResults.Add(vec.GetCSVString() + "," + mistakes);

            //Make the next puzzle vector
            vec = vec.GenNextPuzzle(mistakes);
        }

        //End of the loop, report to the file
        StreamWriter writer = new StreamWriter(Path.Combine(Application.dataPath, "testresult.csv"));
        writer.WriteLine("diff,type,depth,unknown,mistakes");
        foreach (string str in testResults)
            writer.WriteLine(str);
        writer.Close();
    }
}
