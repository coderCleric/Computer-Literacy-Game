using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TutorialSuccessTrigger : MonoBehaviour 
{
    //Let's others listen for success
    public delegate void SuccessEvent();
    public event SuccessEvent OnSuccess;

    //Need to do this for inheritance
    protected void InvokeOnSuccess()
    {
        if(OnSuccess != null)
            OnSuccess();
    }
}
