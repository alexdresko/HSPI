Write-Host "Building nuget package"
nuget pack $PsScriptRoot\hspi -verbosity detailed -version 1.0.0

Write-Host "Copying to packages folder"
copy $PsScriptRoot\HSPI.1.0.0.nupkg $PsScriptRoot\Templates\HomeSeerTemplates\Packages -Verbose