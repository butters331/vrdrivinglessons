using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Vehicles.Car;

public class TrafficLights : MonoBehaviour
{
    private int step = 0;
    private int directionToTravel = 0;
    private bool firstLoad = true;
    public CarController car;
    public Indicators indicators;
    public Light greenLight;
    public Waypoint juncToStopAt;
    public Waypoint endRight;
    public Waypoint endAhead;
    public Waypoint endLeft;
    public GameObject aiCars;

    public AudioSource intro;
    public AudioSource overview;
    public AudioSource startCar;
    public AudioSource directionStraight;
    public AudioSource directionLeft;
    public AudioSource directionRight;
    public AudioSource indicate;
    public AudioSource moveRight;
    public AudioSource moveLeft;
    public AudioSource green;
    public AudioSource red;
    public AudioSource checkLane;
    public AudioSource finished;
    public AudioSource pressX;
    public AudioSource stall;

    private bool introPlayed = false;
    private bool overviewPlayed = false;
    private bool startCarPlayed = false;
    private bool directionPlayed = false;
    private bool indicatePlayed = false;
    private bool lanePositioningPlayed = false;
    private bool greenRedPlayed = false;
    private bool checkLanePlayed = false;
    private bool finishPlayed = false;
    private bool xPlayed = false;
    private bool stallPlayed = false;

    private bool currentLightStatus;

    private int noOfcarsTotal;
    private int noOfActiveCars;
    private GameObject[] cars;
    private long psudoRandom = 0;
    private void Start()
    {
        noOfcarsTotal = aiCars.transform.childCount;
        cars = new GameObject[noOfcarsTotal];
        noOfActiveCars = 0;
        for (int x = 0; x < noOfcarsTotal; x++)
        {
            if (aiCars.transform.GetChild(x).gameObject.activeInHierarchy)
            {
                cars[noOfActiveCars] = aiCars.transform.GetChild(x).gameObject;
                noOfActiveCars++;
            }
        }
        setCarsInactive();
    }

