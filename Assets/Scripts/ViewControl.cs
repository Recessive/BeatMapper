using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewControl : MonoBehaviour
{
    private static ViewControl vcont;

    public static ViewControl instance {
        get {
            return vcont;
        }
    }

    void OnEnable() {
        if (vcont != null)
            Debug.LogError("Only one view controller can be active per scene.");

        vcont = this;
    }

    void OnDisable() {
        vcont = null;
    }

    public static event System.Action<bool> onScroll;
    public static event System.Action onToggle1;


    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0f) // forward
        {
            onScroll?.Invoke(true);
        } 
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f) // backward
        {
            onScroll?.Invoke(false);
        }


        if (Input.GetKeyUp(KeyCode.Space)) {
            onToggle1?.Invoke();
        }

    }
}
