using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SetTargetAction : MonoBehaviour
{
    public GameObject[] SetTargets;
    public GameObject[] Callbacks;

    public void ObjectAction(Collider2D collision)
    {
        Vector3 pos = collision.transform.position;

        for (int i = 0; i < SetTargets.Length; i++)
        {
            SetTargets[i].SendMessage("SetTargetPosition", pos);
        }

        // Callback
        for (int i = 0; i < Callbacks.Length; i++)
        {
            Callbacks[i].SendMessage("ObjectAction", collision);
        }
    }
}
