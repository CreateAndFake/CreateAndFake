@echo off
cd %~dp0

set artifactsDir=artifacts
set packagesDir=%artifactsDir%\packages
set testingDir=%artifactsDir%\testing
set coverageDir=%artifactsDir%\coverage
set coverageRawFile=%coverageDir%\CoverageRaw.xml

set testProj=tests/CreateAndFakeTests/CreateAndFakeTests.csproj
set testSettingsPath=../TestSettings.runsettings

@echo Variables set.
if NOT "%1"=="-chain" (pause)
