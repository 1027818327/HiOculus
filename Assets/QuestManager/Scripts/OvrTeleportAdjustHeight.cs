using UnityEngine;

namespace Quest
{
    public class OvrTeleportAdjustHeight : MonoBehaviour
    {
        public Transform destTransform;
        private Transform playerTrans;

        public float minX = -1f;
        public float maxX = 1f;
        public float minZ = -1f;
        public float maxZ = 1f;

        public float adjustHeight = 0;

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
            if (pos.x >= destTransform.position.x + minX && pos.x <= destTransform.position.x + maxX && pos.z >= destTransform.position.z + minZ && pos.z <= destTransform.position.z + maxZ && pos.y - destTransform.position.y >= 0.99f && pos.y - destTransform.position.y <= 1.01f) 
            {
                //LogManager.instance.DebugLog("目标点是" + destTransform.position);
                //LogManager.instance.DebugLog("sdk传送点" + pos);
                playerTrans = playTrans;
                Invoke("DelaySetPos", 0.1f);
            }
        }

        private void DelaySetPos()
        {
            playerTrans.position += adjustHeight * Vector3.up;
        }
    }
}
