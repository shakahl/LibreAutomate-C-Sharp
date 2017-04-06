out
 out SendMessage(id(2201 _hwndqm) SCI_STYLEGETCHARACTERSET 32 0)
 int j
 for j 0 33
	 SendMessage(id(2201 _hwndqm) SCI_STYLESETCHARACTERSET j SC_CHARSET_BALTIC)

 out setlocale(LC_CTYPE "")
 out setlocale(LC_CTYPE _s.format(".%i" GetACP))
int i
str s
for i 128 256
	 int lo1=islower(i)!0
	 int lo2=IsCharLower(i)!0
	 int up1=isupper(i)!0
	 int up2=IsCharUpper(i)!0
	 if(lo1=lo2 and up1=up2) continue
	 out "%s %i  %i %i  %i %i" _outc(i) i lo1 lo2 up1 up2
	
	 int al1=isalpha(i)!0
	 int al2=IsCharAlpha(i)!0
	 if(al1=al2) continue
	 out "%s %i  %i %i" _outc(i) i al1 al2
	
	 if(!isspace(i)) continue
	 if(!isdigit(i)) continue
	 if(!isxdigit(i)) continue
	 if(!IsCharAlphaNumeric(i)) continue
	 if(!isgraph(i)) continue
	
	out "%s %i" _outc(i) i
	