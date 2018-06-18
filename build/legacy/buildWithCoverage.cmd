@echo off
cd %~dp0

call setTargets.cmd -chain

call buildWithTests.cmd -chain
call testCoverage.cmd -chain

pushd ..\..
for /D %%x in (%packagesDir%\codecov\*) do (set latestCov=%%x)
if "%1"=="-WithUpload" (
 %latestCov%\tools\codecov.exe -f %coverageRawFile%
)
popd

echo Build process completed.
if NOT "%1"=="-chain" (pause)
