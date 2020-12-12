using System;
using System.Collections;
using UnityEngine;

public class DUSDJUtil
{
    public static IEnumerator ActionAfterSecondCoroutine(float duration, Action action)
    {
        float t = 0;

        while(t < duration)
        {
            t += Time.deltaTime;
            yield return null;

        }

        action();
    }
}
