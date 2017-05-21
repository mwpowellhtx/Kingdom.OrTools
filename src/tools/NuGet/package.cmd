@echo off

setlocal

::set nuget_exe=C:\Dev\NuGet\bin\NuGet.exe
set nuget_exe=.nuget\NuGet.exe
set config=Release
REM :: AnyCPU is necessary in order to persuade the MSBuild system that it can properly build the CSPROJ.
REM :: Coupled with forcing the CSPROJ to build for x64 (64-bit) Platform.
REM set platform=anycpu
:: TODO: TBD: could potentially receive a further local directory here, i.e. <PathToNuGet/>\packages
set packages_dir=tools\NuGet\packages
set package_files=*.nupkg

:: Run the package request from the root solution directory.
pushd ..\..

:: Package each of the known projects.
call :packprojects Kingdom.OrTools.Core
call :packprojects Kingdom.OrTools.ConstraintSolver.Core

:: Now move the packages.
call :movepackages

popd

goto end

:packprojects
%nuget_exe% pack %1\%1.csproj -Prop Configuration=%config%
exit /b

:movepackages
if exist %package_files% (
    @echo Moving the packages to %packages_dir% ...
    if not exist %packages_dir% mkdir %packages_dir%
    move /Y %package_files% %packages_dir%
)
exit /b

:end

endlocal

pause
