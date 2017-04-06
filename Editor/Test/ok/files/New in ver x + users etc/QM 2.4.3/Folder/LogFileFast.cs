 /
function $text

 Writes text to file "$my qm$\LogFileFast.txt". Appends newline.
 This function is fast, faster than out. Writes to file asynchronously, every 1 s and when QM exits.
 Deletes old file when called first time in current QM session.
 To change file, edit this function. Or execute this when QM starts, before calling this function first time: str+ g_LFF_File="C:\...\your file\txt"


lock _LogFileFast
str+ g_LFF_File __LFF_Buffer
int+ __LFF_Thread
__Handle+ __LFF_Event

if !__LFF_Thread
	if(!g_LFF_File.len) g_LFF_File="$my qm$\LogFileFast.txt"
	if(FileExists(g_LFF_File)) del- g_LFF_File; err out "Error in LogFileFast: failed to delete old file."
	__LFF_Buffer.flags=3
	__LFF_Event=CreateEvent(0 0 0 0)
	__LFF_Thread=1
	mac "sub.Thread"

__LFF_Buffer.addline(text)

lock- _LogFileFast
if(__LFF_Buffer.len>4000000) SetEvent __LFF_Event; 0.001


#sub Thread
atend sub.Atend
rep
	wait 1 H __LFF_Event; err
	if(__LFF_Buffer.len) sub.WriteToFile


#sub WriteToFile
lock _LogFileFast
__LFF_Buffer.swap(_s)
lock- _LogFileFast
_s.setfile(g_LFF_File -1); err out "Error in LogFileFast: failed to write to file."


#sub Atend
if(__LFF_Buffer.len) sub.WriteToFile
