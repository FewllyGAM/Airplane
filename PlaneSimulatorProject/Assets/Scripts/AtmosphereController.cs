using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtmosphereController : MonoBehaviour
{
    public static AtmosphereController instance;

    public float densityFactor = 1.5f;

    private void Awake()
    {
        instance = this;
    }

    public float AirDensity(float altitude)
    {
        //return 100 * densityFactor / altitude + 100;
        return (altitude > 800) ? 800 / altitude : 1;
    }
}
