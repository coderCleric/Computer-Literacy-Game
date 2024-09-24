using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GreyoutPath : Animateable
{
    [SerializeField]
    private TutorialPage page;
    [SerializeField]
    private TreeAnt ant;
    [SerializeField]
    private Text textObj;
    private string[] names;
    private int greyIndex = -1;
    private string origText;

    /**
     * On start, do some init stuff
     */
    private void Awake()
    {
        origText = textObj.text;
        names = page.AntPath.Split('/');
        textObj.text = string.Join("/", names);
        textObj.text = origText + textObj.text;
        ant.OnArriveAtNode += IncrementGreyout;
    }

    /**
     * Starts the greyout animation
     */
    public override void StartAnimation()
    {
        textObj.text = string.Join("/", names);
        textObj.text = origText + textObj.text;
        greyIndex = -1;
    }

    /**
     * Increments the greyout
     */
    public void IncrementGreyout()
    {
        greyIndex++;

        //Build the grey part
        string text = "<color=lime>";
        bool isFirst = true;
        for(int i = 0; i <= greyIndex; i++)
        {
            if (!isFirst) //First name has no slash before
                text += "/";
            text += names[i];
            isFirst = false;
        }
        text += "</color>";

        //Build the non-grey part
        for(int i = greyIndex + 1; i < names.Length; i++)
        {
            if (!isFirst) //First name has no slash before
                text += "/";
            text += names[i];
            isFirst = false;
        }
        text = origText + text;

        textObj.text = text;
    }

    /**
     * Unlink from the ant if deleted
     */
    private void OnDestroy()
    {
        ant.OnArriveAtNode -= IncrementGreyout;
    }
}
