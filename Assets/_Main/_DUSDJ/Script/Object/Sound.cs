using System;
using System.Collections.Generic;
using UnityEngine;

public class Sound
{
    public float Duration;
    public int Value;
    public Vector2 Position;

    public GameObject SoundFrom;

    public Sound(float duration, int value, Vector2 pos, GameObject from = null)
    {
        this.Duration = duration;
        this.Value = value;
        this.Position = pos;
        this.SoundFrom = from;
    }
}
