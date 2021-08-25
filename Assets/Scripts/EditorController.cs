using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EditorController : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler 
{
    public EditorDraw edDraw;

    public AudioSource aud;
    public WaveformTex wf;
    public WaveformInteract wfi;

    List<bool> mapping = new List<bool>();

    Image im;
    bool pointerInside = false;

    void OnEnable() {
        wfi.onBeat += beat;
    }

    private void OnDisable() {
        wfi.onBeat -= beat;
    }


    private void Start() {
        im = GetComponent<Image>();
        updateBpm();

        UpdateEditor();
    }


    public void updateBpm() {

        int totalBeats = (int)(aud.clip.samples / edDraw.spb) + 1;
        if (totalBeats > mapping.Count) {
            for (int i = mapping.Count; i < totalBeats; i++) {
                mapping.Add(false);
            }
        } else if (totalBeats < mapping.Count) {
            for (int i = mapping.Count; i > totalBeats; i--) {
                mapping.RemoveAt(i - 1);
            }
        }

    }

    public void OnPointerEnter(PointerEventData eventData) {
        pointerInside = true;
    }

    public void OnPointerExit(PointerEventData eventData) {
        pointerInside = false;
    }

    public void OnPointerClick(PointerEventData eventData) {
        if (eventData.button != PointerEventData.InputButton.Left) {
            return;
        }
        int samp = wf.getMusicPoint(Input.mousePosition);
        int beat;
        if (samp < edDraw.offset * edDraw.samplesPerSecond + edDraw.spb) {
            beat = 0;
        } else {
            beat = edDraw.SampleToBeat(samp);
        }


        mapping[beat] = !mapping[beat];
        UpdateEditor();
    }

    public void UpdateEditor() {
        UpdateEditor(wf.midpoint - wf.window / 2);
    }

    public void UpdateEditor(int sample) {
        RectTransform rectTrans = GetComponent<RectTransform>();
        int width = (int)rectTrans.rect.width;
        int height = (int)rectTrans.rect.height;

        Texture2D tex = edDraw.PaintMap(sample, width, height, mapping);

        Rect rect = new Rect(Vector2.zero, new Vector2(width, height));


        Debug.Log(im);

        im.sprite = Sprite.Create(tex, rect, Vector2.zero);
    }

    private void beat(int beat) {
        if(mapping[beat]) wfi.met.Play();
    }

    private void Update() {
        if (Input.GetMouseButton(1) && pointerInside) {
            int samp = wf.getMusicPoint(Input.mousePosition);
            int beat;

            if (samp < edDraw.offset * edDraw.samplesPerSecond + edDraw.spb) {
                beat = 0;
            } else {
                beat = edDraw.SampleToBeat(samp);
            }

            if (mapping[beat]) {
                mapping[beat] = false;
                UpdateEditor();
            }

        }

    }

}
