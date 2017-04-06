#ret
def XDF_NEED_MINIMAL 0x00000002
def XDL_BDOP_CPY 2
def XDL_BDOP_INS 1
def XDL_BDOP_INSB 3
def XDL_MMB_READONLY 0x00000001
def XDL_MMF_ATOMIC 0x00000001
def XDL_PATCH_IGNOREBSPACE 0x00000100
def XDL_PATCH_MODEMASK 0x000000FF
def XDL_PATCH_NORMAL 0x0000002D
def XDL_PATCH_REVERSE 0x0000002B
type bdiffparam_t bsize
type memallocator_t !*priv fa_malloc fa_free fa_realloc
type mmblock_t mmblock_t*next flags size bsize $ptr
type mmbuffer_t $ptr size
type mmfile_t flags mmblock_t*head mmblock_t*tail bsize fsize rpos mmblock_t*rcur mmblock_t*wcur
type xdemitcb_t !*priv fa_outf
type xdemitconf_t ctxlen
dll "$qm$\xdiff" #xdl_bdiff mmfile_t*mmf1 mmfile_t*mmf2 bdiffparam_t*bdp xdemitcb_t*ecb
dll "$qm$\xdiff" #xdl_bdiff_mb mmbuffer_t*mmb1 mmbuffer_t*mmb2 bdiffparam_t*bdp xdemitcb_t*ecb
dll "$qm$\xdiff" #xdl_bdiff_tgsize mmfile_t*mmfp
dll "$qm$\xdiff" #xdl_bpatch mmfile_t*mmf mmfile_t*mmfp xdemitcb_t*ecb
dll "$qm$\xdiff" #xdl_bpatch_multi mmbuffer_t*base mmbuffer_t*mbpch n xdemitcb_t*ecb
dll "$qm$\xdiff" #xdl_copy_mmfile mmfile_t*mmf size xdemitcb_t*ecb
dll "$qm$\xdiff" #xdl_diff mmfile_t*mf1 mmfile_t*mf2 xpparam_t*xpp xdemitconf_t*xecfg xdemitcb_t*ecb
dll "$qm$\xdiff" xdl_free !*ptr
dll "$qm$\xdiff" xdl_free_mmfile mmfile_t*mmf
dll "$qm$\xdiff" #xdl_init_mmfile mmfile_t*mmf bsize flags
dll "$qm$\xdiff" !*xdl_malloc size
dll "$qm$\xdiff" #xdl_merge3 mmfile_t*mmfo mmfile_t*mmf1 mmfile_t*mmf2 xdemitcb_t*ecb xdemitcb_t*rjecb
dll "$qm$\xdiff" #xdl_mmfile_cmp mmfile_t*mmf1 mmfile_t*mmf2
dll "$qm$\xdiff" #xdl_mmfile_compact mmfile_t*mmfo mmfile_t*mmfc bsize flags
dll "$qm$\xdiff" !*xdl_mmfile_first mmfile_t*mmf *size
dll "$qm$\xdiff" #xdl_mmfile_iscompact mmfile_t*mmf
dll "$qm$\xdiff" !*xdl_mmfile_next mmfile_t*mmf *size
dll "$qm$\xdiff" #xdl_mmfile_ptradd mmfile_t*mmf $ptr size flags
dll "$qm$\xdiff" #xdl_mmfile_size mmfile_t*mmf
dll "$qm$\xdiff" !*xdl_mmfile_writeallocate mmfile_t*mmf size
dll "$qm$\xdiff" #xdl_patch mmfile_t*mf mmfile_t*mfp mode xdemitcb_t*ecb xdemitcb_t*rjecb
dll "$qm$\xdiff" #xdl_rabdiff mmfile_t*mmf1 mmfile_t*mmf2 xdemitcb_t*ecb
dll "$qm$\xdiff" #xdl_rabdiff_mb mmbuffer_t*mmb1 mmbuffer_t*mmb2 xdemitcb_t*ecb
dll "$qm$\xdiff" #xdl_read_mmfile mmfile_t*mmf !*data size
dll "$qm$\xdiff" !*xdl_realloc !*ptr size
dll "$qm$\xdiff" #xdl_seek_mmfile mmfile_t*mmf off
dll "$qm$\xdiff" #xdl_set_allocator memallocator_t*malt
dll "$qm$\xdiff" #xdl_write_mmfile mmfile_t*mmf !*data size
dll "$qm$\xdiff" #xdl_writem_mmfile mmfile_t*mmf mmbuffer_t*mb nbuf
type xpparam_t flags
