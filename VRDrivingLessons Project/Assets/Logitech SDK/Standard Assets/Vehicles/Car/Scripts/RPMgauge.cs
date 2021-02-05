using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityStandardAssets.Vehicles.Car
{
    public class RPMgauge : MonoBehaviour
    {
        private Vector3 currentEulerAngles;

        public GameObject m_Car;
        private CarController m_CarController;

        // Start is called before the first frame update
        void Start()
        {
            m_CarController = m_Car.GetComponent<CarController>();
            currentEulerAngles = new Vector3(0, 0, 0);
            transform.localEulerAngles = currentEulerAngles;
        }

        // Update is called once per frame
        void Update()
        {
            float rpm = m_CarController.Revs * 10;
            if (LogitechGSDK.LogiUpdate())
            {
                LogitechGSDK.DIJOYSTATE2ENGINES rec;
                rec = LogitechGSDK.LogiGetStateUnity(0);
                if (rpm < 1)
                {
                    currentEulerAngles = new Vector3(0, 0, -rpm * 22);
                }
                else
                {
                    float newDegree = (rpm - 1) * 18 + 22;
                    currentEulerAngles = new Vector3(0, 0, -newDegree);
                }

                transform.localEulerAngles = currentEulerAngles;
            }
        }
    }
}

