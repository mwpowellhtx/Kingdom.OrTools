@echo off

setlocal

rem TODO: TBD: borrowing the approach adopted for my Kingdom.Collections suite.

:set_vars
set local_dir=G:\Dev\NuGet\local\packages
set nupkg_ext=.nupkg

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
rem Setup Constraint Solver
set constraint_projects=Kingdom.OrTools.Core
set constraint_projects=%constraint_projects%%delim%Kingdom.OrTools.ConstraintSolver.Core
rem Setup Linear Solver
set linear_projects=Kingdom.OrTools.Core
set linear_projects=%linear_projects%%delim%Kingdom.OrTools.LinearSolver.Core
rem Setup Solver Projects
set solver_projects=Kingdom.OrTools.Core
set solver_projects=%solver_projects%%delim%Kingdom.OrTools.ConstraintSolver.Core
set solver_projects=%solver_projects%%delim%Kingdom.OrTools.LinearSolver.Core

:parse_args

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
rem echo set_config = %1
if "%1" == "--config" (
    set config=%2
    shift
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

:add_all_projects
rem echo add_all_projects = %1
if "%1" == "--all" (
    set projects=%all_projects%
	goto :next_arg
)

:add_project
rem echo add_project = %1
if "%1" == "--project" (
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

:process_projects
rem Do the main areas here.
pushd ..\..

rem This is an interesting package for local consumption, but I do not think
rem it should be published in a broader context, at least not yet.
if not "%projects%" == "" (
    echo Processing '%config%' configuration for '%projects%' ...
)
:next_project
if not "%projects%" == "" (
    for /f "tokens=1* delims=%delim%" %%p in ("%projects%") do (
        rem Processing now as a function of input arguments.
        call :copy_local %%p
        set projects=%%q
    )
    goto :next_project
)

popd

goto :end

:copy_local
pushd %1\bin\%config%
rem Because Package may exist in the %config%\%TargetFramework% ...
set package_path=%1.*%nupkg_ext%
rem We need to scan beyond the wildcard package path here and get to specifics.
for /r %%x in ("%package_path%") do (
    if not exist "%%x" (
        goto :copy_not_found
    )
    if "%dry%" == "true" (
        echo Set to copy '%%x' to '%local_dir%' ...
    ) else (
        rem echo Wet run '%%x' ...
        if not exist "%local_dir%" mkdir "%local_dir%"
        rem Make sure that we do not inadvertently overwrite already existing package versions.
        if not exist "%local_dir%\%%x" echo Copying '%%x' package to local directory '%local_dir%'...
        if not exist "%local_dir%\%%x" xcopy /Y "%%x" "%local_dir%"
    )
    goto :end_copy
)
:copy_not_found
echo Package '%package_path%' not found.
:end_copy
popd
exit /b

:end

endlocal

pause
