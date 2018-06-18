@echo off
cd %~dp0

if NOT "%1"=="-chain" (
	call setTargets.cmd -chain
)

call onlyBuild.cmd -chain
call onlyTest.cmd -chain

echo Build and tests completed.
if NOT "%1"=="-chain" (pause)
