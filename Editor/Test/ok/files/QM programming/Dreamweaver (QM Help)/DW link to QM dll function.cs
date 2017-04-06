 Takes selected QM dll function from Dreamweaver editor and makes link to its help

str s=DW_GetCurrentDocumentFile
if(!s.endi("IDP_QMDLL.html")) mes- "This macro can be used only in IDP_QMDLL.html."

DW_LinkNone
DW_Css "None"

s.getsel; err ret

 s=F"<a href=''../User/IDP_QMDLL.html#{s}'' class=''dll''>{s}</a>"
s=F"<a href=''#{s}'' class=''dll''>{s}</a>" ;;if in IDP_QMDLL

key C`
0.2
s.setsel
key C`
