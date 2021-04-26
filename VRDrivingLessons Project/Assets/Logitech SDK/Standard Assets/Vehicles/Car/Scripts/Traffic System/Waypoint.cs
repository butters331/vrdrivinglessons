/*
  ** This code for waypoints is based off a tutorial by @GameDevGuide on their YouTube video [Building a Traffic System in Unity, 2019]
 */
using System.Collections;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

public class Waypoint : MonoBehaviour
{
    public Waypoint previousWaypoint;
    public Waypoint nextWaypoint;
    public int speedLimit;
    public int noOfDirections = 1;
    public bool isTrafficLightWaypoint = false;
    public bool dontWorryAboutLanes = false;
    public bool stopSign = false;
    public bool nextIsJunc = false;
    public bool afterJunc = false;
    public Light greenLight;

    public Waypoint juncAhead;
    public Waypoint juncLeft;
    public Waypoint juncRight;
    public int priority;
    public bool missingRight = false;
    public bool missingLeft = false;
    public bool missingAhead = false;

    private bool currentlyHasCar = false;
    private bool carAlreadyStopped = false;
    public CarController car;

    [Range(0f, 5f)]
    public float width = 1f;

    private void Update()
    {

        if (hasCar())
        {
            CarAIControl aiController = getCarAI();
            if (isTrafficLightWaypoint)
            {
                if (greenLight.enabled)
                {
                    if (dontWorryAboutLanes)
                    {
                        aiController.aiStart();
                    }
                    else
                    {
                        if (aiController.turning == 1)
                        {
                            Debug.Log(car.ToString() + "turning left");
                            if (carInLeftLane())
                            {
                                if (juncAhead.hasCar() && juncAhead.getCarAI().turning == 1)
                                {
                                    Debug.Log(car.ToString() + "opposite car turning left");
                                    if (priority < juncAhead.priority)
                                    {
                                        Debug.Log(car.ToString() + "has priority");
                                        aiController.aiStart();
                                    }
                                    else
                                    {
                                        Vector3 distanceToWaypoint = car.transform.position - transform.position;
                                        distanceToWaypoint.y = 0;
                                        if (distanceToWaypoint.magnitude < 8 && !carAlreadyStopped)
                                        {
                                            carAlreadyStopped = true;
                                            aiController.aiStop();
                                        }
                                        StartCoroutine(WaitAtJunc());
                                    }
                                }
                                else
                                {
                                    Debug.Log(car.ToString() + " were waiting!");
                                    Vector3 distanceToWaypoint = car.transform.position - transform.position;
                                    distanceToWaypoint.y = 0;
                                    if (distanceToWaypoint.magnitude < 8 && !carAlreadyStopped)
                                    {
                                        carAlreadyStopped = true;
                                        aiController.aiStop();
                                    }
                                    StartCoroutine(WaitAtJunc());
                                }
                            }
                            else
                            {
                                Debug.Log(car.ToString() + "no car in left lane, continue");
                                aiController.aiStart();
                            }
                        }
                        else
                        {
                            Debug.Log(car.ToString() + "not set as turning");
                            aiController.aiStart();
                        }
                    }
                    
                        
                }
                else
                {
                    Vector3 distanceToWaypoint = car.transform.position - transform.position;
                    distanceToWaypoint.y = 0;
                    Debug.Log(car.ToString() + "not green");
                    if (distanceToWaypoint.magnitude < 8 && !carAlreadyStopped)
                    {
                        carAlreadyStopped = true;
                        aiController.aiStop();
                    }
                        
                }
            }
            
            if (stopSign)
            {
                Vector3 distanceToWaypoint = car.transform.position - transform.position;
                distanceToWaypoint.y = 0;
                if (distanceToWaypoint.magnitude < 8 && !carAlreadyStopped)
                {
                    aiController.aiStop();
                    carAlreadyStopped = true;
                }

                if (car.CurrentSpeed < 0.1f)
                {
                    if (!missingAhead && !missingRight && !missingLeft)
                    {
                        if (carInCentre())
                        {
                            StartCoroutine(WaitAtJunc());
                        }
                        else
                        {
                            if (juncRight.hasCar() && juncLeft.hasCar() && juncAhead.hasCar())
                            {
                                if (priority < juncAhead.priority && priority < juncLeft.priority && priority < juncRight.priority)
                                {
                                    aiController.aiStart();
                                }
                            }
                            else if ((!juncRight.hasCar() && !juncLeft.hasCar() && !juncAhead.hasCar()) || (!juncRight.hasCar() && juncLeft.hasCar() && juncAhead.hasCar()))
                            {
                                aiController.aiStart();
                            }
                            else if (!juncRight.hasCar() && !juncLeft.hasCar() && juncAhead.hasCar())
                            {
                                if (aiController.turning != 1)
                                {
                                    aiController.aiStart();
                                }
                                else
                                {
                                    if (juncAhead.getCarAI().turning == 1)
                                    {
                                        if (priority < juncAhead.priority)
                                        {
                                            aiController.aiStart();
                                        }
                                        else
                                        {
                                            StartCoroutine(WaitAtJunc());
                                        }
                                    }
                                    else
                                    {
                                        StartCoroutine(WaitAtJunc());
                                    }
                                }

                            }
                            else if (juncRight.hasCar())
                            {
                                StartCoroutine(WaitAtJunc());
                            }
                            else
                            {
                                aiController.aiStart();
                            }
                        }
                        

                    }
                }
            }
            if (afterJunc)
            {
                aiController.turning = 0;
            }
        }
        
    }

