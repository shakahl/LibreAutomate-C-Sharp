out
str s=
 <>Links
 
 <link "http://www.quickmacros.com">open web page</link>
 <link "mailto:abc@def.gh?subject=test%20qm%20links&body=test%0Aqm%0Alinks">create email</link>
 <link "notepad.exe">run file notepad.exe</link>
 <link "notepad.exe /$desktop$\test.txt">run notepad with command line parameters</link>
 <macro "MyMacro">run macro MyMacro</macro>
 <macro "MyMacro /abc">run macro MyMacro; the _command variable will contain "abc"</macro>
 <macro "MyMacro /<keyword>">run macro MyMacro; the _command variable will contain the search keyword</macro>
 <open "MyMacro">open MyMacro</open>
 <open "MyMacro /10">open MyMacro and go to position 10</open>
 <help "::/qm_help/IDH_QUICK.html">open a topic in QM Help</help>
 <help "qm2help.chm">open help file qm2help.chm</help>
 <help "qm2help.chm::/qm_help/IDH_QUICK.html>main">open a topic in a help file, "main" window type</help>
 <tip "E_IF">display a tip from $qm$\tips.txt</tip>
 <tip "#MyMacro">display a macro in tips</tip>
 <tip "#MyFunction">display the help section of a function in tips</tip>
 
 Instead of syntax <tag "attribute">text</tag> can be used <tag>text</tag>. Then text is used as attribute. Example:
 
 <link>http://www.quickmacros.com</link>
 
 Styles
 
 <b>bold</b> <i>italic</i> <u>underline</u>
 <color "0x00ff00">green color</color>
 <b><i><color "0xff">nested tags</color></i></b>
 
 Instead of <color> can be used <c>.
 
 Replacements
 
 The search keyword (for which was pressed F1):
 <keyword>
 
 Function's help section:
 <function>
 
 Code
 
 Macro or function:
 <code>
 int i
 for(i 0 5) out 1
 </code>
 
 Menu:
 <code "1">
 abc :run "abc.exe" * icon.ico
 abc :run "abc.exe" * icon.ico
 </code>
 
 TS menu:
 <code "2">
 abc :key "abc"
 abc :key "abc"
 </code>
 
 Ordinary text (ignore tags)
 
 <_>
 Text with <b>tags</b> that are <u>ignored</u>.
 </_>

out s
