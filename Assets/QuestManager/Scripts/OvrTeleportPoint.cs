using UnityEngine;

namespace Quest
{
    public class OvrTeleportPoint : MonoBehaviour
    {
        public Transform destTransform;
        private Transform palyerTrans;
        void Start()
        {
            if (OvrToClient.instance != null)
            {
                OvrToClient.instance.locomotionTeleport.Teleported -= Teleported;
                OvrToClient.instance.locomotionTeleport.Teleported += Teleported;
            }
        }

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
            float distance = Mathf.Abs(Vector3.SqrMagnitude(destTransform.position) - Vector3.SqrMagnitude(pos));
            //LogManager.instance.DebugLog("传送距离差2是" + distance);

            if (distance < 2f) 
            {
                LogManager.instance.DebugLog("传送距离差是" + distance);
                palyerTrans = playTrans;
                Invoke("DelaySetPos", 0.1f);
            }
        }

        private void DelaySetPos() 
        {
            palyerTrans.rotation = destTransform.rotation;
        }
    }
}
