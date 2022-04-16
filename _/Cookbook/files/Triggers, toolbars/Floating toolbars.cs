/// You can create custom toolbars, and attach them to windows or screen edges. Toolbar buttons can execute any code, for example run scripts and programs.

/// Use menu TT to open a script where you can add and edit toolbars. Add window-attached toolbars in script <+recipe>Window toolbars<>, others in <+recipe>Common toolbars<>.

/// To add a toolbar you can use snippet toolbarSnippet:
/// 1. Start typing "tool" and select "toolbarSnippet" in the list. It adds function Toolbar_RenameMe containing a new toolbar.
/// 2. In the added code replace all Toolbar_RenameMe with a unique name, for example Toolbar_X.
/// 3. In the first function (with attribute [Triggers]) add a window trigger, like in examples. Edit window name and other arguments.
/// 4. Edit the toolbar code. Add/delete/edit buttons. Set properties if need.

/// Quick ways to add a toolbar button:
/// - Clone an existing button.
/// - Snippet: start typing "tool" and select toolbarButtonSnippet in the list.
/// - Drag and drop a script, file, folder or link. Then select "t[name] = ...".

/// To set button icon: click the button in the code editor, and double-click an icon in the Icons dialog.

/// Click the Run button to apply changes after editing.
/// To show a window-attached toolbar, open or activate its trigger window. The toolbar initially is in its top-left corner. Shift+drag to move it to another place relative to the window.
/// Toolbar properties can be changed in code and in the right-click menu.

/// To delete a toolbar, delete its function and trigger. To disable temporarily, just comment out the trigger.

/// See also recipe <+recipe>Triggers and toolbars<>.
