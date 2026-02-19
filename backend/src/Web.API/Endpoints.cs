namespace Web.Api;

public static class EndpointPathMapping
{
    private const string ApiPath = "api";
    public const string ById = "{id:ulong}";

    public static class Users
    {
        public const string Base = ApiPath + "/user";
        public const string Login = "login";
        public const string RefreshToken = "refresh-token";
        public const string Revoke = "logout";
    }
}