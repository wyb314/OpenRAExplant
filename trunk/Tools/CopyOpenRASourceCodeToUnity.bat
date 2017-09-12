echo off
set OpenRAPath=%cd%\..\OpenRA
set unityProjPath=%cd%\..\Unity
::echo %unityProjPath%
rd /s /q %unityProjPath%\Assets\Scripts\OpenRA\OpenRA.Game\
rd /s /q %unityProjPath%\Assets\Scripts\OAUnityLayer\

xcopy %OpenRAPath%\OpenRA.Game %unityProjPath%\Assets\Scripts\OpenRA\OpenRA.Game /s /i /k /exclude:uncopy.txt
xcopy %OpenRAPath%\OAUnityLayer %unityProjPath%\Assets\Scripts\OAUnityLayer /s /i /k /exclude:uncopy.txt
::/exclude:%OpenRAPath%\OpenRA.Game\OpenRA.Game.csproj.user
::%OpenRAPath%\"OpenRA.Game"\obj\ 
::%OpenRAPath%\OpenRA.Game\*.csproj %OpenRAPath%\OpenRA.Game\*.user
echo  copy completed!

pause