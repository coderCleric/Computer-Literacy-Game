using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialQuiz : TutorialSuccessTrigger
{
    [SerializeField]
    private string ans;
    [SerializeField]
    private InputField input;
    [SerializeField]
    private Text correctText;
    [SerializeField]
    private AudioSource oopsAudio;
    private static Color WRONG_COLOR = Color.red;
    private static Color RIGHT_COLOR = Color.green;

    /**
     * Tries to submit
     */
    public void Submit()
    {
        if (input.text.Equals(ans))
        {
            Debug.Log("Player was right");
            correctText.text = "Correct!";
            correctText.color = RIGHT_COLOR;
            InvokeOnSuccess();
        }
        else
        {
            Debug.Log("Player was wrong");
            correctText.text = "Incorrect!";
            correctText.color = WRONG_COLOR;
            oopsAudio.Play();
        }
    }
}
