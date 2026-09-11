@echo off
cd /d "%~dp0"

where npm >nul 2>nul
if errorlevel 1 (
  if exist "C:\Program Files\Microsoft Visual Studio\18\Insiders\MSBuild\Microsoft\VisualStudio\NodeJs\npm.cmd" (
    set "PATH=C:\Program Files\Microsoft Visual Studio\18\Insiders\MSBuild\Microsoft\VisualStudio\NodeJs;%PATH%"
  ) else (
    echo No se encontro npm. Instale Node.js o abra Angular desde una terminal con npm disponible.
    pause
    exit /b
  )
)

if not exist node_modules (
  call npm install
)

start "Angular productos" cmd /k "cd /d %cd% && npm start"
timeout /t 5 /nobreak > nul
start "" http://localhost:4200
