 Waits for multiple top-level or child windows.
 At first call an AddX function to define windows to wait for. Call it for each window.
 Then call a WaitX function to wait for a condition.

 This is common to all WaitX functions:
    Return 1-based index of the added window that satisfies the wait condition.
    Succeed if the condition is true when called.
    Use opt waitmsg and opt hidden.

 EXAMPLES

#compile "__WaitWinMulti"
WaitWinMulti x
x.AddWin("Notepad")
x.AddWin("" "" "WORDPAD")
sel x.WaitActive
	case 1 out "notepad"
	case 2 out "wordpad"
