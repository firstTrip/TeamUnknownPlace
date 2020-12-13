using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeLineCallback : MonoBehaviour
{
    [Header("타임라인 끝나고 발동할 함수")]
    public string Key;

    void Start()
    {
        SendMessageUpwards(Key);
    }
}
