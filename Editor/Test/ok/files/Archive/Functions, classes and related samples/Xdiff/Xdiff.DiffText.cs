function str&sOld str&sNew str&sDiff [nContextLines] [flags] ;;flags: 1 append, 2 minimal

 Makes unified diff of 2 strings.

 sOld - string that is considered "old".
 sNew - string that is considered "new".
 sDiff - receives differences (patch).
 nContextLines - number of adjacent unchanged lines to add to sDiff.
 flags:
   1 - append to sDiff.
   2 - requires minimal diff size (may be slow).


__xdiff.mmfile_t f1 f2
if(__xdiff.xdl_mmfile_ptradd(&f1 sOld sOld.len 0)<0 or __xdiff.xdl_mmfile_ptradd(&f2 sNew sNew.len 0)<0) end ES_FAILED

if(flags&1=0) sDiff.fix(0 1)
flags&2

__xdiff.xdemitcb_t cb
cb.fa_outf=&xdiff_cb
cb.priv=&sDiff

if(__xdiff.xdl_diff(&f1 &f2 +&flags +&nContextLines &cb)) end ES_FAILED
