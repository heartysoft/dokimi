$framework = '4.0'
Include .\version.ps1

properties {
    $config= if($config -eq $null) {'Debug' } else {$config}
    $base_dir = resolve-path .\..
    $source_dir = "$base_dir\src"
    $tools_dir = "$base_dir\tools"
    $out_dir = "$base_dir\out\$config"
    $dokimi_dir = "$source_dir\dokimi"
}

task clean {
    rd "$dokimi_dir\dokimi.nunit\bin" -recurse -force  -ErrorAction SilentlyContinue | out-null
    mkdir "$dokimi_dir\dokimi.nunit\bin"  -ErrorAction SilentlyContinue  | out-null
}

task version -depends clean {
     $commitHashAndTimestamp = Get-GitCommitHashAndTimestamp
     $commitHash = Get-GitCommitHash
     $timestamp = Get-GitTimestamp
     $branchName = Get-GitBranchOrTag
	 
	 $assemblyInfos = Get-ChildItem -Path $base_dir -Recurse -Filter AssemblyInfo.cs

	 $assemblyInfo = gc "$base_dir\AssemblyInfo.pson" | Out-String | iex
	 $version = $assemblyInfo.Version
	 #$productName = $assemblyInfo.ProductName
	 $companyName = $assemblyInfo.CompanyName
	 $copyright = $assemblyInfo.Copyright

	 try {
        foreach ($assemblyInfo in $assemblyInfos) {
            $path = resolve-Path $assemblyInfo.FullName -Relative
            #Write-Host "Patching $path with product information."
            Patch-AssemblyInfo $path $Version $Version $branchName $commitHashAndTimestamp $companyName $copyright
        }         
    } catch {
        foreach ($assemblyInfo in $assemblyInfos) {
            $path = resolve-Path $assemblyInfo.FullName -Relative
            Write-Host "Reverting $path to original state."
            & { git checkout --quiet $path }
        }
    }	
}

task compile -depends version {
	try{
		exec { msbuild $dokimi_dir\dokimi.sln /t:Clean /t:Build /p:Configuration=$config /v:q /nologo }
	} finally{
		$assemblyInfos = Get-ChildItem -Path $base_dir -Recurse -Filter AssemblyInfo.cs
		foreach ($assemblyInfo in $assemblyInfos) {
            $path = Resolve-Path $assemblyInfo.FullName -Relative
            Write-Verbose "Reverting $path to original state."
            & { git checkout --quiet $path }
        }
	}
}

task nuget-dokimi-nunit -depends build-dokimi-nunit, publish-dokimi-nunit

task build-dokimi-nunit -depends compile {
	$commitHashAndTimestamp = Get-GitCommitHashAndTimestamp
    $commitHash = Get-GitCommitHash
    $timestamp = Get-GitTimestamp
    $branchName = Get-GitBranchOrTag
	
	$assemblyInfos = Get-ChildItem -Path $base_dir -Recurse -Filter AssemblyInfo.cs

	$assemblyInfo = gc "$base_dir\AssemblyInfo.pson" | Out-String | iex
	$version = $assemblyInfo.Version
	#$productName = $assemblyInfo.ProductName
	$companyName = $assemblyInfo.CompanyName
	$copyright = $assemblyInfo.Copyright

	try {
       foreach ($assemblyInfo in $assemblyInfos) {
           $path = resolve-Path $assemblyInfo.FullName -Relative
           #Write-Host "Patching $path with product information."
           Patch-AssemblyInfo $path $Version $Version $branchName $commitHashAndTimestamp $companyName $copyright
       }         
    } catch {
        foreach ($assemblyInfo in $assemblyInfos) {
            $path = resolve-Path $assemblyInfo.FullName -Relative
            Write-Host "Reverting $path to original state."
            & { git checkout --quiet $path }
        }
    }
	
	try{
		Push-Location "$dokimi_dir\dokimi.nunit"
		exec { & "$dokimi_dir\.nuget\NuGet.exe" "spec"}
		exec { & "$dokimi_dir\.nuget\nuget.exe" pack dokimi.nunit.csproj -IncludeReferencedProjects}
	} finally{
		Pop-Location
		$assemblyInfos = Get-ChildItem -Path $base_dir -Recurse -Filter AssemblyInfo.cs
		foreach ($assemblyInfo in $assemblyInfos) {
            $path = resolve-Path $assemblyInfo.FullName -Relative
            #Write-Verbose "Reverting $path to original state."
            & { git checkout --quiet $path }
        }
	}	
}

task publish-dokimi-nunit -depends build-dokimi-nunit {
	$pkgPath = Get-ChildItem -Path "$dokimi_dir\dokimi.nunit" -Filter "*.nupkg" | select-object -first 1
	exec { & "$dokimi_dir\.nuget\nuget.exe" push "$dokimi_dir\dokimi.nunit\$pkgPath" }
	ri "$dokimi_dir\dokimi.nunit\$pkgPath"
}
