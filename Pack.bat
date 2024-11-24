cls
dotnet pack ConsoleR.sln --configuration Release
meziantou.validate-nuget-package .\src\ConsoleR\bin\Release\ConsoleR.0.1.0.nupkg