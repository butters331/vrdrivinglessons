using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringWheelRotate : MonoBehaviour
{
    Vector3 currentEulerAngles;
    // Start is called before the first frame update
    void Start()
    {
        currentEulerAngles = new Vector3(0, 0, 0);
        transform.localEulerAngles = currentEulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        if (LogitechGSDK.LogiUpdate())
        {
            LogitechGSDK.DIJOYSTATE2ENGINES rec;
            rec = LogitechGSDK.LogiGetStateUnity(0);
            currentEulerAngles = new Vector3(0, 0, (float)((float)rec.lX / -72.816));
            transform.localEulerAngles = currentEulerAngles;
        }
    }
}
