/************************************************************************************

Copyright (c) Facebook Technologies, LLC and its affiliates. All rights reserved.  

See SampleFramework license.txt for license terms.  Unless required by applicable law 
or agreed to in writing, the sample code is provided 揂S IS?WITHOUT WARRANTIES OR 
CONDITIONS OF ANY KIND, either express or implied.  See the license for specific 
language governing permissions and limitations under the license.

************************************************************************************/
using UnityEngine;
using UnityEngine.EventSystems;

namespace Oculus
{
    public class HandedInputSelector : MonoBehaviour
    {
        public OVRCameraRig m_CameraRig;
        public OVRInputModule m_InputModule;

        void Update()
        {
            if (OVRInput.GetActiveController() == OVRInput.Controller.LTouch)
            {
                SetActiveController(OVRInput.Controller.LTouch);
            }
            else
            {
                SetActiveController(OVRInput.Controller.RTouch);
            }

        }

        void SetActiveController(OVRInput.Controller c)
        {
            Transform t;
            if (c == OVRInput.Controller.LTouch)
            {
                t = m_CameraRig.leftHandAnchor;
            }
            else
            {
                t = m_CameraRig.rightHandAnchor;
            }
            m_InputModule.rayTransform = t;
        }
    }
}
