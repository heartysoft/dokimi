properties {
    $BuildDir = $psake.build_script_dir
    $RootDir = "$BuildDir\.."
    $OutDir = "$RootDir\out"
    $CodeDir = "$RootDir\src"
    $ToolsDir = "$RootDir\tools"
    $Config = "Debug"
}


#$env:Path += ";$NUnitDir"

task default -depends CopyToOutput

task CopyToOutput -depends BuildSolution {
    Copy-Item $CodeDir\dokimi\dokimi.nunit\bin\$Config -destination $OutDir\$Config\dokimi.nunit -recurse
}   

task BuildSolution -depends Clean {
    Write-Host "Building dokimi.sln [$Config]" -ForegroundColor Green
    
	Exec { msbuild "$CodeDir\dokimi\dokimi.sln" /t:Build /p:Configuration=$Config /v:quiet } 
}

task Clean {
    Write-host "Cleaning the output directory $OutDir" -ForegroundColor Green
    
    if (Test-Path $OutDir) 
	{	
		rd $OutDir -rec -force | out-null
	}
    
    mkdir $OutDir | out-null
    
    Write-Host "Cleaning $CodeDir\dokimi\dokimi.sln [$Config]" -ForegroundColor Green
    Exec { msbuild "$CodeDir\dokimi\dokimi.sln" /t:Clean /p:Configuration=$Config /v:quiet } 
}
