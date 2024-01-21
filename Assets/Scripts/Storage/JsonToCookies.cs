using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;

public class JsonToCookies : SaveLoadJsonTo
{
    private string _key;
    private UtilityJS _utilityJS;

    public override bool IsValid => UtilityJS.InstanceF.IsCookies();

    public async override UniTask<bool> Initialize(string key)
    {
        _key = key;
        _utilityJS = UtilityJS.InstanceF;

        string json;

        await UniTask.Delay(0, true);
        try
        {
            json = _utilityJS.GetCookies(_key);
        }
        catch (Exception ex)
        {
            json = null;
            Message.Log(ex.Message);
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

        _saved = new();
        return false;
    }

    public override void Save(string key, object data, bool isSaveHard, Action<bool> callback)
    {
        bool result;
        if (!(result = SaveSoft(key, data)) || !isSaveHard)
        {
            callback?.Invoke(result);
            return;
        }

        try
        {
            string json = Serialize(_saved);
            result = _utilityJS.SetCookies(_key, json);

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
