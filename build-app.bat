rd /s /q out
cd src\PublicApi
dotnet build -c Release -o ..\..\out
dotnet publish --no-restore -c Release -o ..\..\out
cd ..\..\out
del Web.config
del nlog.config
del appsettings.*
rd /s /q ca
rd /s /q de
rd /s /q es
rd /s /q fa
rd /s /q fr
rd /s /q nl
rd /s /q pt
rd /s /q pt-BR
rd /s /q pt-PT
rd /s /q zh
rd /s /q zh-TW
rd /s /q runtimes
