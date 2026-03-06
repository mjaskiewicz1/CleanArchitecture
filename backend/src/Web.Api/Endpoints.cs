namespace Web.Api;

public static class EndpointPathMapping
{
    private const string ApiPath = "api";
    public const string ById = "{id:long}";

    public static class Users
    {
        public const string Base = ApiPath + "/user";
        public const string Login = "login";
        public const string RefreshToken = "refresh-token";
        public const string Revoke = "revoke";
        public const string Me = "me";
        public const string SetPassword = "set-password";
    }
}