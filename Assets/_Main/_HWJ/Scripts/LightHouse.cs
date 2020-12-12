using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightHouse : MonoBehaviour
{

    [SerializeField] private GameObject player = null;
    [SerializeField] private Transform topPos = null;

    private LayerMask layerMask;
    private float distance;

    private float angle;

    // Start is called before the first frame update
    void Start()
    {
        distance = Vector2.Distance(player.transform.position, this.transform.position);
        Debug.Log(distance);
        layerMask = 1 << LayerMask.NameToLayer("Floor");

    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(topPos.position, Vector2.down,layerMask);
        
        if (hit.collider != null)
        {
            distance = Vector2.Distance(player.transform.position, this.transform.position);
            float height = topPos.position.y - hit.collider.transform.position.y;

            Debug.Log(height);
            angle = Mathf.Atan2(Mathf.Abs(height), distance) * Mathf.Rad2Deg;

            Debug.Log(angle);

            topPos.rotation = Quaternion.Euler(0, 0,angle);

        }
    }
}
