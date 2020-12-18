using UnityEngine;

[System.Serializable]
public struct StructSoundData
{
    [Header("사운드 키")] public string SoundKey;
    [Header("소리발생 쿨다운")] public float ActionCoolDown;
    [Header("소리 밸류")] public int SoundValue;
}
