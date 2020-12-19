using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveSprite : MonoBehaviour
{
    public GameObject GO;

    public void RemoveSpriteasd()
    {
        GO.GetComponent<MeshRenderer>().enabled = false;
    }

    public void MakeSprite()
    {
        GO.GetComponent<MeshRenderer>().enabled = true;
    }
}
