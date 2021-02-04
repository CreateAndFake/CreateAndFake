@ECHO OFF
powershell -ExecutionPolicy ByPass -NoProfile "%~dp0build.ps1" %*
