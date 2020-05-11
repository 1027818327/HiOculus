using System;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Quest
{
    public class LogManager : MonoBehaviour
    {
        public static LogManager instance;
        public Text mText;

        private StringBuilder stringBuilder = new StringBuilder();

        void Awake() 
        {
            instance = this;
        }

        // Start is called before the first frame update
        void Start()
        {
            bool handTrackingEnabled = OVRPlugin.GetHandTrackingEnabled();
            DebugLog("handTrackingEnabled: " + handTrackingEnabled);
            DebugLog("OVRPlugin.version: " + OVRPlugin.version);
            DebugLog("OVRPlugin.nativeSDKVersion: " + OVRPlugin.nativeSDKVersion);
        }

        public void DebugLog(string log) 
        {
            //mAllLogs.Add(log);
            string tempText = string.Format("{0}: {1}\n", DateTime.Now.ToString("HH:mm:ss:ffff"), log);
            stringBuilder.Append(tempText);

            Debug.Log(log);

            RefreshView();
        }

        private void RefreshView() 
        {
            mText.text = stringBuilder.ToString();
        }

        public void ClearLog() 
        {
            stringBuilder.Length = 0;
            mText.text = null;
        }
    }
}
