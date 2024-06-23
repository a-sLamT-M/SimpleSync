dotnet publish -c release -r win-x64 
dotnet publish -c release -r osx-x64 
dotnet publish -c release -r linux-x64 

# copy ./publish/win-x64/* to J:/CommandLineUtils/