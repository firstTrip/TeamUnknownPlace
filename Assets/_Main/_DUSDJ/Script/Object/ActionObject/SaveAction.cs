using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SaveAction : MonoBehaviour
{
    private bool isUsed = false;

    public void ObjectAction()
    {
        if (isUsed)
        {
            return;
        }
        isUsed = true;

        SaveManager.Instance.SaveAll();
    }
}
