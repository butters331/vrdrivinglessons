using UnityStandardAssets.Vehicles.Car;
using UnityEngine;
public class FourWayWaypoint : Waypoint
{
    public Waypoint leftWaypoint;
    public Waypoint rightWaypoint;
    public Waypoint aheadWayPoint;

    public void chooseDirection()
    {
        System.Random random = new System.Random();
        int chosenDirection = random.Next(0, 2);
        CarAIControl carAi = getCarAI();

        if (nextIsJunc)
        {
            switch (chosenDirection)
            {
                case 0:
                    nextWaypoint.nextWaypoint = leftWaypoint;
                    carAi.turning = 1;
                    break;
                case 1:
                    nextWaypoint.nextWaypoint = rightWaypoint;
                    carAi.turning = 2;
                    break;
                case 2:
                    nextWaypoint.nextWaypoint = aheadWayPoint;
                    carAi.turning = 0;
                    break;

                default:
                    nextWaypoint.nextWaypoint = aheadWayPoint;
                    carAi.turning = 0;
                    break;
            }
        }
        else
        {
            switch (chosenDirection)
            {
                case 0:
                    nextWaypoint = leftWaypoint;
                    carAi.turning = 1;
                    break;
                case 1:
                    nextWaypoint = rightWaypoint;
                    carAi.turning = 2;
                    break;
                case 2:
                    nextWaypoint = aheadWayPoint;
                    carAi.turning = 0;
                    break;

                default:
                    nextWaypoint = aheadWayPoint;
                    carAi.turning = 0;
                    break;
            }
        }
        
    }

    public void setCarFourWay(CarController newCar)
    {
        setCar(newCar);
        chooseDirection();
    }
}
