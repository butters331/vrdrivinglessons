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

    private GameObject userCar;
    private CarController userController;

    [Range(0f, 5f)]
    public float width = 1f;

    private void Start()
    {
        userCar = GameObject.Find("Car");
        userController = userCar.GetComponent<CarController>();
    }

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
                            if (juncRight.hasCarThere() && juncLeft.hasCarThere() && juncAhead.hasCarThere())
                            {
                                if (priority < juncAhead.priority && priority < juncLeft.priority && priority < juncRight.priority)
                                {
                                    aiController.aiStart();
                                }
                            }
                            else if ((!juncRight.hasCarThere() && !juncLeft.hasCarThere() && !juncAhead.hasCarThere()) || (!juncRight.hasCarThere() && juncLeft.hasCarThere() && juncAhead.hasCarThere()))
                            {
                                aiController.aiStart();
                            }
                            else if (!juncRight.hasCarThere() && !juncLeft.hasCarThere() && juncAhead.hasCarThere())
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
                            else if (juncRight.hasCarThere())
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

            if (hasCar() && nextWaypoint.hasCarThere() && nextWaypoint.getCar().CurrentSpeed < car.CurrentSpeed && !isTrafficLightWaypoint && !stopSign && !tJunc)
            {
                stoppedForCar = true;
                aiController.aiStop();
            }
            else if (hasCar() && !nextWaypoint.hasCarThere() && stoppedForCar)
            {
                stoppedForCar = false;
                aiController.aiStart();
            }

            //handle interactions with user car
            
        }
        
    }

    private bool directionClear(Waypoint direction)
    {
        bool carFound = false;
        Waypoint tempWayPoint = direction;
        for (int x = 0; x < safeDistance; x++)
        {
            carFound = carFound || tempWayPoint.hasCarThere();
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
            if (previousWaypoint.nextIsJunc)
            {
                rightCarInCentre = ((FourWayWaypoint)juncRight.previousWaypoint).rightWaypoint.hasCarThere() || ((FourWayWaypoint)juncRight.previousWaypoint).rightWaypoint.nextWaypoint.hasCarThere()
                    || ((FourWayWaypoint)juncRight.previousWaypoint).aheadWayPoint.hasCarThere() ||
                    ((FourWayWaypoint)juncRight.previousWaypoint).leftWaypoint.hasCarThere() || ((FourWayWaypoint)juncRight.previousWaypoint).leftWaypoint.nextWaypoint.hasCarThere() || ((FourWayWaypoint)juncRight.previousWaypoint).leftWaypoint.nextWaypoint.nextWaypoint.hasCarThere();
                aheadCarInCentre = ((FourWayWaypoint)juncAhead.previousWaypoint).rightWaypoint.hasCarThere() || ((FourWayWaypoint)juncAhead.previousWaypoint).rightWaypoint.nextWaypoint.hasCarThere()
                    || ((FourWayWaypoint)juncAhead.previousWaypoint).aheadWayPoint.hasCarThere() ||
                    ((FourWayWaypoint)juncAhead.previousWaypoint).leftWaypoint.hasCarThere() || ((FourWayWaypoint)juncAhead.previousWaypoint).leftWaypoint.nextWaypoint.hasCarThere() || ((FourWayWaypoint)juncAhead.previousWaypoint).leftWaypoint.nextWaypoint.nextWaypoint.hasCarThere();
                leftCarInCentre = ((FourWayWaypoint)juncLeft.previousWaypoint).rightWaypoint.hasCarThere() || ((FourWayWaypoint)juncLeft.previousWaypoint).rightWaypoint.nextWaypoint.hasCarThere()
                    || ((FourWayWaypoint)juncAhead.previousWaypoint).aheadWayPoint.hasCarThere() ||
                    ((FourWayWaypoint)juncLeft.previousWaypoint).leftWaypoint.hasCarThere() || ((FourWayWaypoint)juncLeft.previousWaypoint).leftWaypoint.nextWaypoint.hasCarThere() || ((FourWayWaypoint)juncLeft.previousWaypoint).leftWaypoint.nextWaypoint.nextWaypoint.hasCarThere();
            }
            else
            {
                rightCarInCentre = ((FourWayWaypoint)juncRight).rightWaypoint.hasCarThere() || ((FourWayWaypoint)juncRight).rightWaypoint.nextWaypoint.hasCarThere()
                    || ((FourWayWaypoint)juncRight).aheadWayPoint.hasCarThere() || ((FourWayWaypoint)juncRight).leftWaypoint.hasCarThere()
                    || ((FourWayWaypoint)juncRight).leftWaypoint.nextWaypoint.hasCarThere() || ((FourWayWaypoint)juncRight).leftWaypoint.nextWaypoint.nextWaypoint.hasCarThere();
                aheadCarInCentre = ((FourWayWaypoint)juncAhead).rightWaypoint.hasCarThere() || ((FourWayWaypoint)juncAhead).rightWaypoint.nextWaypoint.hasCarThere()
                    || ((FourWayWaypoint)juncAhead).aheadWayPoint.hasCarThere() || ((FourWayWaypoint)juncAhead).leftWaypoint.hasCarThere()
                    || ((FourWayWaypoint)juncAhead).leftWaypoint.nextWaypoint.hasCarThere() || ((FourWayWaypoint)juncAhead).leftWaypoint.nextWaypoint.nextWaypoint.hasCarThere();
                leftCarInCentre = ((FourWayWaypoint)juncLeft).rightWaypoint.hasCarThere() || ((FourWayWaypoint)juncLeft).rightWaypoint.nextWaypoint.hasCarThere()
                    || ((FourWayWaypoint)juncAhead).aheadWayPoint.hasCarThere() || ((FourWayWaypoint)juncLeft).leftWaypoint.hasCarThere() 
                    || ((FourWayWaypoint)juncLeft).leftWaypoint.nextWaypoint.hasCarThere() || ((FourWayWaypoint)juncLeft).leftWaypoint.nextWaypoint.nextWaypoint.hasCarThere();
            }

            

            return (rightCarInCentre || aheadCarInCentre || leftCarInCentre);
        }
        else
        {   
            if (nextIsJunc)
            {
                if (missingRight)
                {
                    aheadCarInCentre = ((ThreeWayWaypoint)juncAhead.previousWaypoint).rightWaypoint.hasCarThere() || ((ThreeWayWaypoint)juncAhead.previousWaypoint).rightWaypoint.nextWaypoint.hasCarThere()
                        || ((ThreeWayWaypoint)juncAhead.previousWaypoint).leftWaypoint.hasCarThere() || ((ThreeWayWaypoint)juncAhead.previousWaypoint).leftWaypoint.nextWaypoint.hasCarThere() || ((ThreeWayWaypoint)juncAhead.previousWaypoint).leftWaypoint.nextWaypoint.nextWaypoint.hasCarThere();
                    leftCarInCentre = ((ThreeWayWaypoint)juncLeft.previousWaypoint).rightWaypoint.hasCarThere() || ((ThreeWayWaypoint)juncLeft.previousWaypoint).rightWaypoint.nextWaypoint.hasCarThere()
                        || ((ThreeWayWaypoint)juncLeft.previousWaypoint).leftWaypoint.hasCarThere() || ((ThreeWayWaypoint)juncLeft.previousWaypoint).leftWaypoint.nextWaypoint.hasCarThere() || ((ThreeWayWaypoint)juncLeft.previousWaypoint).leftWaypoint.nextWaypoint.nextWaypoint.hasCarThere();

                    return (aheadCarInCentre || leftCarInCentre);
                }
                else if (missingLeft)
                {
                    aheadCarInCentre = ((ThreeWayWaypoint)juncAhead.previousWaypoint).rightWaypoint.hasCarThere() || ((ThreeWayWaypoint)juncAhead.previousWaypoint).rightWaypoint.nextWaypoint.hasCarThere()
                        || ((ThreeWayWaypoint)juncAhead.previousWaypoint).leftWaypoint.hasCarThere() || ((ThreeWayWaypoint)juncAhead.previousWaypoint).leftWaypoint.nextWaypoint.hasCarThere() || ((ThreeWayWaypoint)juncAhead.previousWaypoint).leftWaypoint.nextWaypoint.nextWaypoint.hasCarThere();
                    rightCarInCentre = ((ThreeWayWaypoint)juncRight.previousWaypoint).rightWaypoint.hasCarThere() || ((ThreeWayWaypoint)juncRight.previousWaypoint).rightWaypoint.nextWaypoint.hasCarThere()
                    || ((ThreeWayWaypoint)juncRight.previousWaypoint).leftWaypoint.hasCarThere() || ((ThreeWayWaypoint)juncRight.previousWaypoint).leftWaypoint.nextWaypoint.hasCarThere() || ((ThreeWayWaypoint)juncRight.previousWaypoint).leftWaypoint.nextWaypoint.nextWaypoint.hasCarThere();

                    return (aheadCarInCentre || rightCarInCentre);
                }
                else
                {
                    leftCarInCentre = ((ThreeWayWaypoint)juncLeft.previousWaypoint).rightWaypoint.hasCarThere() || ((ThreeWayWaypoint)juncLeft.previousWaypoint).rightWaypoint.nextWaypoint.hasCarThere()
                        || ((ThreeWayWaypoint)juncLeft.previousWaypoint).leftWaypoint.hasCarThere() || ((ThreeWayWaypoint)juncLeft.previousWaypoint).leftWaypoint.nextWaypoint.hasCarThere() || ((ThreeWayWaypoint)juncLeft.previousWaypoint).leftWaypoint.nextWaypoint.nextWaypoint.hasCarThere();
                    rightCarInCentre = ((ThreeWayWaypoint)juncRight.previousWaypoint).rightWaypoint.hasCarThere() || ((ThreeWayWaypoint)juncRight.previousWaypoint).rightWaypoint.nextWaypoint.hasCarThere()
                    || ((ThreeWayWaypoint)juncRight.previousWaypoint).leftWaypoint.hasCarThere() || ((ThreeWayWaypoint)juncRight.previousWaypoint).leftWaypoint.nextWaypoint.hasCarThere() || ((ThreeWayWaypoint)juncRight.previousWaypoint).leftWaypoint.nextWaypoint.nextWaypoint.hasCarThere();

                    return (leftCarInCentre || rightCarInCentre);
                }
            }
            else
            {
                if (missingRight)
                {
                    aheadCarInCentre = ((ThreeWayWaypoint)juncAhead).rightWaypoint.hasCarThere() || ((ThreeWayWaypoint)juncAhead).rightWaypoint.nextWaypoint.hasCarThere()
                        || ((ThreeWayWaypoint)juncAhead).leftWaypoint.hasCarThere() || ((ThreeWayWaypoint)juncAhead).leftWaypoint.nextWaypoint.hasCarThere() || ((ThreeWayWaypoint)juncAhead).leftWaypoint.nextWaypoint.nextWaypoint.hasCarThere();
                    leftCarInCentre = ((ThreeWayWaypoint)juncLeft).rightWaypoint.hasCarThere() || ((ThreeWayWaypoint)juncLeft).rightWaypoint.nextWaypoint.hasCarThere()
                        || ((ThreeWayWaypoint)juncLeft).leftWaypoint.hasCarThere() || ((ThreeWayWaypoint)juncLeft).leftWaypoint.nextWaypoint.hasCarThere() || ((ThreeWayWaypoint)juncLeft).leftWaypoint.nextWaypoint.nextWaypoint.hasCarThere();

                    return (aheadCarInCentre || leftCarInCentre);
                }
                else if (missingLeft)
                {
                    aheadCarInCentre = ((ThreeWayWaypoint)juncAhead).rightWaypoint.hasCarThere() || ((ThreeWayWaypoint)juncAhead).rightWaypoint.nextWaypoint.hasCarThere()
                        || ((ThreeWayWaypoint)juncAhead).leftWaypoint.hasCarThere() || ((ThreeWayWaypoint)juncAhead).leftWaypoint.nextWaypoint.hasCarThere() || ((ThreeWayWaypoint)juncAhead).leftWaypoint.nextWaypoint.nextWaypoint.hasCar();
                    rightCarInCentre = ((ThreeWayWaypoint)juncRight).rightWaypoint.hasCarThere() || ((ThreeWayWaypoint)juncRight).rightWaypoint.nextWaypoint.hasCarThere()
                    || ((ThreeWayWaypoint)juncRight).leftWaypoint.hasCarThere() || ((ThreeWayWaypoint)juncRight).leftWaypoint.nextWaypoint.hasCarThere() || ((ThreeWayWaypoint)juncRight).leftWaypoint.nextWaypoint.nextWaypoint.hasCar();

                    return (aheadCarInCentre || rightCarInCentre);
                }
                else
                {
                    leftCarInCentre = ((ThreeWayWaypoint)juncLeft).rightWaypoint.hasCarThere() || ((ThreeWayWaypoint)juncLeft).rightWaypoint.nextWaypoint.hasCarThere()
                        || ((ThreeWayWaypoint)juncLeft).leftWaypoint.hasCarThere() || ((ThreeWayWaypoint)juncLeft).leftWaypoint.nextWaypoint.hasCarThere() || ((ThreeWayWaypoint)juncLeft).leftWaypoint.nextWaypoint.nextWaypoint.hasCarThere();
                    rightCarInCentre = ((ThreeWayWaypoint)juncRight).rightWaypoint.hasCarThere() || ((ThreeWayWaypoint)juncRight).rightWaypoint.nextWaypoint.hasCarThere()
                    || ((ThreeWayWaypoint)juncRight).leftWaypoint.hasCarThere() || ((ThreeWayWaypoint)juncRight).leftWaypoint.nextWaypoint.hasCarThere() || ((ThreeWayWaypoint)juncRight).leftWaypoint.nextWaypoint.nextWaypoint.hasCarThere();

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
            aheadCarInCentre = juncAhead.hasCarThere() || ((FourWayWaypoint)juncAhead.previousWaypoint).rightWaypoint.hasCarThere()
                || ((FourWayWaypoint)juncAhead.previousWaypoint).aheadWayPoint.hasCarThere() ||
                ((FourWayWaypoint)juncAhead.previousWaypoint).leftWaypoint.hasCarThere() || ((FourWayWaypoint)juncAhead.previousWaypoint).leftWaypoint.nextWaypoint.hasCarThere()
                || ((FourWayWaypoint)juncAhead.previousWaypoint).leftWaypoint.nextWaypoint.nextWaypoint.hasCarThere();
        }
        else
        {
            aheadCarInCentre = juncAhead.hasCarThere() || ((ThreeWayWaypoint)juncAhead.previousWaypoint).rightWaypoint.hasCarThere()
                || ((ThreeWayWaypoint)juncAhead.previousWaypoint).rightWaypoint.nextWaypoint.hasCarThere() || ((ThreeWayWaypoint)juncAhead.previousWaypoint).leftWaypoint.hasCarThere();
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

    private bool hasCarThere()
    {
        Vector3 carVector = userController.transform.position;
        Vector3 waypointVector = this.transform.position;
        carVector.y = 0;
        waypointVector.y = 0;

        Vector3 diff = waypointVector - carVector;
        diff.y = 0;

        float angle = Vector3.Angle(carVector, waypointVector);

        return (hasCar() || (diff.magnitude < 6 && angle < 30));
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
