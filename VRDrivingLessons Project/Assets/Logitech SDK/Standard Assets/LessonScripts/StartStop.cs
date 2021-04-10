using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Vehicles.Car;

public class StartStop : MonoBehaviour
{
    private float startTime;
    private float heldTime;
    private bool counting = false;
    private bool firstLoad = true;

    private int step = 0;
    public CarController car;
    public Indicators indicators;
    public CarUserControl userControl;

    public AudioSource intro;
    public AudioSource overview;
    public AudioSource turnOn;
    public AudioSource clutchIn;
    public AudioSource intoFirst;
    public AudioSource indicateLeft;
    public AudioSource bite;
    public AudioSource handbrakeOff;
    public AudioSource increaseRevs;
    public AudioSource startMoving;
    public AudioSource optionalCancelIndication;
    public AudioSource indicateRight;
    public AudioSource clutchIn2;
    public AudioSource startBraking;
    public AudioSource outOfGear;
    public AudioSource releaseClutch;
    public AudioSource applyHandbrake;
    public AudioSource releaseBrake;
    public AudioSource cancelIndication;
    public AudioSource outro;
    public AudioSource returnToMainMenu;
    public AudioSource stallHelp;

    private bool introPlayed = false;
    private bool overviewPlayed = false;
    private bool turnOnPlayed = false;
    private bool clutchInPlayed = false;
    private bool intoFirstPlayed = false;
    private bool indicateLeftPlayed = false;
    private bool bitePlayed = false;
    private bool handbrakeOffPlayed = false;
    private bool increaseRevsPlayed = false;
    private bool startMovingPlayed = false;
    private bool optionalCancelIndicationPlayed = false;
    private bool indicateRightPlayed = false;
    private bool clutchIn2Played = false;
    private bool startBrakingPlayed = false;
    private bool outOfGearPlayed = false;
    private bool releaseClutchPlayed = false;
    private bool applyHandbrakePlayed = false;
    private bool releaseBrakePlayed = false;
    private bool cancelIndicationPlayed = false;
    private bool outroPlayed = false;
    private bool returnToMainMenuPlayed = false;
    private bool stallHelpPlayed = false;

    private int currentClutchPosition;
    // Start is called before the first frame update
/*    void Start()
    {
        //car = GetComponent<CarController>();
        indicators = GetComponent<Indicators>();
        userControl = GetComponent<CarUserControl>();
    }*/

