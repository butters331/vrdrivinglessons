using UnityStandardAssets.Vehicles.Car;

public class ThreeWayWaypoint : Waypoint
{
    public Waypoint leftWaypoint;
    public Waypoint rightWaypoint;

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

                default:
                    nextWaypoint.nextWaypoint = leftWaypoint;
                    carAi.turning = 1;
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

                default:
                    nextWaypoint = leftWaypoint;
                    carAi.turning = 1;
                    break;
            }
        }
        
    }
public void setCarThreeWay(CarController newCar)
    {
        setCar(newCar);
        chooseDirection();
    }

}
