@echo off
setlocal
pushd "%~dp0"
dotnet msbuild EdBPrepareCarefully.csproj -restore -t:Dist -p:Configuration=Release %*
set "result=%errorlevel%"
popd
exit /b %result%
