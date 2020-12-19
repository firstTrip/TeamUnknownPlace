using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugManager : MonoBehaviour
{
    public static DebugManager Instance;

    public Text IsHuntingBox;
    public Text DistanceBox;
    public Text IsHitBlueLightBox;

    public Text IsArriveText;
    public Text TracingTimeText;
    public Text SoundFromText;
    public Text NowSoundNullText;
    public Text PrevTargetText;
    
    public Text StayTimeText;


    public Text PlayerHPText;
    public Text PlayerIsAliveText;
    public Text PlayerCanMoveText;

    public Text GameStateText;
    

    private void Awake()
    {
        Instance = this;
    }

    public void SetText(Text box, string content)
    {
        box.text = content;
    }

}
