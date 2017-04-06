 /Macro1407
function'str $text $langTo [$langFrom]

 Translates text using Google Translate.
 Returns translated text, or empty string if failed.
 Timeout 5 s.

 text - text to translate.
 langTo - output language.
 langFrom - input language. If omitted or "", autodetects.

 Languages are like "en", "es". <link "http://code.google.com/apis/ajaxlanguage/documentation/reference.html#LangNameArray">Reference</link>.


if(empty(text) or empty(langTo)) ret

opt waitmsg 1
int h=ShowDialog("" &dlg_google_translate 0 0 1)

lpstr html=
 <html>
   <head>
     <script type="text/javascript" src="http://www.google.com/jsapi">
     </script>
     <script type="text/javascript">
 
     google.load("language", "1");
 
     function Translate() {
       google.language.translate("%s", "%s", "%s", function(result) {
         document.getElementById("translation").innerHTML = result.error ? "-error-" : result.translation;
       });
     }
     google.setOnLoadCallback(Translate);
 
     </script>
   </head>
   <body>
     <div id="translation"></div>
   </body>
 </html>

str t=text
t.findreplace("''" "&quot;")

str s.format(html t langFrom langTo)
HtmlToWebBrowserControl id(3 h) s
rep 500
	0.02
	Htm el=htm("DIV" "translation" "" h 0 0 0x101); if(!el) ret
	s=el.Text
	if s.len
		DestroyWindow h
		if(s="-error-") ret ""
		ret s
DestroyWindow h

 BEGIN DIALOG
 0 "" 0x800000C8 0x0 0 0 285 142 "Translate"
 3 ActiveX 0x54030000 0x0 0 0 284 140 "SHDocVw.WebBrowser"
 END DIALOG
