@echo Copying test modules over to the project

mkdir "bin\Debug\net8.0-windows\Modules"

copy "..\TestApps\ChatServer\bin\Debug\net8.0\ChatServer.dll" "bin\Debug\net8.0-windows\Modules\ChatServer.dll"
copy "..\TestApps\ChatWithHistoryServer\bin\Debug\net8.0\ChatWithHistoryServer.dll" "bin\Debug\net8.0-windows\Modules\ChatWithHistoryServer.dll"
copy "..\TestApps\MouseInputServer\bin\Debug\net8.0\MouseInputServer.dll" "bin\Debug\net8.0-windows\Modules\MouseInputServer.dll"
