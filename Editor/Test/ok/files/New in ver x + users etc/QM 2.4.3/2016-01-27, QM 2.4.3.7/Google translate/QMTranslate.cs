 /
function'str $text $langTo [$langFrom]

 Translates text using Google Translate.
 Returns translated text.

 text - text to translate.
 langTo - output language.
 langFrom - input language. English by default

 Author: AlexZ, http://www.quickmacros.com/forum/viewtopic.php?f=11&t=6575

str stext=text

stext.findreplace(" " "%20")

if empty(langFrom)
	langFrom="en"

str Url=F"https://translate.google.com/#{langFrom}/{langTo}/{stext}"

HtmlDoc d
d.SetOptions(2)

d.InitFromWeb(Url)
 FirefoxWait "" 1
str s=d.GetHtml

int i
str translated_text
ARRAY(str) a

if findrx(s "span class=''hps''>(.+?)</span" 0 1|4 a)>=0
	for i 0 a.len
		translated_text+F"{a[1,i]} "
	translated_text.rtrim
else translated_text= "not found"

ret translated_text
