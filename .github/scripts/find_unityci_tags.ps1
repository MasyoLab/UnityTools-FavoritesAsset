Param()

# Query Docker Hub for unityci/editor tags and pick candidate linux/ubuntu tags for specified unity versions
$versions = @('6000.3f1','2022.3.30f1','2021.3.32f1','2020.3.48f1','2019.4.40f1')
$repo = 'unityci/editor'

Function Get-TagsForVersion([string] $version) {
    $url = "https://registry.hub.docker.com/v2/repositories/$repo/tags/?page_size=100&name=$version"
    Write-Host "Querying $url"
    try {
        $resp = Invoke-RestMethod -Uri $url -UseBasicParsing -ErrorAction Stop
    } catch {
        $msg = $_.Exception.Message
        Write-Error ("Failed to fetch tags for {0}: {1}" -f $version, $msg)
        return @()
    }
    if ($null -eq $resp.results) { return @() }
    return $resp.results | ForEach-Object { $_.name }
}

$mapping = @{}
foreach ($v in $versions) {
    $tags = Get-TagsForVersion $v
    $chosen = $null
    if ($tags.Count -gt 0) {
        # prefer ubuntu linux il2cpp or base
        $chosen = $tags | Where-Object { $_ -like "ubuntu*linux*il2cpp*" } | Select-Object -First 1
        if (-not $chosen) { $chosen = $tags | Where-Object { $_ -like "ubuntu*linux*" } | Select-Object -First 1 }
        if (-not $chosen) { $chosen = $tags | Where-Object { $_ -like "ubuntu*" } | Select-Object -First 1 }
        if (-not $chosen) { $chosen = $tags | Select-Object -First 1 }
    }
    $mapping[$v] = $chosen
}

Write-Host "Found mapping:" 
foreach ($k in $mapping.Keys) {
    Write-Host "$k -> $($mapping[$k])"
}

# Output as JSON to stdout for parsing
$mapping | ConvertTo-Json -Depth 3
