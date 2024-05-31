// See https://aka.ms/new-console-template for more information

#region
using System.IO.Compression;
using LanguageExt;
using Models;
using Newtonsoft.Json;
using static LanguageExt.Prelude;
#endregion

// Zip("./shit/").IfFail(x => Console.WriteLine(x.Message));

var config = new SyncConfig();
Console.WriteLine(JsonConvert.SerializeObject(config));

Try<Unit> Zip(string path)
{
    return Try(() => {
        path = Path.GetFullPath(path);

        var info = new DirectoryInfo(path);

        var zipPath = Path.Combine(path, $"{info.Name}.zip");

        if (File.Exists(zipPath))
        {
            File.Delete(zipPath);
        }
        ZipFile.CreateFromDirectory(path, zipPath);
        return unit;
    });
}
