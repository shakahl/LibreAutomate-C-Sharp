function! VS_FIXEDFILEINFO&ffi

 Gets fixed part of version-info resource.
 Returns: 1 success, 0 failed.

 ffi - variable that receives it. Its members will contain version number etc.


if(!m_block.len) end ERR_INIT
VS_FIXEDFILEINFO* _ffi
if(!VerQueryValueW(m_block L"\" &_ffi &_i) or _i!=sizeof(VS_FIXEDFILEINFO)) ret
ffi=*_ffi
ret 1
