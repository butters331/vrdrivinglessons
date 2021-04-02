using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class AllAroundCheck : MonoBehaviour
{
    public GameObject rightShoulder;
    public GameObject rightMirror;
    public GameObject interiorMirror;
    public GameObject leftMirror;
    public GameObject leftShoulder;
    public GameObject loaderCanvas;
    public Image loaderFill;

    public Camera mainCam;

    private int panelToPointAt;
    private bool endOfLesson = false;
    private bool completedLoop;

    // Start is called before the first frame update
    void Start()
    {
        rightShoulder.SetActive(true);
        rightMirror.SetActive(false);
        interiorMirror.SetActive(false);
        leftMirror.SetActive(false);
        leftShoulder.SetActive(false);
        loaderCanvas.SetActive(true);
        panelToPointAt = 1;
    }

    // Update is called once per frame
    void Update()
    {
        loaderCanvas.transform.position = mainCam.transform.position + mainCam.transform.forward * 0.3f;
        loaderCanvas.transform.rotation = mainCam.transform.rotation;
        if (panelToPointAt == 1)
        {
            
            if (checkLooking(rightShoulder))
            {
                rightShoulder.SetActive(false);
                rightMirror.SetActive(true);
                panelToPointAt++;
            }
        }
        else if (panelToPointAt == 2)
        {

            if (checkLooking(rightMirror))
            {
                rightMirror.SetActive(false);
                interiorMirror.SetActive(true);
                panelToPointAt++;
            }
        }
        else if (panelToPointAt == 3)
        {

            if (checkLooking(interiorMirror))
            {
                interiorMirror.SetActive(false);
                leftMirror.SetActive(true);
                panelToPointAt++;
            }
        }
        else if (panelToPointAt == 4)
        {

            if (checkLooking(leftMirror))
            {
                leftMirror.SetActive(false);
                leftShoulder.SetActive(true);
                panelToPointAt++;
            }
        }
        else if (panelToPointAt == 5)
        {

            if (checkLooking(leftShoulder))
            {
                leftShoulder.SetActive(false);
                endOfLesson = true;
            }
            //end the lesson (by pressing a?) -- wheel input not needed till now
            if (endOfLesson && LogitechGSDK.LogiUpdate())
            {
                //get data from wheel
                LogitechGSDK.DIJOYSTATE2ENGINES rec;
                rec = LogitechGSDK.LogiGetStateUnity(0);

                PlayerPrefs.SetInt("AlLAroundCheck", 1);
                //if x pressed
                if (rec.rgbButtons[2] == 128)
                {
                    SceneManager.LoadScene(0);
                }
            }
            
        }


    }
    
    bool checkLooking(GameObject panel)
    {
        //get panel pos - player pos
        Vector3 positionDifference = panel.transform.position - mainCam.transform.position;

        //normalize it
        positionDifference = positionDifference.normalized;

        //find dot product with mainCam.transform.forward
        float amountLookingAt = Vector3.Dot(positionDifference, mainCam.transform.forward);

        //closer to 1 you are the more youre looking at it

        //if close to 1
        //set panel inactive
        //set next active
        //panelToPointAt ++;

        if (amountLookingAt > 0.99f)
        {
            loaderCanvas.SetActive(true);
            completedLoop = false;
            StartCoroutine(barLoad(panel));
            if (completedLoop)
            {
                return true;
            }
            else
            {
                loaderFill.fillAmount = 0;
                return false;
            }
            
        }
        else
        {
            loaderCanvas.SetActive(true);
            loaderFill.fillAmount = 0;
            return false;
        }
            //perform disappearing animation?
    }

    IEnumerator barLoad(GameObject panel)
    {
        Vector3 positionDifference = panel.transform.position - mainCam.transform.position;

        //normalize it
        positionDifference = positionDifference.normalized;

        //find dot product with mainCam.transform.forward
        float amountLookingAt = Vector3.Dot(positionDifference, mainCam.transform.forward);

        while (amountLookingAt > 0.99f && !completedLoop)
        {
            positionDifference = panel.transform.position - mainCam.transform.position;
            positionDifference = positionDifference.normalized;
            amountLookingAt = Vector3.Dot(positionDifference, mainCam.transform.forward);
            loaderFill.fillAmount += 0.1f / 3;
            if (loaderFill.fillAmount >= 0.99f)
            {
                completedLoop = true;
            }
            yield return new WaitForSeconds(0.01f);
        }
       
    }
}