/*
  ** This code for waypoints is based off a tutorial by @GameDevGuide on their YouTube video [Building a Traffic System in Unity, 2019]
 */
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

public class Waypoint : MonoBehaviour
{
    public Waypoint previousWaypoint;
    public Waypoint nextWaypoint;
    public int speedLimit;
    public int noOfDirections = 1;
    private CarController car;

    [Range(0f, 5f)]
    public float width = 1f;

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

    public void setCar(CarController newCar)
    {
        car = newCar;
    }

    public void removeCar()
    {
        car = null;
    }

}
