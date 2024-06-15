@echo OFF

rem VS2015
rem SET MSBUILD="C:\Program Files (x86)\MSBuild\14.0\Bin\MSBuild.exe"
rem VS2019
SET MSBUILD="C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\MSBuild\Current\Bin\MSBuild.exe"
SET VSTESTER="C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe"
rem SET MSBUILD="C:\Program Files (x86)\Microsoft Visual Studio\2019\Professional\MSBuild\Current\Bin\MSBuild.exe"
rem SET VSTESTER="C:\Program Files (x86)\Microsoft Visual Studio\2019\Professional\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe"

SET LAST_DIR=%CD%
SET PROJECT_NAME=CSCoreLibrary
SET SOLUTION_NAME=_buildAll.sln
SET BUILD_PATH="%LAST_DIR%\\%PROJECT_NAME%\\src\\%SOLUTION_NAME%"
SET OUTPUT_PATH="%LAST_DIR%\\%PROJECT_NAME%\\build"
SET ARTIFACT_PATH=%LAST_DIR%\artifact\%PROJECT_NAME%\%date:~0,4%%date:~5,2%%date:~8,2%\
SET LATEST_ARTIFACT_PATH=%LAST_DIR%\artifact\%PROJECT_NAME%\latest_version\
SET UNIT_TEST_FILE=999.UnitTest.dll

echo *****************************
echo ** Compile and Link
echo *****************************
SET CONFIG=Release

SET PLATFORM="Any CPU"
SET PLATFORM_DIR=AnyCPU

echo *****************************
echo ** Compile and Link - %PLATFORM_DIR%%CONFIG%
echo *****************************
%MSBUILD% %BUILD_PATH% /m /t:Rebuild /p:Configuration=%CONFIG%;Platform=%PLATFORM%
IF %ERRORLEVEL% NEQ 0 GOTO Failurel
%VSTESTER% %OUTPUT_PATH%\\%PLATFORM_DIR%%CONFIG%\\%UNIT_TEST_FILE% /Logger:trx;LogFileName=%PLATFORM_DIR%%CONFIG%.trx
IF %ERRORLEVEL% NEQ 0 GOTO Failure

call "%LAST_DIR%/%PROJECT_NAME%/ArtifactMerge.bat" %PROJECT_NAME% "%LAST_DIR%\%PROJECT_NAME%\artifact\%PLATFORM_DIR%%CONFIG%"
rem %ILMERGE% /out:"%LAST_DIR%\%PROJECT_NAME%\artifact\%PLATFORM_DIR%%CONFIG%\CSCore.dll" /wildcards "%LAST_DIR%\%PROJECT_NAME%\artifact\%PLATFORM_DIR%%CONFIG%\0*.dll"
rem del "%LAST_DIR%\%PROJECT_NAME%\artifact\%PLATFORM_DIR%%CONFIG%\0*.dll"
IF %ERRORLEVEL% NEQ 0 GOTO Failure

SET PLATFORM="x64"
SET PLATFORM_DIR=x64

echo *****************************
echo ** Compile and Link - %PLATFORM_DIR%%CONFIG%
echo *****************************
%MSBUILD% %BUILD_PATH% /m /t:Rebuild /p:Configuration=%CONFIG%;Platform=%PLATFORM%
IF %ERRORLEVEL% NEQ 0 GOTO Failure
%VSTESTER% %OUTPUT_PATH%\\%PLATFORM%%CONFIG%\\%UNIT_TEST_FILE% /Logger:trx;LogFileName=%PLATFORM%%CONFIG%.trx
IF %ERRORLEVEL% NEQ 0 GOTO Failure

call "%LAST_DIR%/%PROJECT_NAME%/ArtifactMerge.bat" %PROJECT_NAME% "%LAST_DIR%\%PROJECT_NAME%\artifact\%PLATFORM_DIR%%CONFIG%"
rem %ILMERGE% /out:"%LAST_DIR%\%PROJECT_NAME%\artifact\%PLATFORM_DIR%%CONFIG%\CSCore.dll" /wildcards "%LAST_DIR%\%PROJECT_NAME%\artifact\%PLATFORM_DIR%%CONFIG%\*.dll"
rem del "%LAST_DIR%\%PROJECT_NAME%\artifact\%PLATFORM_DIR%%CONFIG%\0*.dll"
IF %ERRORLEVEL% NEQ 0 GOTO Failure

cd %LAST_DIR%

:Succeed
echo *****************************
echo ** Successfully finished.
echo *****************************
cd %LAST_DIR%\%PROJECT_NAME%

if exist "artifact\\history.%PROJECT_NAME%.txt" (
del "artifact\\history.%PROJECT_NAME%.txt"
)
git log >> "artifact\\history.%PROJECT_NAME%.txt"

echo "%LAST_DIR%\%PROJECT_NAME%\artifact\" "%ARTIFACT_PATH%" /e /y
xcopy "%LAST_DIR%\%PROJECT_NAME%\artifact\" "%ARTIFACT_PATH%" /e /y

echo "%LAST_DIR%\%PROJECT_NAME%\artifact\" "%LATEST_ARTIFACT_PATH%" /e /y
xcopy "%LAST_DIR%\%PROJECT_NAME%\artifact\" "%LATEST_ARTIFACT_PATH%" /e /y

cd %LAST_DIR%
exit /b 0

:Failure
echo *****************************
echo ** Varifying failed.
echo *****************************
pause

cd %LAST_DIR%
exit /b 3
