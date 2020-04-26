using ARdEZ.Render;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Common 
{
    public class ButtonEffect : MonoBehaviour
    {
        private Outline mOutLine; 

        void Start() 
        {
            HoverButton hb = GetComponent<HoverButton>();
            hb.onButtonEnter.AddListener(OnButtonEnter);
            hb.onButtonExit.AddListener(OnButtonExit);
            hb.onButtonDown.AddListener(OnButtonDown);
            hb.onButtonUp.AddListener(OnButtonUp);

            mOutLine = GetComponent<Outline>();
        }


        public void OnButtonEnter()
        {
            mOutLine.OutlineColor = Color.yellow;
            mOutLine.enabled = true;
        }

        public void OnButtonExit()
        {
            mOutLine.enabled = false;
        }

        public void OnButtonUp()
        {
            mOutLine.enabled = false;
            /*
            mOutLine.OutlineColor = Color.yellow;
            mOutLine.enabled = true;
            */
        }

        public void OnButtonDown()
        {
            mOutLine.OutlineColor = Color.cyan;
            mOutLine.enabled = true;
        }
    }
}
