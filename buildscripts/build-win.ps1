$config ='Release'
$slnFolder = '..\source\'
$appProjectPath = '..\source\JpegExifReader.Startup\JpegExifReader.Startup.csproj'
$publicFolder = '..\public\'

Push-Location
cd "$PSScriptRoot"

Write-Host 'Building the solution...'
dotnet publish $appProjectPath -c:$config --self-contained -r:win-x64 -f:net10.0 -p:PublishSingleFile=true -o:$publicFolder

Pop-Location
Write-Host 'Build complete.'