using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.IO;

public class EditorController : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler 
{
    public int id;
    public EditorDraw edDraw;

    public AudioSource aud;
    public WaveformTex wf;
    public WaveformInteract wfi;

    public Toggle tapNoteToggle;
    public Toggle playSoundToggle;
    public Dropdown metSoundDropdown;
    public Slider volumeSlider;
    public Toggle deleteToggle;
    public GameObject buttonGO;
    public Button deleteButton;

    bool tapEnabled = true;
    bool playSoundEnabled = true;

    public List<bool> mapping = new List<bool>();

    Image im;
    AudioSource metAud;

    bool pointerInside = false;

    void OnEnable() {
        wfi.onBeat += beat;
    }

    private void OnDisable() {
        wfi.onBeat -= beat;
    }

    private void Start() {
        tapNoteToggle.onValueChanged.AddListener(tapChange);
        playSoundToggle.onValueChanged.AddListener(playSoundChange);
        volumeSlider.onValueChanged.AddListener(volumeChange);
        metSoundDropdown.onValueChanged.AddListener(metSoundChange);
        deleteToggle.onValueChanged.AddListener(deleteToggleChange);
        deleteButton.onClick.AddListener(deleteButtonClicked);

        metAud = GetComponent<AudioSource>();

        metSoundDropdown.options.Clear();
        string datPath = Application.dataPath;
        DirectoryInfo dir = new DirectoryInfo(datPath + "/Resources/Sounds");
        FileInfo[] info = dir.GetFiles("*.wav");
        List<string> fnames = new List<string>();

        foreach (FileInfo f in info) {
            string fn = Path.GetFileNameWithoutExtension(f.ToString());
            fnames.Add(fn);
        }
        metSoundDropdown.AddOptions(fnames);
        metSoundChange(0);

        im = GetComponent<Image>();
        
        updateBpm();

        UpdateEditor();
    }

    void tapChange(bool val) {
        tapEnabled = val;
    }

    void playSoundChange(bool val) {
        playSoundEnabled = val;
    }

    void metSoundChange(int val) {
        
        AudioClip clip = Resources.Load<AudioClip>("Sounds/" + metSoundDropdown.options[val].text);
        metAud.clip = clip;
    }

    void volumeChange(float val) {
        metAud.volume = val;
    }

    void deleteToggleChange(bool val) {
        buttonGO.SetActive(val);
    }

    void deleteButtonClicked() {
        edDraw.deleteMap(id);
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

        edDraw.unsaved = true;
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

        im.sprite = Sprite.Create(tex, rect, Vector2.zero);
    }

    private void beat(int beat) {
        if(mapping[beat] && playSoundEnabled) metAud.Play();
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
                edDraw.unsaved = true;
                mapping[beat] = false;
                UpdateEditor();
            }

        }


        if (tapEnabled &&
            Input.anyKeyDown &&
            Input.inputString.Length > 0 &&
            char.IsLetter(Input.inputString[0])) {
            int samp = aud.timeSamples - (int)(edDraw.inputDelay * edDraw.samplesPerSecond);
            int beat;
            if (samp < edDraw.offset * edDraw.samplesPerSecond + edDraw.spb) {
                beat = 0;
            } else {
                beat = edDraw.SampleToBeat(samp);
            }

            if (!mapping[beat]) {
                edDraw.unsaved = true;
                mapping[beat] = true;
                UpdateEditor();
            }
                
        }

    }

}
