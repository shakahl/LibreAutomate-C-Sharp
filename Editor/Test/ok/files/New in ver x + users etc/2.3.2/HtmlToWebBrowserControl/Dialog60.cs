\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str ax3SHD
if(!ShowDialog("Dialog60" &Dialog60 &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 529 306 "Dialog"
 3 ActiveX 0x54030000 0x0 0 0 530 304 "SHDocVw.WebBrowser"
 END DIALOG
 DIALOG EDITOR: "" 0x2030208 "*" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	goto g1
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
 g1
 parameters
 str url="http://rapidshare.com/files/342182548/Cambridge_Advanced_Learners_Dictionary.part4.rar"
str url="http://www.google.lt"
str save_folder="$desktop$"

 -------------------

out

str shtml
 IntGetFile url shtml
 out shtml
 shtml="<html><head></head><body><p>test</p></body></html>"
 HtmlToWebBrowserControl3 id(3 hDlg) shtml

 shtml="<html><head></head><body>"
 str b
 b.all(5000 2 'p')
  rep(600) b+"<p>i</p>"
 shtml+"<b>bold</b>"
 shtml+b
 shtml+"</body></html>"

 IntGetFile url shtml




shtml=
 <!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "DTD/xhtml1-transitional.dtd">
 <html>
  <head>
  <script type="text/javascript">
  if (top != self)   top.location = self.location;
  if (document.URL.substr(7,7) == "intern.")
     document.write('<base href="http://rapidshare.com">');
  </script>
  <title>RapidShare: 1-CLICK Web hosting - Easy Filehosting</title>
  <link rel="icon" href="/img2/favicon.ico" type="image/ico" />
 <link rel="SHORTCUT ICON" href="/img2/favicon.ico" />
 <meta http-equiv="Content-Type" content="text/html;charset=UTF-8" />
 <meta name="author" content="Saso Nikolov" />
 <meta name="keywords" content="File Hosting, File Distributor, File Sharing, upload several files" />
 <meta name="description" content="1-Click web hosting, upload several files, Easy File Distribution, Easy, Fast and Secure" />
  <link rel="stylesheet" type="text/css" href="/img2/styles.css" />
  </head>
 
 <body>
 <script type="text/javascript">
 function einblenden(p) {
   for(var i=0;i<p.childNodes.length;i++) {
    if(p.childNodes[i].nodeName.toLowerCase()=="div")
     p.childNodes[i].style.display="block";
   }
 }
 function ausblenden(p) {
   for(var i=0;i<p.childNodes.length;i++) {
    if(p.childNodes[i].nodeName.toLowerCase()=="div")
     p.childNodes[i].style.display="none";
   }
 }
 </script>
 <center>
 <div style="padding-top:10px;padding-bottom:20px;">
 <table><tr><td><div class="navigation"><ul>
         <li><a href="http://rapidshare.com/index.html">Home</a></li>
         <li><a href="http://rapidshare.com/news.html">News</a></li>
 <!--
         <li><a href="http://rapidshare.com/rewards.html">Rewards</a></li>
 -->
         <li onmouseover="einblenden(this);" onmouseout="ausblenden(this);">
             <div class="subnavi">
             <ul>
                 <li><a href="https://ssl.rapidshare.com/premzone.html">Premium Zone Login</a></li>
                 <li><a href="http://rapidshare.com/premium.html">Create Account</a></li>
                 <li><a href="http://rapidshare.com/forgotpw.html">Forgot Password</a></li>
             </ul>
             </div>
             <a href="https://ssl.rapidshare.com/premzone.html">Premium Zone</a>
         </li>
         <li><a href="http://tainment.rapidshare.com/" target="_blank">RapidTainment</a></li>
         <li onmouseover="einblenden(this);" onmouseout="ausblenden(this);">
             <div class="subnavi">
             <ul>
                 <li><a href="http://rapidshare.com/rsm.html">RapidShare Manager</a></li>
                 <li><a href="http://rapidshare.com/rapiduploader.html">RapidUploader</a></li>
                 <li><a href="http://rapidshare.com/checkfiles.html">RapidShare Checker</a></li>
                 <li style="border:0px;"><a href="http://rapidshare.com/dev.html">API</a></li>
             </ul>
             </div>
             <a href="http://rapidshare.com/rapidtools.html">RapidTools</a>
         </li>
         <li onmouseover="einblenden(this);" onmouseout="ausblenden(this);">
             <div class="subnavi">
             <ul>
                 <li><a href="http://rapidshare.com/faqx.html">FAQ</a></li>
                 <li><a href="http://rapidshare.com/support.html">Support Contact</a></li>
                 <li><a href="http://rapidshare.com/abuse.html">Abuse Contact</a></li>
                 <li style="border:0px;"><a href="http://rapidshare.com/security.html">Security advice</a></li>
             </ul>
             </div>
             <a href="http://rapidshare.com/supportseite.html">Support</a>
         </li>
         <li onmouseover="einblenden(this);" onmouseout="ausblenden(this);">
             <div class="subnavi">
             <ul>
                 <li><a href="http://rapidshare.com/wiruberuns.html">About us</a></li>
                 <li><a href="http://rapidshare.com/jobs.html">Jobs</a></li>
                 <li><a href="http://rapidshare.com/testimonials.html">Testimonials</a></li>
                 <li><a href="http://rapidshare.com/banners.html">Banner</a></li>
                 <li><a href="http://rapidshare.com/agb.html">Conditions of use</a></li>
                 <li style="border:0px;"><a href="http://rapidshare.com/imprint.html">Imprint</a></li>
             </ul>
             </div>
             <a href="http://rapidshare.com/rapidshare.html">RapidShare AG</a>
         </li>
         <li style="border:0px;"><a href="http://rapidshare.com/privacypolicy.html">Privacy Policy</a></li>
     </ul></div></td></tr></table>
 </div>
 <a href="http://rapidshare.com">
 <script type="text/javascript">
 var ld=new Date();
 if(ld.getMonth()==11&&ld.getDate()>=21&&ld.getDate()<28)
     document.write('<img src="/img2/rslogo_weihnachten.gif" width="252" height="180" alt="logo" />');
 else
 if(ld.getMonth()==11&&ld.getDate()>=28&&ld.getDate()<=31)
     document.write('<img src="/img2/rslogo_silvester.gif" width="300" height="214" alt="logo" />');
 else
 if(ld.getMonth()==0&&ld.getDate()<=3)
     document.write('<img src="/img2/rslogo_silvester.gif" width="300" height="214" alt="logo" />');
 else
     document.write('<img src="/img2/rslogo.gif" width="252" height="180" alt="logo" />');
 </script>
 </a>
 <noscript><h1>This page needs JavaScript, to display all information correct!</h1></noscript>
 
 <div id="inhaltbox">
 <script type="text/javascript">
 <!--
 function anzeigen(name, auf) {
     // bei auf = true, bleibt es offen
     if (document.getElementById("p" + name)) {
         var elem = document.getElementById("p" + name);
         if (elem.style.display == "" && !auf) {
             elem.style.display = "none";
         } else {
             elem.style.display = "";
         }
         var elem2 = document.getElementById("pb" + name);
         if (elem2) {
             if (elem.style.display == "") {
                 elem2.src = "/img2/pfeil_auf.jpg";
             } else {
                 elem2.src = "/img2/pfeil_zu.jpg";
             }
         }
     }
 }
 
 //-->
 </script>
 <h1>FILE DOWNLOAD</h1>
 <div class="klappbox">
     <p class="downloadlink">http://rapidshare.com/files/342182548/Cambridge_Advanced_Learners_Dictionary.part4.rar <font style="color:#8E908F;">| 51119 KB</font></p>
     <center>
         <table>
             <tr valign="top">
                 <td width="300" style="text-align:center;">
                     <form action="http://rs716.rapidshare.com/files/342182548/Cambridge_Advanced_Learners_Dictionary.part4.rar" method="post">
                         <input type="hidden" name="dl.start" value="PREMIUM" />
                         <img src="/img2/dl_schnell.gif" />
                         <br />
                         <input type="submit" value="Premium user" />
                     </form>
                 </td>
                 <td width="300" style="text-align:center;">
                     <form id="ff" action="http://rs716.rapidshare.com/files/342182548/Cambridge_Advanced_Learners_Dictionary.part4.rar" method="post">
                         <input type="hidden" name="dl.start" value="Free" />
                         <img src="/img2/dl_langsam.gif">
                         <br />
                         <input type="submit" value="Free user" />
                     </form>
                     <script type="text/javascript">
                     <!--
                     if (window.location.hash == "#dlt")
                         document.getElementById("ff").action += "#dlt";
                     //-->
                     </script>
                 </td>
             </tr>
         </table>
     </center>    
     <p>Do you want to send your files with ease and speed? You can find out how to do that <a href="http://rapidshare.com/howto_upload.html" target="_blank">here</a>.</p>
     
     <script type="text/javascript">
 <!--
         if(typeof(system) == "undefined")
             var system = new Object();
         document.domain = 'rapidshare.com';        
         function RSAjaxBridge(obj) {
             if (typeof(system.RSAjaxBridgeOut) == "undefined")
                 system.RSAjaxBridgeOut =   {};   
             if (!document.getElementById('RSAjaxBridge_frames')) {
                 var framebehaelter = document.createElement("div");
                 framebehaelter.id = "RSAjaxBridge_frames";
                 framebehaelter.style.display="none";
                 document.getElementsByTagName("body")[0].appendChild(framebehaelter);
             }
             var bridgepath = "/img2/RSAjaxBridge.html";
             if (typeof(obj.bridgepath) != "undefined")
                 bridgepath = obj.bridgepath;                                       
                         
           var url = obj.url;
           var prot ="";        
           // URL-Daten aufbereiten
           if(url.substr(0,4)=="http") {
             prot = url.substr(0,url.indexOf('//'));
             url=url.substr(url.indexOf('//')+2); // abschneiden
           }
           var svrid = RSAjaxBridgeErstelleSrvID(url);
 
             if (prot == "")
                 prot = location.protocol;
         
           var file = url.substr(url.indexOf("/"));
           url = url.substr(0,url.indexOf("/"));
           obj.file = file;
           obj.svrid= svrid;
         
           // Anfrage immer in Queue stecken
           if(!system.RSAjaxBridgeOut[svrid])
             system.RSAjaxBridgeOut[svrid] = {loading:false, queue: new Array()};
           system.RSAjaxBridgeOut[svrid].queue.push(obj);
         
           // IFrameErstellung
           if(!document.getElementById(svrid)) {
             system.RSAjaxBridgeOut[svrid].loading = true;
             var iframe  = document.createElement('iframe');
             iframe.id   = svrid;
             iframe.src  = prot+"//"+url+bridgepath; // geht auch https?
             document.getElementById('RSAjaxBridge_frames').appendChild(iframe);
           } else RSAjaxBridgeProcessCalls(document.getElementById(svrid).contentWindow,obj);
         }
         //  HTML5-postMessage-Event
         function RSAjaxBridgeAntwort(e) {
           // origin prüfen
           var url = "";
           if (e.uri || e.origin) {
             url = ((e.uri)?e.uri:e.origin);
             if (url.substr(0,4)=="http")
                 url=url.substr(url.indexOf('//')+2); // abschneiden
             if (url.indexOf("/")>0)
                 url = url.substr(0,url.indexOf("/"));
           } else {
             url = e.source.location.hostname;
           }
           if(url.substr(url.length-14)!="rapidshare.com") return;
 
           // URL-Daten aufbereiten
           var svrid = RSAjaxBridgeErstelleSrvID(url);
 
           // Nachricht über Initialisierung?
           var p = system.RSAjaxBridgeOut[svrid].queue;
           if(e.data=="RSAjaxBridge_Ready") {
             system.RSAjaxBridgeOut[svrid].loading = false;
           } else {
             // Diese Daten der callbackfunktion übergeben
             if(p && p.length>0 && p[0].onSuccess)   {
                 p[0].onSuccess({"responseText": e.data.substr(13)});
             }
             system.RSAjaxBridgeOut[svrid].queue.shift();
             system.RSAjaxBridgeOut[svrid].loading = false;
           }
           // Ausstehende Calls verarbeiten:
           if(p && p.length>0 && !system.RSAjaxBridgeOut.loading) {
             RSAjaxBridgeProcessCalls(e.source,p[0]);
           }
         }
         function RSAjaxBridgeProcessCalls(hWin, obj) {
             if(system.RSAjaxBridgeOut[obj.svrid].loading)
                 return false;   // solange er noch lädt keinen neuen request starten
             system.RSAjaxBridgeOut[obj.svrid].loading = true;
             //  Ich muss GET/POST berücksichtigen sowie query!
             var szMessage = ((obj.method)?obj.method:"")+"\n"+obj.file+"\n"+escape((obj.str)?obj.str:"");
             var target = null;
             try{var target = hWin.postMessage ? hWin : (hWin.document.postMessage ? hWin.document : null)} catch(e) {}
             if (target)
                 target.postMessage(szMessage,"*");
             else
                 hWin.RSAjaxBridgeEmpfang({"data": szMessage, "origin": "http://"+window.location.hostname, "source": window});
         }
         
         if (window.addEventListener)
           window.addEventListener('message', RSAjaxBridgeAntwort, false);
         else if (window.attachEvent)
           window.attachEvent('onmessage', function() { RSAjaxBridgeAntwort(window.event) });
         
         function RSAjaxBridgeErstelleSrvID(url)
         {
           var svrid = "RSAjaxBridge_frame_";
           if(url.indexOf(".")<url.indexOf("rapidshare.com"))
             svrid += url.split(".")[0];
             return svrid;
         }
 //-->
 </script>
 
 
 <script type="text/javascript">
 <!-- 
 function trim(text) {
     if (!text)
         return "";
     return text.replace(/^\s+|\s+$/g, '');
 }
 function floatval(text, maxwert, minwert) {
     var zahl = parseFloat(text);
     if (isNaN(zahl))
         zahl = 0.0;
     if (typeof(maxwert) != "undefined" && maxwert < zahl)
         zahl = maxwert;
     if (typeof(minwert) != "undefined" && minwert > zahl)
         zahl = minwert;
     return zahl;
 }
 function intval(text, maxwert, minwert) {
     var zahl = parseInt(text, 10);
     if (isNaN(zahl))
         zahl = 0;
     if (typeof(maxwert) != "undefined" && maxwert < zahl)
         zahl = maxwert;
     if (typeof(minwert) != "undefined" && minwert > zahl)
         zahl = minwert;
     return zahl;
 }
 function number_format(zahl) {
     return Math.floor((zahl*100)+.5)/100
 }
 function isset(variable) {
     if (typeof(variable) == "undefined")
         return false;
     return true;
 }
 //-->
 </script>
 <script type="text/javascript">
 var apiurl = "api.rapidshare.com/cgi-bin/rsapi.cgi?";
 $ = function(id) {return document.getElementById(id);}
 function ajaxen(obj) {
     var text = obj.url;
     if (obj.str)
         text += "&" + obj.str;
     RSAjaxBridge(obj);
 }
 function checkResponse(text, keinealert) {
     if (text.substr(0, 7) == "ERROR: ") {
         if (keinealert) {
         } else {
             alert(text);
         }
         return false;
     }
     return true;
 }
 
 //  System initialisieren
 if (typeof(system) == "undefined")
     system = new Object();
 //  Defaults
 system["package"]   =   0;
 system["packages"]  =   {};
 system["packages"][1]   =   {"rapids": 400,     "euro": 4.99};
 system["packages"][2]   =   {"rapids": 1000,    "euro": 9.95};
 system["packages"][3]   =   {"rapids": 2000,    "euro": 19.90};
 system["packages"][4]   =   {"rapids": 5000,    "euro": 49.75};
 system["packages"][5]   =   {"rapids": 20000,   "euro": 199.00};
 
 system["onload"]    =   window.onload;
 window.onload       =   function() {
     if(system["onload"])    system["onload"]();
     vxGoStep(1);
     system["COOKIE"] = new Object();
     system["COOKIE"]["vorhanden"] = false;
     if (navigator.cookieEnabled && document.cookie) {
         var meinCookie = new Object();
         var teile = document.cookie.split(";");
         for (var i = 0; i < teile.length; i++) {
             var teil = teile[i].split("=");
             var key = trim(teil[0]);
             var value = trim(teil[1]);
             meinCookie[key] = value;
         }
         if (meinCookie["enc"] && meinCookie["enc"] != "") {
             system["COOKIE"] = meinCookie;
             system["COOKIE"]["vorhanden"] = true;
         }
     }
 }
 
 function    vxChoosePaket(nr) {
     system["package"]   =   nr;
     vxGoStep(2);
 }
 
 function    vxGoStep(nr, hash) {
     $("pp2_step1div").style.display="none";
     $("pp2_step2div").style.display="none";
     $("pp2_step3div").style.display="none";
     $("pp2_step3div_psc").style.display="none";
     $("pp2_step3div_wire").style.display="none";
     $("pp2_step6div").style.display="none";
     $("pp2_step6div_redeemed").style.display="none";
     //  classNames
     $("pp2_step1_a").className = "";
     $("pp2_step2_a").className = "";
     $("pp2_step3_a").className = "";
     $("pp2_step4_a").className = "";
 
     switch(nr) {
         case 1:
             $("pp2_step1div").style.display="";
             $("pp2_step1_a").className = "active";
             break;
         case 2:
             //  Logindaten prüfen:
             var szLoginData = "";
             if (system["COOKIE"]["vorhanden"])
                 szLoginData = "&cookie="+encodeURIComponent(system["COOKIE"]["enc"]);
 
             if (hash && hash["login"])
                 szLoginData = "&login="+encodeURIComponent(hash["login"])+"&password="+encodeURIComponent(hash["password"])+"&type=prem&withcookie=1";
 
             //  Testen ob User schon eingeloggt - dann überspringen oder Einloggseite
             if(szLoginData) {
                 ajaxen({
                     url: apiurl + "sub=getaccountdetails_v1" + szLoginData,
                     onSuccess: function(h) {
                         if (!checkResponse(h.responseText)) {
                             system["COOKIE"]["vorhanden"] = false;
                             //  Login einblenden
                             $("pp2_step2div").style.display="";
                             $("pp2_step2_a").className = "active";
                             return false;
                         }
                         system["account"] = new Object();
                         var zeilen = h.responseText.split("\n");
                         for (var a = 0; a < zeilen.length; a++) {
                             var teile = zeilen[a].split("=");
                             system["account"][teile[0]] = teile[1];
                         }
                         system["account"]["geladen"] = true;
                         if (system["account"]["cookie"]) {
                             var d = new Date();
                             d.setTime(d.getTime() + (7 * 24 * 60 * 60 * 1000));
                             document.cookie = "enc=" + system["account"]["cookie"] + "; domain=rapidshare.com; path=/; expires=" + d.toGMTString();
                             document.cookie = "enc=" + system["account"]["cookie"] + "; domain=.rapidshare.com; path=/; expires=" + d.toGMTString();
                             system["COOKIE"]["vorhanden"] = true;
                             system["COOKIE"]["enc"] = system["account"]["cookie"];
                         }
                         //  Account vorhanden - weiter Schritt 3
                         if(system["package"]!=6)
                             vxGoStep(3);
                         else
                             vxGoStep("ppc");
                     }
                 });
             } else {
                 //  Login einblenden
                 $("pp2_step2div").style.display="";
                 $("pp2_step2_a").className = "active";
             }
             break;
         case 3:
             $("pp2_step3div").style.display="";
             $("pp2_step3_a").className = "active";
             //  Werte setzen
             $("js_pp2_reseller_link").onclick = function() {
                 window.location.href='http://rapidshare.com/resellers.html?rapids='+system["packages"][system["package"]]["rapids"]+'&accountid='+system["account"]["accountid"];
             }
             $("js_pp2_paypal_link").onclick = function() {
                 window.location.href='https://ssl.rapidshare.com/cgi-bin/buy.cgi?pid='+system["package"]+'&itemname='+encodeURIComponent(system["packages"][system["package"]]["rapids"]+' Rapids for #'+system["account"]["accountid"])+'&custom=0F60752EAC47EE5C1677EE0D458C1E2588D48FB718A136CF2FFFBAC712C81813';
             };
             $("js_pbclink").onclick = function() {
                 window.location.href='http://rapidshare.com/paybycall.html?accountid='+system["account"]["accountid"]+'&custom=0F60752EAC47EE5C1677EE0D458C1E2588D48FB718A136CF2FFFBAC712C81813';
             };
 
             //  Paket 1 darf über PBC
             if (system["package"]==1) {
                 $("js_pp2_paybycall").style.display="";
                 $("js_pp2_wiretransfer").style.display="none";
             } else {
                 $("js_pp2_paybycall").style.display="none";
                 $("js_pp2_wiretransfer").style.display="";
             }
 
             break;
         case "psc":
             $("pp2_step3div_psc").style.display="";
             $("pp2_step3_a").className = "active";
             $("pp2_step3div_psc").innerHTML = '<iframe src="http://extpay.rapidshare.com/cgi-bin/psc.cgi?pid='+system["package"]+'&custom=0F60752EAC47EE5C1677EE0D458C1E2588D48FB718A136CF2FFFBAC712C81813&email='+encodeURIComponent(system["account"]["email"])+'&accid='+encodeURIComponent(system["account"]["accountid"])+'" style="border: 0pt none; height: 550px; width: 100%;"></iframe>';
             break;
         case "wire":
             $("pp2_step3div_wire").style.display= "";
             $("pp2_step3_a").className = "active";
             $("pp2_wire_form").style.display    = "";
             $("pp2_wire_comit").style.display   = "none";
             $("pp2_wire_payrapids").innerHTML   = system["packages"][system["package"]]["rapids"]
             $("pp2_wire_payamount1").innerHTML  = system["packages"][system["package"]]["euro"];
             $("pp2_wire_payamount2").innerHTML  = system["packages"][system["package"]]["euro"];
             break;
         case "ppc":
             $("pp2_step6div").style.display="";
             $("pp2_step4_a").className = "active";
             $("pp2_step6div_redeemed").style.display="none";
             break;
     }
 }
 function    vxCheckLogin(p) {
     vxGoStep(2, {"login": p.login.value, "password": p.password.value});
 }
 
 function    vxCreateAccount(p) {
     var szLogin = p.login.value;
     var szEMail = p.email.value;
     var szPass  = p.password.value;
     var szError = "";
     if (p.email_repeat.value != szEMail)
         szError += "Your repeated e-mail addresses does not match.\n";
     if (szEMail.length<8)
         szError += "Please provide a valid e-mail address.\n";
     if (szLogin.length<4)
         szError += "Please provide a valid username with at least 6 characters.\n";
     if (szPass.length<6)
         szError += "Please provide a valid password with at least 6 characters.\n";
     if (szError) {
         alert(szError);
         return;
     }
     //  Account anlegen:
     ajaxen({
         url: apiurl + "sub=newpremiumaccount_v1" +
             "&username="+encodeURIComponent(szLogin)+
             "&password="+encodeURIComponent(szPass)+
             "&email="+encodeURIComponent(szEMail),
         onSuccess: function(h) {
             if (!checkResponse(h.responseText)) {
                 //  Login einblenden
                 $("pp2_step2div").style.display="";
                 return false;
             }
             //  Hinweis anzeigen das EMail bestätigt werden muss.
             $("js_mailsent").style.display= "none";
             $("js_mailinfo").style.display= "";
         }
     });
 }
 function vxDoWireTransfer(p) {
     var szName = p.name.value;
     var szEMail = p.email.value;
     var szError = "";
     if (p.email2.value != szEMail)
         szError += "Your repeated e-mail addresses does not match.\n";
     if (szEMail.length<8)
         szError += "Please provide a valid e-mail address.\n";
     if (szName.length<4)
         szError += "Please provide your full name.\n";
     if (szError) {
         alert(szError);
         return;
     }
     //  Zahlung anlegen:
     ajaxen({
         url: apiurl + "sub=wiretransfer_v1" +
             "&email="+encodeURIComponent(szEMail)+
             "&name="+encodeURIComponent(szName)+
             "&accountid="+system["account"]["accountid"],
         onSuccess: function(h) {
             if (!checkResponse(h.responseText)) {
                 return false;
             }
             //  Hinweis anzeigen das Zahlung bestätigt werden muss.
             $("pp2_wire_form").style.display= "none";
             $("pp2_wire_comit").style.display= "";
             $("pp2_wire_subject").innerHTML = h.responseText;
         }
     });
 }
 function vxRedeemCard(p) {
     var szLogin = p.login.value;
     var szPasswd= p.password.value;
     //  Zahlung abschicken:
     ajaxen({
         url: apiurl + "sub=ppcforextension_v1&cookie="+encodeURIComponent(system["COOKIE"]["enc"]) +
             "&ppcaccountid="+encodeURIComponent(szLogin)+
             "&ppcpassword=" +encodeURIComponent(szPasswd),
         onSuccess: function(h) {
             if (!checkResponse(h.responseText)) {
                 return false;
             }
             //  Hinweis anzeigen das Zahlung bestätigt werden muss.
             $("pp2_step6div").style.display="none";
             $("pp2_step6div_redeemed").style.display="";
             $("pp2_step6div_redeemed_rapids").innerHTML = h.responseText;
         }
     });
 }
 </script>
 
 <style>
 div, p, ul, li {
     margin:0;
     padding:0;
 }
 
 #content_packages {
     width: 900px;
 }
 
 #steps {
     width:100%;
     height:100%;
     padding: 0 0 0 20px;
 }
 
 #steps ul li {
     float:left;
     list-style:none;
     margin: 0 10px 0 0;
 }
 
 #steps ul li p {
     line-height:42px;
 }
 
 #steps ul li a {
     display:block;
     width:182px;
     height:42px;
     background:url('/img2/steps_bg.png') no-repeat center;
     text-decoration:none;
     color:#333;
     text-align:center;
     line-height:42px;
     font-size:12px;
 }
 
 #steps ul li a:hover, #steps ul li a.active {
     background:url('/img2/steps_bg_hover.png') no-repeat center;
     font-weight:bold;
 }
 
 #packages {
     clear:both;
     width:100%;
     height:100%;
 }
 
 #packages h3{
     padding: 20px 20px 0 20px;
 }
 
 #packages ul li {
     float:left;
     list-style:none;
     margin: 4px;
     border:solid 1px #CCC;
     cursor:pointer;
 }
 
 #packages ul li div.package_container {
     width:114px;
     height:180px;
     background:url('/img2/package_bg.png') repeat-x  center;
     text-align:center;
     position:relative;
 }
 
 div.package_container p {
     text-align:center;
 }
 
 div.package_container input {
     text-align:center
 }
 
 #packages p.rapids_nr {
     font-weight:bold;
     font-size:30px;
     padding: 10px 0 0 0;
 }
 
 #packages p.rapids {
     font-weight:bold;
     font-size:18px;
     padding:0 0 40px 0;
 }
 
 #packages p.prize {
     font-weight:bold;
     font-size:20px;
     color:#05224e;
 }
 
 #package_overview {
     clear:both;
     padding: 30px 0 0 0;
 }
 
 #package_overview #legend_table {
     font-size:12px;
     margin:10px 0 0 0;
 }
 
 #package_overview #features_explanation {
     font-size:12px;
     margin:10px 0 0 0;
 }
 
 #features_explanation p {
     font-size:12px;
     margin: 0 0 20px 0;
 }
 #produkttabelle td, #produkttabelle th {
     border-right: 1px solid #ccc;
     border-bottom: 1px solid #ccc;
     width:10%;
 }
 #produkttabelle th {
     font-size:11px;
     font-weight:bold;
     background-color: #ccc;
     text-align:center;
 }
 </style>
 
 
 <div id="content_packages" style="padding-top:12px;">
 
     <div id="steps">
         <ul>
             <li><a id="pp2_step1_a" href="javascript:void(vxGoStep(1));">1. Select Rapids Package</a></li>
             <li><p>&raquo;</p></li>
             <li><a id="pp2_step2_a" href="javascript:void(1);">2. Login / Register</a></li>
             <li><p>&raquo;</p></li>
             <li><a id="pp2_step3_a" href="javascript:void(1);">3. Payment Options</a></li>
             <li><p>&raquo;</p></li>
             <li><a id="pp2_step4_a" href="javascript:void(1);">4. Activation</a></li>
         </ul>
     </div>
 
     <div id="pp2_step1div" style="display:none;">
         <div id="packages">
             <h3>Select Your Preferred Rapids Package:</h3>
             <ul>
                 <li onclick="vxChoosePaket(6);vxGoStep(2);">
                     <div class="package_container">
                         <div style="clear:both;position:absolute;bottom: 24px;text-align:center;left:0px;right:0px;"><button style="font-size:12px;">Redeem</button></div>
                         <p class="rapids" style="padding: 0 0 4px 0;">RapidShare Prepaidcard</p>
                         <img src="/img2/system_prepaid.gif" />
                     </div>
                 </li>
                 <li onclick="location.href='/resellers.html'">
                     <div class="package_container">
                         <div style="clear:both;position:absolute;bottom: 24px;text-align:center;left:0px;right:0px;"><button style="font-size:12px;width:90%;">Choose country</button></div>
                         <p class="rapids" style="padding: 0 0 4px 0;">Buy in your country</p>
                         <img src="/img2/resellerwelt.jpg" />
                     </div>
                 </li>
 
                 <li onclick="vxChoosePaket(1);">
                     <div class="package_container">
                         <div style="clear:both;position:absolute;bottom: 24px;text-align:center;left:0px;right:0px;"><button style="font-size:12px;">Buy</button></div>
                         <p class="rapids_nr">400</p>
                         <p class="rapids">Rapids</p>
                         <p class="prize">4,99&euro;</p>
                     </div>
                 </li>
                 <li onclick="vxChoosePaket(2);">
                     <div class="package_container">
                         <div style="clear:both;position:absolute;bottom: 24px;text-align:center;left:0px;right:0px;"><button style="font-size:12px;">Buy</button></div>
                         <p class="rapids_nr">1000</p>
                         <p class="rapids">Rapids</p>
                         <p class="prize">9,95&euro;</p>
                     </div>
                 </li>
                 <li onclick="vxChoosePaket(3);">
                     <div class="package_container">
                         <div style="clear:both;position:absolute;bottom: 24px;text-align:center;left:0px;right:0px;"><button style="font-size:12px;">Buy</button></div>
                         <p class="rapids_nr">2000</p>
                         <p class="rapids">Rapids</p>
                         <p class="prize">19,90&euro;</p>
                     </div>
                 </li>
                 <li onclick="vxChoosePaket(4);">
                     <div class="package_container">
                         <div style="clear:both;position:absolute;bottom: 24px;text-align:center;left:0px;right:0px;"><button style="font-size:12px;">Buy</button></div>
                         <p class="rapids_nr">5000</p>
                         <p class="rapids">Rapids</p>
                         <p class="prize">49,75&euro;</p>
                     </div>
                 </li>
                 <li onclick="vxChoosePaket(5);">
                     <div class="package_container">
                         <div style="clear:both;position:absolute;bottom: 24px;text-align:center;left:0px;right:0px;"><button style="font-size:12px;">Buy</button></div>
                         <p class="rapids_nr">20000</p>
                         <p class="rapids">Rapids</p>
                         <p class="prize">199,00&euro;</p>
                     </div>
                 </li>
             </ul>
         </div>
 
         <div id="package_overview">
             <h2>Product Details:</h2>
             <h4>Now: RapidPro</h4>
             <table id="produkttabelle" width="60%" cellspacing="0" cellpadding="4" border="0" style="border-top: 1px solid #ccc; border-left: 1px solid #ccc;">
               <tr>
                 <th>Price/ 30 days</th>
                 <th>Storage</th>
                 <th>Traffic/ 30 days</th>
               </tr>
               <tr style="text-align:center;">
                 <td style="color:#00204e;">99 Rapids</td>
                 <td style="color:#00204e;">10 GB</td>
                 <td style="color:#00204e;">30 GB, use as required</td>
               </tr>
             </table>
 
 
             <h4>Before: RapidSmall</h4>
             <table id="produkttabelle" width="60%" cellspacing="0" cellpadding="4" border="0" style="border-top: 1px solid #ccc; border-left: 1px solid #ccc;">
               <tr style="text-align:center;">
                 <td style="color:#aaaaaa;">120 Rapids</td>
                 <td style="color:#aaaaaa;">10 GB</td>
                 <td style="color:#aaaaaa;">30 GB, divided into 30 packages of 1 GB each</td>
               </tr>
             </table>
 
             <p style="margin:20px 0 10px 0;"><b>Your savings: 21 Rapids per month with full flexibility and full cost control!</b></p>
             <p style="margin:10px 0 10px 0;">Do you need more storage or more traffic?</p>
 
             <table width="60%" cellspacing="0" cellpadding="4" border="0" style="text-align:center;">
               <tr>
                 <th>Additional Storage:</th>
                 <th>Additional Traffic:</th>
               </tr>
               <tr style="text-align:center;">
                 <td>1 GB = 2Rapids/30 days</td>
                 <td>5 GB = 14 Rapids</td>
               </tr>
             </table>
 
             <p style="margin:10px 0 20px 0;">Users can upload as many files as they want, but these files will be deleted if not downloaded for 60 days. In order to store files permanently, the option "Keep files forever" (subject to fees) must be activated in the account settings.<br />Acquired traffic does not expire anymore.</p>
 
         </div>
 
         <div id="features_explanation">
 
             <h2>Features RapidPro:</h2>
 
             <p><b>File Management</b><br />With the file manager you have the option to view the download links of your uploaded files.</p>
             <p><b>Direct Download</b><br />This feature provides your family, relatives, friends and business partners with a convenient and fast downloading capability, without them having to own a RapidShare Premium Account. They then have the possibility to download at the same speed as a Premium User without the required Hosting-Packages.</p>
             <p><b>Folder Management</b><br />The folder management feature allows our customers to organize their uploaded files in folders and sub folders in a structured way. The folders will not be deleted in case of a downgrade.   </p>
             <p><b>Verification <font style="font-size:10px;">(available soon)</font></b><br />With the RapidBig package you have the option to verify your account. This way you receive the full control over your files stored on the RapidShare servers. Through the verification of your account, files are protected from accidental deletion by our Abuse team. With the verification you confirm that you are permitted by the license holder to use the stored files as you intend.  </p>
             <p><b>License Manager / License Manager+  <font style="font-size:10px;">(available soon)</font></b><br />With the Licensemanager and the Licensemanager+ you have the possibility to use RapidShare.com to sell your own products (music, programs...). A verification of the account is required in order to take advantage of these features. We will be introducing these features as from July, 2010.</p>
 
         </div>
     </div>
     <!-- Login / Register -->
     <div id="pp2_step2div" style="display:none;">
         <br style="clear:both" />
         <h3>Choose to login into your account or create a new one:</h3>
 
         <!-- Login: -->
         <table width="100%" cellpadding="8" cellspacing="0" border="0">
             <tr valign="top">
                 <td width="49%">
                     <h3><u>Login into existing account</u></h3>
                     <form onsubmit="vxCheckLogin(this);return false;">
                     <table cellspacing="0" cellpadding="4" border="0" width="100%">
                         <tr>
                             <th>Login / Alias:</th>
                             <td align="right"><input type="text" name="login" style="width:250px;"></td>
                         </tr>
                         <tr>
                             <th>Your password:</th>
                             <td align="right"><input type="password" name="password" style="width:250px;"></td>
                         </tr>
                         <tr>
                             <th></th>
                             <td align="right"><input type="submit" value="Login"></td>
                         </tr>
                     </table>
                     </form>
                 </td>
                 <td style="font-size:32px;color:#ccc;vertical-align:middle;">
                 OR
                 </td>
                 <td width="49%">
                     <div style="border:2px solid red;padding:4px;display:none;" id="js_mailinfo">
                         <b>Your account was created!</b><br />
                         We have sent a confirmation to your e-mail address. Please follow the further instructions provided inside the email to activate your account. <br /> <br /> The Payment can be continued after the successful activation of your account.
                     </div>
                     <div id="js_mailsent">
                     <h3><u>Create a new account</u></h3>
                     <form onsubmit="try{vxCreateAccount(this)} catch(e) {alert(e.message)};return false;">
                     <table cellspacing="0" cellpadding="4" border="0" width="100%">
                         <tr>
                             <th>Choose a username:</th>
                             <td align="right"><input type="text" name="login" style="width:250px;"></td>
                         </tr>
                         <tr>
                             <th>Your e-mail address:</th>
                             <td align="right"><input type="text" name="email" style="width:250px;"></td>
                         </tr>
                         <tr>
                             <th>Retype your e-Mail:</th>
                             <td align="right"><input type="text" name="email_repeat" style="width:250px;"></td>
                         </tr>
                         <tr>
                             <th>Choose a password:</th>
                             <td align="right"><input type="password" name="password" style="width:250px;"></td>
                         </tr>
                         <tr>
                             <th></th>
                             <td align="right"><input type="submit" value="Create account"></td>
                         </tr>
                     </table>
                     </form>
                     </div>
                 </td>
             </tr>
         </table>
     </div>
     <!-- Pay with PrePaidCart -->
     <div id="pp2_step6div" style="display:none;">
         <br style="clear:both" />
         <h3>Redeem a Prepaidcard:</h3>
 
         <!-- PPC: -->
         <form onsubmit="vxRedeemCard(this);return false;">
         <table cellspacing="0" cellpadding="4" border="0" width="100%">
             <tr>
                 <th>Login as noted on your card</th>
                 <td align="right"><input type="text" name="login" style="width:250px;"></td>
             </tr>
             <tr>
                 <th>Password from card</th>
                 <td align="right"><input type="password" name="password" style="width:250px;"></td>
             </tr>
             <tr>
                 <th></th>
                 <td align="right"><input type="submit" value="Redeem"></td>
             </tr>
         </table>
         </form>
     </div>
     <div id="pp2_step6div_redeemed" style="display:none;border:2px solid red;padding:4px;clear:both;margin-top:60px;">
     Your account was credited with <span id="pp2_step6div_redeemed_rapids"></span> Rapids.
     </div>
     <!-- Paymentoptions -->
     <div id="pp2_step3div" style="display:none;">
         <br style="clear:both" />
         <h3>Please choose your favorite payment option:</h3>
         <p>Choose your preferred payment method. Depending on the payment method you may be transferred to an external website. Upon completion of the payment process you will be transferred back to this site in order to receive your account details.</p>
 
 <!-- PayPal -->
 <div id="js_pp2_paypal" style="margin: 5px; float: left; width: 400px; height: 150px;">
     <table width="100%">
         <tr valign="top">
         <td width="85" height="85"><img src="/img2/system_paypal.gif"></td>
         <td><b>PayPal</b><br>PayPal accepts payments via credit card, bank transfer or PayPal balance.
         <div style="text-align: right;">
         <input type="button" value="pay now" id="js_pp2_paypal_link"></div>
         </td>
         </tr>
     </table>
 </div>
 <!-- PaySafeCard -->
 <div id="js_pp2_paysafecard" style="margin: 5px; float: left; width: 400px; height: 150px;">
     <table width="100%">
         <tr valign="top">
         <td width="85" height="85"><img src="/img2/system_paysavecard.gif"></td>
         <td><b>PaySafeCard</b><br>Paysafecard is the secure payment method, you don't need a credit card or a bank account for. You can buy paysafecards in many places (online and offline) and use it like cash to pay on the internet.
         <div style="text-align: right;"><input type="submit" value="pay now" onclick="vxGoStep('psc');"></div>
         </td>
         </tr>
     </table>
 </div>
 <!-- WireTransfer -->
 <div id="js_pp2_wiretransfer" style="margin: 5px; float: left; width: 400px; height: 150px;">
     <table width="100%">
         <tr valign="top">
         <td width="85" height="85"><img src="/img2/system_bank.gif"></td>
         <td>Bank transfer</b><br>Click here to receive the data you need to initiate a bank transfer.
         <div style="text-align: right;"><input type="button" value="pay now" onclick="vxGoStep('wire');"></div>
         </td>
         </tr>
     </table>
 </div>
 <!--PayByCall-->
 <div id="js_pp2_paybycall" style="margin: 5px; float: left; width: 400px; height: 150px; display:none;">
     <table width="100%">
         <tr valign="top">
         <td width="85" height="85"><img src="/img2/system_paybycall.gif"></td>
         <td><b>PayByCall</b><br>The fast and easy way to purchase an account for 30 days.
         <div style="text-align: right;"><input type="button" value="pay now" id="js_pbclink" onclick=""></div>
         </td>
         </tr>
     </table>
 </div>
 <!-- Reseller-->
 <div id="js_pp2_reseller" style="margin: 5px; float: left; width: 400px; height: 150px;">
     <table width="100%">
         <tr valign="top">
         <td width="85" height="85"><img src="/img2/system_rs-reseller.gif"></td>
         <td>Alternative payment methods</b><br>If none of these payment options suits you, you can also purchase an account via one of our resellers, which offers different payment methods.
         <div style="text-align: right;"><input type="button" value="pay now" id="js_pp2_reseller_link"></div>
         </td>
         </tr>
     </table>
 </div>
 
     </div>
     <!-- PaySafeCard -->
     <div id="pp2_step3div_psc" style="display:none;"></div>
     <!--WireTransfer-->
     <div id="pp2_step3div_wire" style="display:none;">
 
         <div id="pp2_wire_comit" style="display:none;">
             <h2>Thank you for your selection.</h2>
             <p>You can now make the bank wire transfer.</p>
             <p><b>Please inform your bank that YOU are going to carry the payment fees for the money transfer.</b><br>Otherwise we will not be able to activate the purchased Premium Account.</p>
 
             <h2>Your order</h2>
             <table>
                 <tr><td width="200">Duration:</td><td><span id="pp2_wire_payrapids"></span> Rapids</td></tr>
                 <tr><td>Amount:</td><td><span id="pp2_wire_payamount1"></span> EURO</td></tr>
             </table>
 
             <h2>Reason for payment/Payment memo</h2>
             <div style="margin: 10px; padding: 10px; border: 2px solid rgb(0, 32, 78);" id="pp2_wire_subject"></div>
             <h2>Our bank details</h2>
             <div style="margin: 10px; padding: 10px; border: 2px solid rgb(0, 32, 78);">
                 <table>
                     <tr><td width="200">Name of Account:</td><td>RapidShare AG</td></tr>
                     <tr><td>IBAN</td><td>CH91 0483 5009 1878 3200 0</td></tr>
                     <tr><td>BIC/SWIFT:</td><td>CRESCHZZ80A</td></tr>
                     <tr><td>Bank:</td><td>CREDIT SUISSE, ZUERICH</td></tr>
                 </table>
             </div>
             <h2>Amount</h2>
             <div style="margin: 10px; padding: 10px; border: 2px solid rgb(0, 32, 78);">
                 <span id="pp2_wire_payamount2"></span> EURO<br>
                 Please inform your bank that YOU are going to carry the payment fees for the money transfer.
             </div>
 
             <h2>Notice</h2>
             <p>* It is vital for you to include the reason for payment/payment memo.  If your bank is not able to process the characters "+ -", you can replace the characters with another.</p>
             <p>Due to international banking regulations, a wire transfer can take up to 14 business days. Most wire transfers are completed much sooner.  In some instances the fee to be paid may exceed the actual amount to be transferred for a Premium Account.  As we have no influence on this, we have a lot of Reseller partners around the world that you can purchase Premium Account from locally. You can view our list of Resellers and their countries <a href="http://rapidshare.com/resellers.html">here</a></p>
             <p>The bank sort code and bank account number is included in the IBAN number.  Based on the IBAN number, your bank will be able to carry out the payment.</p>
         </div>
 
         <div id="pp2_wire_form">
             <br style="clear:both" />
             <h2>Pay by bank wire transfer</h2>
             <p>If you wish to buy Rapids by bank wire transfer, please pay attention to the following:</p>
             <ul>
                 <li style="margin-left:50px;">Please ensure to provide a current email address, to which we are going to confirm the payment.</li>
                 <li style="margin-left:50px;">Please inform yourself if additional payment fees are calculated by your bank.  We can only activate Accounts, which we have received the correct amount for.  The payment fees have to be carried by you.</li>
                 <li style="margin-left:50px;">For the payment memo/reason for payment, please include the code we are going to generate for you in the next step.</li>
             </ul>
 
             <form onsubmit="try {vxDoWireTransfer(this);} catch(e){alert(e.message)};return false;">
             <h2>User information</h2>
                 <table width="100%" style="text-align: left;">
                     <tr>
                         <td>Full name:</td>
                         <td align="right"><input type="text" maxlength="64" size="50" name="name" id="name"></td>
                     </tr>
                     <tr>
                         <td>Email address</td>
                         <td align="right"><input type="text" maxlength="64" size="50" name="email" id="email"></td>
                     </tr>
                     <tr>
                         <td>Repeat email address</td>
                         <td align="right"><input type="text" maxlength="64" size="50" name="email2" id="email2"></td>
                     </tr>
                 </table>
                 <div style="text-align: right;"><input type="submit"></div>
             </form>
             <h2>Notice</h2>
             <p>Due to international banking regulations, a wire transfer can take up to 14 business days.  Most wire transfers are completed much sooner.  In some instances the fee to be paid may exceed the actual amount to be transferred for a Premium Account.  As we have no influence on this, we have a lot of Reseller partners around the world that you can purchase Premium Account from locally. You can view our list of Resellers and their countries <a href="http://rapidshare.com/resellers.html">here</a></p>
         </div>
     </div>
 </div>
 
     
 </div>
 <div class="untermenue">
  <a href="http://rapidshare.com/wiruberuns.html">About us</a> | <a href="http://rapidshare.com/jobs.html">Jobs</a> | <a href="http://rapidshare.com/agb.html">Terms of use</a> | <a href="http://rapidshare.com/imprint.html">Imprint</a>
 </div>
 
 </div>
 </center>
 <p>&nbsp;</p>
 </body>
 </html>
 

 shtml=
 shtml+"</body></html>"

 out find(shtml "<head>")
 out shtml.len

HtmlToWebBrowserControl id(3 hDlg) shtml 0
