[CmdletBinding()]
Param(
    [string]$Script = "build.cake",
    [string]$Target = "Default",
    [ValidateSet("Release", "Debug")]
    [string]$Configuration = "Release",
    [ValidateSet("Quiet", "Minimal", "Normal", "Verbose", "Diagnostic")]
    [string]$Verbosity = "Verbose",
    [Alias("DryRun","Noop")]
    [switch]$WhatIf,
    [Parameter(Position=0,Mandatory=$false,ValueFromRemainingArguments=$true)]
    [string[]]$ScriptArgs
)

Write-Host "Preparing to run build script..."

if(!$PSScriptRoot){
    $PSScriptRoot = Split-Path $MyInvocation.MyCommand.Path -Parent
}

$TOOLS_DIR = Join-Path $PSScriptRoot "tools"
$TEMP_DIR = Join-Path $TOOLS_DIR "build"
$TEMP_PROJECT = Join-Path $TEMP_DIR "build.csproj"

# Is this a dry run?
$UseDryRun = "";
if($WhatIf.IsPresent) {
    $UseDryRun = "--dryrun"
}

& dotnet new classlib -o "$TEMP_DIR" --no-restore
& dotnet add "$TEMP_PROJECT" package --package-directory "$TOOLS_DIR" Cake.CoreCLR
Remove-Item -Recurse -Force "$TEMP_DIR"
$CakePath = Get-ChildItem -Filter Cake.dll -Recurse | Sort-Object -Descending | Select-Object -Expand FullName -first 1

Write-Host "Running build script..."
& dotnet "$CakePath" $Script --nuget_useinprocessclient=true --target=$Target --configuration=$Configuration --verbosity=$Verbosity $UseDryRun $ScriptArgs
exit $LASTEXITCODE