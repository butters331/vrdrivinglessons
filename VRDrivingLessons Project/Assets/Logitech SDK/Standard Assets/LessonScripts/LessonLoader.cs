using UnityEngine;

public class LessonLoader : MonoBehaviour
{
    private GameObject cameraVariables;
    public GameObject allAroundCheck;
    public GameObject startStop;
    public GameObject gearChange;
    public GameObject trafficLight;
    public GameObject stopSign;
    public GameObject aiCars;
    public GameObject userCar;
    // Start is called before the first frame update
    void Start()
    {
        int noOfcars = aiCars.transform.childCount;
        GameObject [] cars = new GameObject[noOfcars];
        for (int x = 0; x < noOfcars; x++)
        {
            cars[x] = aiCars.transform.GetChild(x).gameObject;
        }

        cameraVariables = GameObject.Find("CameraVariables");
        switch (cameraVariables.GetComponent<CameraVariables>().lessonSelection)
        {
            case 1:
                allAroundCheck.SetActive(true);
                startStop.SetActive(false);
                gearChange.SetActive(false);
                aiCars.SetActive(false);
                trafficLight.SetActive(false);
                stopSign.SetActive(false);
                break;
            case 2:
                allAroundCheck.SetActive(false);
                startStop.SetActive(true);
                gearChange.SetActive(false);
                aiCars.SetActive(false);
                trafficLight.SetActive(false);
                stopSign.SetActive(false);
                break;
            case 3:
                allAroundCheck.SetActive(false);
                startStop.SetActive(false);
                gearChange.SetActive(true);
                aiCars.SetActive(false);
                trafficLight.SetActive(false);
                stopSign.SetActive(false);
                break;
            case 4:
                allAroundCheck.SetActive(false);
                startStop.SetActive(false);
                gearChange.SetActive(false);
                aiCars.SetActive(true);
                trafficLight.SetActive(true);
                stopSign.SetActive(false);

                //so only cars for lesson are in game
                foreach(GameObject car in cars)
                {
                    if (car.ToString().Contains("(4)") || car.ToString().Contains("(3)") || car.ToString().Contains("(11)"))
                    {
                        car.SetActive(true);
                    }
                    else
                    {
                        car.SetActive(false);
                    }
                }
                float keepY1 = userCar.transform.position.y;
                Vector3 tempVec1 = trafficLight.GetComponent<TrafficLights>().juncToStopAt.previousWaypoint.previousWaypoint.previousWaypoint.previousWaypoint.transform.position;
                tempVec1.y = keepY1;
                tempVec1.z += 3;
                userCar.transform.position = tempVec1;
                userCar.transform.forward = trafficLight.GetComponent<TrafficLights>().juncToStopAt.previousWaypoint.previousWaypoint.previousWaypoint.previousWaypoint.transform.forward;
                break;
            case 5:
                allAroundCheck.SetActive(false);
                startStop.SetActive(false);
                gearChange.SetActive(false);
                aiCars.SetActive(true);
                trafficLight.SetActive(false);
                stopSign.SetActive(true);
                //so only cars for lesson are in game
                foreach (GameObject car in cars)
                {
                    if (car.ToString().Contains("(6)") || car.ToString().Contains("(5)") || car.ToString().Contains("(7)"))
                    {
                        car.SetActive(true);
                    }
                    else
                    {
                        car.SetActive(false);
                    }
                }
                float keepY = userCar.transform.position.y;
                Vector3 tempVec = stopSign.GetComponent<StopSign>().juncToStopAt.previousWaypoint.previousWaypoint.previousWaypoint.previousWaypoint.previousWaypoint.transform.position;
                tempVec.y = keepY;
                tempVec.z -= 3;
                userCar.transform.position = tempVec;
                userCar.transform.forward = stopSign.GetComponent<StopSign>().juncToStopAt.previousWaypoint.previousWaypoint.previousWaypoint.previousWaypoint.previousWaypoint.transform.forward;
                break;
            default:
                allAroundCheck.SetActive(false);
                startStop.SetActive(false);
                gearChange.SetActive(false);
                trafficLight.SetActive(false);
                stopSign.SetActive(false);
                aiCars.SetActive(true);
                break;
        }
    }
}