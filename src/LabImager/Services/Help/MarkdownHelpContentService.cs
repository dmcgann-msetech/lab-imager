using System;
using System.IO;

namespace LabImager.Services.Help
{
    public sealed class MarkdownHelpContentService : IHelpContentService
    {
        public string LoadHelpContent()
        {
            return LoadDocument("docs", "help", "HELP.md");
        }

        public string LoadDocument(params string[] pathParts)
        {
            var fullPath = Path.Combine(AppContext.BaseDirectory, Path.Combine(pathParts));

            if (!File.Exists(fullPath))
            {
                return "# Document Not Found" +
                       "\r\n\r\nThe requested help document could not be loaded." +
                       "\r\n\r\n## Expected Location" +
                       "\r\n\r\n```text" +
                       "\r\n" + fullPath +
                       "\r\n```";
            }

            return File.ReadAllText(fullPath);
        }
    }
}
