 /
function str&s str&cls str&sdecl str&sdef withunknown
if(!s.len) end

AC_Prepare &s

s.findreplace("([]" "(")
s.findreplace("( []" "(")
s.findreplace(",[]" ",")
s.findreplace(", []" ",")
s.findreplace(" = 0;[]" ";")

 int i k n
 rep
	 i=findc(s '(' i+1); if(i<0) break
	 k=findcr(s 32 i-2)+1
	 if(!memcmp(s+k "STDMETHOD" 9)) k=i+1; i=findc(s ')' k); if(i<0) break
	 ss.geta(s k i-k); ss+" "; n+1
	 i=findc(s ';' i); if(i<0) break
 ss.rtrim; ss+"[]"
CH_Compact s &sdecl
sdecl.findreplace("( " "(" 8)
sdecl.findreplace("; " ";" 8)
sdecl.findreplace("virtual HRESULT STDMETHODCALLTYPE" "[9]STDMETHODIMP")

sdef.from(sdecl "[]")
sdef.findreplace("[9]STDMETHODIMP " _s.from("STDMETHODIMP " cls "::"))
sdef.findreplace(";[]" "[]{[]	[]	return S_OK;[]}[]//__________________________________________________________________[][]")
 insert ZZ
int i j k;
rep
	i=find(sdef "::" k)
	j=findc(sdef '(' i)
	k=findc(sdef '{' j)+1
	if(i<0 or j<0 or k<1) break
	s.get(sdef i j-i)
	s-cls; s-"[]	ZZ(''"; s+"'');"
	sdef.insert(s k s.len)
	k+s.len

 out "%i members found" n

if(withunknown)
	s.getmacro("II_IUnknown"); s+cls; s+" members[]"
	sdecl-s
	
	s.getmacro("II_IUnknown2")
	s.findreplace("##" cls)
	sdef-s
