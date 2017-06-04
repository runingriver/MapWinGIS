rem @setlocal enableextensions
rem @cd /d "%~dp0"
regsvr32 /u MapWinGIS.ocx
rem regsvr32 MapWinGIS.ocx
rem setenv -a PROJ_LIB "%cd%\PROJ_NAD"
rem setenv -a GDAL_DATA "%cd%\gdal_data"
pause