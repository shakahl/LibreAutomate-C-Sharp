 /exe

 let this run when exe starts. This code adds 2 wav files to exe resources, and in exe loads from resources.
#exe addfile "$windows$\Media\Garden\Windows Feed Discovered.wav" 10
#exe addfile "$windows$\Media\Garden\Windows Logoff Sound.wav" 11
lpstr+ g_wav10=+ExeGetResourceData(0 10 0)
lpstr+ g_wav11=+ExeGetResourceData(0 11 0)

 use this code to play
PlaySound g_wav10 0 SND_MEMORY ;;play synchronously
PlaySound g_wav10 0 SND_MEMORY|SND_ASYNC ;;play asynchronously
1 ;;if async, exe must not exit soon, or you will not hear the sound
PlaySound g_wav11 0 SND_MEMORY ;; play other file

 BEGIN PROJECT
;
 END PROJECT


 these don't work

 bee ":10 $windows$\Media\Garden\Windows Feed Discovered.wav" ;;bee does not support it

 PlaySound(+10 _hinst SND_RESOURCE|SND_ASYNC) ;;need resource type "WAVE"; QM cannot add
