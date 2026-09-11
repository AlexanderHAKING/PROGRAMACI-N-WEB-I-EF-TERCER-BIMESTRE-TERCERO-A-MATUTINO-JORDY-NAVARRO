@echo off
set "COLECCION=%~dp0POSTMAN_MovimientosREST.postman_collection.json"

if not exist "%COLECCION%" (
  echo No se encontro la coleccion de Postman.
  pause
  exit /b
)

start "" "%COLECCION%"
echo Si Postman no se abre automaticamente, importe este archivo manualmente:
echo %COLECCION%
pause
