cd $PSScriptRoot

gci templates\*.dev -Directory | foreach {
    $sourceDirectory = $_.FullName
    "Source directory: $sourceDirectory"

    $destinationDirectory = $sourceDirectory.Replace(".Dev", "")
    "Destination directory: $destinationDirectory"

    gci "$sourceDirectory\*" -Include *.cs, *.vb, app.config | foreach {
        copy $_.FullName $destinationDirectory -Verbose
    }

    gci "$destinationDirectory\*" -Include *.cs, *.vb, app.config | foreach {
        "Fixing $($_.FullName)"
        $content = [System.IO.File]::ReadAllText($_.FullName)
        $content = [System.Text.RegularExpressions.Regex]::Replace($content, "(?<prefix>//|<!--)\s*TODO:\s*?(?<whatiwant>.*?)(?<ending>-->).*(?<newline>\n)(?<possibleblank>^\s*)(?<toreplace>.*)\n", "`$2", [System.Text.RegularExpressions.RegexOptions]::Multiline)
        [System.IO.File]::WriteAllText($_.FullName, $content)
    }
}

gci templates\all | % {
    $file = $_.FullName
    $name = $_.Name

    gci templates -Directory | where { $_.Name -notmatch "All|HomeSeerTemplates"} | % { 
        copy-item $file -Destination $_ -Force -Verbose
    }
}