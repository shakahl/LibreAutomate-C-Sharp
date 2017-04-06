 is drag distance >=4 pixels?
POINT+ g_tbds
POINT p; xm p
if(_hypot(p.x-g_tbds.x p.y-g_tbds.y)<4) ret

 is text selection?
str s.getsel
if(!s.len) ret

int h=win("TB_DRAG_SELECT" "QM_Toolbar")
if(h) mov xm-10 ym+10 h
else h=mac("tb_drag_select")

 wait for click, and close
rep
	wait 0 ML
	if(win(mouse)!=h)
		clo h
		break
