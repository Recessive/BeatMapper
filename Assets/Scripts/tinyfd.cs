using System;
using System.Runtime.InteropServices;
using System.IO;

class tinyfd {
    public const string mDllLocation = "tinyfiledialogs64.dll";

    [DllImport(mDllLocation, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern int tinyfd_messageBox(string aTitle, string aMessage, string aDialogTyle, string aIconType, int aDefaultButton);

    [DllImport(mDllLocation, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr tinyfd_saveFileDialog(string aTitle, string aDefaultPathAndFile, int aNumOfFilterPatterns, string[] aFilterPatterns, string aSingleFilterDescription);

    [DllImport(mDllLocation, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr tinyfd_openFileDialog(string aTitle, string aDefaultPathAndFile, int aNumOfFilterPatterns, string[] aFilterPatterns, string aSingleFilterDescription, int aAllowMultipleSelects);


    public static string stringFromAnsi(IntPtr ptr) // for UTF-8/char
    {
        return System.Runtime.InteropServices.Marshal.PtrToStringAnsi(ptr);
    }

}

