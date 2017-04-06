 This does not work.

 create a test page and open in new IE
str testPage.expandpath("$temp$\test.html")
str html=
 <html><head>
 <script type="text/javascript">
 function OnTimer()
 {
 //alert("timer");
 var d=new Date();
 var t=d.toLocaleTimeString();
 document.getElementById("notifyQM").value=t;
 //var ei=document.getElementById("notifyQM")
 //ei.value=t;
 //ei.onchange();
 }
 function OnLoad()
 {
 //alert("loaded");
 setInterval(OnTimer, 1000);
 }
 </script>
 </head>
 <body onload="OnLoad()">
 <input type="hidden" id="notifyQM" />
 <p>Text.</p>
 </body></html>
html.setfile(testPage)
SHDocVw.WebBrowser b=web(testPage 9)

 get the HTML element and set events for onpropertychange
MSHTML.HTMLInputElement ei=b.Document.getElementById("notifyQM")
ei._setevents("ei_HTMLInputTextElementEvents")

 wait until IE closed
int hwnd=b.HWND
opt waitmsg 1
wait 0 -WC hwnd
