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
    public EditorDraw edDraw;
    public AudioSource met;

    float lastBeatSample;

    private void Start() {
        wf = GetComponent<WaveformTex>();
        aud = GetComponent<AudioSource>();
        lastBeatSample = edDraw.offset * edDraw.samplesPerSecond - edDraw.spb;
    }


    public void OnPointerClick(PointerEventData eventData) {
        int samp = wf.getMusicPoint(Input.mousePosition);
        aud.timeSamples = samp;
        barRect.localPosition = new Vector2(wf.sampleToPoint(aud.timeSamples), barRect.localPosition.y);
    }

    public bool playing = false;
    void playpause() {
        playing = !playing;
        if (playing) {
            aud.Play();
            lastBeatSample = edDraw.LastBeatSample(aud.timeSamples);
        } else {
            aud.Stop();
            barRect.localPosition = new Vector2(wf.sampleToPoint(aud.timeSamples), barRect.localPosition.y);
        }
        
    }

    private void Update() {
        if (playing) {
            barRect.localPosition = new Vector2(wf.sampleToPoint(aud.timeSamples), barRect.localPosition.y);
            if(edDraw.LastBeatSample(aud.timeSamples) != lastBeatSample) {
                lastBeatSample = edDraw.LastBeatSample(aud.timeSamples);

                if (edDraw.SampleToBeat(aud.timeSamples) != -1 && edDraw.mapping[0][edDraw.SampleToBeat(aud.timeSamples)]) {
                    met.Play();
                }
            }
        }
    }

}
