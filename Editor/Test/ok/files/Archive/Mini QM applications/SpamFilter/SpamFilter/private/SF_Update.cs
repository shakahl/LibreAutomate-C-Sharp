function [Init] [only] ;;only: 0 all, 1 good, 2 spam.

int i h(__sfmain) hlg(id(3 h)) hls(id(6 h)) ns
str s status

if(Init)
	SendMessage hlg LVM_SETEXTENDEDLISTVIEWSTYLE LVS_EX_INFOTIP|LVS_EX_FULLROWSELECT -1
	SF_LvAddColumn hlg 0 "Subject" 180
	SF_LvAddColumn hlg 1 "From" 180
	SF_LvAddColumn hlg 2 "Status" 70
	SF_LvAddColumn hlg 3 "Comments" 130
	SF_LvAddColumn hlg 4 "Size" 50
	SF_LvAddColumn hlg 5 "Att" 30
	SendMessage hls LVM_SETEXTENDEDLISTVIEWSTYLE LVS_EX_INFOTIP|LVS_EX_FULLROWSELECT -1
	SF_LvAddColumn hls 0 "Subject" 180
	SF_LvAddColumn hls 1 "From" 180
	SF_LvAddColumn hls 2 "Deleted" 70
	SF_LvAddColumn hls 3 "Comments" 180
	h=id(7 h)
	SendMessage h LVM_SETEXTENDEDLISTVIEWSTYLE LVS_EX_INFOTIP|LVS_EX_FULLROWSELECT -1
	SF_LvAddColumn h 0 "Action" 150
	SF_LvAddColumn h 1 "Error" 300
	SF_LvAddColumn h 2 "Server" 150
	CheckDlgButton __sfmain 18 1
	ret

 good
if(only!=2)
	SF_LvDeleteAll hlg
	for(i 0 a.len)
		status=""
		if(a[i].flags&0x8000) status="Delete"
		else if((a[i].flags&12)) status=iif((a[i].flags&4) "Good" "Spam")
		else s=a[i].m.GetHeader("X-QMSF"); if(s~"restored") status="Restored"
		SF_LvAdd hlg i 0 a[i].m.Subject a[i].m.FromAddr status a[i].comm s.from((a[i].size/1024+1) "KB") a[i].m.Attachments.Count

 deleted
if(only!=1)
	SF_LvDeleteAll hls
	MailBee.Message m._create; m.CodepageMode=1
	Dir d; str sPath sDate
	s.from(SF_DIR "\*.eml")
	foreach(d s FE_Dir)
		sPath=d.FileName(1)
		SFDELETED* sd._new; sd.sf=sPath; sd.d=d.TimeCreated2
		sDate.time(sd.d "%#d,  %H:%M")
		m.ImportFromFile(sPath)
		SF_LvAdd hls ns sd m.Subject m.FromAddr sDate m.GetHeader("X-QMSF-Comments")
		ns+1
	SendMessage hls LVM_SORTITEMS 0 &SF_LvSort2

err+
SF_Select
