 /
function hDlg index [~map] [flags] ;;flags: 1 don't show controls that were hidden when hiding the page or in dialog editor

 Shows dialog page and hides other pages.
 Dialog procedure can call it, for example, when listbox item is clicked.
 Read more in <help #IDH_DIALOG_EDITOR#A10>Help</help>.

 index - zero-based page index. Use -1 to hide all pages.
 map - can be used to map index to different page. Read more in Help.
 flags - added in QM 2.3.4.


int i n h c show
ARRAY(lpstr) as
ARRAY(POINT) ap

if(index>=0)
	if(map.len)
		n=tok(map as index+1 " ()" 25); if(n<=index) goto g1
		_s=as[index]
		n=tok(_s as -1 " " 1)
		ap.create(n)
		for(i 0 n) ap[i].x=val(as[i])*100+1000; ap[i].y=ap[i].x+100
	else
		 g1
		n=1; ap.create(1)
		ap[0].x=index*100+1000
		ap[0].y=ap[0].x+100
else
	n=1; ap[].y=1000

h=GetWindow(hDlg GW_CHILD)
rep
	if(!h) break
	c=GetDlgCtrlID(h); if(c<1000) goto gNext
	show=0; for(i 0 n) if(c>=ap[i].x and c<ap[i].y) show=SW_SHOW; break
	
	if flags&1
		int ft(0) k=GetProp(h "qm_page") ;;0 not set, 1 visible in page, 2 hidden in page, |4 the last page
		if(k=0) k=1+(GetWindowLong(h GWL_STYLE)&WS_VISIBLE=0); ft=1
		if(show) SetProp(h "qm_page" k|4)
		else if(k&4) k=1+(GetWindowLong(h GWL_STYLE)&WS_VISIBLE=0); SetProp(h "qm_page" k) ;;prev page
		else if(ft) SetProp(h "qm_page" k)
		else k|2
		if(k&2) goto gNext
	
	ShowWindow(h show)
	 gNext
	h=GetWindow(h GW_HWNDNEXT)
