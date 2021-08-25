using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeController : MonoBehaviour
{
    public Slider slider;
    public InputField input;
    public AudioSource aud;

    private void Start() {
        slider.onValueChanged.AddListener(sliderChange);
        input.onValueChanged.AddListener(inputChange);
    }

    void sliderChange(float val) {
        input.text = "" + val;
        aud.volume = val;
    }

    void inputChange(string val) {
        float f;
        if (float.TryParse(val, out f) && f > 0 && f < 1) {
            slider.value = f;
            aud.volume = f;
        }
    }
}
