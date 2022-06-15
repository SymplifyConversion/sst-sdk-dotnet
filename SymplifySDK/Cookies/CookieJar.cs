namespace SymplifySDK.Cookies
{
    /// <summary>
    /// ICookieJar is an interface the SDK depends on for reading and writing cookies to persist visitor and test data.
    /// </summary>
    public interface ICookieJar
    {
        /// <summary>
        /// Gets the value of the cookie with the given name.
        /// </summary>
        public string GetCookie(string name);

        /// <summary>
        /// Sets the cookie with the given name to the given value.
        /// </summary>
        public void SetCookie(string name, string value);
    }
}
