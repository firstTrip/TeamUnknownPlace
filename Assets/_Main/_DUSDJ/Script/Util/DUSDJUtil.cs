using System;
using System.Collections;
using System.Text;
using TMPro;
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

    public static IEnumerator SetTextWithDelay(TextMeshProUGUI box, string content, float delay, Action afterAction = null)
    {
        float t = 0;
        int index = 0;

        StringBuilder sb = new StringBuilder();
        sb.Append(content[index]);
        box.text = sb.ToString();

        while (index < content.Length)
        {
            if(t >= delay)
            {
                t -= delay;

                index += 1;
                sb.Append(content[index]);
                box.text = sb.ToString();
            }
            else
            {
                t += Time.deltaTime;
            }

            yield return null;
        }

        afterAction?.Invoke();
    }
}
