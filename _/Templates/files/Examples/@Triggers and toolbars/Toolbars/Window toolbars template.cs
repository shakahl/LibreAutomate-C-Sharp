//This file can be used as a template for new files containing window toolbars. To create new file:
//1. Clone this file with Ctrl + drag. Open the clone.
//2. Rename function RenameMeToolbars. It must be a unique name in this project folder. For example CalculatorToolbars.
//3. Add and edit one or more toolbars in this file. You can add/delete/edit everything later too.

//To add a toolbar you can use snippet toolbarSnippet:
//1. Start typing "tool" and in the completion list select "toolbarSnippet".
//2. In the added code replace all Toolbar_RenameMe with a unique name, for example Toolbar_X.
//3. In the Toolbar_X function add a window trigger, like in the example. Edit window name and other arguments.
//4. Edit the toolbar. Add/delete/edit buttons, menus. Set properties if need. To add buttons use toolbarX snippets.

//To apply changes after editing, click Run to restart the script. To show the toolbar(s), open or activate the window.
//If the trigger is correct, the toolbar(s) should be in its top-left corner. Shift+drag to some other place. Try to move
//the window to see how it works. You can right-click a toolbar and change its properties.

using Au; using Au.Types; using System; using System.Collections.Generic;
using Au.Triggers; using System.Windows.Forms; using System.Drawing;

partial class Script {

[Toolbars]
void RenameMeToolbars() {
	//Add trigger(s) here. Examples:
	//Triggers.Window[TWEvent.ActiveOnce, "*Name1", "ClassName1"] = Toolbar_RenameMe;
	//Triggers.Window[TWEvent.ActiveOnce, "*Name2", "ClassName2"] = o => { Toolbar_One(o); Toolbar_Two(o); } //attach 2 toolbars to this window
}

//Add toolbar(s) here.



}
