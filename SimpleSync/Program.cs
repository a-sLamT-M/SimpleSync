// See https://aka.ms/new-console-template for more information


#region
using System.CommandLine;
using SimpleSync;
#endregion

var rootCommand = new RootCommand("A sftp file uploader.");
var commands = new Commands(rootCommand);
await rootCommand.InvokeAsync(args);
