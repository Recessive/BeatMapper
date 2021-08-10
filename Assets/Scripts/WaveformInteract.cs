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
    public RectTransform barRect;

    private void Start() {
        wf = GetComponent<WaveformTex>();
        aud = GetComponent<AudioSource>();
    }


    public void OnPointerClick(PointerEventData eventData) {
        int samp = wf.getMusicPoint(Input.mousePosition);
        aud.timeSamples = samp;
        barRect.localPosition = new Vector2(wf.sampleToPoint(aud.timeSamples), barRect.localPosition.y);
    }

    bool playing = false;
    void playpause() {
        playing = !playing;
        if (playing) {
            aud.Play();
        } else {
            aud.Stop();
            barRect.localPosition = new Vector2(wf.sampleToPoint(aud.timeSamples), barRect.localPosition.y);
        }
        
    }

    private void Update() {
        if (playing) {
            barRect.localPosition = new Vector2(wf.sampleToPoint(aud.timeSamples), barRect.localPosition.y);
        }
    }

}
