# Packages the mod for release.  Creates a Guil-晓月食物.zip file in the current folder.

$ModName = "Guil-晓月食物";
$ModFolder = "./Package/" + $ModName

mkdir -ErrorAction SilentlyContinue $ModFolder
Remove-Item -ErrorAction SilentlyContinue -Recurse ./Package/*
Remove-Item -ErrorAction SilentlyContinue "$ModName.zip"

dotnet publish .\src\晓月食物.csproj -o $ModFolder

Copy-Item -Recurse ./Guil-晓月食物/* $ModFolder

Compress-Archive -DestinationPath "$ModName.zip" -Path ./Package/*

