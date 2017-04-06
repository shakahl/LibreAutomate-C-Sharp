OsdHide

spe 10
act win(mouse)
str+ g_mfst ;;shared with macro "Menu from selected text" and other similar macros. Because these macros are often used together.
str s
s.getsel; s.trim
if(!s.len or s=g_mfst)
	dou
	0.1
	s.getsel; s.trim
	if(!s.len) inp- s "word" "Translate"
g_mfst=s

if(!TranslateEnglish(s s)) ;;often error
	 s=GoogleTranslate(s "lt" "en") ;;does not work anymore
	s+" *"
OnScreenDisplay s 0 xm-50 ym+50 0 0 0 5|16
ret

if(findc(g_mfst " ")>0) ret ;;if more than one word, don't speak etc

 if(!IsCommonEnglishWord(g_mfst _i)) ;;does not work anymore
	 _i/1000000
	 if(_i<10) OnScreenDisplay F"{_i} millions in Google" 0 xm-50 ym+100 0 10 0xff 5|16

Speak g_mfst
