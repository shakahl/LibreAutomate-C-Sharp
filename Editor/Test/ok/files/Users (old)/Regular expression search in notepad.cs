str rx="find this"
int flags=1 ;;1 insens, 2 whole word

int h=id(15 "Notepad")
int i length
int+ g_newsearch
str s.getwintext(h); if(!s.len) ret ;;get all text
if(!g_newsearch) SendMessage(h EM_GETSEL 0 &i) ;;get caret pos
i=findrx(s rx i flags|8 length)
if(i<0) g_newsearch=1; mes- "Not found."
g_newsearch=0
SendMessage(h EM_SETSEL i i+length)
