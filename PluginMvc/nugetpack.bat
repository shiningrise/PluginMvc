echo off

path %SYSTEMROOT%\Microsoft.NET\Framework\v4.0.30319\

msbuild.exe PluginMvc.csproj /t:Rebuild /p:Configuration=Release /p:VisualStudioVersion=12.0

NuGet pack PluginMvc.csproj -Prop Configuration=Release
pause