#region
using LanguageExt;
using Utils.Utils;
#endregion

namespace Models;

public class SyncConfig
{
    public string Host { get; set; }
    public int Port { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string KeyPath { get; set; }
    public string KnownHostsPath { get; set; }
    public List<Target> Targets { get; set; }

    public string KeyPathParsed => PathUtils.PathParser(KeyPath);
    public string KnownHostsPathParsed => PathUtils.PathParser(KnownHostsPath);
    public SyncConfig()
    {

    }

    public SyncConfig(string? host, int? port, string? username, string? password, string? keyPath, string? knownHostsPath,
                      string[]? targetStrings)
    {
        Host = host ?? "127.0.0.1";
        Port = port ?? 22;
        Username = username ?? "root";
        Password = password ?? "";
        KeyPath = keyPath ?? "~/.ssh/id_ed25519";
        KnownHostsPath = knownHostsPath ?? "~/.ssh/known_hosts";

        if (targetStrings.IsNull() || targetStrings!.Length == 0)
        {
            Targets = new()
            {
                new(),
            };
        }
        else
        {
            Targets = targetStrings!.Select(x => {
                var split = x.Split(":");
                var result =new Target
                {
                    From = split[0],
                    To = split[1],
                }; 
                if(split.Length > 2)
                {
                    result.AfterSync = split[2];
                }
                return result;
            }).ToList();
        }
    }

}
