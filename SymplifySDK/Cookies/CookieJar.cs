namespace SymplifySDK.Cookies
{
    public interface ICookieJar
    {
        public string GetCookie(string name);
        public void SetCookie(string name, string value);
    }
}
