namespace SymplifySDK.Cookies
{
    /// <summary>
    /// ICookieJar is an interface the SDK depends on for reading and writing cookies to persist visitor and test data.
    /// </summary>
    public interface ICookieJar
    {
        /// <summary>
        /// Gets the value of the cookie with the given name. Decode it if your web framework does not do that.
        /// </summary>
        public string GetCookie(string name);

        /// <summary>
        /// Sets the cookie with the given name to the given value. Encode it if your web framework does not do that.
        /// The cookie should be set for your zone apex domain (see https://www.rfc-editor.org/rfc/rfc8499).
        /// </summary>
        public void SetCookie(string name, string value, uint expireInDays);
    }
}
