using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveformTex : MonoBehaviour
{
    void OnEnable() {
        ViewControl.onScroll += scroll;
    }

    private void OnDisable() {
        ViewControl.onScroll -= scroll;
    }


    public float zoomSpeed = 0.05f;
    public AnimationCurve zoomCurve;
    float zoom = 1f;
    public int window, midpoint, audioLength;
    Image im;
    AudioSource aud;
    RectTransform rectTrans;

    public Scrollbar scrollbar;
    public EditorDraw edDraw;

    public void SongLoaded() {
        rectTrans = GetComponent<RectTransform>();
        im = GetComponent<Image>();
        aud = GetComponent<AudioSource>();

        audioLength = aud.clip.samples;
        midpoint = audioLength / 2;
        scrollbar.value = 0.5f;
        window = audioLength;

        scrollbar.onValueChanged.AddListener(drag);

        UpdateWaveform(true);
    }

    public float sampleToPoint(int sample) {
        float percentageComplete = (float) (sample - (midpoint - window / 2)) / window;
        return rectTrans.rect.width*percentageComplete - (rectTrans.rect.width / 2);
    }

    public int getMusicPoint(Vector2 point) {
        Vector2 truePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTrans, point, GetComponentInParent<Canvas>().worldCamera, out truePos);
        float x = (truePos.x + rectTrans.rect.width / 2) / rectTrans.rect.width;

        int sample = (int) (midpoint - window / 2 + window * x);

        return sample;
    }

    void drag(float val) {

        midpoint = (int) (val * (audioLength - window) + window / 2);

        UpdateWaveform();
    }


    void scroll(bool forward) {

        if (forward) {
            zoom -= zoomSpeed;
        } else {
            zoom += zoomSpeed;
        }
        zoom = Mathf.Clamp(zoom, zoomSpeed, 1f);
        window = (int)(zoomCurve.Evaluate(zoom) * audioLength);
        scrollbar.size = zoom;

        UpdateWaveform();

    }

    public void UpdateWaveform() {
        UpdateWaveform(false);
    }

    public void UpdateWaveform(bool awake) {
      
        int start = midpoint - window / 2;
        int end = midpoint + window / 2;

        if (start < 0) {
            drag(0f);
            return;
        }

        if (end > audioLength) {
            drag(1f);
            return;
        }

        if(!awake) edDraw.UpdateEditor(start);

        int width = (int)rectTrans.rect.width;
        int height = (int)rectTrans.rect.height;


        Texture2D waveform = PaintWaveformSpectrum(aud.clip, width, height, start, end, Color.blue);

        Rect rect = new Rect(Vector2.zero, new Vector2(width, height));

        im.sprite = Sprite.Create(waveform, rect, Vector2.zero);
    }

    public Texture2D PaintWaveformSpectrum(AudioClip audio, int textWidth, int textHeight, int audioStart, int audioEnd, Color col) {
        if(audioEnd > audioLength) {
            Debug.LogError("Audio end cannot be past the end of the audio");
            return null;
        }


        Texture2D tex = new Texture2D(textWidth, textHeight, TextureFormat.RGBA32, false);
        float[] samples = new float[audioLength];
        float[] waveform = new float[textWidth+1];
        audio.GetData(samples, 0);
        int packSize = ((audioEnd - audioStart) / textWidth) + 1;

        audioStart = (int) Mathf.Floor(audioStart / packSize) * packSize;

        int s = 0;
        for (int i = audioStart; i < audioEnd; i += packSize) {
            waveform[s] = Mathf.Abs(samples[i]);
            s++;
        }

        for (int x = 0; x < textWidth; x++) {
            for (int y = 0; y < textHeight; y++) {
                tex.SetPixel(x, y, Color.gray);
            }
        }

        for (int x = 0; x < waveform.Length; x++) {
            for (int y = 0; y <= waveform[x] * ((float)textHeight * .75f); y++) {
                tex.SetPixel(x, (textHeight / 2) + y, col);
                tex.SetPixel(x, (textHeight / 2) - y, col);
            }
        }
        tex.Apply();

        return tex;
    }
}
