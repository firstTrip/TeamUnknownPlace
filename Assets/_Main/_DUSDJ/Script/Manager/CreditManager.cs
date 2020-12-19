using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditManager : MonoBehaviour
{
    public GameObject TransitionObject;

    private bool isEscape = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)
            || Input.GetKeyDown(KeyCode.Z)
            || Input.GetKeyDown(KeyCode.Space)
            || Input.GetKeyDown(KeyCode.Return))
        {
            EscapeCredit();
        }
    }


    public void EscapeCredit()
    {
        if (isEscape)
        {
            return;
        }

        isEscape = true;
        
        TransitionObject.SetActive(true);

        SceneManager.LoadSceneAsync(0);
    }
}
