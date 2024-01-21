using System;
using System.Collections.Generic;
using UnityEngine;

public partial class Localization : Singleton<Localization>
{
    [SerializeField] private string _path = "Languages";
    [SerializeField] private string _defaultLang = "en";

    private Dictionary<string, string> _language = new();

    private LanguageType[] _languages;
    public LanguageType[] Languages => _languages;

    public int CurrentIdLang { get; private set; } = -1;

    public event Action EventSwitchLanguage;

    public bool Initialize()
    {
        return LoadFromResources();
    }

    public bool LoadFromResources()
    {
        var (result, value) = StorageResources.LoadFromJson<LanguageType[]>(_path);
        if (result)
        {
            _languages = value;
            return SwitchLanguage(_defaultLang);
        }

        return false;
    }

    public bool TryIdFromCode(string codeISO639_1, out int id)
    {
        id = -1;
        if (string.IsNullOrEmpty(codeISO639_1)) 
            return false;

        foreach (LanguageType language in _languages)
        {
            if (language.CodeISO639_1.ToLowerInvariant() == codeISO639_1.ToLowerInvariant())
            {
                id = language.Id;
                return true;
            }
        }
        return false;  
    }

    public bool SwitchLanguage(string codeISO639_1)
    {
        if (TryIdFromCode(codeISO639_1, out int id))
            return SwitchLanguage(id);

        return false;
    }

    public bool SwitchLanguage(int id)
    {
        if (CurrentIdLang == id) return true;

        foreach (LanguageType language in _languages)
            if (language.Id == id)
                return SetLanguage(language);

        return false;
    }

    public string GetText(string name)
    {
        
        if (_language.TryGetValue(name, out string str))
            return str;

        return "ERROR!";
    }
       

    private bool SetLanguage(LanguageType type)
    {
        var (result, value) = StorageResources.LoadFromJson<Dictionary<string, string>>(type.File);
        if (result)
        {
            CurrentIdLang = type.Id;
            _language = new(value, new StringComparer());
            EventSwitchLanguage?.Invoke();
        }

        return result;
    }

    public class StringComparer : IEqualityComparer<string>
    {
        public bool Equals(string str1, string str2)
        {
            return str1.ToLowerInvariant() == str2.ToLowerInvariant();
        }
        public int GetHashCode(string str)
        {
            return str.ToLowerInvariant().GetHashCode();
        }

    }

}
