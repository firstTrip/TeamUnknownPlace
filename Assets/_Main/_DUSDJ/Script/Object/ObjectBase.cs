using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class ObjectBase : MonoBehaviour
{
    #region Struct

    [System.Serializable]
    public struct StructSoundData
    {
        [Header("사운드 키")] public string SoundKey;
        [Header("사운드웨이브 크기")] public float SoundWaveScale;
        [Header("소리 밸류")] public int SoundValue;
    }

    [System.Serializable]
    public struct StructSoundPoint
    {
        public float Timings;
        public Vector3 PositionAdder;
    }

    #endregion

    [Header("Sound Data")] public StructSoundData SoundData;
    protected Vector3 SoundWaveScale;

    protected virtual void Awake()
    {
        Init();    
        
    }

    public virtual void Init()
    {
        SoundWaveScale = new Vector3(SoundData.SoundWaveScale, SoundData.SoundWaveScale, 1.0f);
    }

    public abstract void ObjectAction();

    protected abstract void OnTriggerEnter2D(Collider2D collision);

    protected abstract void OnTriggerExit2D(Collider2D collision);
}
