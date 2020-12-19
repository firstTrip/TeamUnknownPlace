using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class SetActiveAction : ActionObject
{
    public bool TrueOrFalse = true;
    public GameObject[] Targets;

    protected override void AfterCheckAction()
    {
        
        for (int i = 0; i < Targets.Length; i++)
        {
            Targets[i].SetActive(TrueOrFalse);
        }
    }
}

