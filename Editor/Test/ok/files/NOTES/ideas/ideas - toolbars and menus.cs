Autotext option "erase delimiter".

Toolbar: option to sit only on single-color area.
	For example, then could be placed in a code control.
	When the control eg scrolled, the toolbar would move away from text to a white place.

Toolbar: option to use specified theme background (eg OpenThemeData(0 L"Taskbar"); DrawThemeBackground; CloseThemeData).

Toolbar Properties: add "Attach to window" wizard.

Pie menus. Now something similar can be made with toolbars. Or menus/toolbars with custom shape and icon positions.

Menu: settings for separate items (override menu settings).

Extract icon from any string that looks like path. Now works only with run, but does not work with RunConsole2 etc.

Icon: ... * FunctionThatReturnsIconFileOrHandle. Also could use UDFs for label and code. Or even UDFs that return whole menu text, or part.

Menu: should create menu each time (flag).

Menu: expanding folders: file filters, eg "*.txt,*.doc".

Menu: auto multicolumn.

Menu: mac(menu command): if command begins with " /", interpret is as options.

Menu: make items accessible (attach MSAAMENUINFO struct).

Menu, tb: in context menu, add shell context menu for files (IContextMenu).

New menu text style: /Label /Icon etc, followed by any number of lines of code.

Toolbars: autosize when buttons added.

Toolbar menu option: show hotkeys for macro-items that have key triggers.

Toolbars: rename "auto shrink" to "auto hide". Also, when checking, ask to what edge to move etc.

Toolbar option: if auto shrink, don't shrink on button click

Toolbars: files from folders, submenus, delete, file properties, open file folder, sort.

Menu option: incremental.

Toolbars: Skins. Graphic toolbar, where buttons are associated with parts of the image. Maybe html. <suggestion>

Toolbar flag: move with left button.

Menu: create submenus on demand.

Menus: dynamically created submenus (and maybe whole menu)
  (eg call the specified function that returns submenu contents).
  See how powerpro does it.
  Syntax may be: >>Function [param]
  Or menu template, where parts are replaced with variables.

Menu launched from toolbar should know toolbar handle.

Non-on-top free toolbars.

When toolbar child (9999) is created, send spec message to toolbar so that toolbar hook proc would know when it is created or recreated.

For web, use default url icon, because eg on Vista current icon is outdated.

Folder menus: user-defined sorting/filtering function. (user request)

____________________________________________

REJECTED

In menus: * "A" would dynamically create A icon.

Toolbar option: follow caret.

Menus: don't create thread for simple men.
