using UnityEngine;

namespace Quest
{
    public class GamepadCtrl : MonoBehaviour
    {
        public GameObject leftGamepad;
        public GameObject rightGamepad;

        public GameObject leftTransparentHand;
        public GameObject rightTransparentHand;

        public GameObject leftHand;
        public GameObject rightHand;

        public Renderer rightHandTriggerRender;
        public Renderer rightHandTeleportRender;

        public Renderer leftHandTriggerRender;
        public Renderer leftHandTeleportRender;

        /// <summary>
        /// 正常按键材质
        /// </summary>
        public Material normalKeyMat;
        /// <summary>
        /// 高亮按键材质
        /// </summary>
        public Material[] highLightKeyMats;


        void Start() 
        {
            ShowHand();
            ShowGamepad();
            //OpenTeleportBtnGuide();
            //OpenGrapBtnGuide();
        }


        public void ShowGamepad()
        {
            leftGamepad.SetActive(true);
            rightGamepad.SetActive(true);
        }

        public void HideGamepad() 
        {
            leftGamepad.SetActive(false);
            rightGamepad.SetActive(false);
        }

        [ContextMenu("CloseBtnGuide")]
        public void CloseBtnGuide() 
        {
            rightHandTriggerRender.sharedMaterial = normalKeyMat;
            rightHandTeleportRender.sharedMaterial = normalKeyMat;
            leftHandTriggerRender.sharedMaterial = normalKeyMat;
            leftHandTeleportRender.sharedMaterial = normalKeyMat;
        }

        [ContextMenu("OpenTeleportBtnGuide")]
        private void OpenTeleportBtnGuide() 
        {
            rightHandTeleportRender.materials = highLightKeyMats;
            leftHandTeleportRender.materials = highLightKeyMats;
        }

        [ContextMenu("OpenGrapBtnGuide")]
        private void OpenGrapBtnGuide()
        {
            rightHandTriggerRender.materials = highLightKeyMats;
            leftHandTriggerRender.materials = highLightKeyMats;
        }

        /// 手切换逻辑，透明手和不透明的手
        public void ShowTransparentHand() 
        {
            leftTransparentHand.SetActive(true);
            rightTransparentHand.SetActive(true);

            leftHand.SetActive(false);
            rightHand.SetActive(false);
        }

        public void ShowHand() 
        {
            leftTransparentHand.SetActive(false);
            rightTransparentHand.SetActive(false);

            leftHand.SetActive(true);
            rightHand.SetActive(true);
        }
    }
}
