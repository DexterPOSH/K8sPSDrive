#!/usr/bin/env pwsh
[CmdletBinding()]
param(
    [Switch]$BootStrapOnly
)
begin {
    $ErrorActionPreference = 'Stop'

    Set-Location -LiteralPath $PSScriptRoot
    
    $env:DOTNET_SKIP_FIRST_TIME_EXPERIENCE = '1'
    $env:DOTNET_CLI_TELEMETRY_OPTOUT = '1'
    $env:DOTNET_NOLOGO = '1'

    $requiredModules = @('Pester', 'ShiPS')
}
end {
    if ($BootStrapOnly.IsPresent) {
        'BootStrapOnly Mode...'
        dotnet tool restore
        if ($LASTEXITCODE -ne 0) {
            exit $LASTEXITCODE 
        }

        foreach ($module in $requiredModules) {
            $modAvailable = Get-Module -Name $module -ListAvailable

            if (-not $modAvailable) {
                Install-Module -Name $module -Repository PSGallery -Scope CurrentUser -Force -Verbose
            }
        }
    }
    else {
        dotnet cake @args
        if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
    }
}
