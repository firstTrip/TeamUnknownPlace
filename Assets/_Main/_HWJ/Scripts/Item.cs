using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{

    public bool isGet;
    // Start is called before the first frame update
    void Start()
    {
        isGet = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!isGet)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                Debug.Log(other.GetComponentInParent<Player>().GetItemGet());
                if (other.GetComponentInParent<Player>().GetItemGet())
                {
                    isGet = true;
                    gameObject.transform.SetParent(other.transform.GetChild(0));

                }

            }

        }
        else return;

        
    }
  
}
