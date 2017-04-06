function $_file

 Call this function to add a file to the menu.


str s
if(rget(s "Recent" m_rkey)) s.replacerx(_s.format("^\Q%s\E[]" _file) "" 9)
s.from(_file "[]" s)
int i=findl(s m_nitems); if(i>=0) s.fix(i)
rset s "Recent" m_rkey
