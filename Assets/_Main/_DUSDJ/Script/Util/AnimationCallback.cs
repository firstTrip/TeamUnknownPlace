using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationCallback : MonoBehaviour
{
    public GameObject Target;

    public void CallBack(string msg)
    {
        Target.SendMessage(msg);
    }
}
