﻿/************************************************************************************

See SampleFramework license.txt for license terms.  Unless required by applicable law 
or agreed to in writing, the sample code is provided “AS IS” WITHOUT WARRANTIES OR 
CONDITIONS OF ANY KIND, either express or implied.  See the license for specific 
language governing permissions and limitations under the license.

************************************************************************************/

using UnityEngine;

namespace Quest
{
    public class OculusTeleport : MonoBehaviour
    {
        [SerializeField]
        private LocomotionController lc;

        [SerializeField]
        private GameObject linePointer;
        private LocomotionTeleport locomotionTeleport;

        [SerializeField]
        private TeleportInputHandlerTouch teleportInputHandlerTouch;

        [SerializeField]
        private TeleportAimHandler teleportAimHandler;

        [SerializeField]
        private TeleportTargetHandlerNode teleportTargetHandler;

        private LocomotionTeleport TeleportController
        {
            get
            {
                return lc.GetComponent<LocomotionTeleport>();
            }
        }

        public void Start()
        {
            if (lc == null)
            {
                lc = FindObjectOfType<LocomotionController>();
            }

            SetupTwoStickTeleport();

            locomotionTeleport = lc.GetComponent<LocomotionTeleport>();
            locomotionTeleport.EnterStateAim += EnterStateAim;
            locomotionTeleport.ExitStateAim += ExitStateAim;


            // SAMPLE-ONLY HACK:
            // Due to restrictions on how Unity project settings work, we just hackily set up default
            // to ignore the water layer here. In your own project, you should set up your collision
            // layers properly through the Unity editor.
            Physics.IgnoreLayerCollision(0, 4);
        }

        private void OnDestroy()
        {

            locomotionTeleport.EnterStateAim -= EnterStateAim;
            locomotionTeleport.ExitStateAim -= ExitStateAim;

        }

        // Symmetrical controls. Forward or back on stick initiates teleport, then stick allows orient.
        // Snap turns allowed.
        void SetupTwoStickTeleport()
        {
            TeleportController.enabled = true;

            TeleportController.EnableRotation(true, false, false, true);
            TeleportController.EnableMovement(false, false, false, false);

            lc.PlayerController.RotationEitherThumbstick = true;

            TeleportInputHandlerTouch input = teleportInputHandlerTouch;
            input.InputMode = TeleportInputHandlerTouch.InputModes.ThumbstickTeleportForwardBackOnly;
            input.AimingController = OVRInput.Controller.Touch;
            input.AimButton = OVRInput.RawButton.RThumbstick;
            input.TeleportButton = OVRInput.RawButton.RThumbstick;
            input.CapacitiveAimAndTeleportButton = TeleportInputHandlerTouch.AimCapTouchButtons.A;
            input.FastTeleport = false;
            
            var orient = TeleportController.GetComponent<TeleportOrientationHandlerThumbstick>();
            orient.Thumbstick = OVRInput.Controller.Touch;
        }

        private void EnterStateAim()
        {
            linePointer.SetActive(false);
        }

        private void ExitStateAim()
        {
            linePointer.SetActive(true);
        }

        public void ControlTeleport(bool on) 
        {
            if (!on) 
            {
                TeleportController.CurrentIntention = LocomotionTeleport.TeleportIntentions.None;
                locomotionTeleport.OnUpdateAimData(new LocomotionTeleport.AimData());
                locomotionTeleport.OnUpdateTeleportDestination(false, null, null, null);
                teleportInputHandlerTouch.StopAllCoroutines();
                teleportTargetHandler.StopAllCoroutines();
            }
            teleportInputHandlerTouch.enabled = on;
            TeleportController.enabled = on;
            if (locomotionTeleport.AimHandler == null)
            {
                locomotionTeleport.AimHandler = teleportAimHandler;
            }

            enabled = on;
        }

        void Update() 
        {
            bool b = OVRInput.Get(OVRInput.Button.One) || OVRInput.Get(OVRInput.Button.Two) || OVRInput.Get(OVRInput.Button.Three) || OVRInput.Get(OVRInput.Button.Four) || OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger) || OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger) || OVRInput.Get(OVRInput.Button.Start);

            if (b)
            {
                TeleportController.CurrentIntention = LocomotionTeleport.TeleportIntentions.None;
                locomotionTeleport.OnUpdateAimData(new LocomotionTeleport.AimData());
                locomotionTeleport.OnUpdateTeleportDestination(false, null, null, null);
                teleportInputHandlerTouch.StopAllCoroutines();
                teleportTargetHandler.StopAllCoroutines();
            }
            
            teleportInputHandlerTouch.enabled = !b;
            TeleportController.enabled = !b;

            if (locomotionTeleport.AimHandler == null) 
            {
                locomotionTeleport.AimHandler = teleportAimHandler;
            }
        }
    }
}
