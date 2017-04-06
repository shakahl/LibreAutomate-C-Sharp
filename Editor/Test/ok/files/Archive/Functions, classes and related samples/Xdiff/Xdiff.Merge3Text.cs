function str&sOld str&sNew1 str&sNew2 str&sMerged [str&sRejected]

 Merges 3 strings.
 The function works by creating a diff between sOld and sNew2 and by applying the diff to sNew1. Because of this sequence, sNew1 changes will be privileged against the ones of sNew2.

 sOld - original string.
 sNew1 - modified version of sOld.
 sNew2 - another modified version of sOld.
 sRejected - if used, receives all differential chunks that cannot be applied.


__xdiff.mmfile_t f0 f1 f2
if(__xdiff.xdl_mmfile_ptradd(&f0 sOld sOld.len 0)<0 or __xdiff.xdl_mmfile_ptradd(&f1 sNew1 sNew1.len 0)<0 or __xdiff.xdl_mmfile_ptradd(&f2 sNew2 sNew2.len 0)<0) end ES_FAILED

__xdiff.xdemitcb_t cb cbr
cb.fa_outf=&xdiff_cb
sMerged.fix(0 1); cb.priv=&sMerged
cbr.fa_outf=&xdiff_cb
if(!&sRejected) &sRejected=_s
sRejected.fix(0 1); cbr.priv=&sRejected

if(__xdiff.xdl_merge3(&f0 &f1 &f2 &cb &cbr)) end ES_FAILED
