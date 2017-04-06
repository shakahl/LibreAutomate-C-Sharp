 change this code

out
str chars="人 有 我 他 这 个 们 中 来 上 大 为 和 国 地 到 以 说 时"
str text=
 人来源: 谷歌 更新时间: 2009年6月11日12时 排名标准: 谷歌搜索频度个个个个个个个个个个个个个个个个变化率
 介绍: 展现排名上升变化最快的各类关键词，范围覆盖整体 热榜类别，根据关键词当人日的搜索频度与前一日此关键词的搜索频度比较产生，差值越大，排序越前，每日更新。人

 text.getsel

 -------------------------------

 optionally change this code

int needHTML=0 ;;set to 1 if need HTML

str s ;;all output will be stored into this variable
str sfmt ;;found text formatting
str footer ;;footer
if(!needHTML)
	s="<>" ;;formatted output
	sfmt="<color ''0x80E0''>%s</color>" ;;0x80E0 is color value; see out help
else
	s="<html>[]<head>[]<meta http-equiv=''Content-Type'' content=''text/html; charset=utf-8''>[]</head>[]<body>[]" ;;header
	sfmt="<font color=''#E08000''>%s</font>"
	footer="[]</body>[]</html>"

 -------------------------------

 don't change this code

BSTR bchars(chars) btext(text) ;;convert to UTF-16 because manipulating UTF-8 is difficult because of variable character width
str si.all(btext.len 2 0) ;;shadow of btext. 0-s for nonfound characters, 1 for found
ARRAY(str) astat.create(bchars.len) ;;character statistics
int i n nn
word* w w0=btext
for i 0 bchars.len ;;for each character in chars
	int c=bchars[i]
	if(c=' ') continue ;;skip spaces
	w=w0
	for n 0 1000000000 ;;for each instance of the character in text
		w=wcschr(w c); if(!w) break ;;find UTF-16 character
		si[w-w0/2]=1 ;;set 1 for found characters
		w+2
	if(n)
		lpstr s1=+&c
		astat[i].format("%9i %s (%.2f %%)" n _s.ansi(s1) 100.0*n/btext.len)
	nn+n
 out nn ;;see total number of found characters

 format output with colors
int ip ;;prev start of found or nonfound characters
for i 0 si.len
	if(!si[i]) continue
	 start of found characters
	n=i-ip; if(n) s+_s.ansi(&btext[ip] -1 n) ;;add prev nonfound characters
	ip=i
	for i i si.len
		if(si[i]) continue
		 start of nonfound characters
		n=i-ip; s.formata(sfmt _s.ansi(&btext[ip] -1 n)) ;;add prev found characters
		ip=i
		break

n=i-ip; if(n) s.formata(iif(si[ip] sfmt "%s") _s.ansi(&btext[ip] -1 n)) ;;remainder

 character statistics
astat.sort(1)
_s=astat
_s.rtrim
_s.formata("[]Total %i (%.2f %%)" nn 100.0*nn/btext.len)
_s-"[][]Statistics:[]"
if(needHTML) _s.findreplace("[]" "[]<br>")
s+_s

s+footer

 display
out s
