rmdir /q /s LiveSplit\bin\Release
"C:\Program Files (x86)\MSBuild\14.0\Bin\MSBuild.exe" LiveSplit\LiveSplit.sln /p:Configuration=Release /p:DebugSymbols=false /p:DebugType=None
cd LiveSplit\bin\Release
del /s *.xml
del /s *.pdb
cd ..\..\..\