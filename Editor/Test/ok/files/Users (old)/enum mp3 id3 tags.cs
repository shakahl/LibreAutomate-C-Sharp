out
typelib Id3Lib "id3com.dll"

Dir d; int n
foreach(d "Q:\MP3\*.mp3" FE_Dir 0x4)
	n+1; if(n>10) break
	str sPath=d.FileName(1)
	out "--- %s ---" sPath
	Id3Lib.ID3ComTag t._create
	BSTR b=sPath
	t.Link(b)
	 out t.NumFrames
	Id3Lib.ID3ComFrame f
	 foreach f t ;;does not work. why?
		 out f.FrameName
	int i j
	for i 0 t.NumFrames
		f=t.FrameByNum(i)
		out f.FrameName
		Id3Lib.ID3ComField fi=f.Field(Id3Lib.ID3_FIELD_TEXT)
		 Id3Lib.ID3ComField fi=f.Field(Id3Lib.ID3_FIELD_DESCRIPTION)
		if(fi)
			str s
			for j 1 fi.NumTextItems+1
				s=fi.Text(j)
				s-"[9]"
				out s
