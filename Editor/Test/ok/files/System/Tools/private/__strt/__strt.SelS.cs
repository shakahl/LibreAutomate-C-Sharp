function# str&sRet $tokens [flags] ;;flags: if none, select first

 Gets string i=val(this) from tokens and stores in sRet.
 Returns i, or -1 on error.
 Use with a combo/list box var, to select one of strings.
 Strings must be separated with spaces and optionally enclosed in < >. Use <> for empty string.

ARRAY(str) a
int i(val(s)) nt=tok(tokens a -1 " <>" 64)
if(i<0 or i>=nt) if(flags&1) i=0; else ret -1
sRet=a[i]
ret i
