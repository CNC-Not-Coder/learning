@echo off
set CUR_DIR=%~dp0
set PROTOC=%CUR_DIR%protoc.exe
set SRC_DIR=%CUR_DIR%protofile
set DST_DIR=%CUR_DIR%generate
set ProtoFile=%SRC_DIR%\MessageDefine.proto

echo %PROTOC%
%PROTOC% -I=%SRC_DIR% --csharp_out=%DST_DIR% %ProtoFile%

pause