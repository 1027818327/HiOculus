using UnityEngine;

namespace Quest
{
    public class OvrTeleportPoint : MonoBehaviour
    {
        public Transform destTransform;
        private Transform playerTrans;

        private void OnEnable()
        {
            OvrToClient.instance.locomotionTeleport.Teleported += Teleported;
        }

        private void OnDisable()
        {
            OvrToClient.instance.locomotionTeleport.Teleported -= Teleported;
        }

        void Teleported(Transform playTrans, Vector3 pos, Quaternion q) 
        {
            float distance = Mathf.Abs(Vector3.SqrMagnitude(destTransform.position) - Vector3.SqrMagnitude(pos));

            if (distance < 2f) 
            {
                LogManager.instance.DebugLog("传送距离差是" + distance + gameObject.name);
                LogManager.instance.DebugLog("目标点是" + destTransform.position);
                LogManager.instance.DebugLog("sdk传送点" + pos);

                playerTrans = playTrans;
                Invoke("DelaySetPos", 0.1f);
            }
        }

        private void DelaySetPos() 
        {
            playerTrans.rotation = destTransform.rotation;
        }
    }
}
