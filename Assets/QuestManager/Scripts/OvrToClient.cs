using UnityEngine;

namespace Quest
{
    public class OvrToClient : MonoBehaviour
    {
        public static OvrToClient instance;
        public LocomotionTeleport locomotionTeleport;
        public GrabManager grabManager;

        // Start is called before the first frame update
        void Awake()
        {
            instance = this;
        }

        void Start() 
        {
#if UNITY_EDITOR
            gameObject.AddComponent<Common.MosueKeyboardUtils>();
#endif
        }
    }
}
