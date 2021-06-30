
#Requires -Module Pester, SHiPS
[CmdletBinding()]
param (
    [Parameter()]
    [String]
    $ModulePath='../ci/artifacts'
)

BeforeDiscovery {
    $(kubectl cluster-info) 2> $null
    if ($LASTEXITCODE -ne 0) {
        Write-Warning -Message "kubectl clouldn't fetch clusterinfo. Skipping K8s cluster based tests..."
        $SkipKubeTests = $true
    }
    else {
        $SkipKubeTests = $false
    }
    'ModulePath -> {0}' -f $ModulePath
}

BeforeAll {
    Import-Module -Name SHiPS -ErrorAction Stop
    $fileName = Split-Path $PSCommandPath -Leaf
    $moduleName = $fileName.Replace('.Tests.ps1', ".psd1")
    $moduleFile = Join-Path -Path $ModulePath -ChildPath $moduleName
    Import-Module $moduleFile -ErrorAction Stop -Verbose
}

Describe "K8sPSDrive Integration tests" {
    
    Context "Module tests" {
        It "Should load the module" {
            Get-Module -Name K8sPSDrive | Should -Not -BeNullOrEmpty
        }
    }
    
    Context "K8sPSDrive map tests" -Skip:$SkipKubeTests {

        BeforeAll {
            New-PSDrive -Name K8sPSDrive -PSProvider SHiPS -Root K8sPSDrive#K8sPSDrive.Root -ErrorAction Stop
            Set-Location -Path K8sPSDrive:\
        }

        It "Should have mapped a PSDrive" {
            Get-PSDrive -PSProvider SHiPS | Should -Not -BeNullOrEmpty
        }
    }
}


