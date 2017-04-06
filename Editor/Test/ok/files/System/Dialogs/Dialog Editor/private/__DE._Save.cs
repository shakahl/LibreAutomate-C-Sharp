 /Dialog_Editor
function!

#region  format dialog definition (DD)
str DD controls1 controls2
_FormatDD(DD controls1 controls2)

 options
if(_page>=0) str page=_page
DD+F" DIALOG EDITOR: ''{_userType}'' 0x{QMVER} ''{`*`+!_updateCode}'' ''{page}'' ''{_pageMap}'' ''{_userIdsVarAdd}''[]"

#region-  find destination macro/sub, and get text into txtAll/txtSub
 get _ddMacro text into txtAll
rep
	QMITEM q
	if(_ddMacro) qmitem _ddMacro 0 q 1|8; err _ddMacro=0 ;;error if macro deleted
	if(_ddMacro) break
	if(mes("Save the dialog in current macro?" "Dialog Editor" "OC?")!='O') ret
	_ddMacro=qmitem
str txtAll.swap(q.text) txtSub sList
if(!txtAll.len) txtAll="\Dialog_Editor[]"

 find our sub and get its text into txtSub
__Subs x.Init(0 txtAll)
int iSub=x.FindSub(_ddSub)
if iSub<0 ;;our sub renamed or deleted
	if(x.a.len=1) iSub=0
	else
		x.List(sList 1 _ddMacro)
		iSub=ListDialog(sList "Where to save the dialog?" "Dialog Editor" 2 _hwnd)-1
		if(iSub<0) ret
	_ddSub=x.a[iSub].name
__Sub& r=x.a[iSub]
txtSub.get(txtAll r.codeOffset r.codeLen)

#region-  update DD in txtSub
int ddOffset
if(r.dd.len) ddOffset=r.dd.offset-r.codeOffset
else txtSub+"[]str dd=[]"; ddOffset=txtSub.len ;;no DD in the sub. Insert at the end.
txtSub.replace(DD ddOffset r.dd.len)

#region-  create show-dialog code
str sdCode sdVariables sdVarAddr("0") sdFuncAddr sdDD sdBefore sdAfter

 variables
if controls1.len
	if _userType.len
		controls2.findreplace(" " " ~")
		sdVariables.format("type %s ~controls%s[]%s d.controls=''%s''[]" _userType controls2 _userType controls1)
		sdVarAddr="&d"
	else
		sdVariables.format("str controls = ''%s''[]str%s[]" controls1 controls2)
		sdVarAddr="&controls"

 find existing ShowDialog, and get sdDD and sdFuncAddr from it
int iSD=-1; ARRAY(CHARRANGE) arxSD
if(_updateCode) iSD=findrx(txtSub "^([\t,]*(?=[^ ;/\\\t,]).*?\bShowDialog[\( ] *)(''.*?''|[^\s,\)]+)(?:[, ]+([^,\s\)]+))?(?:[, ]+([^,\s\)]+))?([^[]]*)" 0 8 arxSD)
if iSD>=0
	CHARRANGE& k
	&k=arxSD[1]; sdBefore.get(txtSub k.cpMin k.cpMax-k.cpMin)
	&k=arxSD[2]; if(k.cpMax>=0) sdDD.get(txtSub k.cpMin k.cpMax-k.cpMin)
	&k=arxSD[3]; if(k.cpMax>=0) sdFuncAddr.get(txtSub k.cpMin k.cpMax-k.cpMin)
	&k=arxSD[5]; if(k.cpMax>=0) sdAfter.get(txtSub k.cpMin k.cpMax-k.cpMin)

 dlgproc address
if !sdFuncAddr
	int iDlgproc=-1
	if(x.a.len>iSub+1 and x.IsDlgProc(iSub+1)) iDlgproc=iSub+1 ;;is next sub dlgproc?
	if(iDlgproc<0 and x.IsDlgProc(iSub)) iDlgproc=iSub ;;is our sub dlgproc?
	
	if(iDlgproc>0) sdFuncAddr.from("&sub." x.a[iDlgproc].name)
	else if(iDlgproc=0 and q.itype=1) sdFuncAddr.from("&" q.name)
	if(!sdFuncAddr.len) sdFuncAddr="0"

 DD variable or DD macro name
if !sdDD.len
	if(_updateCode or iSub) sdDD="dd"
	else sdDD.from("''" q.name "''")

 ShowDialog statement
if(!sdBefore.len) sdBefore="if(!ShowDialog("; sdAfter=")) ret"
sdCode.format("%s%s %s %s%s" sdBefore sdDD sdFuncAddr sdVarAddr sdAfter)

#region-  update show-dialog code in txtSub, or send to QM output
if _updateCode
	if(iSD<0) txtSub.from(txtSub "[]" sdVariables sdCode "[]") ;;if ShowDialog did not exist, simply append variables and ShowDialog line
	else
		txtSub.replace(sdCode iSD (arxSD[0].cpMax-iSD)) ;;replace ShowDialog line
		
		 find old variables
		str rxv; ARRAY(str) av
		if(_userType.len) rxv.format("^type %s ~controls [\w ~]+[]%s \w+\.controls ?= ?''[\d ]+''[]" _userType _userType)
		else rxv="^str controls ?= ?''[\d ]+''([]\w+ )[\w ]+[]"
		int iVar=findrx(txtSub rxv 0 8 av)
		
		 insert or replace variables
		if(iVar<0) txtSub.insert(sdVariables iSD) ;;if not found, insert before ShowDialog
		else ;;replace old variables
			if(!_userType.len and sdVariables.len) sdVariables.findreplace("[]str " av[1] 4) ;;allow other str-based type for dialog variables, eg __strt instead of str
			txtSub.replace(sdVariables iVar av[0].len)
			DE_UpdateVariables(txtSub av[0] sdVariables)
else
	out "<> You can use this code to show dialog. <help #IDH_DIALOG_EDITOR>Help</help>.[]<code>%s%s</code>" sdVariables sdCode

#region-  replace macro text, return 1
txtAll.replace(txtSub r.codeOffset r.codeLen) ;;replace old sub text in txtAll with the new sub text
txtAll.setmacro(_ddMacro)
err ;;error if read-only
	sel mes(F"{_error.description}[][]Failed to update dialog in current macro. Possibly it is read-only. Create new macro for it?" "Error" "OCx")
		case 'O' _ddMacro=newitem("" txtAll q.name "" "" 4)
		case else ret

_save=0
ret 1
err+ mes _error.description "Error" "x"
#endregion
