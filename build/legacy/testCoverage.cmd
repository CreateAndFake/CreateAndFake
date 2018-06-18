@echo off
cd %~dp0

if NOT "%1"=="-chain" (
	call setTargets.cmd -chain
)

if exist ..\..\%coverageDir% (
 rmdir /s /q ..\..\%coverageDir%
)
mkdir ..\..\%coverageDir%

for /D %%x in (..\..\%packagesDir%\opencover\*) do set latestCover=%%x

%latestCover%\tools\OpenCover.Console.exe ^
 -target:"C:\Program Files\dotnet\dotnet.exe" ^
 -targetargs:"test ..\..\%testProj% -c Full -s %testSettingsPath% --no-build --no-restore" ^
 -searchdirs:..\..\%testingDir%\Full ^
 -output:..\..\%coverageRawFile% ^
 -filter:"+[*]* -[*Tests]*" ^
 -register:user -hideskipped:Filter -mergeoutput -oldStyle

for /D %%x in (..\..\%packagesDir%\reportgenerator\*) do set latestReporter=%%x

%latestReporter%\tools\ReportGenerator.exe ^
 -reports:..\..\%coverageRawFile% ^
 -targetdir:..\..\%coverageDir%

@echo Coverage analysis completed.
if NOT "%1"=="-chain" (pause)