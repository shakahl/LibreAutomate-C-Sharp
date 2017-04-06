 typelib Id3Lib "id3com.dll"

Dir d
foreach(d "$desktop$\MP3\*.mp3" FE_Dir 0x4)
	str sPath=d.FileName(1)
	str s=sPath
	out "--- %s ---" sPath
	 IDispatch t._create("{AEBA98BD-C36C-11D3-841B-0008C782A257}")
	IDispatch t._create("Id3COM.ID3ComTag")
	BSTR b=sPath
	t.Link(&b)
	 out t.NumFrames
	 IDispatch f=0
	 int i j
	 for i 0 t.NumFrames
		 out i
		 f=t.FrameByNum(i)
		 out f.FrameName
		 IDispatch fi=f.Field(2)
		 if(fi)
			 for j 1 fi.NumTextItems+1
				 s=fi.Text(j)
				 BSTR bs=s
				  fi.Text(j)=_s.RandomString(1 100 "a-z")
				 s-"[9]"
				 out s
	t.SaveV1Tag
	t.SaveV2Tag
