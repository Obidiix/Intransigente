using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Dazed.Vitals
{
    public static class PathUtils
    {
        public const string VertexShaderFileExtension = ".vert";
        public const string FragmentShaderFileExtension = ".frag";

        public static string BaseDirectory => AppDomain.CurrentDomain.BaseDirectory;
        public static string ShadersDirectory => Path.Combine(BaseDirectory, "shaders");

        public static string GetExtension(this string value)
        {
            StringBuilder sb = new(value.Length);
            for (int i = value.Length - 1; i >= 0; i--)
            {
                if (value[i] != '.')
                    sb.Insert(0, value[i]);
                else
                {
                    sb.Insert(0, value[i]);
                    break;
                }
            }
            return sb.ToString();
        }

        public static string RemoveExtension(this string value)
        {
            StringBuilder sb = new(value.Length);
            for (int i = value.Length - 1; i >= 0; i--)
            {
                if (value[i] != '.')
                    sb.Insert(0, value[i]);
                else
                {
                    sb.Insert(0, value[i]);
                    break;
                }
            }

            return value.Remove(value.Length - 1 - (sb.Length - 1));
        }

        public static string GetToLastDash(this string value)
        {
            StringBuilder sb = new(value.Length);
            for (int i = value.Length - 1; i >= 0; i--)
            {
                if (value[i].ToString() != @"\")
                    sb.Insert(0, value[i].ToString());
                else
                {
                    if (i == value.Length - 1)
                        continue;
                    break;
                }
            }
            return sb.ToString();
        }

        public static string ToRelativePath(string value)
        {
            StringBuilder sb = new(value.Length);
            StringBuilder tempStringBuilder = new();
            for (int i = value.Length - 1; i >= 0; i--)
            {
                if (value[i].ToString() != @"\")
                {
                    tempStringBuilder.Insert(0, value[i].ToString());
                }
                else
                {
                    if (tempStringBuilder.ToString().ToLower() == "resources")
                    {
                        sb.Insert(0, tempStringBuilder);
                        sb.Insert(0, @"\");
                        break;
                    }
                    sb.Insert(0, tempStringBuilder);
                    sb.Insert(0, @"\");
                    tempStringBuilder = new();
                }
            }
            if (sb[0].ToString() == @"\")
                sb.Remove(0, 1);
            return sb.ToString();
        }

        public static string EndWithPoint(this string s) => s + (s[s.Length - 1] == '.' ? null : '.');

        private const string WindowsLongPathMark = "\\\\?";
        public static bool IsValidPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return false;

            // You can remove these checks for the OS, leaving the check only for length, or add new ones with length
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && path.Length > 260 && path.IndexOf(WindowsLongPathMark) == -1)
                return false;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && path.Length > 4096)
                return false;

            if (path.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
                return false;

            if (Uri.TryCreate(path, UriKind.Absolute, out Uri uri) && uri != null && uri.IsUnc && uri.Host != string.Empty)
                path = uri.AbsolutePath;

            var clearedPath = path;
            if (clearedPath.IndexOf(WindowsLongPathMark) == 0)
                clearedPath = clearedPath.Replace(WindowsLongPathMark, string.Empty);

            if (Path.IsPathRooted(clearedPath))
            {
                var root = Path.GetPathRoot(clearedPath);
                if (clearedPath.IndexOf(root!) > 0)
                    return false;

                // Relative path
                if (root!.Length == 1 && root[0] == Path.DirectorySeparatorChar)
                    clearedPath = clearedPath.Substring(1);
                // Absolute path
                else
                    clearedPath = clearedPath.Replace(root, string.Empty);
            }

            var segments = clearedPath.Split(new[] { Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar });
            if (segments.Any(string.IsNullOrWhiteSpace))
                return false;

            return segments.All(segment => segment.IndexOfAny(Path.GetInvalidFileNameChars()) == -1);
        }

        internal static string BaseDirectoryCombine(string s2) => Path.Combine(BaseDirectory, s2);
    }
}
