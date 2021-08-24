using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EditorDraw : MonoBehaviour, IPointerClickHandler
{
    public List<List<bool>> mapping = new List<List<bool>>();
    public AudioSource aud;
    public WaveformTex wf;
    public WaveformInteract wfi;
    public float offset; // Offset in seconds
    public float inputDelay = 0.144f; // Delay for input
    Image im;


    public float bpm = 110;
    float crothet;
    public float spb; // Samples per beat
    public float samplesPerSecond;

    private void Start() {
        mapping.Add(new List<bool>());

        samplesPerSecond = aud.clip.samples / aud.clip.length;
        updateBpm(bpm);

        im = GetComponent<Image>();

        UpdateEditor(0);

    }

    public float LastBeatSample(float sample) {
        return LastBeatSample(sample, true);
    }

    public float LastBeatSample(float sample, bool inSamples) {
        if(sample < offset * samplesPerSecond) {
            return -1;
        } else {
            return Mathf.Floor((sample - offset * samplesPerSecond) / spb) * (inSamples ? spb : 1);
        }
    }

    public int SampleToBeat(float sample) {
        int lbs = (int) LastBeatSample(sample, false);
        return lbs == -1 ? -1 : lbs;
    }

    public void updateBpm(float newBpm) {
        bpm = newBpm;
        crothet = bpm / 60f;
        spb = samplesPerSecond / crothet;

        int totalBeats = (int) (aud.clip.samples / spb) + 1;
        if(totalBeats > mapping[0].Count) {
            for(int i = mapping[0].Count; i < totalBeats; i++) {
                mapping[0].Add(false);
            }
        }else if (totalBeats < mapping[0].Count) {
            for (int i = mapping[0].Count; i > totalBeats; i--) {
                mapping[0].RemoveAt(i-1);
            }
        }

    }

    public void OnPointerClick(PointerEventData eventData) {
        if (eventData.button != PointerEventData.InputButton.Left) {
            return;
        }
        int samp = wf.getMusicPoint(Input.mousePosition);
        int beat;
        if(samp < offset * samplesPerSecond + spb) {
            beat = 0;
        } else {
            beat = SampleToBeat(samp);
        }
        

        mapping[0][beat] = !mapping[0][beat];
        UpdateEditor();
    }


    public void UpdateEditor() {
        UpdateEditor(wf.midpoint - wf.window / 2);
    }

    public void UpdateEditor(int sample) {
        RectTransform rectTrans = GetComponent<RectTransform>();
        int width = (int)rectTrans.rect.width;
        int height = (int)rectTrans.rect.height;

        Texture2D tex = PaintMap(sample, width, height);

        Rect rect = new Rect(Vector2.zero, new Vector2(width, height));

        im.sprite = Sprite.Create(tex, rect, Vector2.zero);
    }

    public Texture2D PaintMap(int sample, int textWidth, int textHeight) {

        Texture2D tex = new Texture2D(textWidth, textHeight, TextureFormat.RGBA32, false);

        float samplesPerPixel = wf.window / (float) textWidth;

        float lastBeat = LastBeatSample(sample);
        int beat = SampleToBeat(lastBeat);
        bool isBeat;
        float pixSamp;

        for (int x = 0; x < textWidth; x++) {
            isBeat = false;
            pixSamp = x * samplesPerPixel + sample;
            if (LastBeatSample(pixSamp) != lastBeat) {
                lastBeat = LastBeatSample(pixSamp);
                isBeat = true;

                beat = SampleToBeat(pixSamp);
            }
            for (int y = 0; y < textHeight; y++) {
                if(isBeat) {
                    tex.SetPixel(x, y, Color.black);
                } else if(beat >= 0 && mapping[0][beat]) {
                    tex.SetPixel(x, y, Color.white);
                } else if(beat % 2 == 0){
                    tex.SetPixel(x, y, new Color(0.4f, 0.4f, 0.4f));
                } else {
                    tex.SetPixel(x, y, new Color(0.5f, 0.5f, 0.5f));
                }
            }
        }
        tex.Apply();


        return tex;
    }




    private void Update() {
        if (Input.GetMouseButton(1)) {
            int samp = wf.getMusicPoint(Input.mousePosition);
            int beat;

            if (samp < offset * samplesPerSecond + spb) {
                beat = 0;
            } else {
                beat = SampleToBeat(samp);
            }

            if (mapping[0][beat]) {
                mapping[0][beat] = false;
                UpdateEditor();
            }
            
        }

        



        if (Input.anyKeyDown && !Input.GetKeyDown(KeyCode.Space) && !Input.GetKeyDown(KeyCode.Escape) && !Input.GetMouseButtonDown(0) && !Input.GetMouseButtonDown(1) && wfi.playing) {
            // Debug.Log(inputDelay * samplesPerSecond + ", " + samplesPerSecond + ", " + spb);
            int samp = aud.timeSamples - (int) (inputDelay * samplesPerSecond);
            int beat;
            if (samp < offset * samplesPerSecond + spb) {
                beat = 0;
            } else {
                beat = SampleToBeat(samp);
            }


            mapping[0][beat] = true;
            UpdateEditor();
        }
    }

}
