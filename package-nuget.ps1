nuget pack $PsScriptRoot\hspi -verbosity detailed -version 1.0.0
copy $PsScriptRoot\HSPI.1.0.0.nupkg $PsScriptRoot\Templates\HomeSeerTemplates\Packages -Verbose