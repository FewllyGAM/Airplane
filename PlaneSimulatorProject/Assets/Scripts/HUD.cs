using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public Slider slider;

    public void UpdateThrust(float rate)
    {
        slider.value = rate;
    }
}
