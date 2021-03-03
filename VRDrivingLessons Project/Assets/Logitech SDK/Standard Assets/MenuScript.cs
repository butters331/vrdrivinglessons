using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{

    public Button option1;
    public Button option2;
    public Button option3;
    public Button option4;
    public Button option5;

    private ColorBlock colours;

    private int numberOfOptions = 5;
    private bool wheelConnected = false;
    private bool dPadPressed = false;

    private int selectedOption;

    // Use this for initialization

    private void Start()
    {

        colours = GetComponent<Button>().colors;
        colours.normalColor = Color.white;
        colours.selectedColor = Color.green;
        selectedOption = 1;
        option1.colors = colours;
        option2.colors = colours;
        option3.colors = colours;
        option4.colors = colours;
        option5.colors = colours;
        if (LogitechGSDK.LogiIsConnected(0))
        {
            Debug.Log("wheel is connected");
        }
        option1.Select();
    }

    // Update is called once per frame
    private void Update()
    {
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
            { //Input telling it to go up or down.
                if (!dPadPressed)
                {
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
                    case 4:
                        option4.Select();
                        break;
                    case 5:
                        option5.Select();
                        break;
                    default:
                        option1.Select();
                        break;
                }
            }

            else if (rec.rgdwPOV[0] == 0 /*|| up selected*/)
            { //Input telling it to go up or down.
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
                    case 4:
                        option4.Select();
                        break;
                    case 5:
                        option5.Select();
                        break;
                    default:
                        option1.Select();
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

            if (rec.rgbButtons[0] == 128)
            {
               // Debug.Log("Picked: " + selectedOption); //For testing as the switch statment does nothing right now.

                switch (selectedOption) //Set the visual indicator for which option you are on.
                {
                    case 1:
                        SceneManager.LoadScene(1);
                        break;
                    case 2:
                        SceneManager.LoadScene(1);
                        break;

                    //other options to be implimented
                    case 3:
                        /*Do option two*/
                        break;
                }
            }
        }
        else
        {
            Debug.Log("Not picking up wheel");
        }
    }
}