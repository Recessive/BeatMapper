using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;

public class SaveMapsController : MonoBehaviour
{
    
    public EditorDraw edDraw;

    Button button;
    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(saveMaps);
    }

    private void saveMaps() {
        IntPtr fileName = tinyfd.tinyfd_saveFileDialog("Save beat map file", "map0", 2, new string[] { "*.beatmap", "*.txt" }, "Beat map files (*.beatmap, *.txt)");
        string fileNameString = tinyfd.stringFromAnsi(fileName);

        if(fileNameString != null) {
            string text = edDraw.bpm + "," + edDraw.offset + "\n";
            foreach (EditorController edCtr in edDraw.maps) {
                text += string.Join("", edCtr.mapping.ConvertAll(b => b ? "1" : "0").ToArray()) + "\n";
            }

            File.WriteAllText(fileNameString, text);
            edDraw.unsaved = false;
        }
    }
}