    // Update is called once per frame
    void Update()
    {
        if (LogitechGSDK.LogiUpdate())
        {
            //get data from wheel
            LogitechGSDK.DIJOYSTATE2ENGINES rec;
            rec = LogitechGSDK.LogiGetStateUnity(0);

            switch (step)
            {
                case 0: // explain how to start and stop -- ask to turn on the car
                    if (firstLoad)
                    {
                        firstLoad = false;
                        if (!introPlayed)
                        {
                            StartCoroutine(playVoiceOver(intro));
                            introPlayed = true;
                        }

                        if (!overviewPlayed && !intro.isPlaying)
                        {
                            StartCoroutine(playVoiceOver(overview));
                            overviewPlayed = true;
                        }
                    }

                    if (!turnOnPlayed && !intro.isPlaying && !overview.isPlaying && !stallHelp.isPlaying)
                    {
                        StartCoroutine(playVoiceOver(turnOn));
                        turnOnPlayed = true;
                    }

                    if (!car.isTurnedOff() && !turnOn.isPlaying && !intro.isPlaying && !overview.isPlaying)
                    {
                        step++;
                    }
                    break;

                case 1: // check clutch pushed
                    //dialogue
                    if (!clutchInPlayed && !stallHelp.isPlaying)
                    {
                        StartCoroutine(playVoiceOver(clutchIn));
                        clutchInPlayed = true;
                    }

                    if (car.getClutchDown() && !clutchIn.isPlaying && !stallHelp.isPlaying)
                    {
                        step++;
                    }
                    break;

                case 2: // check put into gear
                    //dialogue
                    if (!intoFirstPlayed)
                    {
                        StartCoroutine(playVoiceOver(intoFirst));
                        intoFirstPlayed = true;
                    }

                    if (car.isInGear() && car.getGear() == 1 && !intoFirst.isPlaying)
                    {
                        step++;
                    }
                    break;

                case 3: // indication
                    //dialogue

                    if (!indicateLeftPlayed)
                    {
                        StartCoroutine(playVoiceOver(indicateLeft));
                        indicateLeftPlayed = true;
                    }

                    if (indicators.leftIndicatorOn() && !indicateLeft.isPlaying)
                    {
                        step++;
                    }
                    break;

                case 4: // bite
                    //dialogue

                    if (!bitePlayed)
                    {
                        StartCoroutine(playVoiceOver(bite));
                        bitePlayed = true;
                    }

                    currentClutchPosition = rec.rglSlider[0];
                    Debug.Log("Clutch: " + currentClutchPosition);
                    Debug.Log("Speed: " + car.CurrentSpeed);
                    if (!bite.isPlaying && currentClutchPosition < 2500 && currentClutchPosition > -2500)
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

                case 5: //handbrake off
                    //dialogue

                    if (!handbrakeOffPlayed)
                    {
                        StartCoroutine(playVoiceOver(handbrakeOff));
                        handbrakeOffPlayed = true;
                    }

                    if (!userControl.hasHandbrakeOn() && !handbrakeOff.isPlaying)
                    {
                        step++;
                    }
                    break;

                case 6: // add revs
                    //dialogue

                    if (!increaseRevsPlayed)
                    {
                        StartCoroutine(playVoiceOver(increaseRevs));
                        increaseRevsPlayed = true;
                    }

                    if (!increaseRevs.isPlaying && car.Revs > 0.15 && car.Revs < 0.35)
                    {
                        heldTime = Time.time;
                        //make hold for a bit
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

                case 7: // lift clutch / start moving
                    //dialogue

                    if (!startMovingPlayed)
                    {
                        StartCoroutine(playVoiceOver(startMoving));
                        startMovingPlayed = true;
                    }

                    currentClutchPosition = rec.rglSlider[0];
                    if (!startMoving.isPlaying && !car.stalled && currentClutchPosition == 32767 && car.isInGear())
                    {
                        step++;
                    }
                    break;

                case 8: //drive for a bit
                    if (indicators.indicatorsOn())
                    {
                        if (!optionalCancelIndicationPlayed)
                        {
                            StartCoroutine(playVoiceOver(optionalCancelIndication));
                            optionalCancelIndicationPlayed = true;
                        }
                    }
                    //dialogue
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

                case 9: //indicate off road
                    //dialogue
                    if (!indicateRightPlayed && !startMoving.isPlaying && !optionalCancelIndication.isPlaying)
                    {
                        StartCoroutine(playVoiceOver(indicateRight));
                        indicateRightPlayed = true;
                    }

                    if (indicators.rightIndicatorOn() && !indicateRight.isPlaying)
                    {
                        step++;
                    }
                    break;

                case 10: // check clutch down
                    //dialogue
                    if (!clutchIn2Played)
                    {
                        StartCoroutine(playVoiceOver(clutchIn2));
                        clutchIn2Played = true;
                    }

                    currentClutchPosition = rec.rglSlider[0];
                    if (currentClutchPosition < -2500 && !clutchIn2.isPlaying)
                    {
                        step++;
                    }
                    break;

                case 11: // apply brake
                    //dialogue
                    if (!startBrakingPlayed)
                    {
                        StartCoroutine(playVoiceOver(startBraking));
                        startBrakingPlayed = true;
                    }

                    if (car.CurrentSpeed <= 0.001f && !startBraking.isPlaying)
                    {
                        step ++;
                    }
                    break;

                case 12: // take out of gear
                    //dialogue
                    if (!outOfGearPlayed)
                    {
                        StartCoroutine(playVoiceOver(outOfGear));
                        outOfGearPlayed = true;
                    }

                    if (!car.isInGear() && !outOfGear.isPlaying)
                    {
                        step ++;
                    }
                    break;

                case 13: //lift clutch
                    //dialogue
                    if (!releaseClutchPlayed)
                    {
                        StartCoroutine(playVoiceOver(releaseClutch));
                        releaseClutchPlayed = true;
                    }

                    currentClutchPosition = rec.rglSlider[0];
                    if (currentClutchPosition > 2500 && !releaseClutch.isPlaying)
                    {
                        step++;
                    }
                    break;

                case 14: //apply handbrake
                    //dialogue

                    if (!applyHandbrakePlayed)
                    {
                        StartCoroutine(playVoiceOver(applyHandbrake));
                        applyHandbrakePlayed = true;
                    }

                    if (userControl.hasHandbrakeOn() && !applyHandbrake.isPlaying)
                    {
                        step++;
                    }
                    break;

                case 15: //lift brake
                    //dialogue
                    if (!releaseBrakePlayed)
                    {
                        StartCoroutine(playVoiceOver(releaseBrake));
                        releaseBrakePlayed = true;
                    }

                    //brake released
                    if (rec.lRz == 32767 && !releaseBrake.isPlaying)
                    {
                        step++;
                    }
                    break;

                case 16: // turn off indicators
                    //dialogue
                    if (!cancelIndicationPlayed )
                    {
                        StartCoroutine(playVoiceOver(cancelIndication));
                        cancelIndicationPlayed = true;
                    }

                    if (!indicators.indicatorsOn() && !cancelIndication.isPlaying)
                    {
                        step++;
                    }
                    break;

                case 17: //end of lesson
                    PlayerPrefs.SetInt("StartStop", 1);
                    //dialogue
                    if (!outroPlayed)
                    {
                        StartCoroutine(playVoiceOver(outro));
                        outroPlayed = true;
                    }

                    if (!returnToMainMenuPlayed && !outro.isPlaying)
                    {
                        StartCoroutine(playVoiceOver(returnToMainMenu));
                        returnToMainMenuPlayed = true;
                    }


                    //if x pressed
                    if (rec.rgbButtons[2] == 128)
                    {
                        SceneManager.LoadScene(0);
                    }

                    //after speaking return to main menu and apply tick
                    break;
            }

            if (car.stalled && step != 17)
            {
                //stop all audio
                turnOn.Stop();
                clutchIn.Stop();
                intoFirst.Stop();
                indicateLeft.Stop();
                bite.Stop();
                handbrakeOff.Stop();
                increaseRevs.Stop();
                startMoving.Stop();
                optionalCancelIndication.Stop();
                indicateRight.Stop();
                clutchIn2.Stop();
                startBraking.Stop();
                outOfGear.Stop();
                releaseClutch.Stop();
                applyHandbrake.Stop();
                releaseBrake.Stop();
                cancelIndication.Stop();

                //reset booleans
                turnOnPlayed = false;
                clutchInPlayed = false;
                intoFirstPlayed = false;
                indicateLeftPlayed = false;
                bitePlayed = false;
                handbrakeOffPlayed = false;
                increaseRevsPlayed = false;
                startMovingPlayed = false;
                optionalCancelIndicationPlayed = false;
                indicateRightPlayed = false;
                clutchIn2Played = false;
                startBrakingPlayed = false;
                outOfGearPlayed = false;
                releaseClutchPlayed = false;
                applyHandbrakePlayed = false;
                releaseBrakePlayed = false;
                cancelIndicationPlayed = false;
                outroPlayed = false;
                returnToMainMenuPlayed = false;

                //dialogue

                step = 1;
                if (!stallHelpPlayed)
                {
                    StartCoroutine(playVoiceOver(stallHelp));
                    stallHelpPlayed = true;
                }
            }
            else
            {
                stallHelpPlayed = false;
            }
        }
    }

    IEnumerator playVoiceOver(AudioSource source)
    {
        source.Play();
        yield return new WaitForSeconds(source.clip.length);
    }
}
