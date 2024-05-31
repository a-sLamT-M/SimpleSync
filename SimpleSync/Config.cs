#region
using LanguageExt;
using Models;
using Tomlyn;
using Utils.Utils;
using static LanguageExt.Prelude;
#endregion

namespace SimpleSync;

public class Config
{
    private readonly string _path;
    public Config(string? path)
    {
        _path = PathUtils.PathParser(path);
        _path = PathUtils.GetFullConfigPath(_path).IfFailThrow();
    }
    public Option<SyncConfig> Load()
    {
        if (!File.Exists(_path)) return None;
        var str = File.ReadAllText(_path);
        var config = Toml.ToModel<SyncConfig>(str);
        return config;
    }

    public Try<Unit> Save(SyncConfig config)
    {
        return Try(() => {
            if (File.Exists(_path))
            {
                throw new("Config file already exists.");
            }
            File.Create(_path).Close();
            var text = Toml.FromModel(config);
            File.WriteAllText(_path, text);
            return unit;
        });
    }
}
