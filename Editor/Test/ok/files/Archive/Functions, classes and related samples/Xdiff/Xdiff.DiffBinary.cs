function str&sOld str&sNew str&sDiff [flags] ;;flags: 1 Rabin's algorithm

 Makes binary diff of 2 strings.

 sOld - variable containing data that is considered "old".
 sNew - variable containing data that is considered "new".
 sDiff - receives differences (patch).
 flags:
   1 - use the Rabin's polynomial fingerprinting algorithm. This algorithm typically performs faster and produces smaller diff.

 The variables can contain text or binary data.


__xdiff.mmbuffer_t b1 b2
b1.ptr=sOld; b1.size=sOld.len
b2.ptr=sNew; b2.size=sNew.len

__xdiff.xdemitcb_t cb
cb.fa_outf=&xdiff_cb
sDiff.fix(0 1); cb.priv=&sDiff

int r
if(flags&1) r=__xdiff.xdl_rabdiff_mb(&b1 &b2 &cb)
else _i=32; r=__xdiff.xdl_bdiff_mb(&b1 &b2 +&_i &cb)

if(r) end ES_FAILED
