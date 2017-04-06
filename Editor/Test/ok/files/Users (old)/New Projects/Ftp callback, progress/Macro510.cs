 out
 Ftp f.Connect("ftp.quickmacros.com" "quickmac" "*")
 f.DirSet("public_html/alpha")
 Dir d
 foreach(d "$Desktop$\*" FE_Dir)
	 str sPath=d.FileName(1)
	 if(d.FileSize>1000000) continue
	 out sPath
	  Deb
	 if(!f.FilePutWithProgress(sPath "" 2)) mes- "failed"
	  if(!f.FilePut(sPath _s.getfilename(sPath 1))) mes- "failed"

Ftp f.Connect("ftp.mysite.com" "user" "password")
f.DirSet("public_html/testqmftp")
Dir d
foreach(d "$Desktop$\*" FE_Dir)
	str sPath=d.FileName(1)
	if(d.FileSize>=1024*1024) continue
	 out sPath
	if(!f.FilePutWithProgress(sPath "" 2)) mes- "failed"
