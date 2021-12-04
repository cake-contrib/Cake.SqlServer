#-------------------------------
# Installation
#-------------------------------

Write-Host "Downloading .NET SDK 6.0 ..."

(New-Object System.Net.WebClient).DownloadFile('https://download.visualstudio.microsoft.com/download/pr/0f71eaf1-ce85-480b-8e11-c3e2725b763a/9044bfd1c453e2215b6f9a0c224d20fe/dotnet-sdk-6.0.100-win-x64.exe','dotnet-sdk-6.0.100-win-x64.exe')
# Invoke-WebRequest "https://go.microsoft.com/fwlink/?linkid=841686" -OutFile "dotnet-core-sdk.exe"

Write-Host "Installing .NET SDK 6.0 ..."

Invoke-Command -ScriptBlock { ./dotnet-sdk-6.0.100-win-x64.exe /S /v/qn }

Write-Host "Installation succeeded." -ForegroundColor Green