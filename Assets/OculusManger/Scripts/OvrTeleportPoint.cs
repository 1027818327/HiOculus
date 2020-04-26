using UnityEngine;

namespace Oculus
{
    public class OvrTeleportPoint : MonoBehaviour
    {
        [SerializeField]
        private TeleportPoint point;

        private Transform palyerTrans;
        void Start()
        {
            if (OvrToClient.instance != null)
            {
                OvrToClient.instance.locomotionTeleport.Teleported -= Teleported;
                OvrToClient.instance.locomotionTeleport.Teleported += Teleported;
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            point = GetComponent<TeleportPoint>();
        }
#endif
        private void OnEnable()
        {
            if (OvrToClient.instance != null) 
            {
                OvrToClient.instance.locomotionTeleport.Teleported += Teleported;
            }
        }

        private void OnDisable()
        {
            OvrToClient.instance.locomotionTeleport.Teleported -= Teleported;
        }

        void Teleported(Transform playTrans, Vector3 pos, Quaternion q) 
        {
            float distance = Mathf.Abs(Vector3.SqrMagnitude(point.destTransform.position) - Vector3.SqrMagnitude(pos));
            if (distance <= 1f) 
            {
                LogManager.instance.DebugLog("传送距离差是" + distance);
                palyerTrans = playTrans;
                Invoke("DelaySetPos", 0.1f);
            }
        }

        private void DelaySetPos() 
        {
            palyerTrans.rotation = point.destTransform.rotation;
        }
    }
}
