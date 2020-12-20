using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class House : MonoBehaviour
{

    public GameObject player;
    public Transform wayPoint;


    public void ObjectAction()
    {
        player.transform.position = wayPoint.position;
    }


}
