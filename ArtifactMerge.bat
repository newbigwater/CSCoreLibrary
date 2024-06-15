@echo OFF
setlocal enabledelayedexpansion

SET find="
SET replace=

SET LAST_DIR=%CD%
SET PROJECT_NAME=%1
SET FULL_PATH_TEMP=%2
call SET FULL_PATH=%%FULL_PATH_TEMP:!find!=!replace!%%

ECHO FULL_PATH : %FULL_PATH%


SET ILMERGE="%LAST_DIR%/%PROJECT_NAME%/3'rd/exe/ILMerge/3.0.41/ILMerge.exe"
	
echo DLL Merge Target Path : "%FULL_PATH%\???.*.dll"
set "DLL_LIST="
for %%f in ("%FULL_PATH%\???.*.dll") do (
    set "DLL_LIST=!DLL_LIST! %%~nxf"
	rem set "DLL_LIST=!DLL_LIST! %%f"
)
echo Target List:%DLL_LIST%

%ILMERGE% /out:"%FULL_PATH%\CSCore.dll" %DLL_LIST% /lib:"%FULL_PATH%"
if %ERRORLEVEL% equ 0 (
    echo Program executed successfully.
) else (
    echo Program failed with error level %ERRORLEVEL%.
	exit /b %ERRORLEVEL%
)

del "%FULL_PATH%\???.*.dll"
exit /b 0