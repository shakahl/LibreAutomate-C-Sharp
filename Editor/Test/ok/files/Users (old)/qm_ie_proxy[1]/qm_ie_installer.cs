if(!dir("$QM$\context" 1))
	MkDir "$QM$\context"
if(!dir("$QM$\context\qm.htm"))
	_s="<script language=''VBScript''> []sURL = window.external.menuArguments.event.srcElement.href[]createObject(''WScript.Shell'').run ''qmcl.exe '' + ''M ''''qm_ie_proxy'''' A '' + sURL + ''''[]</script>"; _s.setfile("$QM$\context\qm.htm")
str s.from(_qmdir "context\qm.htm")
str name = "contexts"
str regkey = "Software\Microsoft\Internet Explorer\MenuExt\qm_proxy"
rset(s "" regkey 0)
rset(32 name regkey 0)
	
