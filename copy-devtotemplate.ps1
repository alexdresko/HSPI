cd $PSScriptRoot

gci templates\*.dev -Directory | foreach {
    $sourceDirectory = $_.FullName
    $destinationDirectory = $sourceDirectory.Replace(".Dev", "")

    gci $sourceDirectory -Filter *.cs | foreach {
        copy $_.FullName $destinationDirectory -Verbose
    }

    gci $destinationDirectory -Filter *.cs | foreach {
        $content = [System.IO.File]::ReadAllText($_.FullName)
        $content = [System.Text.RegularExpressions.Regex]::Replace($content, "//TODO: (.*\n)\n?(.*\n)", "`$1", [System.Text.RegularExpressions.RegexOptions]::Multiline)
        [System.IO.File]::WriteAllText($_.FullName, $content)
    }
}