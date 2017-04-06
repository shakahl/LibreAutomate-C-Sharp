/b/i/c/p3

 SIMPLE TEXT EXAMPLES
sim :'"simple text" ;;inserts simple text when you type sim and postfix (Ctrl, space, comma, Enter...)
simp :"paste text" ;;same as above, but uses clipboard instead of keys. If 'Hybrid paste' checked in Options, uses keys for short text, clipboard for long text.

 FORMATTED TEXT EXAMPLES
dt :str s.timeformat("{DD} {T}"); key (s) ;;current date and time. To add the code, you can use the Text dialog from the floating toolbar.
inpn :if(inp(_i "a number")) key F"the number is {_i}" ;;manual text input

 POPUP LIST EXAMPLE
 If there are several matching items, shows a popup list. Select an item with mouse, Enter, Tab or a number key.
#region
PL :'"list item 1"
PL :'"list item 2" ;;Label
PL :'"list item 3"
PL :'"list item 4" ;;Label
	#region
