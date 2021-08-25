using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OffsetController : MonoBehaviour
{


    public InputField input;
    public EditorDraw edDraw;
    void Start()
    {
        input.onValueChanged.AddListener(inputChange);
    }


    public void inputChange(string val) {
        float f;
        if (float.TryParse(val, out f)) {
            edDraw.unsaved = true;
            edDraw.offset = f;
            edDraw.UpdateEditor();
        }
    }

}
