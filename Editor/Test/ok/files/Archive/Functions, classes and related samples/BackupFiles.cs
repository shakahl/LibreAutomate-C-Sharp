 /
function $source $destFolder [ifExists] [flags] ;;ifExists - 0 replace, 1 dialog, 2 rename, 3 error, 4 do nothing, 5 replace if source is newer;  flags: 1 no intermediate folder

 Copies files to a folder.
 Error if fails to copy a file.

 source - file, folder or list of files and/or folders. Also can be drive(s). Error if does not exist.
 destFolder - destination folder. The function creates it if does not exist.


ARRAY(str) aS aD
GetFileCopyPaths source destFolder aS aD flags

str sS(aS) sD(aD)
 out "---- src ----[]%s[]---- dest ----[]%s" sS sD

sel ifExists
	case 0
	cop- sS sD
	
	case 1
	cop sS sD
	
	case 2
	cop+ sS sD
	
	case [3,4,5]
	int i
	for i aD.len-1 -1 -1
		if(!dir(aD[i])) continue
		sel ifExists
			case 3
			end "Destination file '%s' already exists" 0 aD[i]
			
			case 5
			long d1 d2
			memcpy &d2 &_dir.fd.ftLastWriteTime 8
			dir aS[i]
			memcpy &d1 &_dir.fd.ftLastWriteTime 8
			if(d1>d2) continue
		
		aD.remove(i)
		aS.remove(i)
		
	sS=aS; sD=aD
	 out "---- src ----[]%s[]---- dest ----[]%s" sS sD
	if(aD.len) cop- sS sD
	
	case else end ES_BADARG

err+ end _error
