using System.Collections;
using System.Collections.Generic;
using System.Threading;
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

        public GameObject leftIndicator;
        public GameObject rightIndicator;

    // Start is called before the first frame update
        void Start()
        {
            rightOn = false;
            leftOn = false;
            rightIndicator.active = false;
            leftIndicator.active = false;
            prevWheelPos = 0;
        }

        // Update is called once per frame
        void Update()
        {
            //get the wheel input
            if (LogitechGSDK.LogiUpdate())
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
                   
                }
                //if left pressed
                else if (rec.rgbButtons[5] == 128 && leftReleased)
                {
                    rightOn = false;
                    leftOn = !leftOn;
                    leftReleased = false;
                }

                if (rightOn)
                {
                    //make right appear and left disappear
                    if (!rightIndicator.active)
                    {
                        rightIndicator.active = true;
                    }
                    if (leftIndicator.active)
                    {
                        leftIndicator.active = false;
                    }

                    //when turning, once the wheel starts to straighten, the indicator should turn off.
                    if (prevWheelPos > 1000 && (prevWheelPos - currentWheelPos > 50))
                    {
                        rightOn = false;
                    }
                }
                else if (leftOn)
                {
                    //make left appear and right disappear
                    if (rightIndicator.active)
                    {
                        rightIndicator.active = false;
                    }
                    if (!leftIndicator.active)
                    {
                        leftIndicator.active = true;
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
                    if (rightIndicator.active)
                    {
                        rightIndicator.active = false;
                    }
                    if (leftIndicator.active)
                    {
                        leftIndicator.active = false;
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
            }
        }
    }
}



