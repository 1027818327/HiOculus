using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public GameObject go;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
    }

    [ContextMenu("CloseCol")]
    public void CloseCol() 
    {
        Collider tempCollider = go.GetComponent<Collider>();
        tempCollider.ClosestPointOnBounds(transform.GetChild(0).position);
    }

    private void CloseSelftCol() 
    {
        Collider tempCollider = transform.GetChild(0).GetComponent<Collider>();
        tempCollider.ClosestPointOnBounds(transform.GetChild(0).position);
    }

    private void Update()
    {
        CloseCol();
        CloseSelftCol();
    }
}
