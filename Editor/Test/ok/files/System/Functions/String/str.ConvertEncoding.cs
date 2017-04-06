function$ `cpFrom `cpTo

 Converts encoding of Unicode characters in text of this variable.
 Returns: self (QM 2.4.2).

 cpFrom - code page identifier of current encoding of the text.
   If -1 or _unicode, uses current QM code page (CP_UTF8 in Unicode mode, else CP_ACP).
   QM 2.4.2. Can be charset name (string). Error if invalid or unknown.
   QM 2.4.2. To auto-detect from text of this variable, use "<detect>" (unreliable) or "<html>" (use if text is HTML).
 cpTo - code page identifier to encode to.
   If -1 or _unicode, uses current QM code page.
   QM 2.4.2. Can be charset name (string). Error if invalid or unknown.

 REMARKS
 <google "site:microsoft.com code page identifiers">Code page identifiers and charset names</google>.
 Charset name is ISO- or IANA-defined character set name. For example, used in HTML <meta ... charset=...>.

 Added in: QM 2.3.1.

 EXAMPLES
  convert from current QM encoding to iso-8859-1
 s.ConvertEncoding(_unicode 28591)
  or
 s.ConvertEncoding(_unicode "iso-8859-1")

  download web page and convert to current QM encoding
 str html
 IntGetFile "http://www.example.com" html
 html.ConvertEncoding("<html>" -1)
 out html


opt noerrorshere 1
if(!this.len) ret

int cp1 cp2
sel cpFrom.vt
	case VT_BSTR cp1=sub.CharsetToCP(cpFrom this this.len)
	case else cp1=cpFrom; if(cp1<0) cp1=_unicode
sel cpTo.vt
	case VT_BSTR cp2=sub.CharsetToCP(cpTo)
	case else cp2=cpTo; if(cp2<0) cp2=_unicode

if cp2!cp1
	this.unicode(this cp1)
	this.ansi(this cp2)
ret this


#sub CharsetToCP
function# `&v [$sToDetect] [sLen]

type ___MIMECSETINFO uiCodePage uiInternetEncoding @wszCharset[50]
type ___DetectEncodingInfo nLangID nCodePage nDocPercent nConfidence
interface ___IMultiLanguage2 :IUnknown _ _ _ _ #GetCharsetInfo(BSTR'Charset ___MIMECSETINFO*pCharsetInfo) _ _ _ _ _ _ _ _ _ _ _ _ _ _ #DetectInputCodepage(dwFlag dwPrefWinCodePage !*pSrcStr *pcSrcSize ___DetectEncodingInfo*lpEncoding *pnScores) {DCCFC164-2B38-11d2-B7EC-00C04F8F5D9A}
def ___CLSID_CMultiLanguage uuidof("{275c23e2-3747-11d0-9fea-00aa003f8646}")

BSTR b=v.bstrVal
___IMultiLanguage2-- m._create(___CLSID_CMultiLanguage)
if sLen and !(StrCmpCW(b L"<html>") and StrCmpCW(b L"<detect>"))
	___DetectEncodingInfo d; _i=1
	if(!m.DetectInputCodepage(iif(b[1]='h' 8 0) 0 sToDetect &sLen &d &_i)) ret d.nCodePage ;;MLDETECTCP_HTML=8
	end F"{ERR_FAILED} to detect encoding"
else
	___MIMECSETINFO ci
	if(!m.GetCharsetInfo(b ci)) ret ci.uiCodePage

err+
end F"{ERR_FAILED} to convert charset name to code page identifier"

 tested: getting from registry much slower and not easy.
