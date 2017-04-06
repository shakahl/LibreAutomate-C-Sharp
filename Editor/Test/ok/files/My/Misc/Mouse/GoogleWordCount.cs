 /
function# $w

 Returns the number of Google results for the word.


str s ss
int n nn

IntGetFile F"http://www.google.com/search?q={w}" s
 out s
if(findrx(s "<div id=resultStats>About ([\d\,]+)" 0 0 ss 1)<0) mes "Google stats not found." "" "!"; ret
else ss.findreplace(","); n=val(ss)

 don't use stemming because it is too unreliable
 if(n<10000000)
	 _s=w; _s.stem
	 if(_s~w=0)
		 out _s
		 nn=GoogleWordCount(_s)
		 if(nn!=n) n+nn

ret n

 note:
 Google returns different number of results for different word forms, eg "backfire" and "backfired".
 After stemming, if the word does not exists, Google results may be various. Sometimes corrects to existing word and searches for it. Sometimes searches for completely different word. Etc.
