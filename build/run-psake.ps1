Param(
    [parameter(Mandatory=$true)]
    [alias("file")]
    $BuildFile
)

$ScriptPath = Split-Path $MyInvocation.InvocationName
$RootPath = "$ScriptPath\.."
$PsakePath = "$RootPath\tools\psake"

Import-Module "$PsakePath\psake.psm1" -force
Invoke-Psake $BuildFile