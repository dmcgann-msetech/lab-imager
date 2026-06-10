using System;
using System.Collections.Generic;
using System.IO;
using PdfSharp.Fonts;

namespace LabImager.Services.Reporting
{
    public sealed class WindowsFontResolver : IFontResolver
    {
        private readonly string _fontsDirectory =
            Environment.GetFolderPath(Environment.SpecialFolder.Fonts);

        private readonly Dictionary<string, string> _fontFiles =
            new(StringComparer.OrdinalIgnoreCase)
            {
                ["Segoe UI#"] = "segoeui.ttf",
                ["Segoe UI#b"] = "segoeuib.ttf",

                ["Calibri#"] = "calibri.ttf",
                ["Calibri#b"] = "calibrib.ttf",

                ["Arial#"] = "arial.ttf",
                ["Arial#b"] = "arialbd.ttf",

                ["Verdana#"] = "verdana.ttf",
                ["Verdana#b"] = "verdanab.ttf",

                ["Tahoma#"] = "tahoma.ttf",
                ["Tahoma#b"] = "tahomabd.ttf",

                ["Georgia#"] = "georgia.ttf",
                ["Georgia#b"] = "georgiab.ttf",

                ["Times New Roman#"] = "times.ttf",
                ["Times New Roman#b"] = "timesbd.ttf",

                ["Cambria#"] = "cambria.ttc",
                ["Book Antiqua#"] = "BKANT.TTF",
                ["Garamond#"] = "GARA.TTF",
                ["Garamond#b"] = "GARABD.TTF",

                ["Consolas#"] = "consola.ttf",
                ["Consolas#b"] = "consolab.ttf",

                ["Courier New#"] = "cour.ttf",
                ["Courier New#b"] = "courbd.ttf"
            };

        public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
        {
            var key = familyName + "#" + (isBold ? "b" : "");

            if (_fontFiles.ContainsKey(key))
            {
                return new FontResolverInfo(key);
            }

            var regularKey = familyName + "#";

            if (_fontFiles.ContainsKey(regularKey))
            {
                return new FontResolverInfo(regularKey);
            }

            return new FontResolverInfo("Segoe UI#");
        }

        public byte[] GetFont(string faceName)
        {
            if (!_fontFiles.TryGetValue(faceName, out var fileName))
            {
                fileName = "segoeui.ttf";
            }

            var path = Path.Combine(_fontsDirectory, fileName);

            if (!File.Exists(path))
            {
                path = Path.Combine(_fontsDirectory, "arial.ttf");
            }

            return File.ReadAllBytes(path);
        }
    }
}
