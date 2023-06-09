﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityStandardAssets.Vehicles.Car
{
    public class Indicators : MonoBehaviour
    {

        private bool leftOn = false;
        private bool rightOn = false;

        //boolean variables to ensure that holding the indicator on has no effect
        private bool rightReleased = true;
        private bool leftReleased = true;

        private int prevWheelPos;
        private float time = 0.0f;

        public GameObject leftIndicator;
        public GameObject rightIndicator;
        public AudioClip indicatorHigh;
        public AudioClip indicatorLow;
        public CarController m_Car;

        public bool indicatorsOn()
        {
            return (leftOn | rightOn);
        }

        public bool leftIndicatorOn()
        {
            return leftOn;
        }

        public bool rightIndicatorOn()
        {
            return rightOn;
        }

        // Start is called before the first frame update
        void Start()
        {
            rightOn = false;
            leftOn = false;
            rightIndicator.SetActive(false);
            leftIndicator.SetActive(false);
            prevWheelPos = 0;
        }

        // Update is called once per frame
        void Update()
        {
            //get the wheel input
            if (!m_Car.isTurnedOff() && LogitechGSDK.LogiUpdate())
            {
                LogitechGSDK.DIJOYSTATE2ENGINES rec;
                rec = LogitechGSDK.LogiGetStateUnity(0);

                int currentWheelPos = rec.lX;
                //if right pressed
                if (rec.rgbButtons[4] == 128 && rightReleased)
                {
                    rightOn = !rightOn;
                    leftOn = false;
                    rightReleased = false;
                    time = 0.0f;
                }
                //if left pressed
                else if (rec.rgbButtons[5] == 128 && leftReleased)
                {
                    rightOn = false;
                    leftOn = !leftOn;
                    leftReleased = false;
                    time = 0.0f;
                }

                if (rightOn)
                {
                    //flash every half a second
                    if (time >= 0.4)
                    {
                        //make right appear and left disappear
                        if (!rightIndicator.activeInHierarchy)
                        {
                            AudioSource.PlayClipAtPoint(indicatorHigh, transform.position);
                            rightIndicator.SetActive(true);
                        }
                        else // to make flash
                        {
                            AudioSource.PlayClipAtPoint(indicatorLow, transform.position);
                            rightIndicator.SetActive(false);
                        }
                        time = 0.0f;
                    }
                    
                    if (leftIndicator.activeInHierarchy)
                    {
                        leftIndicator.SetActive(false);
                    }

                    //when turning, once the wheel starts to straighten, the indicator should turn off.
                    if (prevWheelPos > 1000 && (prevWheelPos - currentWheelPos > 50))
                    {
                        rightOn = false;
                    }
                }
                else if (leftOn)
                {
                    if (time >= 0.4)
                    {
                        if (!leftIndicator.activeInHierarchy)
                        {
                            AudioSource.PlayClipAtPoint(indicatorHigh, transform.position);
                            leftIndicator.SetActive(true);
                        }
                        else
                        {
                            AudioSource.PlayClipAtPoint(indicatorLow, transform.position);
                            leftIndicator.SetActive(false);
                        }
                        time = 0.0f;
                    }
                        //make left appear and right disappear
                    if (rightIndicator.activeInHierarchy)
                    {
                        rightIndicator.SetActive(false);
                    }

                    //when turning, once the wheel starts to straighten, the indicator should turn off.
                    if (prevWheelPos < -1000 && (prevWheelPos - currentWheelPos < -50))
                    {
                        leftOn = false;
                    }

                }
                else
                {
                    //make both disappear
                    if (rightIndicator.activeInHierarchy)
                    {
                        rightIndicator.SetActive(false);
                    }
                    if (leftIndicator.activeInHierarchy)
                    {
                        leftIndicator.SetActive(false);
                    }
                }

                //checks if the indicator button has been released
                if (!rightReleased && rec.rgbButtons[4] != 128)
                {
                    rightReleased = true;
                }
                if (!leftReleased && rec.rgbButtons[5] != 128)
                {
                    leftReleased = true;
                }

                //update previous wheel position
                prevWheelPos = currentWheelPos;

                time += Time.deltaTime;
            }
        }
    }
}



