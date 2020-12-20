using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingManager : MonoBehaviour
{
    public float SkipTime = 3.0f;
    private float nowTime = 0f;

    private bool isEscape = false;

    private void Update()
    {
        if(nowTime < SkipTime)
        {
            nowTime += Time.deltaTime;
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape)
            || Input.GetKeyDown(KeyCode.Z)
            || Input.GetKeyDown(KeyCode.Space)
            || Input.GetKeyDown(KeyCode.Return))
        {
            Escape();
        }
    }


    public void Escape()
    {
        if (isEscape)
        {
            return;
        }

        isEscape = true;

        SceneManager.LoadSceneAsync(0);
    }
}
