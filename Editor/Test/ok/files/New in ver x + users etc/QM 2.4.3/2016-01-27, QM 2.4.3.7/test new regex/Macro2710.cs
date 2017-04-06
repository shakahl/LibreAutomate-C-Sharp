#compile "__Regex"

dll- "pcre2"
	$pcre2qm_last_error
	#pcre2qm_compile Regex*x $rx [options] [jitOptions] [pcre2_compile_context*c]
	#pcre2qm_match Regex*x $s options iFrom iTo

out
WakeCPU
str s rx; int fC fM R
s="aa 7word bb 8kk cc"
 rep(10) s+s
 out s.len
rx="\d(?<na>\w+)"
 rx="\d(?:(?<na>\w+)|(?<na>;+))"
 rx="\d(?:(?<na>;+)|(?<na>\w+))"
 rx="\d(?:(?<na>;+)|(?<na>,+))?"
 rx="\d(?<na>;+)?"
 rx="\d(*MARK:B)\w+(*MARK:C)"
 rx="a(?=bc\K)"; s=",,abcde"
 rx=",(a(?=bc\K))"; s=",,abcde"
 fC=PCRE2_DUPNAMES
 rx="\d(\d)\d\d\d"; s="aa 78"; fM=PCRE2_PARTIAL_SOFT
Regex x
x.Compile(rx fC)
PF
x.Compile(rx fC)
 x.Compile(rx fC 1)
 err out _error.code;ret
PN
rep 100
	R=x.Match(s fM)
PN;PO
out R

PF
Regex y
 int rc=pcre2qm_compile(&y rx)
 if(rc) out rc; ret
y.Compile(rx)
PN
rep 100
	 R=pcre2qm_match(&y s 0 0 -1)
	R=y.MatchD(s)
PN;PO
out R
