namespace KnownHosts;

public class HostRecord
{
    public HostRecord(string host, string key, string encryptMethod)
    {
        Host = host;
        Key = key;
        EncryptMethod = encryptMethod;
    }
    public string Host { get; set; }
    public string Key { get; set; }
    public string EncryptMethod { get; set; }

    public override string ToString() => $"{Host} {EncryptMethod} {Key}";

    public static HostRecord Parse(string line)
    {
        var splited = line.Split(' ');
        return new(splited[0], splited[2], splited[1]);
    }

    public bool IsHostMatch(string host) => Host.Equals(host);

    public bool IsEncryptMethodMatch(string encryptMethod) => EncryptMethod.Equals(encryptMethod);

    public bool IsKeyMatch(byte[] key)
    {
        var base64 = Convert.ToBase64String(key);
        return Key.Equals(base64);
    }
}
