 DDE client functions. A variable of this class represents a DDE connection.
 For single conversation, use Execute, Request and Poke.
 For multiple conversations using same server/topic, at first call Connect, then Execute2, Request2 and Poke2.
 Calling Disconnect is optional.

 Connect, Execute, Request and Poke on error end macro.
 Execute2, Request2 and Poke2 on error return 0.
 Default timeout is 60 seconds.

 EXAMPLES
Dde d; str s
sel ListDialog("Add program group and shortcut[]List program groups[]Send data to Excel[]Get two Excel cells" "" "QM - Test DDE")
	case 1
	d.Execute("PROGMAN" "PROGMAN" "[CreateGroup(TestDDE)][AddItem(notepad.exe)]")
	
	case 2
	d.Request("PROGMAN" "PROGMAN" "Groups" &s)
	out s
	
	case 3
	run "Excel"
	1
	d.Connect("Excel" "Sheet1")
	int i
	str si("R C1") sd
	for(i 1 6)
		si[1]='0'+i
		sd=i
		d.Poke2(si sd)
	
	case 4
	d.Connect("Excel" "Sheet1")
	d.Request2("R1C1" &s)
	s.trim; out s
	d.Request2("R2C1" &s)
	s.trim; out s
