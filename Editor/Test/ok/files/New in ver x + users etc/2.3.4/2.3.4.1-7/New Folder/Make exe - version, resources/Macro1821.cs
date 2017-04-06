 /exe

#exe addfile "C:\Windows\Media\Garden\Windows Information Bar.wav" 5 "WAVE"
#exe addfile "C:\Windows\Media\Garden\Windows Critical Stop.wav" 15 "WAVE"

 #exe addfile "C:\Windows\Media\Garden\Windows Critical Stop.wav" 15 50
 #exe addfile "C:\Windows\Media\Garden\Windows Information Bar.wav" 15 500
 #exe addfile "C:\Windows\Media\Garden\Windows Critical Stop.wav" 15
 #exe addfile "C:\Windows\Media\Garden\Windows Information Bar.wav" 5

PlaySound +5 _hinst SND_RESOURCE
PlaySound +15 _hinst SND_RESOURCE

_s=":10 mouse.ico"

 BEGIN PROJECT
 main_function  Macro1821
 exe_file  $my qm$\Macro1821.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 flags  23
 guid  {5E725C8F-ADE9-448C-8917-8CC8461E2A8F}
 END PROJECT
