function $_file grfMode ;;grfMode: STGM_READ, STGM_WRITE, STGM_READWRITE and other STGM_ flags, documented in MSDN, see "STGM Constants".

 Creates stream on file.
 More info: <google>SHCreateStreamOnFile</google>.


is=0
int hr=SHCreateStreamOnFileW(@_s.expandpath(_file) grfMode &is)
if(hr) end "" 16 hr
