using UnityEngine;

namespace Common
{
    public class MosueKeyboardUtils : MonoBehaviour
    {

        public float speed = 1.0f;

        [HideInInspector]
        float xSpeed = 250.0f;
        float ySpeed = 120.0f;
        float yMinLimit = -20;
        float yMaxLimit = 80;
        private float x = 0.0f;
        private float y = 0.0f;
        // Use this for initialization

        void Start()
        {
            var angles = transform.eulerAngles;
            x = angles.y;
            y = angles.x;
        }
        static float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360)
            {
                angle += 360;
            }
            if (angle > 360)
            {
                angle -= 360;
            }
            return Mathf.Clamp(angle, min, max);
        }
        // Update is called once per frame
        void Update()
        {
            //w键前进  
            if (Input.GetKey(KeyCode.W))
            {
                transform.localPosition += Vector3.forward * Time.deltaTime * speed;

                //transform.Translate(Vector3.forward * Time.deltaTime * speed);
            }
            if (Input.GetKey(KeyCode.S))
            {
                transform.localPosition += Vector3.back * Time.deltaTime * speed;

                //transform.Translate(Vector3.back * Time.deltaTime * speed);
            }
            if (Input.GetKey(KeyCode.A))
            {
                transform.localPosition += Vector3.left * Time.deltaTime * speed;

                //transform.Translate(Vector3.left * Time.deltaTime * speed);
            }
            if (Input.GetKey(KeyCode.D))
            {
                transform.localPosition += Vector3.right * Time.deltaTime * speed;

                //transform.Translate(Vector3.right * Time.deltaTime * speed);
            }

            if (Input.GetKey(KeyCode.UpArrow))
            {
                transform.localPosition += Vector3.up * Time.deltaTime * speed;

                //transform.Translate(Vector3.up * Time.deltaTime * speed);
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                transform.localPosition += Vector3.down * Time.deltaTime * speed;

                //transform.Translate(Vector3.down * Time.deltaTime * speed);
            }

            if (Input.GetMouseButton(1))
            {
                x += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
                y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

                y = ClampAngle(y, yMinLimit, yMaxLimit);

                var rotation = Quaternion.Euler(y, x, 0);
                transform.rotation = rotation;
            }

        }
    }
}