    // Update is called once per frame
    void Update()
    {
        if (LogitechGSDK.LogiUpdate())
        {
            //get data from wheel
            LogitechGSDK.DIJOYSTATE2ENGINES rec;
            rec = LogitechGSDK.LogiGetStateUnity(0);

            Vector3 carForward = car.transform.forward;
            carForward.y = 0;

            switch (step)
            {
                case 0:
                    if (firstLoad)
                    {
                        if (!introPlayed)
                        {
                            StartCoroutine(playVoiceOver(intro));
                            introPlayed = true;
                        }

                        if (!overviewPlayed && !intro.isPlaying)
                        {
                            StartCoroutine(playVoiceOver(overview));
                            overviewPlayed = true;
                            firstLoad = false;
                        }
                        
                    }

                    if (car.isTurnedOff())
                    {
                        psudoRandom++;
                    }
                    else
                    {
                        directionToTravel = (int)(psudoRandom % 3);
                    }

                    if (!startCarPlayed && !overview.isPlaying && !intro.isPlaying)
                    {
                        StartCoroutine(playVoiceOver(startCar));
                        startCarPlayed = true;
                    }

                    if (!car.isTurnedOff() && car.CurrentSpeed > 3)
                    {
                        setCarsActive();
                        step++;
                        
                    }
                    break;

                case 1:

                    if (!directionPlayed && !startCar.isPlaying)
                    {
                        if (directionToTravel == 0)
                        {
                            StartCoroutine(playVoiceOver(directionStraight));
                        }
                        else if (directionToTravel == 1)
                        {
                            StartCoroutine(playVoiceOver(directionLeft));
                        }
                        else
                        {
                            StartCoroutine(playVoiceOver(directionRight));
                        }
                        directionPlayed = true;
                    }

                    if (directionPlayed && !directionStraight.isPlaying && !directionRight.isPlaying && !directionLeft.isPlaying)
                    {
                        step++;
                    }
                    break;

                case 2:
                    if (directionToTravel == 0)
                    {
                        step++;
                    }
                    else
                    {
                        if (!indicatePlayed)
                        {
                            StartCoroutine(playVoiceOver(indicate));
                            indicatePlayed = true;
                        }

                        if (!indicate.isPlaying)
                        {
                            if (directionToTravel == 1)
                            {
                                if (indicators.leftIndicatorOn())
                                {
                                    step++;
                                }
                            }
                            else
                            {
                                if (indicators.rightIndicatorOn())
                                {
                                    step++;
                                }
                            }
                        }
                        
                    }
                    break;

                case 3:
                    if (directionToTravel == 1)
                    {
                        if (!lanePositioningPlayed)
                        {
                            StartCoroutine(playVoiceOver(moveLeft));
                            lanePositioningPlayed = true;
                        }
                    }
                    else
                    {
                        if (!lanePositioningPlayed)
                        {
                            StartCoroutine(playVoiceOver(moveRight));
                            lanePositioningPlayed = true;
                        }
                    }

                    if (lanePositioningPlayed && !moveRight.isPlaying && !moveLeft.isPlaying)
                    {
                        currentLightStatus = greenLight.enabled;
                        step++;
                    }
                    break;


                case 4:
                    if (!greenRedPlayed && currentLightStatus == greenLight.enabled)
                    {
                        if (greenLight.enabled)
                        {
                            StartCoroutine(playVoiceOver(green));
                        }
                        else
                        {
                            StartCoroutine(playVoiceOver(red));
                        }
                        greenRedPlayed = true;
                    }

                    else if (currentLightStatus != greenLight.enabled && !red.isPlaying && !green.isPlaying && !checkLane.isPlaying)
                    {
                        currentLightStatus = greenLight.enabled;
                        if (greenLight.enabled)
                        {
                            StartCoroutine(playVoiceOver(green));
                        }
                        else
                        {
                            StartCoroutine(playVoiceOver(red));
                        }

                    }

                    if (directionToTravel == 1 && !checkLanePlayed && greenLight.enabled && !green.isPlaying && !red.isPlaying)
                    {
                        StartCoroutine(playVoiceOver(checkLane));
                        checkLanePlayed = true;
                    }

                    Vector3 waypointCarDiff = juncToStopAt.transform.position - car.transform.position;
                    waypointCarDiff.y = 0;

                    

                    if (waypointCarDiff.magnitude > 4 && Vector3.Dot(waypointCarDiff, carForward) < -0.3f && greenLight.enabled && !green.isPlaying && !red.isPlaying)
                    {
                        step++;
                    }
                    break;

                case 5:
                    Waypoint destination;
                    if (directionToTravel == 0)
                    {
                        destination = endAhead;
                    }
                    else if (directionToTravel == 1)
                    {
                        destination = endLeft;
                    }
                    else
                    {
                        destination = endRight;
                    }

                    Vector3 endWaypointDif = destination.transform.position - car.transform.position;
                    endWaypointDif.y = 0;

                    bool passedDestination = Vector3.Dot(endWaypointDif, carForward) < -0.3f;

                    if (passedDestination && !finishPlayed && !green.isPlaying && !red.isPlaying && !checkLane.isPlaying)
                    {
                        PlayerPrefs.SetInt("TrafficLights", 1);
                        StartCoroutine(playVoiceOver(finished));
                        finishPlayed = true;
                    }

                    if (finishPlayed && !finished.isPlaying && !xPlayed)
                    {
                        StartCoroutine(playVoiceOver(pressX));
                        xPlayed = true;
                    }
                    //if x pressed
                    if (rec.rgbButtons[2] == 128 && xPlayed)
                    {
                        SceneManager.LoadScene(0);
                    }

                    break;
            }

            if (car.stalled && !finishPlayed)
            {
                startCar.Stop();
                directionStraight.Stop();
                directionLeft.Stop();
                directionRight.Stop();
                indicate.Stop();
                moveRight.Stop();
                moveLeft.Stop();
                green.Stop();
                red.Stop();
                checkLane.Stop();
                finished.Stop();
                pressX.Stop();

                if (!stallPlayed)
                {
                    StartCoroutine(playVoiceOver(stall));
                    stallPlayed = true;
                    switch (step)
                    {
                        case 0:
                            startCarPlayed = false;
                            break;
                        case 1:
                            directionPlayed = false;
                            break;
                        case 2:
                            directionPlayed = false;
                            indicatePlayed = false;
                            step--;
                            break;
                        case 3:
                            lanePositioningPlayed = false;
                            break;
                        case 4:
                            greenRedPlayed = false;
                            checkLanePlayed = false;
                            break;
                    }
                }
            }
        }
    }

    private void setCarsActive()
    {
        for(int x = 0; x < noOfActiveCars; x++)
        {
            cars[x].SetActive(true);
        }
    }

    private void setCarsInactive()
    {
        for (int x = 0; x < noOfActiveCars; x++)
        {
            cars[x].SetActive(false);
        }
    }

    IEnumerator playVoiceOver(AudioSource source)
    {
        source.Play();
        yield return new WaitForSeconds(source.clip.length);
    }
}
