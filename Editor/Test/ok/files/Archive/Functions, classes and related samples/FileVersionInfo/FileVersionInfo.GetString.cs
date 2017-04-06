function! str&string [$name] ;;name: Comments InternalName ProductName CompanyName LegalCopyright ProductVersion FileDescription LegalTrademarks PrivateBuild FileVersion OriginalFilename SpecialBuild

 Gets a string from version-info resource.
 Returns: 1 success, 0 failed.

 string - variable that receives the string.
 name - string name. Default "FileDescription". Standard names listed above.


int i
word* lc w

if(empty(name)) name="FileDescription"

if(!VerQueryValueW(m_block L"\VarFileInfo\Translation" &lc &i) or !i) ret

_s.format("\StringFileInfo\%04x%04x\%s" lc[0] lc[1] name)
if(VerQueryValueW(m_block @_s &w &i)) goto gr
_s.format("\StringFileInfo\0409%04x\%s" lc[1] name)
if(VerQueryValueW(m_block @_s &w &i)) goto gr
ret
 gr
string.ansi(w)
ret 1
