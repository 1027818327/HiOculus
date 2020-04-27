using ARdEZ.Render;
using UnityEngine;

namespace Common
{
    public class ButtonEffect : MonoBehaviour
    {
        public HoverButton mHoverBtn;
        public Outline mOutline; 

        void Start() 
        {
            if (mHoverBtn == null) 
            {
                mHoverBtn = GetComponent<HoverButton>();
            }
            mHoverBtn.onButtonEnter.AddListener(OnButtonEnter);
            mHoverBtn.onButtonExit.AddListener(OnButtonExit);
            mHoverBtn.onButtonDown.AddListener(OnButtonDown);
            mHoverBtn.onButtonUp.AddListener(OnButtonUp);

            if (mOutline == null) 
            {
                mOutline = GetComponent<Outline>();
            }
        }

        [ContextMenu("Set Component")]
        private void FindComponentInEditor() 
        {
            mHoverBtn = GetComponent<HoverButton>();
            mOutline = GetComponent<Outline>();
        }

        public void OnButtonEnter()
        {
            mOutline.OutlineColor = Color.yellow;
            mOutline.enabled = true;
        }

        public void OnButtonExit()
        {
            mOutline.enabled = false;
        }

        public void OnButtonUp()
        {
            mOutline.enabled = false;
            /*
            mOutline.OutlineColor = Color.yellow;
            mOutline.enabled = true;
            */
        }

        public void OnButtonDown()
        {
            mOutline.OutlineColor = Color.cyan;
            mOutline.enabled = true;
        }
    }
}
