public partial class UtilityJS
{
    private static readonly UtilityJS s_instance = new();

    public static UtilityJS Instance => s_instance;

    private UtilityJS() { }

#if !UNITY_EDITOR
    public bool IsMobile => IsMobileUnityJS();

    public void Log(string message) => LogJS(message);
    public void Error(string message) => ErrorJS(message);

    public bool SetStorage(string key, string data) => SetStorageJS(key, data);
    public string GetStorage(string key) => GetStorageJS(key);
    public bool IsStorage() => IsStorageJS();

    public bool SetCookies(string key, string data) => SetCookiesJS(key, data);
    public string GetCookies(string key) => GetCookiesJS(key);
    public bool IsCookies() => IsCookiesJS();
#endif
}
