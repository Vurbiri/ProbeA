using Newtonsoft.Json;
using System;
using UnityEngine;
using UnityEngine.Audio;

public class SettingsGame : Singleton<SettingsGame>
{ 
    [Space]
    [SerializeField] private Profile _profileDesktop = new();
    [Space]
    [SerializeField] private AudioMixer _audioMixer;

    private Profile _profileCurrent = null;

    public float MinValue => audioMinValue;
    public float MaxValue => audioMaxValue;

    public bool IsFirstStart { get; set; } = true;
    public bool IsScreenStick { get => _profileCurrent.isScreenStick; set => _profileCurrent.isScreenStick = value; }

    private const float audioMinValue = 0.01f;
    private const float audioMaxValue = 1.5845f;

    private Localization _localization;

    public bool Initialize(bool isLoad)
    {
        _localization = Localization.InstanceF;

        bool result = false;

        DefaultProfile();
        if (isLoad)
            result = Load();
        Apply();

        return result;
    }

    public void SetVolume(MixerGroup type, float volume)
    {
        _audioMixer.SetFloat(type.ToString(), ConvertToDB(volume));

        static float ConvertToDB(float volume)
        {
            volume = Mathf.Log10(volume) * 40f;
            if (volume > 0) volume *= 2.5f;

            return volume;
        }
    }
    public float GetVolume(MixerGroup type) => _profileCurrent.volumes[type.ToInt()];

    public void Save(bool isSaveHard = true, Action<bool> callback = null)
    {
        _profileCurrent.idLang = _localization.CurrentIdLang;
        foreach (var mixer in Enum<MixerGroup>.GetValues())
        {
            _audioMixer.GetFloat(mixer.ToString(), out float volumeDB);
            _profileCurrent.volumes[mixer.ToInt()] = MathF.Round(ConvertFromDB(volumeDB), 3);
        }

        Storage.Save(_profileCurrent.key, _profileCurrent, isSaveHard, callback);

        static float ConvertFromDB(float dB)
        {
            if (dB > 0) dB /= 2.5f;
            dB = Mathf.Pow(10, dB / 40f);

            return dB;
        }
    }
    private bool Load()
    {
        var (result, value) = Storage.Load<Profile>(_profileCurrent.key);
        if (result)
            _profileCurrent.Copy(value);

        return result;
    }

    public void Cancel()
    {
        if (!Load())
            DefaultProfile();
        
        Apply();
    }

    private void DefaultProfile()
    {
        _profileCurrent = _profileDesktop.Clone();
    }

    private void Apply()
    {
        _localization.SwitchLanguage(_profileCurrent.idLang);
        foreach (var mixer in Enum<MixerGroup>.GetValues())
            SetVolume(mixer, _profileCurrent.volumes[mixer.ToInt()]);
    }
        
    #region Nested Classe
    [System.Serializable]
    private class Profile
    {
        [JsonIgnore]
        public string key = "sts";
        [JsonProperty("ilg")]
        public int idLang = 1;
        [JsonProperty("vls")]
        public float[] volumes = { 1f, 1f};
        [JsonProperty("iss")]
        public bool isScreenStick = true;
        [JsonProperty("thr")]
        public float zeroAngle = 0.18f;

        [JsonConstructor]
        public Profile(int idLang, float[] volumes, bool isScreenStick, float zeroAngle)
        {
            this.idLang = idLang;
            volumes.CopyTo(this.volumes, 0);
            this.isScreenStick = isScreenStick;
            this.zeroAngle = zeroAngle;
        }

        public Profile() { }

        public void Copy(Profile profile)
        {
            if (profile == null) return;

            idLang = profile.idLang;
            profile.volumes.CopyTo(volumes, 0);
            isScreenStick = profile.isScreenStick;
            zeroAngle = profile.zeroAngle;
        }

        public Profile Clone()
        {
            return new(idLang, volumes, isScreenStick, zeroAngle) { key = key };
        }

    }
    #endregion
}
