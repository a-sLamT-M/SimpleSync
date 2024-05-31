#region
using LanguageExt;
using static LanguageExt.Prelude;
#endregion

namespace Utils.Utils;

public class PathUtils
{
    public static Try<string> GetFullConfigPath(string path)
    {
        if(Directory.Exists(path))
            return Try(() =>
                           Path.Combine(path, Constants.ConfigName)
            );
        return Try(() =>
                       Path.Combine(path)
        );
    }

    public static string PathParser(string? path)
    {
        if (path is null)
        {
            return Environment.CurrentDirectory;
        }
        var expandedPath = path.Replace("~", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
        var fullPath = Path.GetFullPath(expandedPath);
        return fullPath;
    }
}
