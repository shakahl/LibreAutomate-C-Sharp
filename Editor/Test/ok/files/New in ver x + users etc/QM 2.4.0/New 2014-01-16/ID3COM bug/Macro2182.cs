out
typelib Id3Lib "id3com.dll"

Dir d
foreach(d "$desktop$\MP3\*.mp3" FE_Dir 0x4)
	str sPath=d.FileName(1)
	str s=sPath
	out "--- %s ---" sPath
	Id3Lib.ID3ComTag t._create
	BSTR b=sPath
	t.Link(&b)
	 out t.NumFrames
	Id3Lib.ID3ComFrame f=0
	int i j
	for i 0 t.NumFrames
		out i
		f=t.FrameByNum(i)
		out f.FrameName
		Id3Lib.ID3ComField fi=f.Field(Id3Lib.ID3_FIELD_TEXT)
		 Id3Lib.ID3ComField fi=f.Field(Id3Lib.ID3_FIELD_DESCRIPTION)
		if(fi)
			for j 1 fi.NumTextItems+1
				s=fi.Text(j)
				BSTR bs=s
				fi.Text(j)=_s.RandomString(1 100 "a-z")
				s-"[9]"
				out s
	 t.SaveV1Tag
	t.SaveV2Tag
