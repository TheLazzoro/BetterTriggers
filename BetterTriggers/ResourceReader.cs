using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace BetterTriggers;
internal static class ResourceReader
{
    public static Stream ReadAsStream(string path)
    {
        path = "/BetterTriggers;component/" + path;
        var uri = new Uri(path, UriKind.RelativeOrAbsolute);
        var info = Application.GetResourceStream(uri);
        return info.Stream;
    }

    public static byte[] ReadAllBytes(string path)
    {
        path = "/BetterTriggers;component/" + path;
        var uri = new Uri(path, UriKind.RelativeOrAbsolute);
        var info = Application.GetResourceStream(uri);
        using (MemoryStream ms = new MemoryStream())
        {
            info.Stream.CopyTo(ms);
            return ms.ToArray();
        }
    }

    public static string ReadAllText(string path)
    {
        path = "/BetterTriggers;component/" + path;
        var uri = new Uri(path, UriKind.RelativeOrAbsolute);
        var info = Application.GetResourceStream(uri);
        StreamReader reader = new StreamReader(info.Stream);
        string content = reader.ReadToEnd();
        return content;
    }

    public static List<string> ReadAllLines(string path)
    {
        path = "/BetterTriggers;component/" + path;
        var uri = new Uri(path, UriKind.RelativeOrAbsolute);
        var info = Application.GetResourceStream(uri);
        StreamReader reader = new StreamReader(info.Stream);
        List<string> lines = new List<string>();
        string? line = null;
        while (true)
        {
            line = reader.ReadLine();
            if (line == null) break;
            lines.Add(line);
        }
        return lines;
    }
}
