function str&sOld str&sDiff str&sNew [flags] [str&sRejected] ;;flags: 1 reverse, 0x100 ignore whitespace

 Patches string using a unified diff.
 Adds sDiff to string sOld, and stores result in sNew.
 sDiff should contain text that was created previously by DiffText from text that now is in sOld. This function recreates text of sNew that was used then.

 sOld - string that is considered "old". If flag 1 - "new".
 sDiff - unified diff (patch).
 sNew - where the results will be stored.
 flags:
   1 - reverse patch. sOld should contain text that was "new" with DiffText. This function recreates "old" text and stores in sNew.
   0x100 - ignore whitespace at the beginning and the end of a line.
 sRejected - if used, receives all differential chunks that cannot be applied. It can happen if sOld text is not the same as with DiffText.


__xdiff.mmfile_t f1 f2
if(__xdiff.xdl_mmfile_ptradd(&f1 sOld sOld.len 0)<0 or __xdiff.xdl_mmfile_ptradd(&f2 sDiff sDiff.len 0)<0) end ES_FAILED

__xdiff.xdemitcb_t cb cbr
cb.fa_outf=&xdiff_cb
sNew.fix(0 1); cb.priv=&sNew
cbr.fa_outf=&xdiff_cb
if(!&sRejected) &sRejected=_s
sRejected.fix(0 1); cbr.priv=&sRejected

_i=iif(flags&1 __xdiff.XDL_PATCH_REVERSE __xdiff.XDL_PATCH_NORMAL)|(flags~255)

if(__xdiff.xdl_patch(&f1 &f2 _i &cb &cbr)) end ES_FAILED
