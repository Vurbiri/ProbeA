#if UNITY_EDITOR
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class JsonToFile : SaveLoadJsonTo
{
    private string _path;

    public override bool IsValid => true;

    public async override UniTask<bool> Initialize(string fileName)
    {
        _path = Path.Combine(Application.persistentDataPath, fileName);

        if (File.Exists(_path))
        {
            string json;

            using (StreamReader sr = new(_path))
            {
                json = await sr.ReadToEndAsync();
            }

            if (!string.IsNullOrEmpty(json))
            {
                var (result, value) = Deserialize<Dictionary<string, string>>(json);

                if (result)
                {
                    _saved = value;
                    return true;
                }
            }
        }

        _saved = new();
        return false;
    }

    public override void Save(string key, object data, bool isSaveHard, Action<bool> callback)
    {
        bool result;
        if (!((result = SaveSoft(key, data)) && isSaveHard && _dictModified))
        {
            callback?.Invoke(result);
            return;
        }

        SaveToFileAsync(callback).Forget();
    }

    public async UniTaskVoid SaveToFileAsync(Action<bool> callback)
    {
        bool result = true;
        try
        {
            string json = Serialize(_saved);
            using StreamWriter sw = new(_path);
            await sw.WriteAsync(json);
            _dictModified = false;
        }
        catch (Exception ex)
        {
            result = false;
            Message.Log(ex.Message);
        }
        finally
        {
            callback?.Invoke(result);
        }
    }
}
#endif