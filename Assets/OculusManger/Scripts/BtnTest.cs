using Oculus;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Oculus 
{
    public class BtnTest : MonoBehaviour
    {
        public Text mText;
        public StringBuilder mBuilder = new StringBuilder();

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            /*
            mBuilder.Length = 0;

            if (OVRInput.Get(OVRInput.Button.One)) 
            {
                mBuilder.Append("One 按下 ");
            }

            if (OVRInput.Get(OVRInput.Button.Two))
            {
                mBuilder.Append("Two 按下 ");
            }

            if (OVRInput.Get(OVRInput.Button.Three))
            {
                mBuilder.Append("Three 按下 ");
            }

            if (OVRInput.Get(OVRInput.Button.Four))
            {
                mBuilder.Append("Four 按下 ");
            }

            if (OVRInput.Get(OVRInput.Button.PrimaryHandTrigger))
            {
                mBuilder.Append("PrimaryHandTrigger 按下 ");
            }

            if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger))
            {
                mBuilder.Append("PrimaryIndexTrigger 按下 ");
            }

            if (OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger))
            {
                mBuilder.Append("SecondaryIndexTrigger 按下 ");
            }

            if (OVRInput.Get(OVRInput.Button.SecondaryHandTrigger))
            {
                mBuilder.Append("SecondaryHandTrigger 按下 ");
            }

            if (OVRInput.Get(OVRInput.Button.SecondaryShoulder))
            {
                mBuilder.Append("SecondaryShoulder 按下 ");
            }

            if (OVRInput.Get(OVRInput.Button.SecondaryThumbstick))
            {
                mBuilder.Append("SecondaryThumbstick 按下 ");
            }

            if (OVRInput.Get(OVRInput.Button.SecondaryThumbstickUp))
            {
                mBuilder.Append("SecondaryThumbstickUp 按下 ");
            }

            if (OVRInput.Get(OVRInput.Button.SecondaryThumbstickDown))
            {
                mBuilder.Append("SecondaryThumbstickDown 按下 ");
            }

            if (OVRInput.Get(OVRInput.Button.SecondaryThumbstickLeft))
            {
                mBuilder.Append("SecondaryThumbstickLeft 按下 ");
            }

            if (OVRInput.Get(OVRInput.Button.SecondaryThumbstickRight))
            {
                mBuilder.Append("SecondaryThumbstickRight 按下 ");
            }

            if (OVRInput.Get(OVRInput.Button.SecondaryTouchpad))
            {
                mBuilder.Append("SecondaryTouchpad 按下 ");
            }

            if (OVRInput.Get(OVRInput.Button.DpadUp))
            {
                mBuilder.Append("DpadUp 按下 ");
            }

            if (OVRInput.Get(OVRInput.Button.DpadDown))
            {
                mBuilder.Append("DpadDown 按下 ");
            }

            if (OVRInput.Get(OVRInput.Button.DpadLeft))
            {
                mBuilder.Append("DpadLeft 按下 ");
            }

            if (OVRInput.Get(OVRInput.Button.DpadRight))
            {
                mBuilder.Append("DpadRight 按下 ");
            }

            if (OVRInput.Get(OVRInput.Button.Up))
            {
                mBuilder.Append("Up 按下 ");
            }

            if (OVRInput.Get(OVRInput.Button.Down))
            {
                mBuilder.Append("Down 按下 ");
            }

            if (OVRInput.Get(OVRInput.Button.Left))
            {
                mBuilder.Append("Left 按下 ");
            }

            if (OVRInput.Get(OVRInput.Button.Right))
            {
                mBuilder.Append("Right 按下 ");
            }

            if (OVRInput.Get(OVRInput.RawButton.RThumbstick))
            {
                mBuilder.Append("RThumbstick 按下 ");
            }

            if (OVRInput.Get(OVRInput.RawButton.RThumbstickDown))
            {
                mBuilder.Append("RThumbstickDown 按下 ");
            }

            if (OVRInput.Get(OVRInput.RawButton.RThumbstickUp))
            {
                mBuilder.Append("RThumbstickUp 按下 ");
            }

            if (OVRInput.Get(OVRInput.RawButton.RThumbstickLeft))
            {
                mBuilder.Append("RThumbstickLeft 按下 ");
            }

            if (OVRInput.Get(OVRInput.RawButton.RThumbstickRight))
            {
                mBuilder.Append("RThumbstickRight 按下 ");
            }

            if (OVRInput.Get(OVRInput.RawButton.RTouchpad))
            {
                mBuilder.Append("RTouchpad 按下 ");
            }

            if (OVRInput.Get(OVRInput.RawTouch.RThumbstick))
            {
                mBuilder.Append("RThumbstick 触摸 ");
            }

            if (OVRInput.Get(OVRInput.RawTouch.RTouchpad))
            {
                mBuilder.Append("RTouchpad 触摸 ");
            }

            mText.text = mBuilder.ToString();
            */


            string text = "";
            if (OVRInput.GetDown(OVRInput.RawButton.RThumbstick))
            {
                text += "RThumbstick 按下 ";
            }
            if (OVRInput.GetUp(OVRInput.RawButton.RThumbstick))
            {
                text += "RThumbstick 抬起 ";
            }

            if (OVRInput.GetDown(OVRInput.RawButton.RHandTrigger))
            {
                text += "RHandTrigger 按下 ";
            }
            if (OVRInput.GetUp(OVRInput.RawButton.RHandTrigger))
            {
                text += "RHandTrigger 抬起 ";
            }
            if (!string.IsNullOrEmpty(text))
            {
                LogManager.instance.DebugLog(text);
            }
        }
    }
}
