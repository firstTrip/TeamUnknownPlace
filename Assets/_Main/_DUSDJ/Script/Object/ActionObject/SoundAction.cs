using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class SoundAction : MonoBehaviour
{
    [Header("사운드 데이터")] public StructSoundData SoundData;

    private Vector3 TargetPosition;
    private bool SetTarget = false;

    private bool isUsed = false;


    private void Awake()
    {
        isUsed = false;
    }

    private void ObjectAction()
    {
        if (isUsed)
        {
            return;
        }

        isUsed = true;

        if (SetTarget)
        {
            AudioManager.Instance.PlaySound(SoundData.SoundKey, SoundData.SoundValue, TargetPosition, transform.root.gameObject);
        }
        else
        {
            AudioManager.Instance.PlaySound(SoundData.SoundKey, SoundData.SoundValue, transform.position, transform.root.gameObject);
        }

        
    }

    public void SetTargetPosition(Vector3 pos)
    {
        TargetPosition = pos;
        SetTarget = true;
    }


}
