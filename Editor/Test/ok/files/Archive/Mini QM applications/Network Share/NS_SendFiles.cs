 \
function $computer $files

 Sends (copies) files to computer.

 computer - computer name on the local network, or IP.
 files - full path of a file or folder. Can be list (multiple lines) of files or/and folders.


str sp
if(!NS_GetPassword(computer sp))
	int iid=getopt(itemid 1)
	str sn se
	if(iid) se.format("Run NS_Setup function and add the computer, or edit %s function and change the computer name." sn.getmacro(iid 1))
	else se="If used SendTo menu, delete invalid shortcut from SendTo folder. To add computers, run NS_Setup function."
	mes- "Unknown computer. %s" "" "" se

#compile __CNetworkShare
CNetworkShare ns.Init(computer sp)
ns.SendFiles(files)
