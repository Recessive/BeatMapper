using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WaveformInteract : MonoBehaviour, IPointerClickHandler 
{


    void OnEnable() {
        ViewControl.onToggle1 += playpause;
    }

    private void OnDisable() {
        ViewControl.onToggle1 -= playpause;
    }

    WaveformTex wf;
    AudioSource aud;

    private void Start() {
        wf = GetComponent<WaveformTex>();
        aud = GetComponent<AudioSource>();
    }


    public void OnPointerClick(PointerEventData eventData) {
        int samp = wf.getMusicPoint(Input.mousePosition);
        aud.timeSamples = samp;
    }

    bool playing = false;
    void playpause() {
        playing = !playing;
        if (playing) {
            aud.Play();
        } else {
            aud.Stop();
        }
        
    }

}
