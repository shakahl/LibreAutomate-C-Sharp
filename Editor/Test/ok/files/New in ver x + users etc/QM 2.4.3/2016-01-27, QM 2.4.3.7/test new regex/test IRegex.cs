#compile "__Regex"

out
 WakeCPU
str s rx; int fC fM R i
ARRAY(str) as; ARRAY(OFFSETS) ao
 s="aa 7word bb 8mm cc"
s="aa 7word bb 8;; cc"
rep(1) s+s
 out s.len
rx="\d(?<na>\w+)"
 rx="\d(?:(?<na>\w+)|(?<na>;+))"; fC=PCRE2_DUPNAMES
 rx="\d(?:(?<na>;+)|(?<na>\w+))"; fC=PCRE2_DUPNAMES
 rx="\d(?:(?<na>;+)|(?<na>,+))?"; fC=PCRE2_DUPNAMES
 rx="\d(?<na>;+)?"
 rx="\d(*MARK:B)\w+(*MARK:C)"
 rx="a(?=bc\K)"; s=",,abcde"
 rx=",(a(?=bc\K))"; s=",,abcde"
 fC=PCRE2_DUPNAMES
 rx="\d(\d)\d\d\d"; s="78"; fM=PCRE2_PARTIAL_SOFT

PF
IRegex x=CreateRegex(rx fC)
 err out _error.code; out _error.description; ret
PN
 R=x.Match(s fM)
 R=x.MatchFromTo(s 10 -1 fM)
 out x.Find(s "na" _s); out _s
 out x.FindFromTo(s 20 -1 "na" _s); out _s
 out RegexFind(s rx 0 _s _i); out _s; out _i
 FROMTO k.from=7; k.to=-1; out RegexFind(s rx 0 _s _i k); out _s; out _i
 REGEXOPTIONS k.compileFlags=fC; k.matchFlags=fM; out RegexFind(s rx 0 _s _i 0 k); out _s; out _i
 out x.FindGetArray(s as ao)
 out x.FindGetArrayFromTo(s 10 -1 as ao)
 out RegexFindGetArray(s rx as ao)
 out R
 out x.Get(0 _s _i); out _i; out _s
 out x.Get("na" _s _i); out _i; out _s
 rep 7
	  if(!x.MatchNext(s fM)) break
	  out x.Get()
	  out x.GetArray(as ao)
	  int* p=x.offsets
	  R=x.FindNext(s 0 _s 0 fM); if(R<0) break
	  out "%i %s" R _s
	 R=x.FindNextGetArray(s as ao fM); if(R<0) break
	 out R
	 for(i 0 as.len) out "%i %i %s" ao[i].b ao[i].e as[i]
 PN
 OFFSETS* p=x.offsets;  out "%i %i  %i %i" p[0].b p[0].e p[1].b p[1].e
 ret
 rep 100
	  R=pcre2qm_match(&x s 0 0 -1)
	 R=x.Match(s)
	  R=x.MatchFromTo(s 7 -1)
	 R=x.Get(0 _i)
	  R=x.GetStr(0 _s)
 PN;PO
 out R
 out _i
  out _s
 out x.mark
 out x.code
 out x.match_data
 
  x=CreateRegex(".+")
  out x
 
 out "-----"
 out x.FindAll(s)
int sub=-1
 sub="na"
 sub=1
  
 out x.FindAll(s sub as ao 3)
   out x.FindAllFromTo(s 0 s.len/2 as ao sub)
   out x.FindAllFromTo(s s.len/2 s.len as ao sub)
 out RegexFindAll(s rx sub as ao)
 REGEXOPTIONS k.compileFlags=fC; k.matchFlags=fM; out RegexFindAll(s rx sub as ao 0 k)
 if(sub>=0) for(i 0 as.len) out "%i %i    '%s'" ao[i].b ao[i].e as[i]
 else for(i 0 as.len) out "%i %i   %i %i    '%s'  '%s'" ao[0 i].b ao[0 i].e  ao[1 i].b ao[1 i].e   as[0 i]  as[1 i]
 
 out x.Replace(s)
 out x.Replace(s "REPL")
 out x.Replace(s "-" 1)
 out x.ReplaceFromTo(s 0 s.len/2 "REPL")
 out x.ReplaceFromTo(s s.len/2 -1 "REPL")
 out x.Replace(s "($1.$0)")
 out x.Replace(s "($2.$0)")
 out x.Replace(s "(${na}.$0)")
 out RegexReplace(s rx "RRR")
 out RegexReplace(s rx)
 out RegexReplace(s rx "" 1)
REGEXOPTIONS k.compileFlags=fC; k.matchFlags=-1; out RegexReplace(s rx "" 1 0 k)
out s
