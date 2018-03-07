@echo off
cd %~dp0

if NOT "%1"=="-chain" (
	call setTargets.cmd -chain
)

dotnet build .. -c Debug
dotnet build .. -c Release
dotnet build .. -c Full

@echo Builds completed.
if NOT "%1"=="-chain" (pause)
