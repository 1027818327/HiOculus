using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OVRTrigger : MonoBehaviour
{
    //by zrp
    //public GameObject eyeObject;
    OVRScreenFade ovrScreenFade;
    //层级
    public LayerMask layerMasks;
    private void Awake()
    {
        //ovrScreenFade = eyeObject.GetComponent<OVRScreenFade>();
        ovrScreenFade = GetComponent<OVRScreenFade>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((layerMasks.value & (int)Mathf.Pow(2, other.gameObject.layer)) == (int)Mathf.Pow(2, other.gameObject.layer))
        {
            ovrScreenFade.SetFadeLevel(1);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if ((layerMasks.value & (int)Mathf.Pow(2, other.gameObject.layer)) == (int)Mathf.Pow(2, other.gameObject.layer))
        {
            ovrScreenFade.SetFadeLevel(0);
        }
    }
}
