 /
function IStringMap&m ~s wordWeight [flags] ;;flags: 1 remove html

if(flags&8) out s

if(flags&1)
	s.replacerx("(?s)<!--.+?-->" " ")
	s.replacerx("(?s)<!DOCTYPE.+?>" " ")
	s.replacerx("(?s)<script.*?>.*?</script>" " ")
	s.replacerx("(?s)<\w.*?>" " ")
	s.replacerx("</\w+>" " ")
	s.replacerx("&#?\w+;" " ")
	 out s

 out "------- %i[]%s" flags s

s.stem(3)

str sf.from(m_curfile "w") K V
int i j n aLast
ARRAY(lpstr) a

 add all unique words and their counts to x
IStringMap x=CreateStringMap(4) ;;key=word, value=count
tok s a -1 " " 1
aLast=a.ubound
for i 0 a.len
	lpstr k=a[i]
	if(isdigit(k[0]) and wordWeight<=1) a[i]=0; continue
	if(m_mstop.Get(k)) a[i]=0; continue ;;stop-word
	x.IntGet(k n)
	x.IntAdd(k n+1)
	 add 2-words, eg "qm item" as "qm-item" to be used as synonym for "macro"
	if(i=aLast) break
	lpstr kk(a[i+1]) syn=0
	int isSyn
	sel k
		case "qm" if(!strcmp(kk "item")) syn="qm-item"
		case "list" if(!strcmp(kk "box")) syn="listbox"
	if(syn and !x.Get(syn)) x.IntAdd(syn 1); isSyn=1
 if(isSyn && !x.Get("macro")) out m_curfilename
 if(isSyn) out m_curfilename

 add words to m
x.EnumBegin
rep
	if(!x.EnumNext(K V)) break
	if wordWeight=1
		n=val(V)
		j=MulDiv(n 100 a.len+50)+sqrt(n)
		 out "%6i %4i %4i%%    %s  <%s>" a.len n j K m_curfilename
	else j=wordWeight
	sub.AddToMap(m K sf j)


#sub AddToMap c
function! IStringMap&m $k str&sf wordWeight

 m format:
  key=word
  value=space-separated list of files that contain the word. File format:
    XwY
    X=fileindex
    w=literal w
    Y=wordweight. Depends on wordWeight (which is different for title, word density in contents, etc).

str s
 out k
 int de=matchw(k "str")

if(m.Get2(k s))
	 if(de) out "%i %i %i   %s" wordWeight nWords wwAdd s
	 get last added file
	int fi ww j i
	i=findcr(s 32)+1
	fi=val(s+i 0 j)
	if(fi=m_curfile)
		ww=val(s+i+j+1)
		sel wordWeight
			case 1
			case else
			if(ww>=wordWeight) ret
		s.fix(i+j+1); s+ww+wordWeight
	else
		s.from(s " " sf wordWeight)
else
	s.from(sf wordWeight)

 if(de) out s
m.Add(k s)
ret 1
