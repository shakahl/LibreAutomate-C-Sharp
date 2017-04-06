Integrate Help.
   Now part is in Help file, part in Tips pane.
   Need a full list of all functions (intrinsic+UDF+dll).
   Auto-create the list from categories and short descriptions of functions (like now is in annotations and status bar).
   Maybe the list would be integrated with tool dialogs, everything in single list in QM window.
   For intrinsic functions maybe better write shorter descriptions and show in Tips, with links to full descriptions.
   Or show intrinsic function help in Tips in web browser control.
   In the future, many intrinsic functions probably will be moved/renamed to UDF and dll.
   Also user could add custom snippets to the list/tree.

Define function (intrinsic and UDF) info and tool in XML.
   Then, using that XML, display/create info and tools in QM dynamically.
   Also add QM dialogs (eg Make Exe) as tools.
   Also add operators, directives.

Add command group buttons in vertical toolbar.
   All actions in the group could be displayed in left pane (tab together with items).
   Or, actions as tree in left side.
   Drag and drop to favorites.
   Search.

Type info popups
   On mouse hover show tooltip with short function description and maybe an example.
   Or make multicolumn (report view). Second column contains short description. Third column - help button.

In help and in tool dialogs, auto create help links for all tech words (class, flags, thread, etc).

Clean macro, eg autodetect and replace duplicate win etc.

CompileAll: Add to menu Run -> Debug. Show dialog with folders with checkboxes (remember checked). Option to not open function on error. Option to exclude some functions.

____________________________

List of categories/functions/tools/help in QM window could be:

Find help, functions, tools
	Last Find results
Favorite
	User-added functions, Help topics, custom snippets, etc.
Functions
	Control
	Dialog
	File
	...
Language
	Create variable (int, str, ...)
	Define function parameters
	Other declarations (type, dll, ...)
		Type or class
		Dll function
		...
	Flow (if, repeat, ...)
		if
		rep
		...
	Operators (+, =, ...)
		Assign (=)
		Arithmetic (+, -, ...)
		Bitwise (&, |, ...)
		Compare (<, =, ...)
		Logical (and, or, ...)
		Unary, convert (-, !, &, +, @, ...)
		Member, element (., [])
		(maybe all these in Tips pane, with links to more help)
	Directives (#if, ...)
	Other (sizeof, ...)
Tips, Short QM Help
	...
QM Help Contents
	...

On click it would show short help in Tips.
Also there could be links (or icons) for tool dialogs, more info, insert function name, etc.
Must show all info (short) to create macro without searching for help. Eg creating variables, mostly used variable types, scopes.
Save Favorites in DB table named 'tools' or 'xTools'. Or in a macro in XML format. Could compare with the default XML (a System macro) using xdiff.
Move Tips contents (folders) from Tips pane to the list/tree.
