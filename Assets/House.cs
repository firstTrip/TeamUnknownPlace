using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class House : MonoBehaviour
{

    public GameObject player;
    public GameObject Panel;
    public Transform wayPoint;
    public Transform LightWayPoint;


    public void ObjectAction()
    {
        Panel.GetComponent<Fade>().CallFade();
        LightManager.Instance.MainLight.transform.position = LightWayPoint.position;
        LightManager.Instance.Banish();
        player.transform.position = wayPoint.position;
    }


}
