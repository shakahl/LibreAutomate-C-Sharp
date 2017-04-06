rset perf "perf"
PF
 wait 0 H mac("test CsScript3")
wait 0 H mac("test CsScript Load resource2")
PN
long t2; rget t2 "perf"; out F"{perf-t2}"
PO
