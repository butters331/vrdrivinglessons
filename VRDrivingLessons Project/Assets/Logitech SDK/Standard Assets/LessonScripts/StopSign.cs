using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Vehicles.Car;

public class StopSign : MonoBehaviour
{
    private int step = 0;
    private int directionToTravel = 0;
    private bool firstLoad = true;
    public CarController car;
    public Indicators indicators;
    public Waypoint juncToStopAt;
    public Waypoint endRight;
    public Waypoint endAhead;
    public Waypoint endLeft;
    public Waypoint juncR;
    public Waypoint juncL;
    public Waypoint juncA;
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
    public AudioSource stopAtJunc;
    public AudioSource gridlock;
    public AudioSource waitHere;
    public AudioSource clear;
    public AudioSource outro;
    public AudioSource pressX;
    public AudioSource stall;

    private bool introPlayed = false;
    private bool overviewPlayed = false;
    private bool startCarPlayed = false;
    private bool directionPlayed = false;
    private bool indicatePlayed = false;
    private bool lanePositioningPlayed = false;
    private bool stopAtJuncPlayed = false;
    private bool gridlockPlayed = false;
    private bool waitHerePlayed = false;
    private bool clearPlayed = false;
    private bool outroPlayed = false;
    private bool xPlayed = false;
    private bool stallPlayed = false;

    private int noOfcarsTotal;
    private int noOfActiveCars;
    private GameObject[] cars;

    private long psudoRandom = 0;


    // Start is called before the first frame update
    void Start()
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
                        if (!introPlayed && !stall.isPlaying)
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

                    if (!startCarPlayed && !overview.isPlaying && !intro.isPlaying && !stall.isPlaying)
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

                    if (!directionPlayed && !startCar.isPlaying && !stall.isPlaying )
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

                    if (directionPlayed && !directionStraight.isPlaying && !directionRight.isPlaying && !directionLeft.isPlaying && !stall.isPlaying)
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
                        if (!indicatePlayed && !stall.isPlaying)
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
                        if (!lanePositioningPlayed && !stall.isPlaying)
                        {
                            StartCoroutine(playVoiceOver(moveLeft));
                            lanePositioningPlayed = true;
                        }
                    }
                    else
                    {
                        if (!lanePositioningPlayed && !stall.isPlaying)
                        {
                            StartCoroutine(playVoiceOver(moveRight));
                            lanePositioningPlayed = true;
                        }
                    }

                    if ((lanePositioningPlayed && !moveRight.isPlaying && !moveLeft.isPlaying))
                    {
                        step++;
                    }
                    break;

                case 4: // stop at junc
                    if (!stopAtJuncPlayed && !moveLeft.isPlaying && !moveRight.isPlaying && !stall.isPlaying)
                    {
                        StartCoroutine(playVoiceOver(stopAtJunc));
                        stopAtJuncPlayed = true;
                    }
                    
                    Vector3 waypointCarDiff = juncToStopAt.transform.position - car.transform.position;
                    waypointCarDiff.y = 0;
                    Debug.Log("dist from waypoint: " + waypointCarDiff.magnitude);
                    Debug.Log("speed: " + car.CurrentSpeed);
                    if (waypointCarDiff.magnitude < 3 && car.CurrentSpeed < 0.1f && !stopAtJunc.isPlaying)
                    {
                        step++;
                    }
                    break;

                case 5:
                    bool rightClear = !juncR.hasCar();
                    bool aheadClear = !juncA.hasCar();
                    bool leftClear = !juncL.hasCar();

                    if (rightClear)
                    {
                        if (!clearPlayed && !waitHere.isPlaying && !gridlock.isPlaying && !stall.isPlaying)
                        {
                            StartCoroutine(playVoiceOver(clear));
                            clearPlayed = true;
                        }
                    }
                    else if (!rightClear && !aheadClear && !leftClear)
                    {
                        if (!gridlockPlayed && !clear.isPlaying && !waitHere.isPlaying && !stall.isPlaying)
                        {
                            StartCoroutine(playVoiceOver(gridlock));
                            gridlockPlayed = true;
                        }
                    }
                    else
                    {
                        if (!waitHerePlayed && !gridlock.isPlaying && !clear.isPlaying && !stall.isPlaying)
                        {
                            StartCoroutine(playVoiceOver(waitHere));
                            gridlockPlayed = true;
                        }
                    }

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

                    if (passedDestination && !gridlock.isPlaying && !clear.isPlaying && !waitHere.isPlaying)
                    {
                        step++;
                    }

                    break;
                case 6:
                    if (!outroPlayed && !stall.isPlaying)
                    {
                        PlayerPrefs.SetInt("Stop sign", 1);
                        StartCoroutine(playVoiceOver(outro));
                        outroPlayed = true;
                    }

                    if (!xPlayed && !outro.isPlaying && !stall.isPlaying)
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

            if (car.stalled && !outroPlayed)
            {
                startCar.Stop();
                directionStraight.Stop();
                directionLeft.Stop();
                directionRight.Stop();
                indicate.Stop();
                moveRight.Stop();
                moveLeft.Stop();
                stopAtJunc.Stop();
                gridlock.Stop();
                waitHere.Stop();
                clear.Stop();
                outro.Stop();
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
                            stopAtJuncPlayed = false;
                            break;
                        case 5:
                            clearPlayed = false;
                            gridlockPlayed = false;
                            waitHerePlayed = false;
                            break;
                    }
                }

            }
         }

    }

    private void setCarsActive()
    {
        for (int x = 0; x < noOfActiveCars; x++)
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
