using System;
using System.Collections.Generic;
using UnityEngine;

public class LightManager : MonoBehaviour
{
    #region SingleTon
    /* SingleTon */
    private static LightManager instance;
    public static LightManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType(typeof(LightManager)) as LightManager;
                if (!instance)
                {
                    GameObject container = new GameObject();
                    container.name = "LightManager";
                    instance = container.AddComponent(typeof(LightManager)) as LightManager;
                }
            }

            return instance;
        }
    }

    #endregion

    public delegate void LightCheckSoundDelegate(Vector3 source, int value);
    public LightCheckSoundDelegate LightCheckSound;


}
