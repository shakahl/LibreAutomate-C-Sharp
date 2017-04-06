__xdiff.memallocator_t+ ___xdiff_ma ;;must be +
if(___xdiff_ma.fa_free) ret
___xdiff_ma.fa_malloc=&xdiff_malloc
___xdiff_ma.fa_realloc=&xdiff_realloc
___xdiff_ma.fa_free=&xdiff_free
__xdiff.xdl_set_allocator(&___xdiff_ma)
