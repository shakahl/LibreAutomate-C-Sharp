\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "7 9 4 3"
str cb7fro cb9to e4tex ax3SHD
cb7fro="<auto>[]&en[]lt"
cb9to="&lt[]en"

if(!ShowDialog("dlg_google_translate" &dlg_google_translate &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 282 189 "Translate"
 5 Button 0x54032000 0x0 0 0 48 14 "Translate"
 6 Static 0x54000000 0x0 58 2 20 10 "From"
 7 ComboBox 0x54230242 0x0 80 0 88 213 "fro"
 8 Static 0x54000000 0x0 178 2 14 10 "To"
 9 ComboBox 0x54230242 0x0 194 0 88 213 "to"
 4 Edit 0x54231044 0x200 0 16 282 78 "tex"
 3 ActiveX 0x54030000 0x0 0 98 282 78 "SHDocVw.WebBrowser"
 END DIALOG
 DIALOG EDITOR: "" 0x2030205 "*" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 5 goto translate
ret 1

 translate
lpstr html=
 <html>
   <head>
     <script type="text/javascript" src="http://www.google.com/jsapi">
     </script>
     <script type="text/javascript">
 
     google.load("language", "1");
 
     function Translate() {
       var text = "%s";
       google.language.detect(text, function(result) {
         if (!result.error && result.language) {
           google.language.translate(text, result.language, "%s",
                                     function(result) {
             var translated = document.getElementById("translation");
             if (result.translation) {
               translated.innerHTML = result.translation;
             }
           });
         }
       });
     }
     google.setOnLoadCallback(Translate);
 
     </script>
   </head>
   <body>
     <div id="translation"></div>
   </body>
 </html>

str langFrom langTo text

 langFrom=
langTo.getwintext(id(9 hDlg))
text.getwintext(id(4 hDlg))

HtmlToWebBrowserControl id(3 hDlg) _s.format(html text langTo)
