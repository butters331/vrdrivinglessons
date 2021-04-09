using UnityEngine;

public class LessonLoader : MonoBehaviour
{
    private GameObject cameraVariables;
    public GameObject allAroundCheck;
    public GameObject startStop;
    public GameObject gearChange;
    // Start is called before the first frame update
    void Start()
    {
        cameraVariables = GameObject.Find("CameraVariables");
        switch (cameraVariables.GetComponent<CameraVariables>().lessonSelection)
        {
            case 1:
                allAroundCheck.SetActive(true);
                startStop.SetActive(false);
                gearChange.SetActive(false);
                break;
            case 2:
                allAroundCheck.SetActive(false);
                startStop.SetActive(true);
                gearChange.SetActive(false);
                break;
            case 3:
                allAroundCheck.SetActive(false);
                startStop.SetActive(false);
                gearChange.SetActive(true);
                break;
            default:
                allAroundCheck.SetActive(false);
                startStop.SetActive(false);
                gearChange.SetActive(false);
                break;
        }
    }
}