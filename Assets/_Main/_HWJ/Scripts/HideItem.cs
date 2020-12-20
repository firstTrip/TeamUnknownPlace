using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideItem : Item
{
    private IEnumerator disableCoroutine;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        UseMaterial();
    }

    public override void UseItem()
    {

    }

    public override void UseMaterial()
    {
        base.UseMaterial();
    }

    private void OnTriggerStay2D(Collider2D other)
    {

    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponentInParent<Player>().HideExit();
        }
    }



}
