function# ARRAY(str)&aq ARRAY(___CHI_FILESCORE)&af ARRAY(int)&astat

 arrays of files for each query word
type ___CHI_INTARRAY ARRAY(___CHI_FILESCORE)a
ARRAY(___CHI_INTARRAY) afi

 enum query words
int i j z
astat.create(aq.len)
for i 0 aq.len
	str& k=aq[i]
	
	___CHI_INTARRAY& r=afi[]
	z=sub.GetFilesThatContainWord(k m_mw_help r 0)
	z+sub.GetFilesThatContainWord(k m_mw_tools r 10000)
	z+sub.GetFilesThatContainWord(k m_mw_func r 20000)
	z+sub.GetFilesThatContainWord(k m_mw_tips r 30000)
	astat[i]=r.a.len

for(i 0 astat.len) if(!astat[i]) ret ;;fail if a word not found
if(!afi.len) ret ;;no valid words

 use first array as base, and remove its elements that are not in other arrays
af=afi[0].a
if(afi.len>1)
	for i af.len-1 -1 -1 ;;for each file that is in the first array
		___CHI_FILESCORE& fs=af[i]
		 find the file in other arrays
		for j 1 afi.len ;;for each subsequent array
			&r=afi[j]
			for(z 0 r.a.len) if(r.a[z].fi=fs.fi) fs.score+r.a[z].score; break ;;find fi in the array
			if(z=r.a.len) ;;if not found, remove the index from the first array
				af.remove(i)
				break ;;don't search in other arrays

 now we have indices of all files that contain all query words
ret af.len


#sub GetFilesThatContainWord c
function# $k IStringMap&m ___CHI_INTARRAY&r indexadd [flags] ;;flags: 1 synonym

 gets file indices in m as array

int i j fi ln score; str s
if m.Get2(k s)
	 out "%s %s" k s
	rep
		fi=val(s+j 0 ln); if(ln) j+ln+1; else break
		fi+indexadd
		score=val(s+j 0 ln); j+ln
		if flags&1 ;;skip duplicates when adding synonyms
			for(i 0 r.a.len) if(r.a[i].fi=fi) break
			if(i<r.a.len) continue
			 out "%i %s" score k
			 score=score+1/2 ;;may tilt results too much, hiding many better non-synonym results
			score=1
		___CHI_FILESCORE& fs=r.a[]; fs.fi=fi; fs.score=score
		 out "%i %i" fi nw

if(flags&1) ret

 #ifdef _DEBUG_HELP ;;when adding synonyms, refresh always
 m_syn.RemoveAll; LoadSynonyms
 #endif

str syn
if(!m_syn.Get2(k syn)) ret
 out "%s=%s" k syn
ARRAY(lpstr) as
tok syn as -1 "," 1
for i 0 as.len
	sub.GetFilesThatContainWord(as[i] m r indexadd 1)
