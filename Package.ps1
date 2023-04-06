# Creates the release's .zip file

$ModName = "Guil-晓月食物";
$ModFolder = "./Package/" + $ModName
$ArchiveName = "Guil-MoonFood.zip"

mkdir -ErrorAction SilentlyContinue $ModFolder
Remove-Item -ErrorAction SilentlyContinue -Recurse ./Package/*
Remove-Item -ErrorAction SilentlyContinue $ArchiveName

dotnet publish .\src\晓月食物.csproj -o $ModFolder

Copy-Item -Recurse ./Guil-晓月食物/* $ModFolder

# English name since github strips Unicode for security purposes.
Compress-Archive -DestinationPath $ArchiveName -Path ./Package/*

