/*
    This code is an adaptation or the source code from the Unity Standard Assets Library
 */

using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Vehicles.Car
{
    [RequireComponent(typeof (CarController))]
    public class CarUserControl : MonoBehaviour
    {
        private CarController m_Car; // the car controller we want to use

        private bool handbrakeOn = true;
        private bool bReleased = true;
        private bool loadedInto = true;

        private GameObject cameraVariables;

        public GameObject handBrakeLight;
        public GameObject cameraMovement;
        public GameObject biteNotification;

        public Camera mainCam;

        public bool hasHandbrakeOn()
        {
            return handbrakeOn;
        }

        private void Awake()
        {
            // get the car controller
            m_Car = GetComponent<CarController>();
            //if (!(LogitechGSDK.LogiSteeringInitialize(false)))
            //{
            //    Debug.Log("Failed wheel init");
            //}
            m_Car.setUserControlled();
            handBrakeLight.SetActive(true);
            
        }

        private void FixedUpdate()
        {
            if (LogitechGSDK.LogiUpdate())
            {
                //get data from wheel
                LogitechGSDK.DIJOYSTATE2ENGINES rec;
                rec = LogitechGSDK.LogiGetStateUnity(0);

                Vector3 offset = new Vector3(0,-0.1f,0);
                biteNotification.transform.position = mainCam.transform.position + offset + mainCam.transform.forward * 0.3f;
                biteNotification.transform.rotation = mainCam.transform.rotation;

                if (loadedInto)
                {
                    loadedInto = false;
                    cameraVariables = GameObject.Find("CameraVariables");
                    Vector3 tempVector = cameraMovement.transform.position;
                    Vector3 cameraOffset = cameraVariables.GetComponent<CameraVariables>().coordinates;
                    tempVector.y += cameraOffset.y * 0.5f;
                    //so that sliding to the left makes you closer
                    tempVector.z -= cameraOffset.z * 0.5f;
                    cameraMovement.transform.position = tempVector;

                }
                // pass the input to the car!
                //float h = CrossPlatformInputManager.GetAxis("Horizontal");
                //float v = CrossPlatformInputManager.GetAxis("Vertical");

                //reads steering from wheel, divides by 32768 to keep between 1 and -1
                float steering = (float)rec.lX / 32768;

                //reads from accelorator , and truncates level from being 0 when up, and 1 when down
                float accel = (float)(rec.lY - 32767) / -65535;
                //reads from break pedal, and truncates level from being 0 when up, and -1 when down
                float footbreak = (float)(rec.lRz - 32767) / 65535;

                
                if (bReleased && rec.rgbButtons[1] == 128)
                {
                    handbrakeOn = !handbrakeOn;
                    handBrakeLight.SetActive(!handBrakeLight.activeInHierarchy);
                    bReleased = false;
                }
                else if (rec.rgbButtons[1] != 128)
                {
                    bReleased = true;
                }
                

                float handbrake;
                if (handbrakeOn)
                {
                    handbrake = 1.0f;
                }
                else
                {
                    handbrake = 0.0f;
                }
                
                m_Car.Move(steering, accel, footbreak, handbrake);
                //Debug.Log("Revs: " + m_Car.Revs);
            }
            
 
#if !MOBILE_INPUT
            
#else
            m_Car.Move(h, v, v, 0f);
#endif
        }
    }
}
