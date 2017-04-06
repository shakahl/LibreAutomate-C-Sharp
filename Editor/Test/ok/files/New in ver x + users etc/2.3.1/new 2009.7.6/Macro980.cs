out

str chars="人 有 我 他 这 个 们 中 来 上 大 为 和 国 地 到 以 说 时"
str text=
 人来源: 谷歌 更新时间: 2009年6月11日12时 排名标准: 谷歌搜索频度变化率
 介绍: 展现排名上升变化最快的各类关键词，范围覆盖整体 热榜类别，根据关键词当人日的搜索频度与前一日此关键词的搜索频度比较产生，差值越大，排序越前，每日更新。人

BSTR bchars(chars) btext(text)
int i n nn
word* w
for i 0 bchars.len ;;for each character in chars
	int c=bchars[i]
	if(c=' ') continue ;;skip spaces; remove this if not needed
	w=btext
	for n 0 1000000000 ;;for each instance of the character in text
		w=wcschr(w c); if(!w) break
		w+2
	lpstr s1=+&c; _s.ansi(s1)
	out "%s %i" _s n
	nn+n