 /CtoQM
function# str&s [rec] [str&sinit] [str'sdb] [db2]

 Expands all simple and function-style C macros.
 Called from H_file, CalcConst, MacrosToFunctions.

if(!s.len) ret
ARRAY(str) aa
int i j rlen db r li
lpstr s1 s2 s3
str s0=s; if(!&sinit) &sinit=&s0

if(db2) out s
 db=1
 if(db) out "(%s) %s[]" sdb s0

rep
	s1=FindNextCIdentifier(s+i &li); if(!s1) break
	i=s1-s
	str name.get(s i li)
	if(db) out "%s %i" name rec
	
	s1=m_mc.Get(name)
	if(s1)
		if(name=s1) goto skip
		for(j 0 rec)
			if(name=*m_aems[j])
				 out "%s[]%s[]%s[] %i" name s0 sinit rec
				goto skip ;;avoid endless recursion if this macro is already expanded in this stack
		str ss=s1
		m_aems[rec]=&name; err end "sinit='%s' name='%s'" 1 sinit name
		ExpandMacros(ss rec+1 sinit rec+1)
		s.replace(ss i name.len)
		i+ss.len
		r|2
		if(s[i]='(') i-ss.len; continue ;;eg MAKEINTRESOURCE(x) to MAKEINTRESOURCEA(x)
	else
		s1=s+i+li; s1+strspn(s1 " ")
		if(s1[0]!='(') goto skip
		s3=m_mcf.Get(name); if(!s3 or name=s3) goto skip
		 for(j 0 rec)
			 if(name=*m_aems[j])
				  out "%s[]%s[]%s[] %i" name s0 sinit rec
				 goto skip ;;avoid endless recursion if this macro is already expanded in this stack
		
		s2=SkipEnclosed(s1 ')'); if(!s2) end s1 1
		rlen=s2-(s+i)
		str args.get(s s1+1-s s2-s1-2)
		
		if(findrx(s3 "\((.*?)\)(?: *(.+))?" 0 0 aa)<0)
			 out aa[2]
			i+rlen; continue
		if(!sub.ExpandFsMacro(aa[2] aa[1] args))
			 out aa[2]
			i+rlen; continue
		
		m_aems[rec]=&name; err end sinit 1
		ExpandMacros(aa[2] rec+1 sinit rec+1)
		s.replace(aa[2] i rlen)
		i+aa[2].len
		r|1
	
	continue
	 skip
	i+li

if(findc(s '#')>=0) sub.PreprocOperators(s)

 if(db) out "(%s) %s[]-->[]%s[]" sdb s0 s
 
ret r


#sub PreprocOperators
function# str&s

 Removes preprocessor operators ##.

int i j k r c
lpstr seq; str ss sss
rep
	i=findrx(s "(''|'| *## *|#@? *(\w+))" i 0 j); if(i<0) break
	sel s[i]
		case [34,39]
		seq=SkipEnclosed(s+i 0); if(!seq) end "unmatched quote" 1
		i=seq-s
		
		case else
		if(s[i]='#' and s[i+1]!='#') ;;# or #@
			k=i+1
			if(s[k]='@') c=39; k+1; else c=34
			ss.get(s k j-(k-i)); ss.ltrim
			sss.format("%c%s%c" c ss c)
			s.replace(sss i j)
			i+sss.len
		else ;;##
			s.remove(i j)
		r=1

 out s
ret r


#sub ExpandFsMacro
function# str&s $argnames $argvalues

 Expands C function-style macro.
 s is macro definition
 argnames and argvalues must be in form a1, a2, ...


 str s0=s

ARRAY(str) an av
int i j na; str rx ss; lpstr seq

na=tok(argnames an);; if(!na) ret
if(sub.ParseCArguments(argvalues av)!=na) ret
if(!na) ret 1

rx="[''']|\b(?:"
for(i 0 na)
	av[i].trim
	rx.formata("%s|" an[i])
rx.set(")\b" rx.len-1)

rep
	j=findrx(s rx j 0 ss); if(j<0) break
	sel(s[j]) case [34,39] seq=SkipEnclosed(s+j 0); if(seq) j=seq-s; continue; else end "enclosing error" 1;;this can be skipped to make it easier to apply # and #@ operators. None of strings had formal parameters inside.
	for(i 0 na)
		if(ss=an[i])
			s.replace(av[i] j ss.len)
			break
	if(i<na) j+av[i].len; else j+ss.len

ret 1


#sub ParseCArguments
function# $s ARRAY(str)&a

 Used by ExpandFsMacro.

int i
lpstr s1=s
a.redim
if(!s[0]) ret

rep
	 i=findcs(s1 ",({<'''"); if(i<0) break ;;bug if < included because it's also an operator
	i=findcs(s1 ",({'''"); if(i<0) break
	s1+i
	sel s1[0]
		case ','
		a[a.redim(-1)].left(s s1-s)
		s1+1; s=s1
		case else
		s1=SkipEnclosed(s1 0); if(!s1) ret

a[a.redim(-1)]=s

for i 0 a.len
	a[i].trim

ret a.len
