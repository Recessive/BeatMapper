using UnityEngine;
using UnityEngine.UI;
using System;
using System.Runtime.InteropServices;



class tinyfd {
    public const string mDllLocation = "Assets\\Scripts\\tinyfile_dlls\\tinyfiledialogs64.dll";

    [DllImport(mDllLocation, CallingConvention = CallingConvention.Cdecl)] public static extern void tinyfd_beep();

    // cross platform UTF8
    [DllImport(mDllLocation, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern int tinyfd_notifyPopup(string aTitle, string aMessage, string aIconType);
    [DllImport(mDllLocation, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern int tinyfd_messageBox(string aTitle, string aMessage, string aDialogTyle, string aIconType, int aDefaultButton);
    [DllImport(mDllLocation, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr tinyfd_inputBox(string aTitle, string aMessage, string aDefaultInput);
    [DllImport(mDllLocation, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr tinyfd_saveFileDialog(string aTitle, string aDefaultPathAndFile, int aNumOfFilterPatterns, string[] aFilterPatterns, string aSingleFilterDescription);
    [DllImport(mDllLocation, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr tinyfd_openFileDialog(string aTitle, string aDefaultPathAndFile, int aNumOfFilterPatterns, string[] aFilterPatterns, string aSingleFilterDescription, int aAllowMultipleSelects);
    [DllImport(mDllLocation, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr tinyfd_selectFolderDialog(string aTitle, string aDefaultPathAndFile);
    [DllImport(mDllLocation, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr tinyfd_colorChooser(string aTitle, string aDefaultHexRGB, byte[] aDefaultRGB, byte[] aoResultRGB);
}

public class SaveMapsController : MonoBehaviour
{
    private static string stringFromAnsi(IntPtr ptr) // for UTF-8/char
    {
        return System.Runtime.InteropServices.Marshal.PtrToStringAnsi(ptr);
    }


    Button button;
    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(saveMaps);
    }

    private void saveMaps() {
        IntPtr fileName = tinyfd.tinyfd_saveFileDialog("Save beat map file", "map0", 1, new string[] {"*.beatmap"}, "Beat map files (*.beatmap)");
        string fileNameString = stringFromAnsi(fileName);

        
    }
}
