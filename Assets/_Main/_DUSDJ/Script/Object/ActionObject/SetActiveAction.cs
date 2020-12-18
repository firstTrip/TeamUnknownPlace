using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class SetActiveAction : MonoBehaviour
{
    public bool TrueOrFalse = true;
    public GameObject[] Targets;


    private bool isUsed = false;

    public void ObjectAction()
    {
        if (isUsed)
        {
            return;
        }
        isUsed = true;

        for (int i = 0; i < Targets.Length; i++)
        {
            Targets[i].SetActive(TrueOrFalse);
        }
    }
}

