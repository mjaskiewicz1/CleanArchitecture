namespace Web.Api.Tests.Extensions;

public static class UriExtensions
{
    extension(string basePath)
    {
        /// <summary>
        /// Converts a relative path to a full URI using the provided base path.
        /// </summary>
        /// <param name="subPath">The relative sub-path to append to the base path.</param>
        /// <returns>A new Uri object representing the combined base path and sub-path.</returns>
        public Uri ToRelativeUri(string subPath)
        {
            return new Uri($"{basePath}/{subPath}", UriKind.Relative);
        }

        /// <summary>
        /// Converts a relative path to a full URI using the provided base path.
        /// </summary>
        public Uri ToRelativeUri()
        {
            return new Uri(basePath, UriKind.Relative);
        }
    }
}