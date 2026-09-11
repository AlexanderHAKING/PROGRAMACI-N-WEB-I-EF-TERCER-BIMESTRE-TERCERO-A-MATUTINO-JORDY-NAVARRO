@echo off
set "ROOT=%~dp0"

where dotnet >nul 2>nul
if errorlevel 1 (
  echo No se encontro dotnet. Instale .NET SDK o abra el proyecto desde Visual Studio.
  pause
  exit /b
)

where npm >nul 2>nul
if errorlevel 1 (
  if exist "C:\Program Files\Microsoft Visual Studio\18\Insiders\MSBuild\Microsoft\VisualStudio\NodeJs\npm.cmd" (
    set "PATH=C:\Program Files\Microsoft Visual Studio\18\Insiders\MSBuild\Microsoft\VisualStudio\NodeJs;%PATH%"
  ) else (
    echo No se encontro npm. Instale Node.js o ejecute Angular desde Visual Studio si lo tiene configurado.
    pause
    exit /b
  )
)

start "ProductosSOAP" cmd /k "cd /d ""%ROOT%ProductosSOAP"" && dotnet run --launch-profile http"
start "MovimientosREST" cmd /k "cd /d ""%ROOT%MovimientosREST"" && dotnet run --launch-profile http"

timeout /t 8 /nobreak > nul

cd /d "%ROOT%productos-angular"
if not exist node_modules (
  call npm install
)

start "Productos Angular" cmd /k "cd /d ""%ROOT%productos-angular"" && npm start"

timeout /t 8 /nobreak > nul
start "" http://localhost:4200
