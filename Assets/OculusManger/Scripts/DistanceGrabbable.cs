/************************************************************************************

Copyright (c) Facebook Technologies, LLC and its affiliates. All rights reserved.  

See SampleFramework license.txt for license terms.  Unless required by applicable law 
or agreed to in writing, the sample code is provided “AS IS” WITHOUT WARRANTIES OR 
CONDITIONS OF ANY KIND, either express or implied.  See the license for specific 
language governing permissions and limitations under the license.

************************************************************************************/

using ARdEZ.Render;
using UnityEngine;

namespace Oculus
{
    public class DistanceGrabbable : OVRGrabbable
    {
        public Outline mOutline;

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

        void RefreshCrosshair()
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
    }
}
