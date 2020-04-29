using UnityEngine;

namespace Quest
{
    public class OVRTrigger : MonoBehaviour
    {
        public OVRScreenFade ovrScreenFade;
        //层级
        public LayerMask layerMasks;

        public Collider mCharacterCollider;

        private void OnTriggerEnter(Collider other)
        {
            if (other == mCharacterCollider)
            {
                // 忽略自身
                return;
            }

            int target = (int)Mathf.Pow(2, other.gameObject.layer);

            if ((layerMasks.value & target) == target)
            {
                Debug.Log("头碰撞到了" + other.name);

                ovrScreenFade.SetFadeLevel(1);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other == mCharacterCollider)
            {
                // 忽略自身
                return;
            }

            int target = (int)Mathf.Pow(2, other.gameObject.layer);

            if ((layerMasks.value & target) == target)
            {
                ovrScreenFade.SetFadeLevel(0);
            }
        }
    }
}
