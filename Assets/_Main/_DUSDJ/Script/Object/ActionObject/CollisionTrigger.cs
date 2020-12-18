using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionTrigger : MonoBehaviour
{
    public string Tag = "Player";
    public string FunctionEnter = "ChildTriggerEnter";
    public string FunctionExit = "ChildTriggerExit";

    public GameObject[] SendMessageTarget;



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (string.IsNullOrEmpty(FunctionEnter))
        {
            return;
        }

        if (collision.CompareTag(Tag))
        {
            if(SendMessageTarget.Length > 0)
            {
                for (int i = 0; i < SendMessageTarget.Length; i++)
                {
                    SendMessageTarget[i].SendMessage(FunctionEnter, collision);
                }
            }
            else
            {
                SendMessageUpwards(FunctionEnter, collision);
            }
        }
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (string.IsNullOrEmpty(FunctionExit))
        {
            return;
        }

        if (collision.CompareTag(Tag))
        {
            if (SendMessageTarget.Length > 0)
            {
                for (int i = 0; i < SendMessageTarget.Length; i++)
                {
                    SendMessageTarget[i].SendMessage(FunctionExit, collision);
                }
            }
            else
            {
                SendMessageUpwards(FunctionExit, collision);
            }
        }

    }
}
