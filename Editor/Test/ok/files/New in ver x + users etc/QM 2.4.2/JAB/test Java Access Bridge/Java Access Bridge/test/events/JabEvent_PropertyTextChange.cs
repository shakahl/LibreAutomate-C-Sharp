 /test Java Access Bridge
function[c] vmID JOBJECT64'event JAB.AccessibleText'at
out "---- text changed ----"

str s
if(JabGetText(vmID at s)) out s
else out "<text deleted>"

JAB.ReleaseJavaObject vmID at
JAB.ReleaseJavaObject vmID event
