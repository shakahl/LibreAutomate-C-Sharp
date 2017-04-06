function%

 Gets current stream size.


long p R q w e r t y u ;;STATSTG
is.Stat(+&p 1)
ret R
err+
 
p=GetPos
LARGE_INTEGER i0
is.Seek(i0 STREAM_SEEK_END +&R)
SetPos(p)
ret R
err+ end _error
#opt nowarnings 1 ;;unused locals

 Stat is faster etc, but not impl for some streams, eg file streams on 2000.
 IStream_Size calls Stat. Missing on 2000.
