Param(
  [string]$configuration,
  [string]$keep
)

if ($configuration -eq "") {
    $configuration = "Debug"
}

if ($keep -eq "") {
    [xml]$proj = get-content (gci $PSScriptRoot\* -Include *.csproj, *.vbproj | select -First 1 -ExpandProperty FullName) 
    $keep = $($proj.Project.PropertyGroup.AssemblyName[0])
}

$keep = $keep.Replace("HSPI_", "")

"Keeping $keep"

$source = "$PSScriptRoot\bin\$configuration"
$destination = "$source\bin\$keep"

if ((test-path $destination) -eq $False) {
    mkdir $destination -Verbose
}


gci $source -File | where { $_.Name -notmatch ".*$keep.*" } | % {
    Move-Item $_.FullName -Destination $destination -Force -Verbose
}

