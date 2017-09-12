echo off
set OpenRAPath=%cd%\..\OpenRA
set unityProjPath=%cd%\..\Unity
::echo %unityProjPath%
del /s /q %unityProjPath%\Assets\Plugins\OpenRA.Game.dll
del /s /q %unityProjPath%\Assets\Plugins\OAUnityLayer.dll


xcopy %OpenRAPath%\OAUnityLayer\bin\Debug\OpenRA.Game.dll %unityProjPath%\Assets\Plugins\OpenRA.Game.dll /s /i /k
xcopy %OpenRAPath%\OAUnityLayer\bin\Debug\OAUnityLayer.dll %unityProjPath%\Assets\Plugins\OAUnityLayer.dll /s /i /k

::/exclude:%OpenRAPath%\OpenRA.Game\OpenRA.Game.csproj.user
::%OpenRAPath%\"OpenRA.Game"\obj\ 
::%OpenRAPath%\OpenRA.Game\*.csproj %OpenRAPath%\OpenRA.Game\*.user
echo  copy dll completed!

pause