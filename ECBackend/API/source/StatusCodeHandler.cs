namespace Carter.App.StatusCodeHandler
{
    using System.IO;
    using System.Threading.Tasks;

    using Carter;

    using HandlebarsDotNet;

    using Microsoft.AspNetCore.Http;

    /// <summary>
    /// Middleware to return a custom page for every error.
    /// </summary>
    public class AnyErrorHandler : IStatusCodeHandler
    {
        /// <inheritdoc/>
        public bool CanHandle(int statusCode)
        {
            // In range [400, 600)
            return statusCode >= 400 && statusCode < 600;
        }

        /// <inheritdoc/>
        public Task Handle(HttpContext ctx)
        {
            string seperator = Path.DirectorySeparatorChar.ToString();
            var source = File.ReadAllText($"Templates{seperator}ErrorPage.html.handlebars");

            var template = Handlebars.Compile(source);

            var data = new
            {
                statusCode = ctx.Response.StatusCode.ToString(),
            };

            return ctx.Response.WriteAsync(template(data));
        }
    }
}