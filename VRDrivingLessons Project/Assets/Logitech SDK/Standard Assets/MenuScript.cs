using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    public GameObject topMenu;

    public Button option1;
    public Button option2;
    public Button option3;

    public Slider sliderVertical;
    public Slider sliderHorizontal;
    public Image verticalSliderHead;
    public Image horizontalSliderHead;

    public GameObject LessonsMenu;

    public Button lesson1;
    public Image lesson1Tick;
    public Button lesson2;
    public Image lesson2Tick;
    public Button lesson3;

    public Button back;

    private ColorBlock colours;

    private int numberOfOptions = 3;
    private int numberOfLessons = 3;
    private bool wheelConnected = false;
    private bool dPadPressed = false;
    private bool sliderSelected = false;
    private bool onButtons = true;
    private bool aPressed = false;

    private bool onTopMenu = true;
    private bool changedState = false;
    private bool onBack = false;

    public GameObject cameraVariables;

    private int sliderSelectedNum;
    private int selectedOption;
    private int selectedOptionLessons;

    //stored like a bool, but playerprefs doesnt support bools
    private int completedAllAroundCheck;
    private int completedStartStop;

    // Use this for initialization

    private void Start()
    {
        //set button clolours
        colours = lesson1.colors;
        colours.normalColor = Color.white;
        colours.selectedColor = Color.green;
        colours.highlightedColor = Color.red;
        option1.colors = colours;
        option2.colors = colours;
        option3.colors = colours;
        sliderHorizontal.colors = colours;
        sliderVertical.colors = colours;

        //initiate selection variable
        selectedOption = 1;
        selectedOptionLessons = 1;
        
        //check wheel connection
        if (LogitechGSDK.LogiIsConnected(0))
        {
            Debug.Log("wheel is connected");
        }
        //make first option active
        option1.Select();
        onButtons = true;
        onTopMenu = true;

        completedAllAroundCheck = PlayerPrefs.GetInt("AlLAroundCheck", 0);
        completedStartStop = PlayerPrefs.GetInt("StartStop", 0);

        if (completedAllAroundCheck == 1)
        {
            lesson1Tick.enabled = true;
        }
        else
        {
            lesson1Tick.enabled = false;
        }

        if (completedStartStop == 1)
        {
            lesson2Tick.enabled = true;
        }
        else
        {
            lesson2Tick.enabled = false;
        }
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
            
            if (onTopMenu)
            {
                //ensures the state is only changed once
                if (changedState)
                {
                    LessonsMenu.SetActive(false);
                    topMenu.SetActive(true);
                    changedState = false;
                }
                
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
                else if (rec.rgdwPOV[0] == 9000) // right pressed
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
                    else if (!onButtons && sliderSelected && sliderSelectedNum == 2)
                    {
                        if (!dPadPressed)
                        {
                            sliderHorizontal.value -= 0.1f;
                            dPadPressed = true;
                        }
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
                                //move to lesson select
                                onTopMenu = false;
                                changedState = true;
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

                        //change colours so user know if selected
                        if (sliderSelected && sliderSelectedNum == 1)
                        {
                            verticalSliderHead.color = new Color32(5, 154, 5, 255);
                            horizontalSliderHead.color = new Color32(0, 245, 0, 255);
                        }
                        else if (sliderSelected && sliderSelectedNum == 2)
                        {
                            horizontalSliderHead.color = new Color32(5, 154, 5, 255);
                            verticalSliderHead.color = new Color32(0, 245, 0, 255);

                        }
                        else
                        {
                            verticalSliderHead.color = new Color32(0, 245, 0, 255);
                            horizontalSliderHead.color = new Color32(0, 245, 0, 255);
                        }
                    }

                }
                else if (rec.rgbButtons[0] != 128) //a is not pressed
                {
                    aPressed = false;
                }
            } // on top menu

            else // on lessons menu
            {
                if (changedState)
                {
                    topMenu.SetActive(false);
                    LessonsMenu.SetActive(true);
                    changedState = false;
                }

                if (rec.rgdwPOV[0] == 18000 /*if down selected*/)
                {
                    //no longer on the back button
                    onBack = false;

                    if (!dPadPressed)
                    {
                        //cycle options
                        selectedOptionLessons += 1;
                        dPadPressed = true;
                    }

                    if (selectedOptionLessons > numberOfLessons) //If at end of list go back to top
                    {
                        selectedOptionLessons = 1;
                    }

                    switch (selectedOptionLessons) //Set the visual indicator for which option you are on.
                    {
                        case 1:
                            lesson1.Select();
                            break;
                        case 2:
                            lesson2.Select();
                            break;
                        case 3:
                            lesson3.Select();
                            break;
                        default:
                            lesson1.Select();
                            break;
                    }

                }

                //made same as down, but with opposite outcomes 
                else if (rec.rgdwPOV[0] == 0 /*|| up selected*/)
                {
                    onBack = false;

                    if (!dPadPressed)
                    {
                        selectedOptionLessons -= 1;
                        dPadPressed = true;
                    }
                    if (selectedOptionLessons < 1) //If at end of list go back to top
                    {
                        selectedOptionLessons = numberOfLessons;
                    }

                    switch (selectedOptionLessons) //Set the visual indicator for which option you are on.
                    {
                        case 1:
                            lesson1.Select();
                            break;
                        case 2:
                            lesson2.Select();
                            break;
                        case 3:
                            lesson3.Select();
                            break;
                        default:
                            lesson1.Select();
                            break;
                    }

                }

                else if (rec.rgdwPOV[0] == 27000) // left pressed
                {
                    back.Select();
                    onBack = true;
                }

                else if (rec.rgdwPOV[0] == 9000)
                {
                    onBack = false;
                    switch (selectedOptionLessons) //Set the visual indicator for which option you are on.
                    {
                        case 1:
                            lesson1.Select();
                            break;
                        case 2:
                            lesson2.Select();
                            break;
                        case 3:
                            lesson3.Select();
                            break;
                        default:
                            lesson1.Select();
                            break;
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
                    if (onBack)
                    {
                        onTopMenu = true;
                        changedState = true;
                    }
                    else
                    {
                        switch (selectedOptionLessons) //Set the visual indicator for which option you are on.
                        {
                            case 1:
                                cameraVariables.GetComponent<CameraVariables>().lessonSelection = 1;
                                SceneManager.LoadScene(1);
                                break;
                            case 2:
                                cameraVariables.GetComponent<CameraVariables>().lessonSelection = 2;
                                SceneManager.LoadScene(1);
                                break;
                            case 3:
                                Application.Quit();
                                break;
                        }
                    }
                    
                }
                else if (rec.rgbButtons[0] != 128) //a is not pressed
                {
                    aPressed = false;
                }
            }

            cameraVariables.GetComponent<CameraVariables>().coordinates.Set(0, sliderVertical.value - 0.5f, sliderHorizontal.value - 0.5f);
            
        }
        else
        {
            Debug.Log("Not picking up wheel");
        }
    }
}