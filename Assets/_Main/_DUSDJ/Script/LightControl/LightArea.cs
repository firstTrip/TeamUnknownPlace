using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class LightArea : MonoBehaviour
{
    public Light2D MainLight;
    public Transform LightPosition;

    public bool IsEnter = false;

    private Transform target;

    private void Update()
    {
        if (IsEnter)
        {
            Look(MainLight.transform, target.transform);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsEnter)
        {
            return;
        }

        if (collision.CompareTag("Player"))
        {
            Debug.Log("Enter");

            MainLight.transform.position = LightPosition.position;

            target = collision.transform;

            IsEnter = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!IsEnter)
        {
            return;
        }

        if (collision.CompareTag("Player"))
        {
            IsEnter = false;
        }
    }

    private void Look(Transform light, Transform target)
    {
        Vector3 ShootPoint = target.transform.position;

        float AngleRad = Mathf.Atan2(ShootPoint.y - light.position.y, ShootPoint.x - light.position.x);
        float AngleDeg = (180 / Mathf.PI) * AngleRad;
        light.rotation = Quaternion.Euler(0, 0, AngleDeg + 90);

    }

}
