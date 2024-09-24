using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public static class DBManager
{
    private static string secretKey = "wowsuchanicesecretkeylol"; //Shared by PHP script
    private static string entryLocation = "/masters/addentry.php?";
    private static string idLocation = "/masters/addid.php?";
    private static string completionLocation = "/masters/addcomplete.php?";
    private static string IP = "http://34.44.114.167";
    private static int id = 0;
    private static string backupPath;
    public static string savePath;
    public static bool saveFound = false;

    /**
     * Sends a new user to the DB
     */
    public static IEnumerator SendID(int id)
    {
        //If ID is 0, do nothing
        if (id == 0)
            yield break;

        //Find the current date
        string dateString = DateTime.Now.Date.ToString("d");

        //Make the hash
        string hash = HashInput(dateString + secretKey);

        //Make the web request
        string post_url = IP + idLocation +
            "id=" + id +
            "&date=" + dateString +
            "&hash=" + hash;

        //Send the web request and make sure that it worked
        UnityWebRequest hs_post = UnityWebRequest.Post(post_url, hash);
        Debug.Log(hs_post.uri);
        yield return hs_post.SendWebRequest();
        if (hs_post.error != null)
            Debug.Log("There was an error posting the new id: "
                    + hs_post.error);
    }

    /**
     * Sends an entry to the DB
     */
    public static IEnumerator SendEntry(int levelNum, float points, float diff, int type, int depth, int unknown, int mistakes, float time)
    {
        //If ID is 0, do nothing
        if (id == 0)
            yield break;

        //Find the current date
        string dateString = DateTime.Now.Date.ToString("d");

        //First, write to the backup file
        StreamWriter writer = new StreamWriter(backupPath, true);
        if(levelNum == 1)
            writer.WriteLine("id,level,points,diff,type,depth,unknown,mistakes,time,date"); //Write the header, if needed
        writer.WriteLine(id + "," + levelNum + "," + points + "," + diff + "," + type + "," + depth + "," + unknown + "," + mistakes + "," + time + "," + dateString);
        writer.Close();

        //Hash the name, score, and key
        string hash = HashInput(type.ToString() + depth.ToString() + unknown.ToString() + secretKey);

        //Create the web request out of the URL, name, score, and hash
        string post_url = IP + entryLocation +
            "id=" + id +
            "&level=" + levelNum +
            "&points=" + points +
            "&diff=" + diff +
            "&type=" + type +
            "&depth=" + depth +
            "&unknown=" + unknown +
            "&mistakes=" + mistakes +
            "&time=" + time +
            "&date=" + dateString +
            "&hash=" + hash;

        //Send the web request and make sure that it worked
        UnityWebRequest hs_post = UnityWebRequest.Post(post_url, hash);
        yield return hs_post.SendWebRequest();
        if (hs_post.error != null)
            Debug.Log("There was an error posting the entry: "
                    + hs_post.error);
    }

    /**
     * Sends a new completion to the DB
     */
    public static IEnumerator SendCompletion()
    {
        //If ID is 0, do nothing
        if (id == 0)
            yield break;

        //Find the current date
        string dateString = DateTime.Now.Date.ToString("d");

        //First, write to the backup file
        StreamWriter writer = new StreamWriter(backupPath, true);
        writer.WriteLine(id + "," + -1 + "," + -1 + "," + -1 + "," + -1 + "," + -1 + "," + -1 + "," + -1 + "," + -1 + "," + dateString);
        writer.Close();

        //Make the hash
        string hash = HashInput(dateString + secretKey);

        //Make the web request
        string post_url = IP + completionLocation +
            "id=" + id +
            "&date=" + dateString +
            "&hash=" + hash;

        //Send the web request and make sure that it worked
        UnityWebRequest hs_post = UnityWebRequest.Post(post_url, hash);
        Debug.Log(hs_post.uri);
        yield return hs_post.SendWebRequest();
        if (hs_post.error != null)
            Debug.Log("There was an error posting the new id: "
                    + hs_post.error);
    }

    //Make a hash from the string input
    private static string HashInput(string input)
    {
        //Convert input to bytes and hash
        SHA256Managed hm = new SHA256Managed();
        byte[] hashValue =
                hm.ComputeHash(System.Text.Encoding.ASCII.GetBytes(input));

        //Make it a string we can send
        string hash_convert =
                 BitConverter.ToString(hashValue).Replace("-", "").ToLower();

        return hash_convert;
    }

    /**
     * Initialize DB management
     * 
     * If id is 0, no data will actually be collected
     */
    public static void Initialize(int newId)
    {
        id = newId;

        //If 0 is given, do nothing else
        if (id == 0)
            return;

        //Otherwise, start writing the backup file
        //Path is one step before data, in the "backups" folder
        string[] parts = Application.dataPath.Split('/');
        parts[parts.Length - 1] = "backups";
        parts[0] += Path.DirectorySeparatorChar;
        string path = Path.Combine(parts);

        //if the backups folder doesn't exist, make it
        if(!Directory.Exists(path))
            Directory.CreateDirectory(path);

        //Make the actual backup file
        backupPath = Path.Combine(path, "backup" + id + ".csv");
        File.Create(backupPath);

        //Figure out the saves path
        parts[parts.Length - 1] = "saves";
        parts[0] += Path.DirectorySeparatorChar;
        path = Path.Combine(parts);

        //if the saves folder doesn't exist, make it
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        //Figure out the full save path
        savePath = Path.Combine(path, "save" + id);

        //If the file already exists, indicate it was found
        if (File.Exists(savePath))
            saveFound = true;

        //Otherwise, create the file
        else
            File.Create(savePath);
    }
}
