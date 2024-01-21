public static class Helper
{
    public static string RandomString(int length)
    {
        string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        chars += chars.ToLower();

        char[] str = new char[length];
        for(int i = 0; i < length; i++)
            str[i] = chars[URandom.Range(0 , chars.Length)];
        return new string(str);
    }
}
