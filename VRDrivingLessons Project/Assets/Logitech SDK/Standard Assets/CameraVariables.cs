using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraVariables : MonoBehaviour
{
    public Vector3 coordinates;

    private void Start()
    {
        coordinates.Set(0, 0, 0);
        DontDestroyOnLoad(this.gameObject);
    }
}
