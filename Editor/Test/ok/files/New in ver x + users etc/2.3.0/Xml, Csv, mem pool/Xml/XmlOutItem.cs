 /
function IXmlNode&xn

if(!XmlIsValid(xn)) out "INVALID (%i)" xn; ret

lpstr sit=
 -
 el
 a
 text
 xml
 DOC
 PI
 CD
 comm
ARRAY(str) at=sit

XMLNODE xi
xn.Properties(&xi)
out "%-15s %-4s F=0x%X L=%i V='%s'", xi.name, at[xi.xtype], xi.flags, xi.level, xi.value
