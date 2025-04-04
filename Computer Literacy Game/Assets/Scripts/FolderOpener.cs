using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;

public class FolderOpener : MonoBehaviour
{
    private static string appendPath = "\\leaving_game_files\\stop\\wrong_way\\seriously\\leaving_play_area\\play_area";
    public static string AppendPath => appendPath;
    public static FolderOpener Instance { get; private set; }
    public string challengeStart = "";

    /**
     * On awake, save the instance
     */
    private void Awake()
    {
        Instance = this;
    }

    /**
     * Returns the starting folder path
     */
    public static string GetStartFolderPath()
    {
        string[] pathParts = Application.dataPath.Split('/');
        pathParts[pathParts.Length - 1] = "gamefolder";
        pathParts[0] += Path.DirectorySeparatorChar;
        string path = Path.Combine(pathParts);
        return path;
    }

    /**
     * Returns the path of the toy folder path
     * 
     * @return The string representation of the toy folder path
     */
    public static string GetToyFolderPath()
    {
        return GetStartFolderPath() + appendPath;
    }

    /**
     * Opens the file explorer at the toy folder path
     */
    public void OpenFolder()
    {
        string path = GetToyFolderPath() + "\\" + challengeStart;
        Process.Start("explorer.exe", path);
    }
}
