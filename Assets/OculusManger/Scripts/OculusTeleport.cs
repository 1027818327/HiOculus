﻿/************************************************************************************

See SampleFramework license.txt for license terms.  Unless required by applicable law 
or agreed to in writing, the sample code is provided “AS IS” WITHOUT WARRANTIES OR 
CONDITIONS OF ANY KIND, either express or implied.  See the license for specific 
language governing permissions and limitations under the license.

************************************************************************************/

#define DEBUG_LOCOMOTION_PANEL

using System.Diagnostics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

namespace Oculus 
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

        [Conditional("DEBUG_LOCOMOTION_PANEL")]
        static void Log(string msg)
        {
            Debug.Log(msg);
        }

        /// <summary>
        /// This method will ensure only one specific type TActivate in a given group of components derived from the same TCategory type is enabled.
        /// This is used by the sample support code to select between different targeting, input, aim, and other handlers.
        /// </summary>
        /// <typeparam name="TCategory"></typeparam>
        /// <typeparam name="TActivate"></typeparam>
        /// <param name="target"></param>
        public static TActivate ActivateCategory<TCategory, TActivate>(GameObject target) where TCategory : MonoBehaviour where TActivate : MonoBehaviour
        {
            var components = target.GetComponents<TCategory>();
            Log("Activate " + typeof(TActivate) + " derived from " + typeof(TCategory) + "[" + components.Length + "]");
            TActivate result = null;
            for (int i = 0; i < components.Length; i++)
            {
                var c = (MonoBehaviour)components[i];
                var active = c.GetType() == typeof(TActivate);
                Log(c.GetType() + " is " + typeof(TActivate) + " = " + active);
                if (active)
                {
                    result = (TActivate)c;
                }
                if (c.enabled != active)
                {
                    c.enabled = active;
                }
            }
            return result;
        }

        /// <summary>
        /// This generic method is used for activating a specific set of components in the LocomotionController. This is just one way 
        /// to achieve the goal of enabling one component of each category (input, aim, target, orientation and transition) that
        /// the teleport system requires.
        /// </summary>
        /// <typeparam name="TInput"></typeparam>
        /// <typeparam name="TAim"></typeparam>
        /// <typeparam name="TTarget"></typeparam>
        /// <typeparam name="TOrientation"></typeparam>
        /// <typeparam name="TTransition"></typeparam>
        protected void ActivateHandlers<TInput, TAim, TTarget, TOrientation, TTransition>()
            where TInput : TeleportInputHandler
            where TAim : TeleportAimHandler
            where TTarget : TeleportTargetHandler
            where TOrientation : TeleportOrientationHandler
            where TTransition : TeleportTransition
        {
            ActivateInput<TInput>();
            ActivateAim<TAim>();
            ActivateTarget<TTarget>();
            ActivateOrientation<TOrientation>();
            ActivateTransition<TTransition>();
        }

        protected void ActivateInput<TActivate>() where TActivate : TeleportInputHandler
        {
            ActivateCategory<TeleportInputHandler, TActivate>();
        }

        protected void ActivateAim<TActivate>() where TActivate : TeleportAimHandler
        {
            ActivateCategory<TeleportAimHandler, TActivate>();
        }

        protected void ActivateTarget<TActivate>() where TActivate : TeleportTargetHandler
        {
            ActivateCategory<TeleportTargetHandler, TActivate>();
        }

        protected void ActivateOrientation<TActivate>() where TActivate : TeleportOrientationHandler
        {
            ActivateCategory<TeleportOrientationHandler, TActivate>();
        }

        protected void ActivateTransition<TActivate>() where TActivate : TeleportTransition
        {
            ActivateCategory<TeleportTransition, TActivate>();
        }

        protected TActivate ActivateCategory<TCategory, TActivate>() where TCategory : MonoBehaviour where TActivate : MonoBehaviour
        {
            return ActivateCategory<TCategory, TActivate>(lc.gameObject);
        }

        protected void UpdateToggle(Toggle toggle, bool enabled)
        {
            if (enabled != toggle.isOn)
            {
                toggle.isOn = enabled;
            }
        }

        void SetupNonCap()
        {
            var input = TeleportController.GetComponent<TeleportInputHandlerTouch>();
            input.InputMode = TeleportInputHandlerTouch.InputModes.SeparateButtonsForAimAndTeleport;
            input.AimButton = OVRInput.RawButton.A;
            input.TeleportButton = OVRInput.RawButton.A;
        }

        void SetupTeleportDefaults()
        {
            TeleportController.enabled = true;
            //lc.PlayerController.SnapRotation = true;
            lc.PlayerController.RotationEitherThumbstick = false;
            //lc.PlayerController.FixedSpeedSteps = 0;
            TeleportController.EnableMovement(false, false, false, false);
            TeleportController.EnableRotation(false, false, false, false);

            var input = TeleportController.GetComponent<TeleportInputHandlerTouch>();
            input.InputMode = TeleportInputHandlerTouch.InputModes.CapacitiveButtonForAimAndTeleport;
            input.AimButton = OVRInput.RawButton.A;
            input.TeleportButton = OVRInput.RawButton.A;
            input.CapacitiveAimAndTeleportButton = TeleportInputHandlerTouch.AimCapTouchButtons.A;
            input.FastTeleport = false;

            var hmd = TeleportController.GetComponent<TeleportInputHandlerHMD>();
            hmd.AimButton = OVRInput.RawButton.A;
            hmd.TeleportButton = OVRInput.RawButton.A;

            var orient = TeleportController.GetComponent<TeleportOrientationHandlerThumbstick>();
            orient.Thumbstick = OVRInput.Controller.LTouch;
        }


        protected GameObject AddInstance(GameObject template, string label)
        {
            var go = Instantiate(template);
            go.transform.SetParent(transform, false);
            go.name = label;
            return go;
        }

        // Teleport between node with A buttons. Display laser to node. Allow snap turns.
        void SetupNodeTeleport()
        {
            SetupTeleportDefaults();
            SetupNonCap();
            //lc.PlayerController.SnapRotation = true;
            //lc.PlayerController.FixedSpeedSteps = 1;
            lc.PlayerController.RotationEitherThumbstick = true;
            TeleportController.EnableRotation(true, false, false, true);
            ActivateHandlers<TeleportInputHandlerTouch, TeleportAimHandlerLaser, TeleportTargetHandlerNode, TeleportOrientationHandlerThumbstick, TeleportTransitionBlink>();
            var input = TeleportController.GetComponent<TeleportInputHandlerTouch>();
            input.AimingController = OVRInput.Controller.RTouch;
            //var input = TeleportController.GetComponent<TeleportAimHandlerLaser>();
            //input.AimingController = OVRInput.Controller.RTouch;
        }

        // Symmetrical controls. Forward or back on stick initiates teleport, then stick allows orient.
        // Snap turns allowed.
        void SetupTwoStickTeleport()
        {
            SetupTeleportDefaults();
            TeleportController.EnableRotation(false, false, false, false);
            TeleportController.EnableMovement(false, false, false, false);
            //lc.PlayerController.SnapRotation = true;
            lc.PlayerController.RotationEitherThumbstick = false;
            //lc.PlayerController.FixedSpeedSteps = 1;

            var input = TeleportController.GetComponent<TeleportInputHandlerTouch>();
            input.InputMode = TeleportInputHandlerTouch.InputModes.ThumbstickTeleportForwardBackOnly;
            input.AimingController = OVRInput.Controller.Touch;
            ActivateHandlers<TeleportInputHandlerTouch, TeleportAimHandlerParabolic, TeleportTargetHandlerPhysical, TeleportOrientationHandlerThumbstick, TeleportTransitionBlink>();
            var orient = TeleportController.GetComponent<TeleportOrientationHandlerThumbstick>();
            orient.Thumbstick = OVRInput.Controller.Touch;
        }

        /*
        void SetupRightTurnLeftTeleport()
        {
            SetupTeleportDefaults();
            TeleportController.EnableRotation(true, false, false, false);
            TeleportController.EnableMovement(false, false, false, false);
            lc.PlayerController.SnapRotation = true;
            lc.PlayerController.FixedSpeedSteps = 1;

            var input = TeleportController.GetComponent<TeleportInputHandlerTouch>();
            input.InputMode = TeleportInputHandlerTouch.InputModes.ThumbstickTeleport;
            input.AimingController = OVRInput.Controller.LTouch;

            ActivateHandlers<TeleportInputHandlerTouch, TeleportAimHandlerParabolic, TeleportTargetHandlerPhysical, TeleportOrientationHandlerThumbstick, TeleportTransitionBlink>();
            var orient = TeleportController.GetComponent<TeleportOrientationHandlerThumbstick>();
            orient.Thumbstick = OVRInput.Controller.LTouch;
        }
        */

        // Shut down teleport. Basically reverts to OVRPlayerController.
        void SetupWalkOnly()
        {
            SetupTeleportDefaults();
            TeleportController.enabled = false;
            lc.PlayerController.EnableLinearMovement = true;
            //lc.PlayerController.SnapRotation = true;
            lc.PlayerController.RotationEitherThumbstick = false;
            //lc.PlayerController.FixedSpeedSteps = 1;
        }

        // 
        void SetupLeftStrafeRightTeleport()
        {
            SetupTeleportDefaults();
            TeleportController.EnableRotation(true, false, false, true);
            TeleportController.EnableMovement(true, false, false, false);
            //lc.PlayerController.SnapRotation = true;
            //lc.PlayerController.FixedSpeedSteps = 1;

            var input = TeleportController.GetComponent<TeleportInputHandlerTouch>();
            input.InputMode = TeleportInputHandlerTouch.InputModes.ThumbstickTeleportForwardBackOnly;
            input.AimingController = OVRInput.Controller.RTouch;
            ActivateHandlers<TeleportInputHandlerTouch, TeleportAimHandlerParabolic, TeleportTargetHandlerPhysical, TeleportOrientationHandlerThumbstick, TeleportTransitionBlink>();
            var orient = TeleportController.GetComponent<TeleportOrientationHandlerThumbstick>();
            orient.Thumbstick = OVRInput.Controller.RTouch;
        }

        private void EnterStateAim()
        {
            linePointer.SetActive(false);
        }

        private void ExitStateAim()
        {
            linePointer.SetActive(true);
        }

        void Update() 
        {
            bool b = OVRInput.Get(OVRInput.Button.One) || OVRInput.Get(OVRInput.Button.Two) || OVRInput.Get(OVRInput.Button.Three) || OVRInput.Get(OVRInput.Button.Four) || OVRInput.Get(OVRInput.Button.PrimaryHandTrigger) || OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger) || OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger) || OVRInput.Get(OVRInput.Button.SecondaryHandTrigger) || OVRInput.Get(OVRInput.Button.Start);
            teleportInputHandlerTouch.enabled = !b;
            TeleportController.enabled = !b;
            if (b)
            {
                TeleportController.ForceCancleTeleport();
            }
        }
    }
}
