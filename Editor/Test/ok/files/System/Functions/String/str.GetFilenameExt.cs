function [$pathname]

 Gets filename extension.

 pathname - file name with or without path. If omitted or 0, initial path must be stored in this variable.
   Can be enclosed in ".

 EXAMPLE
 str s
 s.GetFilenameExt("c:\aaa\fff.eee")
 out s


if(!pathname) pathname=this

int i=findcr(pathname '\')+1
int j=findcr(pathname '.')+1
if(j>i) this.get(pathname j); this.rtrim("''")
else this=""
