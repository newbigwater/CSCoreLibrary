@echo OFF
setlocal enabledelayedexpansion


SET PLATFORM=%1
SET CONFIG=%2
SET TARGET_DIR=%3
SET TARGET_NAME=%4
SET TARGET_EXT=%5
SET TARGET_FULL_PATH=%6

SET TARGET_PATH=%TARGET_DIR%%TARGET_NAME%%TARGET_EXT%
SET TARGET_SUB_PATH=%TARGET_DIR%%TARGET_NAME%.xml
SET ARTIFACT_DIR=%TARGET_DIR%..\..\artifact\%PLATFORM%%CONFIG%
SET ARTIFACT_INFO=%ARTIFACT_DIR%\%TARGET_NAME%.buildInfo.txt

echo TARGET_PATH_TEMP 00 : %TARGET_FULL_PATH

SET find=\
SET replace=\\
call SET TARGET_PATH_TEMP=%%TARGET_FULL_PATH:!find!=!replace!%%
echo TARGET_PATH_TEMP 01 : %TARGET_PATH_TEMP%

SET find="
SET replace='
call SET TARGET_PATH_TEMP=%%TARGET_PATH_TEMP:!find!=!replace!%%
echo TARGET_PATH_TEMP 02 : %TARGET_PATH_TEMP%

WMIC datafile where name=%TARGET_PATH_TEMP% GET version | find "." > %temp%\cmdtemp_%TARGET_NAME%
set /p VERSION=<%temp%\cmdtemp_%TARGET_NAME% & del /q %temp%\cmdtemp_%TARGET_NAME%

echo Platform       : %PLATFORM%
echo Configuration  : %CONFIG%
echo Target Dir     : %TARGET_DIR%
echo Target Name    : %TARGET_NAME%
echo Target Ext     : %TARGET_EXT%
echo Target Version : %VERSION%
echo Target Path    : %TARGET_PATH%
echo Artifact Dir   : %ARTIFACT_DIR%
echo Artifact Info  : %ARTIFACT_INFO%

if not exist "%ARTIFACT_DIR%" (
  md "%ARTIFACT_DIR%"
)

if exist "%ARTIFACT_INFO%" (
  del /F /Q /A "%ARTIFACT_INFO%"
)

xcopy %TARGET_SUB_PATH% "%ARTIFACT_DIR%" /Y
xcopy %TARGET_PATH% "%ARTIFACT_DIR%" /Y
echo Date Time              : %date% %time% >> "%ARTIFACT_INFO%"
echo Artifact Name          : %TARGET_NAME%%TARGET_EXT% >> "%ARTIFACT_INFO%"
echo Artifact Version       : %VERSION% >> "%ARTIFACT_INFO%"
echo Artifact Platform      : %PLATFORM% >> "%ARTIFACT_INFO%"
echo Artifact Configuration : %CONFIG% >> "%ARTIFACT_INFO%"
