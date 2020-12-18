using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionTrigger : MonoBehaviour
{
    public string Tag = "Player";
    public string FunctionEnter = "ChildTriggerEnter";
    public string FunctionExit = "ChildTriggerExit";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (string.IsNullOrEmpty(FunctionEnter))
        {
            return;
        }

        if (collision.CompareTag(Tag))
        {
            SendMessageUpwards(FunctionEnter);
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
            SendMessageUpwards(FunctionExit);
        }
        
    }
}
