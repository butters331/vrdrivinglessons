using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraVariables : MonoBehaviour
{
    public Vector3 coordinates;
    public int lessonSelection;
    public bool carsActive;

    private void Start()
    {
        coordinates.Set(0, 0, 0);
        lessonSelection = 0;
        DontDestroyOnLoad(this.gameObject);
    }
}
