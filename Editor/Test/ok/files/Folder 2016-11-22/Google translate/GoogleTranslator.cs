 /
function'str str'sourceText str'targetLang [str'sourceLang]

 Does not support Unicode input.


lpstr PatternLanguageCode="^(af|ga|sq|it|ar|ja|az|kn|eu|ko|bn|la|be|lv|bg|lt|ca|mk|zh\-CN|ms|zh\-TW|mt|hr|no|cs|fa|da|pl|nl|pt|en|ro|eo|ru|et|sr|tl|sk|fi|sl|fr|es|gl|sw|ka|sv|de|ta|el|te|gu|th|ht|tr|iw|uk|hi|ur|hu|vi|is|cy|id|yi)$"

if findrx(targetLang PatternLanguageCode)<0
	end F"targetLang: '{targetLang}' Language Code not supported"

if empty(sourceLang)
	sourceLang="auto"

if sourceLang<>"auto" and findrx(sourceLang PatternLanguageCode)<0
	end F"sourceLang: '{sourceLang}' Language Code not supported"

sourceText.escape(11)

str s

IntGetFile F"https://translate.googleapis.com/translate_a/single?client=gtx&sl={sourceLang}&tl={targetLang}&dt=t&q={sourceText}" s

int i=find(s "]],,''")
if(i>0) s.get(s 2 i) ;;phrase, else word

s.findreplace("\''" "[1]")
s.replacerx("\[''([^'']+)'',''[^'']+'',,,\d\](,|\]$)" "$1")
s.findreplace("[1]" "''")
s.findreplace("\r" "[13]")
s.findreplace("\n" "[10]")
s.findreplace("\t" "[9]")
s.findreplace("\\" "\")
s.findreplace("\u003c" "<")
s.findreplace("\u003e" ">")
if(!_unicode) s.ConvertEncoding(CP_UTF8 CP_ACP)

ret s
