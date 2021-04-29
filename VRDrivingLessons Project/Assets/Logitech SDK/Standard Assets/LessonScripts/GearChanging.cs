using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Vehicles.Car;

public class GearChanging : MonoBehaviour
{
    private float startTime;
    private float heldTime;
    private bool firstLoad = true;
    private bool counting = false;
    private int step = 0;
    private int currentClutchPosition;
    public CarController car;
    public Indicators indicators;
    public CarUserControl userControl;

    public AudioSource intro;
    public AudioSource overview;
    public AudioSource startEngine;
    public AudioSource intoFirst;
    public AudioSource indicateLeft;
    public AudioSource bringToBite;
    public AudioSource releaseHandbrake;
    public AudioSource pullAway;
    public AudioSource upToSecond;
    public AudioSource driveOn;
    public AudioSource upToThird;
    public AudioSource wellDone;
    public AudioSource brake;
    public AudioSource downToSecond;
    public AudioSource goodJob;
    public AudioSource indicateRight;
    public AudioSource stopAtSide;
    public AudioSource outOfGear;
    public AudioSource releaseClutch;
    public AudioSource applyHandbrake;
    public AudioSource stopBraking;
    public AudioSource cancelIndication;
    public AudioSource outro;
    public AudioSource stallHelp;

    private bool introPlayed = false;
    private bool overviewPlayed = false;
    private bool startEnginePlayed = false;
    private bool intoFirstPlayed = false;
    private bool indicateLeftPlayed = false;
    private bool bringToBitePlayed = false;
    private bool releaseHandbrakePlayed = false;
    private bool pullAwayPlayed = false;
    private bool upToSecondPlayed = false;
    private bool driveOnPlayed = false;
    private bool upToThirdPlayed = false;
    private bool wellDonePlayed = false;
    private bool brakePlayed = false;
    private bool downToSecondPlayed = false;
    private bool goodJobPlayed = false;
    private bool indicateRightPlayed = false;
    private bool stopAtSidePlayed = false;
    private bool outOfGearPlayed = false;
    private bool releaseClutchPlayed = false;
    private bool applyHandbrakePlayed = false;
    private bool stopBrakingPlayed = false;
    private bool cancelIndicationPlayed = false;
    private bool outroPlayed = false;
    private bool stallHelpPlayed = false;

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
                case 0: // turn car on
                    //intro dialogue

                    if (firstLoad)
                    {
                        firstLoad = false;
                        if (!introPlayed)
                        {
                            //StartCoroutine(playVoiceOver(intro));
                            introPlayed = true;
                        }

                        if (!overviewPlayed && !intro.isPlaying)
                        {
                            StartCoroutine(playVoiceOver(overview));
                            overviewPlayed = true;
                        }
                    }

                    if (!startEnginePlayed && !intro.isPlaying && !overview.isPlaying && !stallHelp.isPlaying)
                    {
                        StartCoroutine(playVoiceOver(startEngine));
                        startEnginePlayed = true;
                    }

                    if (!car.isTurnedOff() && !startEngine.isPlaying && !intro.isPlaying && !overview.isPlaying && !stallHelp.isPlaying)
                    {
                        step++;
                    }
                    break;

                case 1: //clutch down and into first

                    if (!intoFirstPlayed && !stallHelp.isPlaying)
                    {
                        StartCoroutine(playVoiceOver(intoFirst));
                        intoFirstPlayed = true;
                    }

                    if (car.getClutchDown() && car.isInGear() && car.getGear() == 1 && !intoFirst.isPlaying && !stallHelp.isPlaying)
                    {
                        step++;
                    }
                    break;

                case 2:// indicate left

                    if (!indicateLeftPlayed && !intoFirst.isPlaying)
                    {
                        StartCoroutine(playVoiceOver(indicateLeft));
                        indicateLeftPlayed = true;
                    }

                    if (indicators.leftIndicatorOn() && !intoFirst.isPlaying)
                    {
                        step++;
                    }
                    break;

                case 3: //bring to bite
                    currentClutchPosition = rec.rglSlider[0];

                    if (!bringToBitePlayed && !indicateLeft.isPlaying)
                    {
                        StartCoroutine(playVoiceOver(bringToBite));
                        bringToBitePlayed = true;
                    }

