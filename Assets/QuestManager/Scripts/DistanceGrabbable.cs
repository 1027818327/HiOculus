/************************************************************************************

Copyright (c) Facebook Technologies, LLC and its affiliates. All rights reserved.  

See SampleFramework license.txt for license terms.  Unless required by applicable law 
or agreed to in writing, the sample code is provided “AS IS” WITHOUT WARRANTIES OR 
CONDITIONS OF ANY KIND, either express or implied.  See the license for specific 
language governing permissions and limitations under the license.

************************************************************************************/

using ARdEZ.Render;
using OVRTouchSample;
using TMPro;
using UnityEngine;

namespace Quest
{
    public class DistanceGrabbable : OVRGrabbable
    {
        public Outline mOutline;

        /// <summary>
        /// TextMeshPro-标记物体名字
        /// </summary>
        public TextMeshPro nameTextMesh;

        /// <summary>
        /// 丢弃物品后是否显示
        /// </summary>
        public bool grabEndEnable;

        public bool InRange
        {
            get { return m_inRange; }
            set
            {
                m_inRange = value;
                RefreshCrosshair();
            }
        }
        bool m_inRange;

        public bool Targeted
        {
            get { return m_targeted; }
            set
            {
                m_targeted = value;
                RefreshCrosshair();
            }
        }
        bool m_targeted;

        protected override void Start()
        {
            base.Start();
            if (mOutline == null) 
            {
                mOutline = GetComponent<Outline>();
            }
            RefreshCrosshair();
        }

        public void RefreshCrosshair()
        {
            if (isGrabbed || !InRange)
            {
                mOutline.OutlineColor = Color.white;
                mOutline.enabled = false;
            }
            else if (Targeted)
            {
                mOutline.OutlineColor = OvrToClient.instance.grabManager.OutlineColorHighlighted;
                mOutline.enabled = true;
            }
            else 
            {
                mOutline.OutlineColor = OvrToClient.instance.grabManager.OutlineColorInRange;
                mOutline.enabled = true;
            }
        }

        public void SetColor(Color focusColor)
        {
            mOutline.OutlineColor = focusColor;
            mOutline.enabled = true;
        }

        public void ClearColor()
        {
            mOutline.OutlineColor = Color.white;
            mOutline.enabled = false;
        }

        [ContextMenu("Set Component")]
        private void FindComponentInEditor()
        {
            mOutline = GetComponent<Outline>();
        }

        [ContextMenu("设置左右抓取点")]
        private void SetLeftRightSnapPoint() 
        {
            m_snapPosition = true;
            m_snapOrientation = true;

            if (m_snapLeftOffset == null) 
            {
                GameObject go = new GameObject("SnapLeft");
                go.transform.SetParent(transform);
                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.identity;
                go.transform.localScale = Vector3.one;
                m_snapLeftOffset = go.transform;
            }
            if (m_snapOffset == null) 
            {
                GameObject go = new GameObject("SnapRight");
                go.transform.SetParent(transform);
                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.identity;
                go.transform.localScale = Vector3.one;
                m_snapOffset = go.transform;
            }
        }

        public override void GrabBegin(OVRGrabber hand, Collider grabPoint)
        {
            if (hand.grabbedObject == null || !hand.grabbedObject.allowOffhandGrab)
            {
                return;
            }

            base.GrabBegin(hand, grabPoint);
            string handName = hand.transform.parent.name;

            if (handName.StartsWith("LeftHand"))
            {
                LogManager.instance.DebugLog("当前被左手抓取");
            }
            else
            {
                LogManager.instance.DebugLog("当前被右手抓取");
            }
        }

        public override void GrabEnd(Vector3 linearVelocity, Vector3 angularVelocity)
        {
            if (m_grabbedBy == null)
            {
                return;
            }

            string handName = m_grabbedBy.transform.parent.name;

            if (handName.StartsWith("LeftHand"))
            {
                LogManager.instance.DebugLog("当前被左手丢弃");
            }
            else
            {
                LogManager.instance.DebugLog("当前被右手丢弃");
            }

            base.GrabEnd(linearVelocity, angularVelocity);
            if (!grabEndEnable) 
            {
                gameObject.SetActive(false);
            }
        }
    }
}
