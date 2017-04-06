email :"my@ema.il" * email.ico
-
>Text
	LCase :str s.getsel; if(s.len) s.lcase; s.setsel
	UCase :str s.getsel; if(s.len) s.ucase; s.setsel
	<
>Files
	Internet Explorer :run "iexplore.exe"
	Documents :run "$documents$"
	<
 Trigger: two times quickly move the mouse up-down in the middle of the screen.
 Note: the Samples folder must be enabled. To enable, right click the folder and click Enable.
