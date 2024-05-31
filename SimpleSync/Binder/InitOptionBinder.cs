#region
using System.CommandLine;
using System.CommandLine.Binding;
using Models;
#endregion

namespace SimpleSync.Binder;

public class InitOptionBinder : BinderBase<SyncConfig>
{
    private readonly Option<string?> _host = new(new[] {"host", "-h"}, "");
    private readonly Option<string?> _keyPath = new(new[]
    {
        "keypath", "-k",
    }, "Private key path of this user");
    private readonly Option<string?> _knownHostsPath = new(new[]
    {
        "knownhosts", "-kh",
    }, "The known hosts path");
    private readonly Option<string?> _password = new(new[]
    {
        "password", "-pd",
    }, "The password of this user");
    private readonly Option<int?> _port = new(new[]
    {
        "port", "-p",
    }, "The port of the server");
    private readonly Option<string[]?> _targetStrings = new(new[]
    {
        "targets", "-t",
    }, "The targets to sync. E.g './to:/home/target:apt update'");
    private readonly Option<string?> _username = new(new[]
    {
        "username", "-u",
    }, "The username");

    public void CommandInit(Command command)
    {
        command.Add(_host);
        command.Add(_port);
        command.Add(_username);
        command.Add(_password);
        command.Add(_keyPath);
        command.Add(_targetStrings);
    }

    protected override SyncConfig GetBoundValue(BindingContext bindingContext) =>
        new(
            bindingContext.ParseResult.GetValueForOption(_host),
            bindingContext.ParseResult.GetValueForOption(_port),
            bindingContext.ParseResult.GetValueForOption(_username),
            bindingContext.ParseResult.GetValueForOption(_password),
            bindingContext.ParseResult.GetValueForOption(_keyPath),
            bindingContext.ParseResult.GetValueForOption(_knownHostsPath),
            bindingContext.ParseResult.GetValueForOption(_targetStrings)
        );
}
