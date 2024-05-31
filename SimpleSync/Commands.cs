#region
using System.CommandLine;
using System.IO.Compression;
using KnownHosts;
using LanguageExt;
using Models;
using Renci.SshNet;
using SimpleSync.Binder;
using static LanguageExt.Prelude;
#endregion

namespace SimpleSync;

public class Commands
{
    private readonly IEnumerable<Command> _commandsDefinition;
    private Config? _config;
    public Commands(Command rootCommand)
    {
        var initCommand = new Command("init", "Init a new config file");
        var syncCommand = new Command("sync", "Upload files to the server");

        var pathOption = new System.CommandLine.Option<string?>(new[] {"path", "-P"}, "The path to the config file.");

        var initBinder = new InitOptionBinder();
        initBinder.CommandInit(initCommand);
        initCommand.Add(pathOption);
        syncCommand.Add(pathOption);

        initCommand.SetHandler((p, syncConfig) => {
            _config = new(p);
            var fail = Init(syncConfig).IfFail(ErrorHandler);
        }, pathOption, initBinder);

        syncCommand.SetHandler(p => {
            _config = new(p);
            var fail = Sync().IfFail(ErrorHandler);
        }, pathOption);
        _commandsDefinition = List(initCommand, syncCommand);
        _commandsDefinition.Iter(x => rootCommand.Add(x));
    }

    private Try<Unit> Init(SyncConfig config)
    {
        return Try(() => {
            var ifFailThrow = _config.Save(config).IfFailThrow();
            Console.WriteLine("Config file created.");
            return unit;
        });
    }

    private Try<Unit> Sync()
    {
        return Try(() => {
            var config = _config?.Load()
                                .IfNone(() => throw new("Config file not found."));

            if (config is null)
            {
                Console.WriteLine("Config file not found. Please use `init` command to create one.");
                return unit;
            }

            var keyFile = new PrivateKeyFile(File.OpenRead(config.KeyPathParsed));
            var keyFiles = new[] {keyFile};
            var username = config.Username;

            var methods = new List<AuthenticationMethod>
            {
                new PasswordAuthenticationMethod(username, config.Password),
                new PrivateKeyAuthenticationMethod(username, keyFiles),
            };

            var con = new ConnectionInfo(config.Host, config.Port, username, methods.ToArray());

            using var sshClient = new SshClient(con);

            sshClient.HostKeyReceived += (sender, e) => {
                // check the known hosts file
                var validator = KnownHostsValidator.New(config.KnownHostsPathParsed).IfFail(
                    x => {
                        Console.WriteLine("Could not load known hosts file. Please Check you config file!");
                        throw x;
                    }
                );
                var status = validator.Validate(config.Host, e.HostKeyName, e.HostKey);

                Func<bool> action = status switch
                {
                    Status.MissMatch => () =>
                        AskAndAddKey(
                            "WARNING! Host key MISS MATCH. Do you want to add it to known hosts? (Make sure you know what you are doing!) (Y/n)"),
                    Status.NotFound => () => AskAndAddKey("Host key not found. Do you want to add it to known hosts? (y/n)"),
                    _ => () => true,
                };

                if (!action())
                {
                    e.CanTrust = false;
                }
                return;

                bool AskAndAddKey(string question)
                {
                    Console.WriteLine(question);
                    Console.WriteLine($"Host key:{Convert.ToBase64String(e.HostKey)}");
                    Console.WriteLine($"Host key name:{e.HostKeyName}");
                    var answer = Console.ReadLine();

                    if (answer?.ToLower() is not "y")
                    {
                        Console.WriteLine("Aborted.");
                        return false;
                    }
                    validator.Push(config.Host, e.HostKeyName, e.HostKey);
                    Console.WriteLine("Host key added.");
                    return true;
                }
            };
            sshClient.Connect();
            using var sftpClient = new SftpClient(sshClient.ConnectionInfo);
            sftpClient.Connect();

            foreach (var configTarget in config.Targets)
            {
                var path = Path.GetFullPath(configTarget.From);
                var info = new DirectoryInfo(path);
                var zipName = $"{info.Name}.zip";
                var zipPath = Path.Combine(Path.GetTempPath(), zipName);

                if (File.Exists(zipPath))
                {
                    File.Delete(zipPath);
                }
                ZipFile.CreateFromDirectory(path, zipPath);
                Console.WriteLine("Zip file created.");
                var targetZipPath = Path.Combine(configTarget.To, zipName);
                targetZipPath = targetZipPath.Replace(@"\", "/");
                var totalBytes = new FileInfo(zipPath).Length;

                using var fileStream = File.OpenRead(zipPath);
                Console.WriteLine("Target path: " + targetZipPath);

                if (!sftpClient.Exists(configTarget.To))
                {
                    Console.WriteLine("Target path does not exist. Do you want to create it? (y/n)");
                    var answer = Console.ReadLine();

                    if (answer is null || answer.ToLower() != "y")
                    {
                        Console.WriteLine("Aborted.");
                        return unit;
                    }
                    sftpClient.CreateDirectory(configTarget.To);
                    Console.WriteLine("Directory created.");
                }
                Console.WriteLine("Uploading...");

                sftpClient.UploadFile(fileStream, targetZipPath, true, uploaded => {
                    var percentage = (float) 100.0 * uploaded / totalBytes;
                    Console.Write("\r");
                    Console.Write($"Uploaded {percentage}%... \r");
                });
                Console.WriteLine();
                var command = sshClient.RunCommand($"unzip -o {targetZipPath} -d {configTarget.To} && rm {targetZipPath}");

                if (command.ExitStatus != 0)
                {
                    throw new(command.Error);
                }
                if (configTarget.AfterSync is null || configTarget.AfterSync.Length <= 0) continue;
                Console.WriteLine("Running after sync command...");
                var afterSyncCommand = sshClient.RunCommand($"cd {configTarget.To} && {configTarget.AfterSync}");

                if (afterSyncCommand.ExitStatus != 0)
                {
                    throw new(afterSyncCommand.Error);
                }
            }
            sftpClient.Disconnect();
            sshClient.Disconnect();
            Console.WriteLine("Done.");
            return unit;
        });
    }

    private static void ErrorHandler(Exception e)
    {
        Console.Error.WriteLine(e);
    }
}
