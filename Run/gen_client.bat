set WORKSPACE=..
set GEN_CLIENT=%WORKSPACE%\Tools\Luban\Luban.dll
set CONF_ROOT=%WORKSPACE%\GameConfigs

dotnet %GEN_CLIENT% ^
    -t client ^
    -c cs-simple-json ^
    -d json ^
    --conf %CONF_ROOT%\luban.conf ^
    -x outputCodeDir=%WORKSPACE%\Assets\LubanGenerated\Generate ^
    -x outputDataDir=%WORKSPACE%\Assets\LubanGenerated\GenData
