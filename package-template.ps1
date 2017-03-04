.\copy-devtotemplate.ps1

.\package-nuget.ps1

.\update-vsixversion.ps1

& 'C:\Program Files (x86)\MSBuild\14.0\bin\MSBuild.exe' /t:Build /p:Configuration=Release

"Updated $file to $newVersion"