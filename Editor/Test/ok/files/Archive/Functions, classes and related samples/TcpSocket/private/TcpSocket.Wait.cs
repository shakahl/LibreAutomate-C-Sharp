function! !read timeout

fd_set f; fd_set* r w; timeval* tp

f.fd_count=1; f.fd_array[0]=m_socket; if(read) r=&f; else w=&f

if(timeout) timeval t.tv_sec=timeout/1000; t.tv_usec=timeout%1000*1000; tp=&t

_i=sock_select(0 r w 0 tp)
if(_i<0) E
ret _i>0
