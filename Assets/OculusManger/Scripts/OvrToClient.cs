using UnityEngine;

namespace Oculus
{
    public class OvrToClient : MonoBehaviour
    {
        public static OvrToClient instance;
        public LocomotionTeleport locomotionTeleport;

        // Start is called before the first frame update
        void Awake()
        {
            instance = this;
        }
    }
}
