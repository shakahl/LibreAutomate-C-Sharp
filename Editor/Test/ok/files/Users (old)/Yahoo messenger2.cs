 run "C:\Program Files\Yahoo!\Messenger\YahooMessenger.exe" "" "" "C:\Program Files\Yahoo!\Messenger"
int w1=act(win("Yahoo! Messenger" "YahooBuddyMain"))
Acc a=acc("" "LISTITEM" w1 "id=101 SysListView32" "" 0x1001)
 out a.Name
Acc alast
a.Navigate("parent last" alast)
a.Select(2)
alast.Select(4)
