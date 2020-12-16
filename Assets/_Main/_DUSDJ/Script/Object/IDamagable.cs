using System;
using System.Collections.Generic;
using UnityEngine;

public interface IDamagable
{
    void Damage(float value);

    GameObject GetGameObject();
}
