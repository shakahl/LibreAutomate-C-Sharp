function! cf hData str&sd

sel(cf) case [CF_UNICODETEXT,CF_TEXT,0] case else ret

lpstr s=DdeAccessData(hData &_i); if(!s or !_i) ret

if(!cf) cf=iif((_i&1 or s[1]) CF_TEXT CF_UNICODETEXT) ;;0 on execute

if(cf=CF_UNICODETEXT) sd.ansi(s -1 _i/2-1); else sd.left(s _i-1)

DdeUnaccessData(hData)
ret 1
