 run ":: 14001F702020EC21EA3A6910A2DD08002B30309D" ;;Control Panel
 run ":: 14001F702020EC21EA3A6910A2DD08002B30309D 9100000038FFFFFF1D002600433A5C57696E646F77735C73797374656D33325C6D61696E2E63706C004B6579626F61726400437573746F6D697A6520796F7572206B6579626F6172642073657474696E67732C20737563682061732074686520637572736F7220626C696E6B207261746520616E6420746865206368617261637465722072657065617420726174652E00" ;;Keyboard
 run "C:\Windows\System32\taskschd.msc" "/s"

 cop "C:\Users\G\Desktop\a.txt" "$Desktop$\a 2.txt" 0x2C0|FOF_SILENT

 del "C:\Users\G\Desktop\a 2.txt"

 mkdir "na" "$common Desktop$"

 Dir d
 foreach(d "$Common Desktop$\*" FE_Dir)
	 str sPath=d.FileName(1)
	 out sPath

 web tested thoroughly in the past. Email functions too.

 file.File f.Open("$desktop$\a.txt" "w")
 f.WriteLine("line")

 rget/rset tested
 zip tested

 file.CreateShortcut "$desktop$\sh.lnk" ":: 14001F702020EC21EA3A6910A2DD08002B30309D"

 ARRAY(str) a; int i
 file.GetClipboardFiles a
 for i 0 a.len
	 out a[i]

 RunAs, RunConsole and others tested

 Database - test later

 out file.GetFileOrFolderSize("$common desktop$")
 out file.GetFileOrFolderSize("c:\")/1024/1024

 Shell32 and Wsh tested
