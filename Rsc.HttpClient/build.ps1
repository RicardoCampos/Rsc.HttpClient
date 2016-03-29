$build="&msbuild Rsc.HttpClient.csproj /t:Build /p:Configuration='Release 4.6.1'"
Invoke-Expression $build
$build="&msbuild Rsc.HttpClient.csproj /t:Build /p:Configuration='Release 4.5.1'"
Invoke-Expression $build
$build="&msbuild Rsc.HttpClient.csproj /t:Build /p:Configuration='Release 4.5'"
Invoke-Expression $build

Remove-Item  build -Recurse
mkdir build

$version = gc Properties\AssemblyInfo.cs | select-string -pattern "AssemblyVersion"
$version -match '^\[assembly: AssemblyVersion\(\"(?<major>[0-9]+)\.(?<minor>[0-9]+)\.(?<revision>[0-9]+)\"\)\]' |Out-Null
$nversion= "{0}.{1}.{2}" -f $matches["major"],$matches["minor"],$matches["revision"]
$output= $nversion+$args
$pack="& ..\NuGet\Nuget.exe pack Rsc.HttpClient.nuspec -Properties Configuration=Debug -Verbosity detailed -Build -OutputDirectory build\ -Version '$output'"
Invoke-Expression $pack