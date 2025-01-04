# Symple Sync

This is a simple application to sync files from client to server via ssh.

## Installation

### Windows (Powershell)
```Powershell
iex ((New-Object System.Net.WebClient).DownloadString('https://raw.githubusercontent.com/a-sLamT-M/SimpleSync/master/scripts/install.ps1'))
```

## Usage
Use the following command to init a config file called `ssync.toml` in the directory that contains the files or folders you want to sync.
```
ssync init
```

Then, edit the config file to set the server address and the files or folders you want to sync. It should look like this:
```
host = "host"
port = 22
username = "username"
password = ""
key_path = "~/.ssh/id_ed25519"
known_hosts_path = "~/.ssh/known_hosts"

[[targets]]
from = "./some_folder"
to = "/www/abc"
after_sync = "" # STILL NOT FULLY IMPLEMENTED"
```
Finally, use the following command in this directory to sync the files to the server.
```
ssync sync
```