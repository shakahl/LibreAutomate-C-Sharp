 /
function hDlg $idList action [_on] ;;action: MCA_Show, MCA_Enable, MCA_Check, MCA_NoTheme

 Performs an action on multiple dialog controls specified as a list of ids.

 hDlg - dialog handle.
 idList - list of control ids. Separate ids using space. Use hyphen to specify range of ids. Use -id to reverse action for some controls.
 action - name or address of user-defined function that performs the action. Beside the predefined functions you can create your own functions. See MCA_Show code.
 _on - if 1, turns the property on (eg shows), if 0 - off (eg hides). Ignored with MCA_NoTheme.

 EXAMPLE
 MultiControlAction hDlg "3 5-7 -11" &MCA_Enable 0


int i v h on
ARRAY(POINT) a; POINT& p
rep
	if(idList[i]='-') i+1; on=!_on; else on=_on!0
	v=val(idList+i); h=id(v hDlg); if(h) &p=a[]; p.x=h; p.y=on
	i=findcs(idList " -" i)+1; if(i=0) break
	if(idList[i-1]='-')
		for(v v+1 val(idList+i)+1) h=id(v hDlg); if(h) &p=a[]; p.x=h; p.y=on
		i=findc(idList 32 i)+1; if(i=0) break

call action &a
