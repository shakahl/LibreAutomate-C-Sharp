function [action] ;;action: 0 changed, 1 undo, 2 redo

___DE_UNDO& u
int i h
sel action
	case 0 ;;changed
	i=_u.len
	if(i<100)
		_u.redim(_upos+1)
		&u=&_u[_u.len-1]
	else
		_u[0].dd.all
		memmove(&_u[0] &_u[1] i-1*sizeof(___DE_UNDO))
		&u=&_u[i-1]; memset(&u 0 sizeof(___DE_UNDO))
	
	_FormatDD(u.dd)
	u.fid=GetDlgCtrlID(_hsel)
	u.page=_page
	sub.EnableButtons(1 0)
	
	_upos=_u.len
	_textChanged=0; _arrowMovSiz=0
	_save=1
	ret
	
	case 1 ;;undo
	if(!_upos) ret
	_upos-1; i=_upos
	
	case 2 ;;redo
	if(_upos=_u.len) ret
	i=_upos; _upos+1

&u=_u[i]

___DE_UNDO uu
_FormatDD(uu.dd)
uu.fid=GetDlgCtrlID(_hsel)
uu.page=_page

_ac.redim

DestroyWindow _hform
_CreateForm(u.dd 1)

_Page(u.page)
if(u.fid) h=id(u.fid _hform 1)
if(!h) h=_hform
_Select(h)

_u[i]=uu

sub.EnableButtons(_upos>0 _upos<_u.len)

_textChanged=0
_save=1


#sub EnableButtons c
function undo redo

SendMessage _htb TB_ENABLEBUTTON 1014 undo
SendMessage _htb TB_ENABLEBUTTON 1015 redo
