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
    public float offset; // Offset in seconds
    Image im;


    int audioLength;
    public float bpm = 105;
    float crothet;
    public float spb; // Samples per beat
    public float samplesPerSecond;

    private void Start() {
        mapping.Add(new List<bool>());

        audioLength = aud.clip.samples;
        samplesPerSecond = aud.clip.samples / aud.clip.length;
        updateBpm(bpm);

        im = GetComponent<Image>();

        UpdateEditor(0);
    }

    public float LastBeat(float sample) {
        return Mathf.Floor((sample + offset * samplesPerSecond) / spb) * spb;
    }

    public int SampleToBeat(float sample) {
        return (int)(LastBeat(sample) / spb);
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
        int samp = wf.getMusicPoint(Input.mousePosition);
        int beat;
        if(samp < offset * samplesPerSecond + spb) {
            beat = 0;
        } else {
            beat = SampleToBeat(samp);
        }
        

        mapping[0][beat] = !mapping[0][beat];
        Debug.Log(beat);
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

        float lastBeat = LastBeat(sample);
        int beat = SampleToBeat(sample);
        bool isBeat;
        for (int x = 0; x < textWidth; x++) {
            isBeat = false;
            if(x*samplesPerPixel + sample >= lastBeat + spb) {
                lastBeat += spb;
                beat++;
                isBeat = true;
            }
            for (int y = 0; y < textHeight; y++) {
                if(isBeat) {
                    tex.SetPixel(x, y, Color.black);
                } else if(mapping[0][beat]) {
                    tex.SetPixel(x, y, Color.white);
                } else {
                    tex.SetPixel(x, y, Color.gray);
                }
            }
        }
        tex.Apply();


        return tex;
    }
}
