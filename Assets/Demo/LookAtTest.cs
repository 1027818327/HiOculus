using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtTest : MonoBehaviour
{
    public Transform trans;
    public bool isForward;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isForward)
        {
            transform.forward = trans.forward;
        }
        else 
        {
            transform.forward = -trans.forward;
        }
    }
}
