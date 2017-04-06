/
function $deviceName

 Opens Control Panel "Sound" dialog and changes device to deviceName.

 deviceName - device name in the list in the "Sound" dialog. Can be with wildcard characters (*?).

 EXAMPLE
 int+ g_switchPlaybackDevice
 g_switchPlaybackDevice^1
 usePlaybackDevice iif(g_switchPlaybackDevice "Speakers" "*Digital*")


run "mmsys.cpl"
if(empty(deviceName)) ret
int w=wait(60 win("Sound" "#32770" "" 1))
Acc a.Find(w "LISTITEM" deviceName "class=SysListView32[]id=1000" 0x1005 5)
a.Select(3)
OnScreenDisplay a.Name
but 1002 w ;;Set default
but 1 w ;;OK

 err+ end _error