                    if (currentClutchPosition < 2500 && currentClutchPosition > -2500 && !bringToBite.isPlaying)
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
                    if (!releaseHandbrakePlayed && !bringToBite.isPlaying)
                    {
                        StartCoroutine(playVoiceOver(releaseHandbrake));
                        releaseHandbrakePlayed = true;
                    }

                    if (!userControl.hasHandbrakeOn() && !releaseHandbrake.isPlaying)
                    {
                        step++;
                    }
                    break;

                case 5: //pull away
                    currentClutchPosition = rec.rglSlider[0];

                    if (!pullAwayPlayed && !releaseHandbrake.isPlaying)
                    {
                        StartCoroutine(playVoiceOver(pullAway));
                        pullAwayPlayed = true;
                    }

                    if (car.isInGear() && car.getCurrentSpeed() > 1 && currentClutchPosition == 32767 && !car.stalled && !pullAway.isPlaying)
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
                    if (!upToSecondPlayed && !pullAway.isPlaying)
                    {
                        StartCoroutine(playVoiceOver(upToSecond));
                        upToSecondPlayed = true;
                    }
                    if (car.isInGear() && car.getGear() == 2 && !upToSecond.isPlaying)
                    {
                        step++;
                    }
                    break;

                case 8: //get revs to 3000rpm
                    if (!driveOnPlayed && !upToSecond.isPlaying)
                    {
                        StartCoroutine(playVoiceOver(driveOn));
                        driveOnPlayed = true;
                    }

                    if (car.Revs >= 0.3f && !driveOn.isPlaying)
                    {
                        step++;
                    }
                    break;

                case 9: //change to third
                    if (!upToThirdPlayed && !driveOn.isPlaying)
                    {
                        StartCoroutine(playVoiceOver(upToThird));
                        upToThirdPlayed = true;
                    }

                    if (car.isInGear() && car.getGear() == 3 && !upToThird.isPlaying)
                    {
                        step++;
                    }
                    break;

                case 10: //drive for a bit
                    if (!wellDonePlayed && !upToThird.isPlaying)
                    {
                        StartCoroutine(playVoiceOver(wellDone));
                        wellDonePlayed = true;
                    }
                    heldTime = Time.time;

                    if (!counting)
                    {
                        startTime = Time.time;
                        counting = true;
                    }
                    else if ((heldTime - startTime) >= 3 && !wellDone.isPlaying)
                    {
                        step++;
                        startTime = 0;
                        heldTime = 0;
                        counting = false;
                    }
                    break;

                case 11: // slow to around 1.5 revs
                    if (!brakePlayed && !wellDone.isPlaying)
                    {
                        StartCoroutine(playVoiceOver(brake));
                        brakePlayed = true;
                    }

                    if (car.Revs <= 0.15f && !brake.isPlaying)
                    {
                        step++;
                    }
                    break;

                case 12: // shift down to second
                    if (!downToSecondPlayed && !brake.isPlaying)
                    {
                        StartCoroutine(playVoiceOver(downToSecond));
                        downToSecondPlayed = true;
                    }

                    if (car.isInGear() && car.getGear() == 2 && !downToSecond.isPlaying)
                    {
                        step++;
                    }
                    break;

                case 13: //drive a bit again

                    if (!goodJobPlayed && !downToSecond.isPlaying)
                    {
                        StartCoroutine(playVoiceOver(goodJob));
                        goodJobPlayed = true;
                    }

                    heldTime = Time.time;

                    if (!counting)
                    {
                        startTime = Time.time;
                        counting = true;
                    }
                    else if ((heldTime - startTime) >= 2 && !goodJob.isPlaying)
                    {
                        step++;
                        startTime = 0;
                        heldTime = 0;
                        counting = false;
                    }
                    break;

                case 14: // indicate right
                    if (!indicateRightPlayed && !goodJob.isPlaying)
                    {
                        StartCoroutine(playVoiceOver(indicateRight));
                        indicateRightPlayed = true;
                    }

                    if (indicators.rightIndicatorOn() && !indicateRight.isPlaying)
                    {
                        step++;
                    }
                    break;

                case 15: // clutchdown and brake to a stop
                    if (!stopAtSidePlayed && !indicateRight.isPlaying)
                    {
                        StartCoroutine(playVoiceOver(stopAtSide));
                        stopAtSidePlayed = true;
                    }

