using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IDScreen : MonoBehaviour
{
    [SerializeField]
    private Text inputText;
    [SerializeField]
    private Text errorText;
    [SerializeField]
    private GameObject skipPrompt;

    /**
     * Submits what is currently in the input text
     */
    public void Submit()
    {
        //If there's no input, do the skip prompt
        if(inputText.text.Length == 0)
        {
            skipPrompt.SetActive(true);
            return;
        }

        //Try and parse the number
        int num;
        if(!int.TryParse(inputText.text, out num))
        {
            errorText.text = "Please enter a valid number!";
            errorText.gameObject.SetActive(true);
            return;
        }

        //Bound check
        if(num < 1)
        {
            errorText.text = "ID not in range!";
            errorText.gameObject.SetActive(true);
            return;
        }

        //It worked, give it to the DB manager
        DBManager.Initialize(num);

        //Send the new user ID to the DB
        StartCoroutine(DBManager.SendID(num));

        //Load the actual game
        SceneManager.LoadScene("Game");
    }

    /**
     * Cancels the skipping of the ID
     */
    public void CancelSkip()
    {
        skipPrompt.SetActive(false);
    }

    /**
     * Skips entering the ID
     */
    public void Skip()
    {
        DBManager.Initialize(0);
        SceneManager.LoadScene("Game");
    }
}
