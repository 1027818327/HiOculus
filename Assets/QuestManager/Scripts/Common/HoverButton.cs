﻿using Quest;
using UnityEngine;
using UnityEngine.Events;

namespace Common
{
    public class HoverButton : MonoBehaviour
    {
        public Transform movingPart;

        public Vector3 localMoveDistance = new Vector3(0, -0.1f, 0);

        [Range(0, 1)]
        public float engageAtPercent = 0.95f;

        [Range(0, 1)]
        public float disengageAtPercent = 0.9f;

        [System.Serializable]
        public class HandEvent : UnityEvent { }

        public HandEvent onButtonEnter;
        public HandEvent onButtonExit;
        public HandEvent onButtonDown;
        public HandEvent onButtonUp;
        public HandEvent onButtonIsPressed;

        public bool engaged = false;
        public bool buttonDown = false;
        public bool buttonUp = false;

        private Vector3 startPosition;
        private Vector3 endPosition;

        private Vector3 handEnteredPosition;

        private bool hovering;

        private Transform lastHoveredHand;

        private void Start()
        {
            if (movingPart == null && this.transform.childCount > 0)
                movingPart = this.transform.GetChild(0);

            startPosition = movingPart.localPosition;
            endPosition = startPosition + localMoveDistance;
            handEnteredPosition = endPosition;
        }

        private void HandHoverUpdate(Transform hand)
        {
            hovering = true;
            lastHoveredHand = hand;

            bool wasEngaged = engaged;

            float currentDistance = Vector3.Distance(movingPart.parent.InverseTransformPoint(hand.position), endPosition);
            float enteredDistance = Vector3.Distance(handEnteredPosition, endPosition);

            if (currentDistance > enteredDistance)
            {
                enteredDistance = currentDistance;
                handEnteredPosition = movingPart.parent.InverseTransformPoint(hand.position);
            }

            float distanceDifference = enteredDistance - currentDistance;

            float lerp = Mathf.InverseLerp(0, localMoveDistance.magnitude, distanceDifference);

            if (lerp > engageAtPercent)
                engaged = true;
            else if (lerp < disengageAtPercent)
                engaged = false;

            movingPart.localPosition = Vector3.Lerp(startPosition, endPosition, lerp);

            InvokeEvents(wasEngaged, engaged);
        }

        private void Update()
        {
            if (lastHoveredHand != null)
            {
                HandHoverUpdate(lastHoveredHand);
            }
        }

        private void LateUpdate()
        {
            if (hovering == false)
            {
                movingPart.localPosition = startPosition;
                handEnteredPosition = endPosition;

                InvokeEvents(engaged, false);
                engaged = false;
            }

            hovering = false;
        }

        private void InvokeEvents(bool wasEngaged, bool isEngaged)
        {
            buttonDown = wasEngaged == false && isEngaged == true;
            buttonUp = wasEngaged == true && isEngaged == false;

            if (buttonDown && onButtonDown != null)
                onButtonDown.Invoke();
            if (buttonUp && onButtonUp != null)
                onButtonUp.Invoke();
            if (isEngaged && onButtonIsPressed != null)
                onButtonIsPressed.Invoke();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (lastHoveredHand == null && other.gameObject.GetComponent<PhysicPinch>() != null)
            {
                if (LogManager.instance != null) 
                {
                    LogManager.instance.DebugLog("HoverButton Enter: " + other.name);
                }

                lastHoveredHand = other.transform;

                if (onButtonEnter != null)
                {
                    onButtonEnter.Invoke();
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (lastHoveredHand != null && lastHoveredHand == other.transform)
            {
                if (LogManager.instance != null) 
                {
                    LogManager.instance.DebugLog("HoverButton Exit: " + other.name);
                }

                if (onButtonExit != null)
                {
                    onButtonExit.Invoke();
                }

                lastHoveredHand = null;
            }
        }

        /*
        private void OnCollisionEnter(Collision collision)
        {
            if (lastHoveredHand == null) 
            {
                lastHoveredHand = collision.transform;

                if (onButtonEnter != null) 
                {
                    onButtonEnter.Invoke();
                }
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            if (lastHoveredHand != null && lastHoveredHand == collision.transform) 
            {
                if (onButtonExit != null)
                {
                    onButtonExit.Invoke();
                }

                lastHoveredHand = null;
            }
        }
        */
    }
}
