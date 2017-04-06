function str&sOld str&sDiff str&sNew

 Patches string using a binary diff.
 sDiff should contain data that was created previously by DiffBinary from data that now is in sOld. This function recreates data of sNew that was used then.

 sOld - variable containing data that is considered "old".
 sDiff - binary diff (patch).
 sNew - where the results will be stored.


__xdiff.mmfile_t f1 f2
if(__xdiff.xdl_mmfile_ptradd(&f1 sOld sOld.len 0)<0 or __xdiff.xdl_mmfile_ptradd(&f2 sDiff sDiff.len 0)<0) end ES_FAILED

__xdiff.xdemitcb_t cb
cb.fa_outf=&xdiff_cb
sNew.fix(0 1); cb.priv=&sNew

if(__xdiff.xdl_bpatch(&f1 &f2 &cb)) end ES_FAILED
