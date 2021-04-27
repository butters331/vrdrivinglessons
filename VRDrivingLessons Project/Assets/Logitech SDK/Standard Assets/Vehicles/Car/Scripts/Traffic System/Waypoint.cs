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
    public bool tJunc = false;
    public int safeDistance = 2;
    public bool leftCrossing = false;
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
    private bool stoppedForCar = false;
    public CarController car;

    [Range(0f, 5f)]
    public float width = 1f;

    private void Update()
    {

        if (hasCar())
        {
            CarAIControl aiController = getCarAI();
            if (missingLeft && aiController.turning == 1)
            {
                aiController.turning = 0;
            }
            else if (missingRight && aiController.turning == 2)
            {
                aiController.turning = 0;
            }
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

                            if (carInLeftLane())
                            {
                                if (juncAhead.hasCar() && juncAhead.getCarAI().turning == 1)
                                {
                                    if (priority < juncAhead.priority)
                                    {
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
                                aiController.aiStart();
                            }
                        }
                        else
                        {
                            aiController.aiStart();
                        }
                    }
                    
                        
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
                    else if (missingRight)
                    {
                        if (carInCentre())
                        {
                            StartCoroutine(WaitAtJunc());
                        }
                        else
                        {
                            if (!juncAhead.hasCar())
                                aiController.aiStart();
                            else
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
                        }
                    }

                    else if (missingAhead)
                    {
                        if (carInCentre() || juncRight.hasCar())
                        {
                            StartCoroutine(WaitAtJunc());
                        }
                        else
                        {
                            aiController.aiStart();
                        }
                    }

                    else // missing left
                    {
                        if (carInCentre() || juncRight.hasCar())
                        {
                            StartCoroutine(WaitAtJunc());
                        }
                        else
                        {
                            if (!juncAhead.hasCar())
                                aiController.aiStart();
                            else
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
                        }
                    }
                }
            }

            if (tJunc)
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
                    if (aiController.turning == 1)
                    {
                        if (directionClear(juncLeft) && directionClear(juncRight))
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
                        if (directionClear(juncLeft))
                        {
                            aiController.aiStart();
                        }
                        else
                        {
                            StartCoroutine(WaitAtJunc());
                        }
                    }
                }
                
            }
            if (leftCrossing)
            {
                if (aiController.turning == 1)
                {
                    if (directionClear(juncAhead))
                    {
                        aiController.aiStart();
                    }
                    else
                    {
                        aiController.aiStop();
                        StartCoroutine(WaitAtJunc());
                    }
                }
            }

            if (afterJunc)
            {
                aiController.turning = 0;
            }

            if (hasCar() && nextWaypoint.hasCar() && nextWaypoint.getCar().CurrentSpeed < car.CurrentSpeed && !isTrafficLightWaypoint && !stopSign && !tJunc)
            {
                stoppedForCar = true;
                aiController.aiStop();
            }
            else if (stoppedForCar)
            {
                stoppedForCar = false;
                aiController.aiStart();
            }
        }
        
    }

    private bool directionClear(Waypoint direction)
    {
        bool carFound = false;
        Waypoint tempWayPoint = direction;
        for (int x = 0; x < safeDistance; x++)
        {
            carFound = carFound || tempWayPoint.hasCar();
            tempWayPoint = tempWayPoint.previousWaypoint;
        }
        return !carFound;
    }

    private bool carInCentre()
    {
        bool rightCarInCentre;
        bool aheadCarInCentre;
        bool leftCarInCentre;
        if (noOfDirections == 3)
        {
            if (nextIsJunc)
            {
                rightCarInCentre = ((FourWayWaypoint)juncRight.previousWaypoint).rightWaypoint.hasCar() || ((FourWayWaypoint)juncRight.previousWaypoint).rightWaypoint.nextWaypoint.hasCar()
                    || ((FourWayWaypoint)juncRight.previousWaypoint).aheadWayPoint.hasCar() ||
                    ((FourWayWaypoint)juncRight.previousWaypoint).leftWaypoint.hasCar() || ((FourWayWaypoint)juncRight.previousWaypoint).leftWaypoint.nextWaypoint.hasCar() || ((FourWayWaypoint)juncRight.previousWaypoint).leftWaypoint.nextWaypoint.nextWaypoint.hasCar();
                aheadCarInCentre = ((FourWayWaypoint)juncAhead.previousWaypoint).rightWaypoint.hasCar() || ((FourWayWaypoint)juncAhead.previousWaypoint).rightWaypoint.nextWaypoint.hasCar()
                    || ((FourWayWaypoint)juncAhead.previousWaypoint).aheadWayPoint.hasCar() ||
                    ((FourWayWaypoint)juncAhead.previousWaypoint).leftWaypoint.hasCar() || ((FourWayWaypoint)juncAhead.previousWaypoint).leftWaypoint.nextWaypoint.hasCar() || ((FourWayWaypoint)juncAhead.previousWaypoint).leftWaypoint.nextWaypoint.nextWaypoint.hasCar();
                leftCarInCentre = ((FourWayWaypoint)juncLeft.previousWaypoint).rightWaypoint.hasCar() || ((FourWayWaypoint)juncLeft.previousWaypoint).rightWaypoint.nextWaypoint.hasCar()
                    || ((FourWayWaypoint)juncAhead.previousWaypoint).aheadWayPoint.hasCar() ||
                    ((FourWayWaypoint)juncLeft.previousWaypoint).leftWaypoint.hasCar() || ((FourWayWaypoint)juncLeft.previousWaypoint).leftWaypoint.nextWaypoint.hasCar() || ((FourWayWaypoint)juncLeft.previousWaypoint).leftWaypoint.nextWaypoint.nextWaypoint.hasCar();
            }
            else
            {
                rightCarInCentre = ((FourWayWaypoint)juncRight).rightWaypoint.hasCar() || ((FourWayWaypoint)juncRight).rightWaypoint.nextWaypoint.hasCar()
                    || ((FourWayWaypoint)juncRight).aheadWayPoint.hasCar() || ((FourWayWaypoint)juncRight).leftWaypoint.hasCar()
                    || ((FourWayWaypoint)juncRight).leftWaypoint.nextWaypoint.hasCar() || ((FourWayWaypoint)juncRight).leftWaypoint.nextWaypoint.nextWaypoint.hasCar();
                aheadCarInCentre = ((FourWayWaypoint)juncAhead).rightWaypoint.hasCar() || ((FourWayWaypoint)juncAhead).rightWaypoint.nextWaypoint.hasCar()
                    || ((FourWayWaypoint)juncAhead).aheadWayPoint.hasCar() || ((FourWayWaypoint)juncAhead).leftWaypoint.hasCar()
                    || ((FourWayWaypoint)juncAhead).leftWaypoint.nextWaypoint.hasCar() || ((FourWayWaypoint)juncAhead).leftWaypoint.nextWaypoint.nextWaypoint.hasCar();
                leftCarInCentre = ((FourWayWaypoint)juncLeft).rightWaypoint.hasCar() || ((FourWayWaypoint)juncLeft).rightWaypoint.nextWaypoint.hasCar()
                    || ((FourWayWaypoint)juncAhead).aheadWayPoint.hasCar() || ((FourWayWaypoint)juncLeft).leftWaypoint.hasCar() 
                    || ((FourWayWaypoint)juncLeft).leftWaypoint.nextWaypoint.hasCar() || ((FourWayWaypoint)juncLeft).leftWaypoint.nextWaypoint.nextWaypoint.hasCar();
            }

            

            return (rightCarInCentre || aheadCarInCentre || leftCarInCentre);
        }
        else
        {
            if (nextIsJunc)
            {
                if (missingRight)
                {
                    aheadCarInCentre = ((ThreeWayWaypoint)juncAhead.previousWaypoint).rightWaypoint.hasCar() || ((ThreeWayWaypoint)juncAhead.previousWaypoint).rightWaypoint.nextWaypoint.hasCar()
                        || ((ThreeWayWaypoint)juncAhead.previousWaypoint).leftWaypoint.hasCar() || ((ThreeWayWaypoint)juncAhead.previousWaypoint).leftWaypoint.nextWaypoint.hasCar() || ((ThreeWayWaypoint)juncAhead.previousWaypoint).leftWaypoint.nextWaypoint.nextWaypoint.hasCar();
                    leftCarInCentre = ((ThreeWayWaypoint)juncLeft.previousWaypoint).rightWaypoint.hasCar() || ((ThreeWayWaypoint)juncLeft.previousWaypoint).rightWaypoint.nextWaypoint.hasCar()
                        || ((ThreeWayWaypoint)juncLeft.previousWaypoint).leftWaypoint.hasCar() || ((ThreeWayWaypoint)juncLeft.previousWaypoint).leftWaypoint.nextWaypoint.hasCar() || ((ThreeWayWaypoint)juncLeft.previousWaypoint).leftWaypoint.nextWaypoint.nextWaypoint.hasCar();

                    return (aheadCarInCentre || leftCarInCentre);
                }
                else if (missingLeft)
                {
                    aheadCarInCentre = ((ThreeWayWaypoint)juncAhead.previousWaypoint).rightWaypoint.hasCar() || ((ThreeWayWaypoint)juncAhead.previousWaypoint).rightWaypoint.nextWaypoint.hasCar()
                        || ((ThreeWayWaypoint)juncAhead.previousWaypoint).leftWaypoint.hasCar() || ((ThreeWayWaypoint)juncAhead.previousWaypoint).leftWaypoint.nextWaypoint.hasCar() || ((ThreeWayWaypoint)juncAhead.previousWaypoint).leftWaypoint.nextWaypoint.nextWaypoint.hasCar();
                    rightCarInCentre = ((ThreeWayWaypoint)juncRight.previousWaypoint).rightWaypoint.hasCar() || ((ThreeWayWaypoint)juncRight.previousWaypoint).rightWaypoint.nextWaypoint.hasCar()
                    || ((ThreeWayWaypoint)juncRight.previousWaypoint).leftWaypoint.hasCar() || ((ThreeWayWaypoint)juncRight.previousWaypoint).leftWaypoint.nextWaypoint.hasCar() || ((ThreeWayWaypoint)juncRight.previousWaypoint).leftWaypoint.nextWaypoint.nextWaypoint.hasCar();

                    return (aheadCarInCentre || rightCarInCentre);
                }
                else
                {
                    leftCarInCentre = ((ThreeWayWaypoint)juncLeft.previousWaypoint).rightWaypoint.hasCar() || ((ThreeWayWaypoint)juncLeft.previousWaypoint).rightWaypoint.nextWaypoint.hasCar()
                        || ((ThreeWayWaypoint)juncLeft.previousWaypoint).leftWaypoint.hasCar() || ((ThreeWayWaypoint)juncLeft.previousWaypoint).leftWaypoint.nextWaypoint.hasCar() || ((ThreeWayWaypoint)juncLeft.previousWaypoint).leftWaypoint.nextWaypoint.nextWaypoint.hasCar();
                    rightCarInCentre = ((ThreeWayWaypoint)juncRight.previousWaypoint).rightWaypoint.hasCar() || ((ThreeWayWaypoint)juncRight.previousWaypoint).rightWaypoint.nextWaypoint.hasCar()
                    || ((ThreeWayWaypoint)juncRight.previousWaypoint).leftWaypoint.hasCar() || ((ThreeWayWaypoint)juncRight.previousWaypoint).leftWaypoint.nextWaypoint.hasCar() || ((ThreeWayWaypoint)juncRight.previousWaypoint).leftWaypoint.nextWaypoint.nextWaypoint.hasCar();

                    return (leftCarInCentre || rightCarInCentre);
                }
            }
            else
            {
                if (missingRight)
                {
                    aheadCarInCentre = ((ThreeWayWaypoint)juncAhead).rightWaypoint.hasCar() || ((ThreeWayWaypoint)juncAhead).rightWaypoint.nextWaypoint.hasCar()
                        || ((ThreeWayWaypoint)juncAhead).leftWaypoint.hasCar() || ((ThreeWayWaypoint)juncAhead).leftWaypoint.nextWaypoint.hasCar() || ((ThreeWayWaypoint)juncAhead).leftWaypoint.nextWaypoint.nextWaypoint.hasCar();
                    leftCarInCentre = ((ThreeWayWaypoint)juncLeft).rightWaypoint.hasCar() || ((ThreeWayWaypoint)juncLeft).rightWaypoint.nextWaypoint.hasCar()
                        || ((ThreeWayWaypoint)juncLeft).leftWaypoint.hasCar() || ((ThreeWayWaypoint)juncLeft).leftWaypoint.nextWaypoint.hasCar() || ((ThreeWayWaypoint)juncLeft).leftWaypoint.nextWaypoint.nextWaypoint.hasCar();

                    return (aheadCarInCentre || leftCarInCentre);
                }
                else if (missingLeft)
                {
                    aheadCarInCentre = ((ThreeWayWaypoint)juncAhead).rightWaypoint.hasCar() || ((ThreeWayWaypoint)juncAhead).rightWaypoint.nextWaypoint.hasCar()
                        || ((ThreeWayWaypoint)juncAhead).leftWaypoint.hasCar() || ((ThreeWayWaypoint)juncAhead).leftWaypoint.nextWaypoint.hasCar() || ((ThreeWayWaypoint)juncAhead).leftWaypoint.nextWaypoint.nextWaypoint.hasCar();
                    rightCarInCentre = ((ThreeWayWaypoint)juncRight).rightWaypoint.hasCar() || ((ThreeWayWaypoint)juncRight).rightWaypoint.nextWaypoint.hasCar()
                    || ((ThreeWayWaypoint)juncRight).leftWaypoint.hasCar() || ((ThreeWayWaypoint)juncRight).leftWaypoint.nextWaypoint.hasCar() || ((ThreeWayWaypoint)juncRight).leftWaypoint.nextWaypoint.nextWaypoint.hasCar();

                    return (aheadCarInCentre || rightCarInCentre);
                }
                else
                {
                    leftCarInCentre = ((ThreeWayWaypoint)juncLeft).rightWaypoint.hasCar() || ((ThreeWayWaypoint)juncLeft).rightWaypoint.nextWaypoint.hasCar()
                        || ((ThreeWayWaypoint)juncLeft).leftWaypoint.hasCar() || ((ThreeWayWaypoint)juncLeft).leftWaypoint.nextWaypoint.hasCar() || ((ThreeWayWaypoint)juncLeft).leftWaypoint.nextWaypoint.nextWaypoint.hasCar();
                    rightCarInCentre = ((ThreeWayWaypoint)juncRight).rightWaypoint.hasCar() || ((ThreeWayWaypoint)juncRight).rightWaypoint.nextWaypoint.hasCar()
                    || ((ThreeWayWaypoint)juncRight).leftWaypoint.hasCar() || ((ThreeWayWaypoint)juncRight).leftWaypoint.nextWaypoint.hasCar() || ((ThreeWayWaypoint)juncRight).leftWaypoint.nextWaypoint.nextWaypoint.hasCar();

                    return (leftCarInCentre || rightCarInCentre);
                }
            }
            
        }
        
    }

    private bool carInLeftLane()
    {

        bool aheadCarInCentre;
        if (juncAhead.noOfDirections == 3)
        {
            aheadCarInCentre = juncAhead.hasCar() || ((FourWayWaypoint)juncAhead.previousWaypoint).rightWaypoint.hasCar()
                || ((FourWayWaypoint)juncAhead.previousWaypoint).aheadWayPoint.hasCar() ||
                ((FourWayWaypoint)juncAhead.previousWaypoint).leftWaypoint.hasCar() || ((FourWayWaypoint)juncAhead.previousWaypoint).leftWaypoint.nextWaypoint.hasCar()
                || ((FourWayWaypoint)juncAhead.previousWaypoint).leftWaypoint.nextWaypoint.nextWaypoint.hasCar();
        }
        else
        {
            aheadCarInCentre = juncAhead.hasCar() || ((ThreeWayWaypoint)juncAhead.previousWaypoint).rightWaypoint.hasCar()
                || ((ThreeWayWaypoint)juncAhead.previousWaypoint).rightWaypoint.nextWaypoint.hasCar() || ((ThreeWayWaypoint)juncAhead.previousWaypoint).leftWaypoint.hasCar();
        }
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
