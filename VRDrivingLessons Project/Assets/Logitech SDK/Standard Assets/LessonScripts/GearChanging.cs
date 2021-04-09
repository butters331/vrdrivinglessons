using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Vehicles.Car;

public class GearChanging : MonoBehaviour
{
    private float startTime;
    private float heldTime;
    private bool counting = false;
    private int step = 0;
    private int currentClutchPosition;
    public CarController car;
    public Indicators indicators;
    public CarUserControl userControl;
    
    // Update is called once per frame
    void Update()
    {
        if (LogitechGSDK.LogiUpdate())
        {
            //get data from wheel
            LogitechGSDK.DIJOYSTATE2ENGINES rec;
            rec = LogitechGSDK.LogiGetStateUnity(0);

            Debug.Log("Step: " + step);

            switch (step)
            {
                case 0: // turn car on
                    //intro dialogue

                    if (!car.isTurnedOff())
                    {
                        step++;
                    }
                    break;

                case 1: //clutch down and into first

                    if (car.getClutchDown() && car.isInGear() && car.getGear() == 1)
                    {
                        step++;
                    }
                    break;

                case 2:// indicate left

                    if (indicators.leftIndicatorOn())
                    {
                        step++;
                    }
                    break;

                case 3: //bring to bite
                    currentClutchPosition = rec.rglSlider[0];
                    if (currentClutchPosition < 2500 && currentClutchPosition > -2500)
                    {
                        heldTime = Time.time;
                        if (!counting)
                        {
                            startTime = Time.time;
                            counting = true;
                        }
                        else if ((heldTime - startTime) >= 1)
                        {
                            step++;
                            startTime = 0;
                            heldTime = 0;
                            counting = false;
                        }
                    }
                    break;

                case 4: //do check then release handbrake
                    if (!userControl.hasHandbrakeOn())
                    {
                        step++;
                    }
                    break;

                case 5: //pull away
                    currentClutchPosition = rec.rglSlider[0];
                    if (car.isInGear() && car.getCurrentSpeed() > 1 && currentClutchPosition == 32767 && !car.stalled)
                    {
                        step++;
                    }
                    break;

                case 6: //get revs to 3000rpm
                    if (car.Revs >= 0.3f)
                    {
                        step++;
                    }
                    break;

                case 7: // change to second
                    if (car.isInGear() && car.getGear() == 2)
                    {
                        step++;
                    }
                    break;

                case 8: //get revs to 3000rpm
                    if (car.Revs >= 0.3f)
                    {
                        step++;
                    }
                    break;

                case 9: //change to third
                    if (car.isInGear() && car.getGear() == 3)
                    {
                        step++;
                    }
                    break;

                case 10: //drive for a bit
                    heldTime = Time.time;

                    if (!counting)
                    {
                        startTime = Time.time;
                        counting = true;
                    }
                    else if ((heldTime - startTime) >= 3)
                    {
                        step++;
                        startTime = 0;
                        heldTime = 0;
                        counting = false;
                    }
                    break;

                case 11: // slow to around 1.5 revs
                    if (car.Revs <= 0.15f)
                    {
                        step++;
                    }
                    break;

                case 12: // shift down to second
                    if (car.isInGear() && car.getGear() == 2)
                    {
                        step++;
                    }
                    break;

                case 13: //drive a bit again
                    heldTime = Time.time;

                    if (!counting)
                    {
                        startTime = Time.time;
                        counting = true;
                    }
                    else if ((heldTime - startTime) >= 2)
                    {
                        step++;
                        startTime = 0;
                        heldTime = 0;
                        counting = false;
                    }
                    break;

                case 14: // indicate right
                    if (indicators.rightIndicatorOn())
                    {
                        step++;
                    }
                    break;

                case 15: // clutchdown and brake to a stop
                    if (car.getClutchDown() && car.getCurrentSpeed() <= 0.001f)
                    {
                        step++;
                    }
                    break;

                case 16: //take out of gear
                    if (!car.isInGear())
                    {
                        step++;
                    }
                    break;

                case 17: //clutch up again
                    currentClutchPosition = rec.rglSlider[0];
                    if (currentClutchPosition > 2500)
                    {
                        step++;
                    }
                    break;

                case 18: //apply handbrake

                    if (userControl.hasHandbrakeOn())
                    {
                        step++;
                    }
                    break;

                case 19: // release brake
                    if (rec.lRz == 32767)
                    {
                        step++;
                    }
                    break;

                case 20: // stop indicating
                    if (!indicators.indicatorsOn())
                    {
                        step++;
                    }
                    break;

                case 21: //end of lesson
                    //if x pressed
                    if (rec.rgbButtons[2] == 128)
                    {
                        SceneManager.LoadScene(0);
                    }

                    //after speaking return to main menu and apply tick
                    break;

            }
        }
    }
}
