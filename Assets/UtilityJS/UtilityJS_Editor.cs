#if UNITY_EDITOR

using System.IO;
using UnityEngine;

public partial class UtilityJS
{
    public bool IsMobile => false;

    public void Log(string message) => Debug.Log(message);
    public void Error(string message) => Debug.LogError(message);

    public bool SetStorage(string key, string data)
    {
        using StreamWriter sw = new(Path.Combine(Application.persistentDataPath, key));
        sw?.Write(data);

        return true;
    }
    public string GetStorage(string key)
    {
        string path = Path.Combine(Application.persistentDataPath, key);
        if (File.Exists(path))
        {
            using StreamReader sr = new(path);
            return sr.ReadToEnd();
        }

        return null;
    }
    public bool IsStorage() => false;

    public bool SetCookies(string key, string data)
    {
        PlayerPrefs.SetString(key, data);
        PlayerPrefs.Save();

        return true;
    }
    public string GetCookies(string key)
    {
        if (PlayerPrefs.HasKey(key))
            return PlayerPrefs.GetString(key); 

        return null;
    }
    public bool IsCookies() => true;
}
#endif