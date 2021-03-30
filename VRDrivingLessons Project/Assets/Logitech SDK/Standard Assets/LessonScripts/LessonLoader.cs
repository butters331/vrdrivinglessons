using UnityEngine;

public class LessonLoader : MonoBehaviour
{
    private GameObject cameraVariables;
    public GameObject allAroundCheck;
    // Start is called before the first frame update
    void Start()
    {
        cameraVariables = GameObject.Find("CameraVariables");
        switch (cameraVariables.GetComponent<CameraVariables>().lessonSelection)
        {
            case 1:
                allAroundCheck.SetActive(true);
                break;
            default:
                allAroundCheck.SetActive(false);
                break;
        }
    }
}