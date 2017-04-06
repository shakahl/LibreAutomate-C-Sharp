 /
function'str $text $langTo [$langFrom] [flags] [str&fullResponse] ;;flags: 1 text is HTML

 Obsolete. Google does not have this service now.

 Translates text using Google Translate.
 Returns translated text.
 If fails, returns empty string. fullResponse may contain error description.

 text - text to translate. Text length is limited to ~5100.
 langTo - output language.
 langFrom - input language. If omitted or "", autodetects.
 fullResponse - if used, receives full response in JSON format.

 Languages are like "en", "es". <link "http://code.google.com/apis/ajaxlanguage/documentation/reference.html#LangNameArray">Reference</link>.
 <link "http://code.google.com/apis/ajaxlanguage/documentation/reference.html#_intro_fonje">More info</link>.
 QM should run in Unicode mode. If not, some characters in result may be incorect, or some words not translated.

 EXAMPLE
 str t r
 t=GoogleTranslate("test translation" "lt" "" 0 r)
 if(t.len) out t
 else out r


ARRAY(POSTFIELD) a.create(5)
a[0].name="v"; a[0].value="1.0"
a[1].name="userip"; GetIpAddress "" a[1].value
a[2].name="langpair"; a[2].value.from(langFrom "|" langTo)
a[3].name="format"; a[3].value=iif(flags&1 "html" "text")
a[4].name="q"; a[4].value=text
if(!_unicode) a[4].value.ConvertEncoding(CP_ACP CP_UTF8)

str s r
Http h.Connect("ajax.googleapis.com")
h.PostFormData("ajax/services/language/translate" a s "Referer: http:\\www.quickmacros.com\[]")
err+ s=_error.description
 out s
if(&fullResponse) fullResponse=s

if(findrx(s "''translatedText'' *: *''(.+?[^\\])''" 0 1 r 1)<0) ret
if(!_unicode) r.ConvertEncoding(CP_UTF8 CP_ACP)

if findc(r '\')>=0
	r.findreplace("\r" "[13]")
	r.findreplace("\n" "[10]")
	r.findreplace("\''" "''")
	r.findreplace("\\" "\")
	r.findreplace("\/" "/")
	r.findreplace("\u0026" "&")
	r.findreplace("\u003c" "<")
	r.findreplace("\u003d" "=")
	r.findreplace("\u003e" ">")

ret r
