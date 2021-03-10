using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{

    public Button option1;
    public Button option2;
    public Button option3;

    public Slider sliderVertical;
    public Slider sliderHorizontal;

    private Camera mainCamera;

    private ColorBlock colours;

    private int numberOfOptions = 3;
    private bool wheelConnected = false;
    private bool dPadPressed = false;
    private bool sliderSelected = false;
    private bool onButtons = true;
    private bool aPressed = false;

    private int sliderSelectedNum;
    private int selectedOption;

    // Use this for initialization

    private void Start()
    {
        //set button clolours
        colours = GetComponent<Button>().colors;
        colours.normalColor = Color.white;
        colours.selectedColor = Color.green;
        option1.colors = colours;
        option2.colors = colours;
        option3.colors = colours;

        //initiate selection variable
        selectedOption = 1;
        
        //check wheel connection
        if (LogitechGSDK.LogiIsConnected(0))
        {
            Debug.Log("wheel is connected");
        }
        //make first option active
        option1.Select();
        onButtons = true;
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    private void Update()
    {
        //ensure that wheel is connected on first call to update, causes bugs otherwise
        if (!wheelConnected)
        {
            LogitechGSDK.LogiSteeringShutdown();
            if (!(LogitechGSDK.LogiSteeringInitialize(false)))
            {
                Debug.Log("Failed wheel init");
            }
            else
            {
                wheelConnected = true;
                Debug.Log("wheel connected");
            }
        }
        //if wheel connected
        if (LogitechGSDK.LogiUpdate())
        {
            //get data from wheel
            LogitechGSDK.DIJOYSTATE2ENGINES rec;
            rec = LogitechGSDK.LogiGetStateUnity(0);
            

            if (rec.rgdwPOV[0] == 18000 /*if down selected*/)
            { 
                //if on sliders to the right
                if (!onButtons)
                {
                    //if hovering over slider and clicked select
                    if (sliderSelected && sliderSelectedNum == 1)
                    {
                        //move slider down (but dont allow for hold)
                        if (!dPadPressed)
                        {
                            sliderVertical.value -= 0.1f;
                            dPadPressed = true;
                        }
                    }
                    else//otherwise if not selected, go to horrizontal bottom slider
                    {
                        sliderHorizontal.Select();
                        sliderSelectedNum = 2;
                    }
                }
                else//if on the main menu buttons
                {
                    if (!dPadPressed)
                    {
                        //cycle options
                        selectedOption += 1;
                        dPadPressed = true;
                    }

                    if (selectedOption > numberOfOptions) //If at end of list go back to top
                    {
                        selectedOption = 1;
                    }

                    switch (selectedOption) //Set the visual indicator for which option you are on.
                    {
                        case 1:
                            option1.Select();
                            break;
                        case 2:
                            option2.Select();
                            break;
                        case 3:
                            option3.Select();
                            break;
                        default:
                            option1.Select();
                            break;
                    }
                }
                
            }

            //made same as down, but with opposite outcomes 
            else if (rec.rgdwPOV[0] == 0 /*|| up selected*/)
            { 
                if (!onButtons)
                {
                    if (sliderSelected && sliderSelectedNum == 1)
                    {
                        if (!dPadPressed)
                        {
                            sliderVertical.value += 0.1f;
                            dPadPressed = true;
                        }
                    }
                    else
                    {
                        sliderVertical.Select();
                        sliderSelectedNum = 1;
                    }
                }
                else
                {
                    if (!dPadPressed)
                    {
                        selectedOption -= 1;
                        dPadPressed = true;
                    }
                    if (selectedOption < 1) //If at end of list go back to top
                    {
                        selectedOption = numberOfOptions;
                    }

                    switch (selectedOption) //Set the visual indicator for which option you are on.
                    {
                        case 1:
                            option1.Select();
                            break;
                        case 2:
                            option2.Select();
                            break;
                        case 3:
                            option3.Select();
                            break;
                        default:
                            option1.Select();
                            break;
                    }
                }
                
            }
            else if(rec.rgdwPOV[0] == 9000) // right pressed
            {
                //if on the main buttons move to slider
                if (onButtons)
                {
                    onButtons = false;
                    sliderVertical.Select();
                    sliderSelectedNum = 1;
                }
                else
                {
                    //if selected the horizontal slider, change it
                    if (sliderSelected && sliderSelectedNum == 2)
                    {
                        if (!dPadPressed)
                        {
                            sliderHorizontal.value += 0.1f;
                            dPadPressed = true;
                        }
                    }
                }
            }

            else if (rec.rgdwPOV[0] == 27000) // left pressed
            {
                if (!onButtons && !sliderSelected)
                {
                    onButtons = true;
                    switch (selectedOption) //Set the visual indicator for which option you are on -- move back to where you were
                    {
                        case 1:
                            option1.Select();
                            break;
                        case 2:
                            option2.Select();
                            break;
                        case 3:
                            option3.Select();
                            break;
                        default:
                            option1.Select();
                            break;
                    }
                }
                else if(!onButtons && sliderSelected && sliderSelectedNum == 2)
                {
                    sliderHorizontal.value -= 0.1f;
                    dPadPressed = true;
                }
            }
            else
            {
                dPadPressed = false;
            }
            //if Y pressed recenter headset
            if (rec.rgbButtons[3] == 128)
            {
                UnityEngine.XR.InputTracking.Recenter();
            }

            //if a clicked
            if (rec.rgbButtons[0] == 128 && !aPressed)
            {
                aPressed = true;

                //if on the buttons, do said command
                if (onButtons)
                {
                    switch (selectedOption) //Set the visual indicator for which option you are on.
                    {
                        case 1:
                            SceneManager.LoadScene(1);
                            break;
                        case 2:
                            SceneManager.LoadScene(1);
                            break;
                        case 3:
                            Application.Quit();
                            break;
                    }
                }
                else//select or deselect the slider
                {
                    sliderSelected = !sliderSelected;
                }
                
            }
            else //a is not pressed
            {
                aPressed = false;
            }

            //update camera position according to the slider
            Vector3 tempVector = mainCamera.transform.position;
            tempVector.y += sliderVertical.value - 0.5f;
            tempVector.z += sliderHorizontal.value - 0.5f;
            mainCamera.transform.position = tempVector;
        }
        else
        {
            Debug.Log("Not picking up wheel");
        }
    }
}