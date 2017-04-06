 /
function Acc&a level lParam

str s st sn sr

st.set(9 0 level)
a.Role(sr)
sn=a.Name; sn.escape(1)

s.format("%s%s ''%s''" st sr sn)

sn=a.Value; if(sn.len) s.formata("  value=''%s''" sn)
sn=a.Description; if(sn.len) s.formata("  descr=''%s''" sn)

out s

ret 1
