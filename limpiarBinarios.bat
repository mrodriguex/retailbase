@setlocal enableextensions enabledelayedexpansion

set bcd="CRY.WEB"
for /F %%i in ('dir "Bin" /D /s /B ^|findstr /L /V ".WEB"') do (
echo s | rmdir /s %%i
)

for /F %%i in ('dir "Obj" /D /s /B') do (
echo s | rmdir /s %%i
)

endlocal

