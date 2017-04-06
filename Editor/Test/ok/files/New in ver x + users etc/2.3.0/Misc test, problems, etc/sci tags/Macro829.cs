 out "<><i>italic</i>"

str s=
 <><b>This <color "0xff">is</color> <i>help</i> section.</b>
 <color "0xff0000"><b><u>This</u> is <i>help</i> section.</b></color>
 ᶚݐᵉᵊᵺﺵﺶﺷﺷ
 <link "notepad.exe">run file notepad.exe</link>
 <link "notepad.exe">run <color "0xff"><b>file</b></color> notepad.exe</link>
 <color "0xff00">text <link "notepad.exe">run <b>file</b> notepad.exe</link> text</color>
 <link>calc.exe</link>
 
 a <code>run</code>
 
 <code "1">
 run :run "abc.exe" * icon.ico
 run :run "abc.exe" * icon.ico
 </code>
 
 <b><code><i>code</i></code></b>
 
 -------------------------------
 <link "http://www.quickmacros.com">open web page</link>
 <link>http://www.quickmacros.com</link>
 <link "mailto:abc@def.gh?Subject=a+b+copoipipoipoiopiopioio">create email</link>
 <link "notepad.exe">run file notepad.exe</link>
 <link "notepad.exe /$desktop$\test.txt">run notepad with command line parameters</link>
 <macro "Macro815">run macro Macro815</macro>
 <macro "Macro815 /abc">run macro Macro815; the _command variable will contain "abc"</macro>
 <macro "Macro815 /''one[]two''">run macro Macro815; the _command variable will contain unescaped text</macro>
 <open "Macro815">open Macro815</open>
 <open "Macro815 /10">open Macro815 and go to position 10</open>
 <help "::/qm_help/IDH_QUICK.html">open a topic in QM Help</help>
 <help "qm2help.chm">open help file qm2help.chm</help>
 <help "qm2help.chm::/qm_help/IDH_QUICK.html>main">open a topic in a help file, "main" window type</help>
 <tip "E_IF">display other tip</tip>
 <tip "#ShowDialog">display macro or function (only help section) in tips</tip>
 
 <b>bold</b> <i>italic</i> <u>underline</u>
 <color "0x00ff00">green color</color>
 <b><i><color "0xff">nested tags</color></i></b>
 <c "0x00ff00">green color</c>
 
 <code>
 int i
 for i 0 5
 	out 1
 </code>
 
 <code "1">
 abc :run "abc.exe" * icon.ico
 abc :run "abc.exe" * icon.ico
 </code>
 
 <code "2">
 abc :run "abc.exe" * icon.ico
 </code>
 
 --------------------
 EXAMPLE
  comments
 out "ᶚݐᵉᵊᵺﺵﺶﺷﺷ"

rep 1
	out s
