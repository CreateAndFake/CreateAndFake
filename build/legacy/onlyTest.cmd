@echo off
cd %~dp0

if NOT "%1"=="-chain" (
	call setTargets.cmd -chain
)

dotnet test ..\..\%testProj% -c Debug -s %testSettingsPath% --no-restore --no-build
dotnet test ..\..\%testProj% -c Release -s %testSettingsPath% --no-restore --no-build

@echo Tests completed.
if NOT "%1"=="-chain" (pause)
