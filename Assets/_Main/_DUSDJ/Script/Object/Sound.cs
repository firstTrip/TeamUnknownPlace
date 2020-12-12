using System;
using System.Collections.Generic;
using UnityEngine;

public class Sound
{
    public float Duration;
    public int Value;
    public Vector3 Position;

    public Sound(float duration, int value, Vector3 pos)
    {
        this.Duration = duration;
        this.Value = value;
        this.Position = pos;
    }
}
