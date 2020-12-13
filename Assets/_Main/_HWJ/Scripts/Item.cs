using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log(other.GetComponentInParent<Player>().GetItemGet());
            if (other.GetComponentInParent<Player>().GetItemGet())
            {
                gameObject.transform.SetParent(other.transform.GetChild(0));
            }

        }
    }
  
}
