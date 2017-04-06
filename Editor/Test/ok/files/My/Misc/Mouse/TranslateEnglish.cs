 /
function! $from str&to

 Translates English to Lithuanian using vertimas.vdu.lt.


int w1=win("dlg_translate1" "#32770")
if(!w1)
	mac "dlg_translate1"
	w1=wait(5 WC win("dlg_translate1" "#32770"))
	0.5
	web "http://vertimas.vdu.lt/twsas/default.aspx" 1 w1

Htm el=htm("TEXTAREA" "ctl00_FormContent_txtSrc" "" w1 0 0 0x121 2)
el.SetText(from)

el=htm("TEXTAREA" "ctl00_FormContent_txtTrd" "" w1 0 1 0x121) ;;after submitting, this el is invalid
el.SetText("")

el=htm("INPUT" "ctl00_FormContent_btnTranslate" "" w1 0 5 0x121)
el.Click

MSHTML.IHTMLDocument2 doc=el.el.document
rep
	0.1
	_s=doc.readyState
	if(_s="complete")
		el=htm("TEXTAREA" "ctl00_FormContent_txtTrd" "" w1 0 1 0x121)
		to=el.Text
		break

ret 1

err+
	to=from
	clo w1
