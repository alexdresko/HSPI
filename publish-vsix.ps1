$vsixpublish = Get-ChildItem -File .\packages -recurse | 
    Where-Object { $_.Name -eq "VsixPublisher.exe" } | 
    Sort-Object -Descending -Property CreationTime | 
    Select-Object -First 1 -ExpandProperty FullName

. $vsixpublish login -publisherName thealexdresko -personalAccessToken $env:homeseertemplatespublish

$overview = (Get-Item .\Templates\HomeSeerTemplates\overview.md).FullName
$manifest = (Get-Item .\Templates\HomeSeerTemplates\publish-manifest.json).FullName
$vsix = (Get-Item .\Templates\HomeSeerTemplates\bin\Release\HomeSeerTemplates.vsix).FullName
Write-Host "vsix: $vsix"
Write-Host "manifest: $manifest"
Write-Host "overview: $overview"

# . $vsixpublish deleteExtension -extensionName "HomeSeerTemplates" -publisherName "thealexdresko"

. $vsixpublish publish -payload "$vsix" -publishManifest "$manifest"
