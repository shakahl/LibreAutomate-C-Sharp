Dir d
 foreach(d "$desktop$\MP3\*.mp3" FE_Dir 0x4)
foreach(d "C:\Users\G\Desktop\MP3\01. cantastorie.mp3" FE_Dir 0x4)
	str sPath=d.FileName(1)
	str s=sPath
	out "--- %s ---" sPath
	str vbs=
	F
	 dim t
	 set t=CreateObject("Id3COM.ID3ComTag")
	 t.Link("{sPath}")
	 t.SaveV1Tag
	 t.SaveV2Tag
	VbsExec vbs
