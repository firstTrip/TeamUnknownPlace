using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueLightArea : MonoBehaviour
{
    public bool IsActivated = false;    
    public bool IsPassed = false;

    private CollisionTrigger BlueLightTrigger;
    private Transform BlueLightPoint;

    private Collider2D col;

    private void Awake()
    {
        col = GetComponent<Collider2D>();

        BlueLightTrigger = transform.Find("BlueLightPoint").GetComponent<CollisionTrigger>();
        BlueLightPoint = transform.Find("BlueLightPoint");
    }


    public void ChildTriggerEnter()
    {
        if (IsPassed)
        {
            return;
        }

        LightManager.Instance.SetBlueLight(BlueLightPoint.position);
        col.enabled = true;
    }

    public void ChildTriggerExit()
    {
        if (IsPassed)
        {
            return;
        }

        if (IsActivated)
        {
            LightManager.Instance.CloseBlueLight();

            col.enabled = false;
            IsPassed = true;
            Debug.Log(string.Format("col.enabled : {0}", col.enabled));
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            IsActivated = true;
        }
    }
}
