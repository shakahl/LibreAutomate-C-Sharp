 out "<><i>italic</i>"

str s=
 <><b>This <color "0xff">is</color> <i>help</i> section.</b>
 <color "0xff0000"><b><u>This</u> is <i>help</i> section.</b></color>
 ᶚݐᵉᵊᵺﺵﺶﺷﺷ
 
 a <code>run</code>
 
 <code "1">
 run :run "abc.exe" * icon.ico
 run :run "abc.exe" * icon.ico
 </code>
 
 <b><code><i>code</i></code></b>
 
 -------------------------------
 
 <b>bold</b> <i>italic</i> <u>underline</u>
 <color "0x00ff00">green color</color>
 <b><i><color "0xff">nested tags</color></i></b>
 
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

rep 100
	out s
