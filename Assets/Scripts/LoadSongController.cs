using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.IO;
using System.Collections;

public class LoadSongController : MonoBehaviour
{
    public EditorDraw edDraw;
    public InputField bpmInput;
    public InputField offsetInput;
    public WaveformTex wf;
    public AudioSource mainAud;

    Button button;
    void Start() {
        button = GetComponent<Button>();
        button.onClick.AddListener(loadSong);
    }


    void loadSong() {
        if (edDraw.unsaved) {
            int choice = tinyfd.tinyfd_messageBox("Warning", "You will lose all unsaved work if you load a new song. Continue?", "yesno", "warning", 1);

            if (choice != 1) return;
        }

        IntPtr fileName = tinyfd.tinyfd_openFileDialog("Load song", "", 0, null, "Audio file", 0);
        string fileNameString = tinyfd.stringFromAnsi(fileName);


        if (fileNameString != null) {
            edDraw.clear();

            edDraw.bpm = 1;
            bpmInput.text = "1";
            edDraw.offset = 0;
            offsetInput.text = "0";
            StartCoroutine(LoadSongCoroutine(fileNameString, SongLoaded));
            
        }
    }

    IEnumerator LoadSongCoroutine(string songName, Action<AudioClip> onSongLoaded) {
        string url = string.Format("file://{0}", songName);
        WWW www = new WWW(url);

        yield return www;

        AudioClip clip = www.GetAudioClip(false, false);
        onSongLoaded?.Invoke(clip);
    }

    void SongLoaded(AudioClip clip) {
        mainAud.clip = clip;
        wf.SongLoaded();
        edDraw.SongLoaded();
        edDraw.samplesPerSecond = mainAud.clip.samples / mainAud.clip.length;
    }
}
