using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityStandardAssets.Vehicles.Car
{
    public class Speedometer : MonoBehaviour
    {
        private Vector3 currentEulerAngles;

        public GameObject m_Car;
        private CarController m_CarController;

        // Start is called before the first frame update
        void Start()
        {
            m_CarController = m_Car.GetComponent<CarController>();
            currentEulerAngles = new Vector3(5, 0, 0);
            transform.localEulerAngles = currentEulerAngles;
        }

        // Update is called once per frame
        void Update()
        {
            float speed = m_CarController.getCurrentSpeed();
            if (LogitechGSDK.LogiUpdate())
            {
                LogitechGSDK.DIJOYSTATE2ENGINES rec;
                rec = LogitechGSDK.LogiGetStateUnity(0);
                if (speed < 20)
                {
                    currentEulerAngles = new Vector3(5, 0, -speed / 2);
                }
                else
                {
                    currentEulerAngles = new Vector3(5, 0, -speed + 10);
                }

                transform.localEulerAngles = currentEulerAngles;
            }
        }
    }
}

