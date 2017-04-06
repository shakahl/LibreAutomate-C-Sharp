function! $_file

 Opens file, finds and loads version resource.
 Returns: 1 success, 0 failed (probably the file does not have a version resource).

 _file - the file. If not full path, uses standard dll search paths.


BSTR f=_s.expandpath(_file)
int vis=GetFileVersionInfoSizeW(f &_i); if(!vis) ret
if(!GetFileVersionInfoW(f 0 vis m_block.all(vis 2))) m_block.all; ret
ret 1
