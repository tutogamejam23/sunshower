using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sunshower
{
    public class CameraMove : MonoBehaviour
    {
        public AudioClip clip;

        public float CameraSpeed = 10;
        public float CameraControlArea = 100;
        public float offsetX;
        Vector3 Pos;
        bool isMouseinArea;
        float height, width;
        public float limitMinX, limitMaxX;

        void Start()
        {
            height = Camera.main.orthographicSize;
            width = height * Screen.width / Screen.height;
        }

        void FixedUpdate()
        {
            LimitCamera();
        }

        void Update()
        {
            ControlCamera();
        }


        void ControlCamera()
        {
            if (Input.mousePosition.x < CameraControlArea && Input.mousePosition.x >= 0 && Input.mousePosition.y > 0 && Input.mousePosition.y < Screen.height)
            {
                offsetX = -1;
                isMouseinArea = true;
                SoundManager.instance.PlayBGM(clip);
            }
            else if (Input.mousePosition.x > Screen.width - CameraControlArea && Input.mousePosition.x <= Screen.width && Input.mousePosition.y > 0 && Input.mousePosition.y < Screen.height)
            {
                offsetX = 1;
                isMouseinArea = true;
            }
            else if (!(isMouseinArea && Input.mousePosition.x < CameraControlArea && Input.mousePosition.y > 0 && Input.mousePosition.y < Screen.height || isMouseinArea && Input.mousePosition.x > Screen.width - CameraControlArea && Input.mousePosition.y > 0 && Input.mousePosition.y < Screen.height))
            {
                offsetX = 0;
                isMouseinArea = false;
            }
        }

        void LimitCamera()
        {
            Pos = new Vector3(transform.position.x + offsetX, transform.position.y, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, Pos, Time.deltaTime * CameraSpeed);

            float clampX = Mathf.Clamp(transform.position.x, limitMinX + width, limitMaxX - width);
            transform.position = new Vector3(clampX, transform.position.y, transform.position.z);
        }

    }
}
