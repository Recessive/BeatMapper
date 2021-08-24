using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BpmController : MonoBehaviour
{
    public EditorDraw edDraw;
    public InputField input;


    void Start()
    {
        input.onValueChanged.AddListener(inputChange);
    }

    void inputChange(string val) {
        float f;
        if (float.TryParse(val, out f)) {
            edDraw.updateBpm(f);
            edDraw.UpdateEditor();
        }
    }
}
