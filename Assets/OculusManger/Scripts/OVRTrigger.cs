﻿using UnityEngine;

public class OVRTrigger : MonoBehaviour
{
    public OVRScreenFade ovrScreenFade;
    //层级
    public LayerMask layerMasks;

    private void OnTriggerEnter(Collider other)
    {
        int target = (int)Mathf.Pow(2, other.gameObject.layer);

        if ((layerMasks.value & target) == target)
        {
            ovrScreenFade.SetFadeLevel(1);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        int target = (int)Mathf.Pow(2, other.gameObject.layer);

        if ((layerMasks.value & target) == target)
        {
            ovrScreenFade.SetFadeLevel(0);
        }
    }
}
