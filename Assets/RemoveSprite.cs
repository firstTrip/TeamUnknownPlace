using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveSprite : MonoBehaviour
{
    private MeshRenderer mr;

    private void Awake()
    {
        GameObject go = transform.root.gameObject;
        mr = go.GetComponentInChildren<MeshRenderer>();
    }

    public void RemoveSpriteasd()
    {
        mr.enabled = false;
    }

    public void MakeSprite()
    {
        mr.enabled = true;
    }
}
