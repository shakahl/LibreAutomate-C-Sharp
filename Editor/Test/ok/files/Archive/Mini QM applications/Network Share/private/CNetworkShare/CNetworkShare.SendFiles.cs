function $files

 Sends (copies) files to computer.

 files - full path of a file or folder. Can be list (multiple lines) of files or/and folders.


 show progress dialog
str wname.format("Sending File(s) to %s" m_computer)
int h=win(wname)
if(h) mes "Already sending. Close the 'Sending File...' dialog and retry." "" "!"; ret
mac "NS_ProgressDialog" wname
m_hprogress=wait(5 WV wname); err ret

if(!m_ipdone)
	 use IP instead of name because resolving name is the slowest part
	m_ipdone=1
	if(inet_addr(m_computer)=-1)
		Status("Getting IP address.")
		if(!GetIpAddress(m_computer m_ip)) m_ip=m_computer

 Q &q

 begin
Status("Connecting.")
if(!SendControl(255))
	clo m_hprogress; err
	ret

 add all files/folders to single zip file. If connection is fast, it may slow down, especially if the file is already compressed (like program setup files, mp3 files, etc), but it is an easy way to pack multiple files/folders.
 Q &qq
Status("Compressing.")
str zf="$temp$\qm_ns_send.zip"
zip zf files 1
 Q &qqq

 send the zip file in parts
Status("Sending.")
long sizesent sizeall=GetFileOrFolderSize(zf)
int pc sizepart=1*1024*1024 ;;1 MB. Make smaller if connection is slow.
double timepart=200.0+(sizeall/(100*1024)) ;;0.2 s + depends on file size
rep
	int tc=GetTickCount
	
	str f.getfile(zf sizesent sizepart) ;;read next sizepart bytes
	sizesent+f.len
	pc=sizesent*100/sizeall
	if(!SendData("NS_ReceiveFiles" f pc)) break
	
	 auto correct sizepart depending on connection speed
	tc=GetTickCount-tc+1 ;;out "%i %i" tc sizepart
	sizepart=sizepart*(timepart/tc)
	if(sizepart>2*1024*1024) sizepart=2*1024*1024 ;;max 2 MB
	else if(sizepart<10*1024) sizepart=10*1024 ;;min 10 KB
	
	 canceled?
	if(!IsWindow(m_hprogress))
		SendControl(253 1)
		break
	
	 update progress dialog
	SendMessage m_hprogress WM_APP pc 0
	
	 finished?
	if(sizesent>=sizeall)
		Status("Decompressing.")
		if(SendControl(254)) Status("Done.")
		1; break
	
	Status(_s.format("Sending.   Size = %.2f MB.   Speed = %.2f MB/s." sizeall/1024.0/1024 1000.0*f.len/tc/1024/1024))

 Q &qqqq; outq

err+ out _error.description
del- zf; err
clo m_hprogress; err
