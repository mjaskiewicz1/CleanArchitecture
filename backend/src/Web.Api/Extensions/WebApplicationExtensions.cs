using Scalar.AspNetCore;


namespace Web.Api.Extensions;

public static class WebApplicationExtensions
{

    extension(WebApplication app)
    {
        public void MapPresentation(bool development)
        {
            ResultExtensions.Init(app.Services.GetRequiredService<IHttpContextAccessor>());
            if (development)
            {
                app.MapScalar();
            }

            app.UseExceptionHandler();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
        }

        private void MapScalar()
        {
            app.MapOpenApi();
            app.MapScalarApiReference("/docs", static x =>
            {
                x.Title = "Clean Architecture API";
                x.Theme = ScalarTheme.Alternate;
                x.DarkMode = true;
                x.DefaultHttpClient =
                    new KeyValuePair<ScalarTarget, ScalarClient>(ScalarTarget.JavaScript, ScalarClient.Axios);
                x.ShowSidebar = true;
                x.DocumentDownloadType = DocumentDownloadType.Json;
                x.HideDarkModeToggle = false;
                x.DotNetFlag = true;
                x.SearchHotKey = "l";
                x.HideTestRequestButton = false;
                x.EnabledClients = [ScalarClient.Axios, ScalarClient.Curl];
                x.EnabledTargets = [ScalarTarget.JavaScript, ScalarTarget.Shell];
                x.HideClientButton = true;
                x.DefaultOpenAllTags = false;
            });

        }
    }
}