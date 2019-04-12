@echo off

setlocal

rem We do not publish the API key as part of the script itself.
if "%my_nuget_api_key%"=="" (
    echo You are prohibited from making these sorts of changes.
    goto :end
)

rem Default list delimiter is Comma (,).
:redefine_delim
if "%delim%" == "" (
    set delim=,
)
rem Redefine the delimiter when a Dot (.) is discovered.
rem Anticipates potentially accepting Delimiter as a command line arg.
if "%delim%" == "." (
    set delim=
    goto :redefine_delim
)

rem Go ahead and pre-seed the Projects up front.
:set_projects
set projects=
rem Setup All Projects
set all_projects=Kingdom.OrTools.Core
set all_projects=%all_projects%%delim%Kingdom.OrTools.ConstraintSolver.Core
set all_projects=%all_projects%%delim%Kingdom.OrTools.LinearSolver.Core
set all_projects=%all_projects%%delim%Kingdom.OrTools.Sat.Core
rem Setup Constraint Solver
set constraint_projects=Kingdom.OrTools.Core
set constraint_projects=%constraint_projects%%delim%Kingdom.OrTools.ConstraintSolver.Core
rem Setup Linear Solver
set linear_projects=Kingdom.OrTools.Core
set linear_projects=%linear_projects%%delim%Kingdom.OrTools.LinearSolver.Core
rem Setup Sat Solver
set sat_projects=Kingdom.OrTools.Core
set sat_projects=%linear_projects%%delim%Kingdom.OrTools.Sat.Core
rem Setup Solver Projects
set solver_projects=Kingdom.OrTools.Core
set solver_projects=%solver_projects%%delim%Kingdom.OrTools.ConstraintSolver.Core
set solver_projects=%solver_projects%%delim%Kingdom.OrTools.LinearSolver.Core
set solver_projects=%solver_projects%%delim%Kingdom.OrTools.Sat.Core


:parse_args

rem Done parsing the args.
if "%1" == "" (
    goto :end_args
)

:set_dry_run
if "%1" == "--dry" (
    set dry=true
    goto :next_arg
)
if "%1" == "--dry-run" (
    set dry=true
    goto :next_arg
)

:set_config
if "%1" == "--config" (
    set config=%2
    shift
    goto :next_arg
)

:set_publish_local
if "%1" == "--local" (
    set function=local
    goto :next_arg
)

:set_publish_nuget
if "%1" == "--nuget" (
    set function=nuget
    goto :next_arg
)

:add_constraint_projects
rem echo add_constraint_projects = %1
if "%1" == "--constraint-solver" (
    if "%projects%" == "" (
        set projects=%constraint_projects%
    ) else (
        set projects=%projects%%delim%%constraint_projects%
    )
	goto :next_arg
)

if "%1" == "--constraint" (
    if "%projects%" == "" (
        set projects=%constraint_projects%
    ) else (
        set projects=%projects%%delim%%constraint_projects%
    )
	goto :next_arg
)

:add_linear_projects
rem echo add_linear_projects = %1
if "%1" == "--linear-solver" (
    if "%projects%" == "" (
        set projects=%linear_projects%
    ) else (
        set projects=%projects%%delim%%linear_projects%
    )
	goto :next_arg
)

if "%1" == "--linear" (
    if "%projects%" == "" (
        set projects=%linear_projects%
    ) else (
        set projects=%projects%%delim%%linear_projects%
    )
	goto :next_arg
)

:add_solver_projects
rem echo add_solver_projects = %1
if "%1" == "--solvers" (
    if "%projects%" == "" (
        set projects=%solver_projects%
    ) else (
        set projects=%projects%%delim%%solver_projects%
    )
	goto :next_arg
)

:add_sat_projects
if "%1" == "--sat" (
    rem Prepare to publish All Projects.
    set projects=%sat_projects%
    goto :next_arg
)

:add_all_projects
if "%1" == "--all" (
    rem Prepare to publish All Projects.
    set projects=%all_projects%
    goto :next_arg
)

:add_project
if "%1" == "--project" (
    rem Add a Project to the Projects list.
    if "%projects%" == "" (
        set projects=%2
    ) else (
        set projects=%projects%%delim%%2
    )
    shift
    goto :next_arg
)

:next_arg

shift

goto :parse_args

:end_args

:verify_args

:verify_function
if "%function%" == "" (
    set function=local
)

:verify_projects
if "%projects%" == "" (
    rem In which case, there is nothing else to do.
    echo Nothing to process.
    goto :end
)

:verify_config
if "%config%" == "" (
    rem Assumes Release Configuration when not otherwise specified.
    set config=Release
)

:publish_projects

:set_vars
set xcopy_exe=xcopy.exe
set xcopy_opts=/y /f
set xcopy_dest_dir=G:\Dev\NuGet\local\packages
rem Expecting NuGet to be found in the System Path.
set nuget_exe=NuGet.exe
set nuget_push_verbosity=detailed
set nuget_push_source=https://api.nuget.org/v3/index.json

set nuget_push_opts=%nuget_push_opts% %my_nuget_api_key%
set nuget_push_opts=%nuget_push_opts% -Verbosity %nuget_push_verbosity%
set nuget_push_opts=%nuget_push_opts% -NonInteractive
set nuget_push_opts=%nuget_push_opts% -Source %nuget_push_source%

rem Do the main areas here.
pushd ..\..

if not "%projects%" == "" (
    echo Processing '%config%' configuration for '%projects%' ...
)
:next_project
if not "%projects%" == "" (
    for /f "tokens=1* delims=%delim%" %%p in ("%projects%") do (
        if "%function%" == "nuget" call :publish_nuget %%p
        if "%function%" == "local" call :publish_local %%p
        set projects=%%q
        goto :next_project
    )
)

popd

goto :end

:publish_local
for %%f in ("%1\bin\%config%\%1.*.nupkg") do (
    if "%dry%" == "true" (
        echo Dry run: %xcopy_exe% %%f %xcopy_dest_dir% %xcopy_opts%
    ) else (
        echo Running: %xcopy_exe% %%f %xcopy_dest_dir% %xcopy_opts%
        %xcopy_exe% %%f %xcopy_dest_dir% %xcopy_opts%
    )
)
exit /b

:publish_nuget
for %%f in ("%1\bin\%config%\%1.*.nupkg") do (
    if "%dry%" == "true" (
        echo Dry run: %nuget_exe% push "%%f"%nuget_push_opts%
    ) else (
        echo Running: %nuget_exe% push "%%f"%nuget_push_opts%
        %nuget_exe% push "%%f"%nuget_push_opts%
    )
)
exit /b

:end

endlocal

pause
