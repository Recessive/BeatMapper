using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.IO;


public class LoadMapsController : MonoBehaviour
{

    public EditorDraw edDraw;
    public InputField bpmInput;
    public InputField offsetInput;


    Button button;
    void Start() {
        button = GetComponent<Button>();
        button.onClick.AddListener(loadMaps);
    }


    private void loadMaps() {
        if(edDraw.unsaved) {
            int choice = tinyfd.tinyfd_messageBox("Warning", "You will lose all unsaved work if you load a file. Continue?", "yesno", "warning", 1);

            if (choice != 1) return;
        }

        IntPtr fileName = tinyfd.tinyfd_openFileDialog("Save beat map file", "", 2, new string[] { "*.beatmap", "*.txt" }, "Beat map files (*.beatmap, *.txt)", 0);
        string fileNameString = tinyfd.stringFromAnsi(fileName);

        if (fileNameString != null) {
            string readText = File.ReadAllText(fileNameString);
            string[] lines = readText.Split('\n');
            
            while(edDraw.maps.Count > 0) {
                edDraw.deleteMap(0);
            }

            edDraw.updateBpm(float.Parse(lines[0].Split(',')[0]));
            bpmInput.text = lines[0].Split(',')[0];
            edDraw.offset = float.Parse(lines[0].Split(',')[1]);
            offsetInput.text = lines[0].Split(',')[1];


            for (int i = 1; i< lines.Length-1; i++) {
                edDraw.addNewMap();
                // Strip the new line, then convert the string of 1's and 0's to a list of booleans
                edDraw.maps[i - 1].mapping = lines[i].Split('\n')[0].Select(c => c == '1').ToArray().ToList();
            }

            edDraw.unsaved = false;

        }

        


    }


}
