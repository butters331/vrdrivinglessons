using UnityStandardAssets.Vehicles.Car;
public class FourWayWaypoint : Waypoint
{
    public Waypoint leftWaypoint;
    public Waypoint rightWaypoint;
    public Waypoint aheadWayPoint;

    public void chooseDirection()
    {
        System.Random random = new System.Random();
        int chosenDirection = random.Next(0, 3);

        switch (chosenDirection)
        {
            case 0:
                nextWaypoint = leftWaypoint;
                break;
            case 1:
                nextWaypoint = rightWaypoint;
                break;
            case 2:
                nextWaypoint = aheadWayPoint;
                break;

            default:
                nextWaypoint = aheadWayPoint;
                break;
        }
    }

    public void setCar(CarController newCar)
    {
        chooseDirection();
        base.setCar(newCar);
    }
}
