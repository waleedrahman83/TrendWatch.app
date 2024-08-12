using PdfSharpCore.Fonts;
using System.IO;

//public class GlobalFontResolver : IFontResolver
//{
//    public string DefaultFontName => "Arial";

//    public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
//    {
//        string name = familyName.ToLower();
//        string suffix = "";

//        if (isBold && isItalic)
//            suffix = "bi";
//        else if (isBold)
//            suffix = "b";
//        else if (isItalic)
//            suffix = "i";

//        string fontName = $"{name}{suffix}";
//        return new FontResolverInfo(fontName);
//    }

//    public byte[] GetFont(string faceName)
//    {
//        // Embed font resources
//        switch (faceName)
//        {
//            case "arial":
//                return LoadFontData("E:\\My Work\\Dxify\\Bill2GO\\Andriod\\TrendWatch.AppV2\\TrendWatch.App\\Resources\\Fonts\\OpenSans-Regular.ttf");  // Update the namespace and font path
//            case "arialb":
//                return LoadFontData("YourAppNamespace.Fonts.arialbd.ttf");
//            // Add more cases for other fonts and styles
//            default:
//                return null;
//        }
//    }

//    private byte[] LoadFontData(string name)
//    {
//        using (var stream = typeof(GlobalFontResolver).Assembly.GetManifestResourceStream(name))
//        {
//            if (stream == null)
//                return null;
//            using (var ms = new MemoryStream())
//            {
//                stream.CopyTo(ms);
//                return ms.ToArray();
//            }
//        }
//    }
//}


public class CustomFontResolver : IFontResolver
{
    public string DefaultFontName => "OpenSans";

    public byte[] GetFont(string faceName)
    {
        switch (faceName)
        {
            case "OpenSans":
                return LoadFontData("Resources/Fonts/din-next-lt-w23-regular.ttf");
            default:
                throw new ArgumentException($"Font '{faceName}' is not known by the CustomFontResolver.");
        }
    }

    private static byte[] LoadFontData(string fontPath)
    {
        if (File.Exists(fontPath))
        {
            return File.ReadAllBytes(fontPath);
        }
        throw new FileNotFoundException($"The font file '{fontPath}' does not exist.");
    }

    public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
    {
        if (familyName == "OpenSans")
        {
            return new FontResolverInfo("OpenSans");
        }

        return null;
    }
}