    private bool carInCentre()
    {
        bool rightCarInCentre = ((FourWayWaypoint)juncRight.previousWaypoint).rightWaypoint.hasCar() || ((FourWayWaypoint)juncRight.previousWaypoint).rightWaypoint.nextWaypoint.hasCar()
            || ((FourWayWaypoint)juncRight.previousWaypoint).aheadWayPoint.hasCar() ||
            ((FourWayWaypoint)juncRight.previousWaypoint).leftWaypoint.hasCar() || ((FourWayWaypoint)juncRight.previousWaypoint).leftWaypoint.nextWaypoint.hasCar() || ((FourWayWaypoint)juncRight.previousWaypoint).leftWaypoint.nextWaypoint.nextWaypoint.hasCar();
        bool aheadCarInCentre = ((FourWayWaypoint)juncAhead.previousWaypoint).rightWaypoint.hasCar() || ((FourWayWaypoint)juncAhead.previousWaypoint).rightWaypoint.nextWaypoint.hasCar()
            || ((FourWayWaypoint)juncAhead.previousWaypoint).aheadWayPoint.hasCar() ||
            ((FourWayWaypoint)juncAhead.previousWaypoint).leftWaypoint.hasCar() || ((FourWayWaypoint)juncAhead.previousWaypoint).leftWaypoint.nextWaypoint.hasCar() || ((FourWayWaypoint)juncAhead.previousWaypoint).leftWaypoint.nextWaypoint.nextWaypoint.hasCar();
        bool leftCarInCentre = ((FourWayWaypoint)juncLeft.previousWaypoint).rightWaypoint.hasCar() || ((FourWayWaypoint)juncLeft.previousWaypoint).rightWaypoint.nextWaypoint.hasCar()
            || ((FourWayWaypoint)juncAhead.previousWaypoint).aheadWayPoint.hasCar() ||
            ((FourWayWaypoint)juncLeft.previousWaypoint).leftWaypoint.hasCar() || ((FourWayWaypoint)juncLeft.previousWaypoint).leftWaypoint.nextWaypoint.hasCar() || ((FourWayWaypoint)juncLeft.previousWaypoint).leftWaypoint.nextWaypoint.nextWaypoint.hasCar();

        return (rightCarInCentre || aheadCarInCentre || leftCarInCentre);
    }

    private bool carInLeftLane()
    {
        bool aheadCarInCentre = juncAhead.hasCar() || ((FourWayWaypoint)juncAhead.previousWaypoint).rightWaypoint.hasCar()
            || ((FourWayWaypoint)juncAhead.previousWaypoint).aheadWayPoint.hasCar() ||
            ((FourWayWaypoint)juncAhead.previousWaypoint).leftWaypoint.hasCar() || ((FourWayWaypoint)juncAhead.previousWaypoint).leftWaypoint.nextWaypoint.hasCar() || ((FourWayWaypoint)juncAhead.previousWaypoint).leftWaypoint.nextWaypoint.nextWaypoint.hasCar(); 
        return aheadCarInCentre;
    }

    public void setNoDirections(int number)
    {
        noOfDirections = number;
    }
    public int getNoDirections()
    {
        return noOfDirections;
    }
    public Vector3 getPosition()
    {
        return transform.position;
    }

    public bool hasCar()
    {
        return currentlyHasCar;
    }
    public void setCar(CarController newCar)
    {
        carAlreadyStopped = false;
        currentlyHasCar = true;
        car = newCar;
    }

    public void removeCar()
    {
        carAlreadyStopped = false;
        currentlyHasCar = false;
        car = null;
    }

    public CarController getCar()
    {
        return car;
    }

    public CarAIControl getCarAI()
    {
        return car.GetComponentInParent<CarAIControl>();
    }

    private IEnumerator WaitAtJunc()
    {
        yield return new WaitForSeconds(3);
    }

}
