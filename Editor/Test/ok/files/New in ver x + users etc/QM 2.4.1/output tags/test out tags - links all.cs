str s=
 <><link "http://www.quickmacros.com">open web page</link>
 <link "mailto:abc@def.gh?subject=test%20qm%20links&body=test%0Aqm%0Alinks">create email</link>
 <link "notepad.exe">run file notepad.exe</link>
 <link "notepad.exe /$desktop$\test.txt">run notepad with command line parameters</link>
 <macro "MyMacro">run macro MyMacro</macro>
 <macro "MyMacro /abc">run macro MyMacro; the _command variable will contain "abc"</macro>
 <open "MyMacro">open MyMacro</open>
 <open "MyMacro /10">open MyMacro and go to position 10</open>
 <open "MyMacro /L10">open MyMacro and go to line 10 (1-based).</open> QM 2.3.2.
 <help "::/qm_help/IDH_QUICK.html">open a topic in QM Help</help>
 <help "qm2help.chm">open help file qm2help.chm</help>
 <help>MyFunction</help> QM 2.3.3. Shows function help. Read more below.
 <help "qm2help.chm::/qm_help/IDH_QUICK.html>main">open a topic in a help file, "main" window type</help>
 <tip "E_IF">display a tip from $qm$\tips.txt</tip>
 <tip "#Macro2221">display a macro or function (only help section) in tips</tip>
 <tip #Macro2221>display a macro or function (only help section) in tips</tip>
 <google "quick macros">Google search for 'quick macros'</google> QM 2.3.3.
 <google "quick macros /&start=10&lr=lang_de">Google search with parameters</google> QM 2.3.3.
 <mes "mes text[]line2">show message box</mes> QM 2.3.4
 <out "out text[]line2">show text in QM output</out> QM 2.3.4
 <link>http://www.quickmacros.com</link>
 <tip>Macro2216</tip>
 <help>FileCopy</help> <help>act</help> <help>SetTimer</help> <help>WM_TIMER</help> <help>str.replacerx</help> <help>Macro2214</help>.
 <help "#IDH_QUICK">open a topic in QM Help</help> QM 2.4.1
 <help #IDP_IF>open a topic in QM Help</help> QM 2.4.1
out s
