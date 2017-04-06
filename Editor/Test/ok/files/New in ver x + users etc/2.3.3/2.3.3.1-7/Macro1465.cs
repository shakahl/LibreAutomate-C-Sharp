
str f1=
 <html><body>
 f1
 </body></html>
f1.setfile("$temp$\qm_f1.htm")

str f2=
 <html><body>
 <script>
 alert("f2");
 </script>
 f2
 </body></html>
f2.setfile("$temp$\qm_f2.htm")

str fs=
 <html>
 <frameset cols="35%,*">
 <frame name="f1" src="qm_f1.htm"">
 <frame name="f2" src="qm_f2.htm"">
 </frameset>
 </html>
str fil="$temp$\qm_fs.htm"
fs.setfile(fil)

web fil 1
