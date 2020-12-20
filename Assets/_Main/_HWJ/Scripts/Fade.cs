using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI
    ;

public class Fade : MonoBehaviour
{

    public Image Panel;
    float time = 0f;
    float F_time = 1f;
    // Start is called before the first frame update

    public void CallFade()
    {
        StartCoroutine(FadeFlow());
    }
    IEnumerator FadeFlow()
    {
        Panel.gameObject.SetActive(true);
        Color alpha = Panel.color;
        while(alpha.a < 1f)
        {
            alpha.a = Mathf.Lerp(0, 1, time);
            Panel.color = alpha;
            yield return null;
        }
        time = 0f;

        yield return new WaitForSeconds(1.5f);

        while (alpha.a > 0f)
        {
            time += Time.deltaTime / F_time;
            alpha.a = Mathf.Lerp(1, 0, time);
            Panel.color = alpha;
            yield return null;
        }


        yield return null;
    }

}