                    if (car.getClutchDown() && car.getCurrentSpeed() <= 0.001f && !stopAtSide.isPlaying)
                    {
                        step++;
                    }
                    break;

                case 16: //take out of gear
                    if (!outOfGearPlayed && !stopAtSide.isPlaying)
                    {
                        StartCoroutine(playVoiceOver(outOfGear));
                        outOfGearPlayed = true;
                    }
                    if (!car.isInGear() && !outOfGear.isPlaying)
                    {
                        step++;
                    }
                    break;

                case 17: //clutch up again
                    currentClutchPosition = rec.rglSlider[0];

                    if (!releaseClutchPlayed && !outOfGear.isPlaying)
                    {
                        StartCoroutine(playVoiceOver(releaseClutch));
                        releaseClutchPlayed = true;
                    }

                    if (currentClutchPosition > 2500 && !releaseClutch.isPlaying)
                    {
                        step++;
                    }
                    break;

                case 18: //apply handbrake
                    if (!applyHandbrakePlayed && !releaseClutch.isPlaying)
                    {
                        StartCoroutine(playVoiceOver(applyHandbrake));
                        applyHandbrakePlayed = true;
                    }
                    if (userControl.hasHandbrakeOn() && !applyHandbrake.isPlaying)
                    {
                        step++;
                    }
                    break;

                case 19: // release brake
                    if (!stopBrakingPlayed && !applyHandbrake.isPlaying)
                    {
                        StartCoroutine(playVoiceOver(stopBraking));
                        stopBrakingPlayed = true;
                    }
                    if (rec.lRz == 32767 && !stopBraking.isPlaying)
                    {
                        step++;
                    }
                    break;

                case 20: // stop indicating
                    if (!cancelIndicationPlayed && !stopBraking.isPlaying)
                    {
                        StartCoroutine(playVoiceOver(cancelIndication));
                        cancelIndicationPlayed = true;
                    }
                    if (!indicators.indicatorsOn() && !cancelIndication.isPlaying)
                    {
                        step++;
                    }
                    break;

                case 21: //end of lesson
                    PlayerPrefs.SetInt("GearChanging", 1);
                    if (!outroPlayed && !cancelIndication.isPlaying)
                    {
                        StartCoroutine(playVoiceOver(outro));
                        outroPlayed = true;
                    }

                    //if x pressed
                    if (rec.rgbButtons[2] == 128)
                    {
                        SceneManager.LoadScene(0);
                    }

                    //after speaking return to main menu and apply tick
                    break;

            }

            if (car.stalled && step != 21)
            {
                intro.Stop();
                overview.Stop();
                startEngine.Stop();
                intoFirst.Stop();
                indicateLeft.Stop();
                bringToBite.Stop();
                releaseHandbrake.Stop();
                pullAway.Stop();
                upToSecond.Stop();
                driveOn.Stop();
                upToThird.Stop();
                wellDone.Stop();
                brake.Stop();
                downToSecond.Stop();
                goodJob.Stop();
                indicateRight.Stop();
                stopAtSide.Stop();
                outOfGear.Stop();
                releaseClutch.Stop();
                applyHandbrake.Stop();
                stopBraking.Stop();
                cancelIndication.Stop();
                outro.Stop();

                startEnginePlayed = false;
                intoFirstPlayed = false;
                bringToBitePlayed = false;
                releaseHandbrakePlayed = false;
                pullAwayPlayed = false;
                upToSecondPlayed = false;
                driveOnPlayed = false;
                upToThirdPlayed = false;
                wellDonePlayed = false;
                brakePlayed = false;
                downToSecondPlayed = false;
                goodJobPlayed = false;
                indicateRightPlayed = false;
                stopAtSidePlayed = false;
                outOfGearPlayed = false;
                releaseClutchPlayed = false;
                applyHandbrakePlayed = false;
                stopBrakingPlayed = false;
                cancelIndicationPlayed = false;
                outroPlayed = false;

                step = 1;
                if (!stallHelpPlayed)
                {
                    StartCoroutine(playVoiceOver(stallHelp));
                    stallHelpPlayed = true;
                }
            }
        }
    }

    IEnumerator playVoiceOver(AudioSource source)
    {
        source.Play();
        yield return new WaitForSeconds(source.clip.length);
    }
}
