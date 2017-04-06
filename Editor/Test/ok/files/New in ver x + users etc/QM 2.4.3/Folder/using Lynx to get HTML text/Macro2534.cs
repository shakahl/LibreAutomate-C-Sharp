out
str html=
 <HTML>
 <BODY>
 <DIV>This is a test.</DIV>
 <DIV>&nbsp;</DIV><DIV align="left">
 <TABLE border="1" cellspacing="1" width="50%">
 <TR height="20">
 <TD valign="middle">text1</TD>
 <TD width="20%" height="20" valign="middle">text2</TD>
 </TR>
 <TR height="20">
 <TD width="20%" height="20" valign="middle">text3</TD>
 <TD width="20%" height="20" valign="middle">text4</TD>
 </TR>
 <TR height="20">
 <TD width="20%" height="20" valign="middle">text5</TD>
 <TD width="20%" height="20" valign="middle">text6</TD>
 </TR>
 </TABLE>
 </DIV>
 <DIV>&nbsp;</DIV>
 <DIV>End.</DIV>
 </BODY>
 </HTML>

str txt
str lynxDir="$program files$\Lynx - web browser"
__TempFile tf.Init("htm" "" "" html)
RunConsole2 F"{lynxDir}\lynx.exe -dump -nomargins {tf}" txt lynxDir
out txt
