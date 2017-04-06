 /test Java Access Bridge
function[c] vmID JOBJECT64'event JAB.AccessibleContext'ac
out "---- mouse entered ----"

 outw JAB.GetHWNDFromAccessibleContext(vmID source) ;;0. Need to get top parent object (try getTopLevelObject or GetAccessibleParentFromContext).

JAB.AccessibleContextInfo c
if JAB.GetAccessibleContextInfo(vmID ac &c)
	out _s.ansi(&c.role_en_US)
	out "name='%S', description='%S', states='%S', childrenCount=%i" &c.name &c.description &c.states_en_US c.childrenCount
	 if(c.accessibleText) out JabGetText(vmID ac) ;;Java app crashes

JAB.ReleaseJavaObject vmID ac
JAB.ReleaseJavaObject vmID event
