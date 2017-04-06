 Notes from summer 2003

1) Missing these statements from CompileDialog function:

		dt.x=val(x)
		dt.y=val(y)
		

2) Ive made a slight change in DefDialogProc (see DefDialogProc2)
	This alows to have multiple "OK" type buttons with different
	text, and returns id of button clicked. -see PushButtons dialog

	Originally i wrote: "Dialog Editor: need way to have ShowDialog return 
	value for more than two "OK" and "Cancel" buttons, ie. buttons with
	different names, with box closing as soon as user hits a button."
	
	But i played around with WINAPI and figured it out :) But for those
	needing a quick usage, it would be frustrating, so i think the change
	i made would help, but i dont know enough about
	styles to add more yet, ive just changed it to suit my need.

	
3) Dialog Editor: Run dialog editor for PushButtons dialog, and its very strange...
	eg.:	a) see that dialog is not in editor window
			b) enlarge the window and see what happens	
			c) try to remove one of the four buttons
