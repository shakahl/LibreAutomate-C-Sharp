 str s.getfile("C:\Documents and Settings\a\Desktop\curl-7.10.6\docs\libcurl\curl_easy_setopt.html")
str s.getfile("C:\Documents and Settings\a\Desktop\curl-7.10.6\docs\libcurl\curl_formadd.html")
int i j
str ss sss so
rep
	i=find(s "<B>" j 1)+3; if(i<3) break
	j=find(s "</B>" i 1); if(j<0) break
	if(!isalpha(s[i])) continue
	ss.get(s i (j-i))
	if(ss[0]!='C') continue
	 out ss
	so.formata("[]z(''def %s #'', %s);[]" ss ss)
	so.setclip
	