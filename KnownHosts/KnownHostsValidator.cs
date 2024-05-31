#region
using LanguageExt;
#endregion

namespace KnownHosts;

public class KnownHostsValidator
{

    private string _path;
    private KnownHostsValidator()
    {
        
    }
    public IEnumerable<HostRecord> KnownHosts { get; set; }

    public Try<KnownHostsValidator> SetPath(string path)
    {
        return () => {
            _path = path;
            var knownHostsValidator = Reload().IfFailThrow();
            return this;
        };
    }
    public static Try<KnownHostsValidator> New(string path)
    {
        return () => {
            var knownHostsValidator = new KnownHostsValidator();
            return knownHostsValidator.SetPath(path).IfFailThrow();
        };
    }
    public Try<KnownHostsValidator> Reload()
    {
        return () => {
            KnownHosts = File.ReadAllLines(_path).Select(x => HostRecord.Parse(x));
            return this;
        };
    }
    public Try<KnownHostsValidator> Push(string host, string encryptMethod, byte[] key)
    {
        return () => {
            var line = $"{host} {encryptMethod} {Convert.ToBase64String(key)}";
            File.AppendAllLines(_path, new[] {line});
            return this;
        };
    }

    public Status Validate(string host, string encryptMethod, byte[] key)
    {
        var base64 = Convert.ToBase64String(key);
        var isHostMatch = KnownHosts.FirstOrDefault(x => x != null && x.IsHostMatch(host), null);
        if (isHostMatch is null) return Status.NotFound;
        var isKeyMatch = isHostMatch.IsKeyMatch(key) && isHostMatch.IsEncryptMethodMatch(encryptMethod);
        return !isKeyMatch ? Status.MissMatch : Status.Match;
    }
}
