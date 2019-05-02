@echo off

setlocal

rem For purposes of our build pipeline, we will assume that the packaging is complete.
rem In the short term we will assume that the repository has already been cloned and
rem has been updated as necessary.

rem set xcopy_exe=C:\Windows\system32\xcopy.exe
set xcopy_exe=xcopy.exe
set xcopy_opts=/s /y /f
set xcopy_src_path=G:\Source\Spikes\Google\or-tools-google\ortools\*.proto
set xcopy_dest_path=tools\Google\or-tools

rem Assumes that we are running the batch file from the context of the project file.
pushd ..

rem Eventually, I want to incorporate a development-only dependency on Google.OrTools with
rem pre-packaged Protocol Buffer files. Until then, we depend on a local machine clone of
rem the Google OrTools repository.
rem https://github.com/google/or-tools
rem See issue for call for Protocol Buffer files being packaged:
rem "Package the .proto files with the NuGet package"
rem https://github.com/google/or-tools/issues/1190
if not exist "%xcopy_dest_path%" mkdir "%xcopy_dest_path%"
echo Running: %xcopy_exe% "%xcopy_src_path%" "%xcopy_dest_path%" %xcopy_opts%
%xcopy_exe% "%xcopy_src_path%" "%xcopy_dest_path%" %xcopy_opts%

popd

endlocal
