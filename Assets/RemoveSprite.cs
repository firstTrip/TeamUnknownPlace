using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveSprite : MonoBehaviour
{
    public GameObject GO;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RemoveSpriteasd()
    {
        GO.GetComponent<MeshRenderer>().enabled = false;
    }

    public void MakeSprite()
    {
        GO.GetComponent<MeshRenderer>().enabled = true;
    }
}
