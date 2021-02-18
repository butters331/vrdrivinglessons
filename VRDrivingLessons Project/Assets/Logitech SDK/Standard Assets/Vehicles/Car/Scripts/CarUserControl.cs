using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Vehicles.Car
{
    [RequireComponent(typeof (CarController))]
    public class CarUserControl : MonoBehaviour
    {
        private CarController m_Car; // the car controller we want to use

        private bool handbrakeOn = false;
        private bool bReleased = true;

        public GameObject handBrakeLight;

        private void Awake()
        {
            // get the car controller
            m_Car = GetComponent<CarController>();
            if (!(LogitechGSDK.LogiSteeringInitialize(false)))
            {
                Debug.Log("Failed wheel init");
            }
            m_Car.setUserControlled();
            handBrakeLight.active = false;
        }

        private void FixedUpdate()
        {
            if (LogitechGSDK.LogiUpdate())
            {
                //get data from wheel
                LogitechGSDK.DIJOYSTATE2ENGINES rec;
                rec = LogitechGSDK.LogiGetStateUnity(0);
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
                    handBrakeLight.active = !handBrakeLight.active;
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
