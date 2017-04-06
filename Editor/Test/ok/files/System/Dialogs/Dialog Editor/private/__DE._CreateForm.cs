 /Dialog_Editor
function# [$dd] [!undo]

int i j h

if(!dd) dd="BEGIN DIALOG[]0 '''' 0x90C80AC8 0x0 0 0 224 136 ''Dialog''[]END DIALOG"
int style(WS_DISABLED|WS_VISIBLE|WS_CHILD|WS_CLIPSIBLINGS|DS_NOFAILCREATE) notstyle(WS_POPUP)
if(undo) notstyle|WS_VISIBLE; style~WS_VISIBLE

_hform=ShowDialog(dd &sub.FormProc 0 _hwnd 3|0x80000000 style notstyle 0 &sub.CompileCallback &this)
err mes _error.description "Error" "x"; ret ;;not mes-, on wm_create it crashes QM

RECT rr; rr.right=4; rr.bottom=8
MapDialogRect _hform &rr
_dbx=rr.right; _dby=rr.bottom

RECT r; GetWindowRect(_hform &r)
if !undo
	r.right+_xform; r.bottom+_yform
	j=dlg_TbGetDimensions(_htb 0 6); i=r.right-r.left; if(i<j) i=j; r.right=r.left+i ;;the last toolbar button must be visible
	GetWindowRect _htv &rr; MapWindowPoints 0 _hwnd +&rr 2; j=rr.bottom+3; i=r.bottom-r.top; if(i<j) i=j; r.bottom=r.top+i ;;treeview must be visible
	TO_AdjustWindowRect r 0 0 _hwnd
	SetWindowPos _hwnd 0 0 0 (r.right-r.left) (r.bottom-r.top) SWP_NOMOVE|SWP_NOZORDER

SetWindowPos _hform 0 _xform _yform 0 0 SWP_NOSIZE|SWP_SHOWWINDOW

 fix Static horz/vert line bug: creates by 2 pixels smaller
for i 1 _ac.len
	___DE_CONTROL& c=_ac[i]
	if c.flags&3
		h=GetDlgItem(_hform c.cid)
		GetWindowRect h &r
		if(c.flags&1) r.right+2; else r.bottom+2
		siz r.right-r.left r.bottom-r.top h

ret _hform


#sub FormProc
function# hDlg message wParam lParam
__DE* d=+GetWindowLong(GetParent(hDlg) 0)
ret d.sub._FormProc(hDlg message wParam lParam)


#sub _FormProc c
function# hDlg message wParam lParam

sel message
	case WM_CTLCOLORDLG
	if(!_brushBack) goto gCreateBrush
	ret _brushBack

ret
 gCreateBrush
 create pattern brush to draw grid
__GdiHandle hbr(CreateSolidBrush(_bgColor)) hpen(CreatePen(0 1 ColorAdjustLuma(_bgColor -100 1)))
int x(_dbx*2) y(_dby)
__MemBmp mb.Create(x y)
int ob(SelectObject(mb.dc hbr)) op(SelectObject(mb.dc hpen))
Rectangle mb.dc 0 0 x+1 y+1
SelectObject(mb.dc ob); SelectObject(mb.dc op)
_brushBack=CreatePatternBrush(mb.bm)
ret _brushBack


#sub CompileCallback
function DLGTEMPLATEEX*dt DLGITEMTEMPLATEEX*dit $cls $txt __DE&d $tooltip
ret d.sub._CompileCallback(dt dit cls txt tooltip)


#sub _CompileCallback c
function DLGTEMPLATEEX*dt DLGITEMTEMPLATEEX*dit $cls $txt $tooltip

___DE_CONTROL& c=_ac[]
c.style=iif(dit dit.style dt.style)
c.exstyle=iif(dit dit.exStyle dt.exStyle)
c.txt=txt
c.tooltip=tooltip

if dit
	c.cid=dit.id
	dit.style|WS_VISIBLE
	sel cls 1
		case "Static" sel(c.style&SS_TYPEMASK) case SS_ETCHEDHORZ c.flags|1; case SS_ETCHEDVERT c.flags|2 ;;later will need to resize
		case ["ComboBox","ComboBoxEx32"] if(dit.cy=0) c.flags|4 ;;on save set 0 height
else
	_dialogFlags=dt.helpID
	_dialogClass=cls
	_xDlg=dt.x
	_yDlg=dt.y

dt.exStyle~WS_EX_LAYERED
