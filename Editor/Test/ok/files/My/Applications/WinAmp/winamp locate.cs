 selects current playing track in library

 note: works on Win7 Pro. Does not work on Win7 RC.

int wa=win("Winamp" "BaseWindow_RootWnd")

 get current track's artist and title
int wo=GetWindow(wa GW_OWNER)
str s.getwintext(wo)
s.findreplace(" - " "[]")
str s1 s2 s3
s1.getl(s 0); s1.gett(s1 1 "."); s1.ltrim
s2.getl(s 1)
 out s1
 out s2

 find in library and select
int hlv=child(1001 "List4" "SysListView32" wa)
Acc a=acc("" "LIST" hlv "" "" 0x1001)
for a.elem 1 1000000000
	s=a.Name; err break
	 out s
	if(s=s1)
		s=a.Description
		 out s
		s3.format("Title: %s" s2)
		if(s.beg(s3))
			sel s[s3.len]
				case [',',';'] ;;list separator can be changed in CP
				a.Select(2)
				SendMessage hlv LVM_ENSUREVISIBLE a.elem-1 0
