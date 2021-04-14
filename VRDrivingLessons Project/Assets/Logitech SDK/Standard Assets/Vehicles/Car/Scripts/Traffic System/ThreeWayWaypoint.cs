using UnityStandardAssets.Vehicles.Car;

public class ThreeWayWaypoint : Waypoint
{
    public Waypoint leftWaypoint;
    public Waypoint rightWaypoint;

    public void chooseDirection()
    {
        System.Random random = new System.Random();
        int chosenDirection = random.Next(0, 2);

        switch (chosenDirection)
        {
            case 0:
                nextWaypoint = leftWaypoint;
                break;
            case 1:
                nextWaypoint = rightWaypoint;
                break;

            default:
                nextWaypoint = leftWaypoint;
                break;
        }
    }

    public new void setCar(CarController newCar)
    {
        chooseDirection();
        base.setCar(newCar);
    }

}
