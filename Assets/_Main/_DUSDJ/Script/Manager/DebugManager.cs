using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugManager : MonoBehaviour
{
    public static DebugManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public Text IsHuntingBox;
    public Text DistanceBox;
    public Text IsHitBlueLightBox;
    
    public void SetText(Text box, string content)
    {
        box.text = content;
    }

}
