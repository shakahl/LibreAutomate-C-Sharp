function# $remotefile str&s [flags] [inetflags] [fa] [fparam] [str&responseheaders]

 Downloads web page or other file.
 Obsolete. Use <help>Http.Get</help>.


int _fa(m_fa) _fparam(m_fparam)
SetProgressCallback(fa fparam)
int r=Get(remotefile s flags inetflags responseheaders)
SetProgressCallback(_fa _fparam)
ret r
