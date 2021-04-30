using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    public static bool isPaused = false;

    public GameObject pauseMenuUI;
    public Button resume;
    public Slider height;
    public Slider distance;
    public Button quit;

    public GameObject cameraMovement;

    private bool dPadPressed = false;
    private bool pausePressed = false;
    private bool sliderChanged = false;
    private int selectedOption = 1;
    private GameObject cameraVariables;

    private float originalHeightValue;
    private float originalDistanceValue;

    private void Start()
    {
        cameraVariables = GameObject.Find("CameraVariables");
        originalHeightValue = cameraVariables.GetComponent<CameraVariables>().coordinates.y;
        originalDistanceValue = cameraVariables.GetComponent<CameraVariables>().coordinates.z;
        height.value = originalHeightValue + 0.5f;
        distance.value = originalDistanceValue + 0.5f;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (LogitechGSDK.LogiUpdate())
        {
            //get data from wheel
            LogitechGSDK.DIJOYSTATE2ENGINES rec;
            rec = LogitechGSDK.LogiGetStateUnity(0);

            //if pause pressed
            if (rec.rgbButtons[6] == 128)
            {
                //if already paused unpause
                if (isPaused && !pausePressed)
                {
                    if (sliderChanged)
                    {
                        float heightChange = height.value - 0.5f - originalHeightValue;
                        Debug.Log("change in hight: " + heightChange);
                        float distanceChange = distance.value - 0.5f - originalDistanceValue;
                        cameraVariables.GetComponent<CameraVariables>().coordinates.Set(0, height.value - 0.5f, distance.value - 0.5f);

                        Vector3 tempVector = cameraMovement.transform.position;
                        tempVector.y += heightChange;
                        tempVector.z += distanceChange;
                        cameraMovement.transform.position = tempVector;
                        originalHeightValue = cameraVariables.GetComponent<CameraVariables>().coordinates.y;
                        originalDistanceValue = cameraVariables.GetComponent<CameraVariables>().coordinates.z;
                        height.value = originalHeightValue + 0.5f;
                        distance.value = originalDistanceValue + 0.5f;
                        sliderChanged = false;
                    }
                    pauseMenuUI.SetActive(false);
                    Time.timeScale = 1f;
                    isPaused = false;
                    pausePressed = true;
                }
                else if (!isPaused && !pausePressed) // pause
                {
                    pauseMenuUI.SetActive(true);
                    Time.timeScale = 0f;
                    isPaused = true;
                    pausePressed = true;
                }
            }
            else
            {
                pausePressed = false;
            }


            if (rec.rgdwPOV[0] == 18000 /*if down selected*/)
            {
                if (!dPadPressed)
                {
                    selectedOption++;
                    if (selectedOption > 4)
                    {
                        selectedOption = 1;
                    }
                }
                dPadPressed = true;
            }
            else if (rec.rgdwPOV[0] == 0 /*|| up selected*/)
            {
                if (!dPadPressed)
                {
                    selectedOption--;
                    if (selectedOption < 1)
                    {
                        selectedOption = 4;
                    }
                }
                dPadPressed = true;
            }
            else if (rec.rgdwPOV[0] == 9000) // right pressed
            {
                if (!dPadPressed)
                {
                    if (selectedOption == 2)
                    {
                        height.value += 0.1f;
                    }
                    else if (selectedOption == 3)
                    {
                        distance.value += 0.1f;
                    }
                    sliderChanged = true;
                }
                dPadPressed = true;
            }
            else if (rec.rgdwPOV[0] == 27000) // left pressed
            {
                if (!dPadPressed)
                {
                    if (selectedOption == 2)
                    {
                        height.value -= 0.1f;
                    }
                    else if (selectedOption == 3)
                    {
                        distance.value -= 0.1f;
                    }
                    sliderChanged = true;
                }
                dPadPressed = true;
            }
            else
            {
                dPadPressed = false;
            }

            //highlight the correct item
            switch (selectedOption)
            {
                case 1:
                    resume.Select();
                    break;
                case 2:
                    height.Select();
                    break;
                case 3:
                    distance.Select();
                    break;
                case 4:
                    quit.Select();
                    break;
                default:
                    resume.Select();
                    break;
            }

            //if Y pressed recenter headset
            if (rec.rgbButtons[3] == 128)
            {
                UnityEngine.XR.InputTracking.Recenter();
            }

            if (rec.rgbButtons[0] == 128) // if a pressed
            {
                if (selectedOption == 1) // unpause
                {
                    pauseMenuUI.SetActive(false);
                    Time.timeScale = 1f;
                    isPaused = false;
                    if (sliderChanged)
                    {
                        float heightChange = height.value - 0.5f - originalHeightValue;
                        float distanceChange = distance.value - 0.5f - originalDistanceValue;
                        cameraVariables.GetComponent<CameraVariables>().coordinates.Set(0, height.value - 0.5f, distance.value - 0.5f);

                        Vector3 tempVector = cameraMovement.transform.position;
                        tempVector.y += heightChange;
                        tempVector.z += distanceChange;
                        cameraMovement.transform.position = tempVector;
                        originalHeightValue = cameraVariables.GetComponent<CameraVariables>().coordinates.y;
                        originalDistanceValue = cameraVariables.GetComponent<CameraVariables>().coordinates.z;
                        height.value = originalHeightValue + 0.5f;
                        distance.value = originalDistanceValue + 0.5f;
                        sliderChanged = false;
                    }
                }
                else if(selectedOption == 4)
                {
                    Destroy(cameraVariables);
                    SceneManager.LoadScene(0); 
                }
            }
        }
    }
}
