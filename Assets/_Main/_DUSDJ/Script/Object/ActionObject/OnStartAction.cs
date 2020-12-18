using System;
using UnityEngine;

public class OnStartAction : MonoBehaviour
{
    public string Message;
    public GameObject Target;
    
    private void Start()
    {
        Target.SendMessage(Message);
    }


}
