MSScript.ScriptControl- _vbs
PF
VbsAddCode ""
PN
rep 5
	_i=VbsFunc("Test" 1 2)
	 _i=VbsFunc_use_qm_api("Test" 1 2)
	 ARRAY(VARIANT) a.create(2); a[0]=1
	 _i=_vbs.Run("Test" a)
	PN
PO
#ret
function Test(byval x, byval y)
'msgbox x
end function
