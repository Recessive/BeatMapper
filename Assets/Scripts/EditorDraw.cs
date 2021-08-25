using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditorDraw : MonoBehaviour
{
    public GameObject mapPrefab;
    public List<EditorController> maps;
    public AudioSource aud;
    public WaveformTex wf;
    public WaveformInteract wfi;
    public float offset; // Offset in seconds
    public float inputDelay; // Delay for input

    float lastMapPos = 400;

    public float bpm = 1;
    float crothet;
    public float spb; // Samples per beat
    public float samplesPerSecond;

    private void Start() {
        EditorController edCtr = mapPrefab.GetComponent<EditorController>();

        edCtr.edDraw = this;
        edCtr.aud = aud;
        edCtr.wf = wf;
        edCtr.wfi = wfi;

        addNewMap();
        addNewMap();

        samplesPerSecond = aud.clip.samples / aud.clip.length;
        updateBpm(bpm);
    }

    private void addNewMap() {
        GameObject go = Instantiate(mapPrefab);
        go.transform.SetParent(transform);
        RectTransform rectTrans = go.GetComponent<RectTransform>();

        rectTrans.anchoredPosition = new Vector2(0, lastMapPos-=80);

        

        EditorController edCtr = go.GetComponent<EditorController>();

        maps.Add(edCtr);
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

        foreach (EditorController edCtr in maps) {
            edCtr.updateBpm();
        }

    }

    


    public void UpdateEditor() {
        UpdateEditor(wf.midpoint - wf.window / 2);
    }

    public void UpdateEditor(int sample) {
        foreach (EditorController edCtr in maps) {
            edCtr.UpdateEditor(sample);
        }
    }

    public Texture2D PaintMap(int sample, int textWidth, int textHeight, List<bool> map) {

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
                } else if(beat >= 0 && map[beat]) {
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
        

        



        /*if (Input.anyKeyDown && !Input.GetKeyDown(KeyCode.Space) && !Input.GetKeyDown(KeyCode.Escape) && !Input.GetMouseButtonDown(0) && !Input.GetMouseButtonDown(1) && wfi.playing) {
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
        }*/
    }

}
