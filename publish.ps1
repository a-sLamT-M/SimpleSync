dotnet publish -c release -r win-x64 
dotnet publish -c release -r osx-x64 
dotnet publish -c release -r linux-x64 

# copy ./publish/win-x64/* to J:/CommandLineUtils/
cp ./SimpleSync/bin/Release/net7.0/win-x64/publish/ssync.exe J:/CommandLineUtils/ssync.exe