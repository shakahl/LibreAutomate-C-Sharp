function$ [codepage]

 Gets charset (character set) string for current or specified ANSI code page.
 Returns self.
 Error if fails.

 codepage - code page identifier. If 0, uses code page of current user (GetACP()).

 EXAMPLE
 out _s.GetCharset


if(!codepage) codepage=GetACP
_s.from("MIME\Database\Codepage\" codepage)
if(!rget(this "WebCharset" _s HKEY_CLASSES_ROOT) and !rget(this "BodyCharset" _s HKEY_CLASSES_ROOT)) end "failed"
ret this
