Recording:
	To avoid problems when finds old window with same name, insert "wait until previous window disappears".
	See: http://www.quickmacros.com/forum/viewtopic.php?f=1&t=6997 (see the final macro posted by the user: http://www.quickmacros.com/forum/viewtopic.php?f=1&t=6997#p32168)

Recording:
	Add function WaitUntilActiveWindowContainsAccObjectX.
	It repeatedly tries to find the acc object in CURRENT (not initial) active window.
	Returns struct containing Wnd and Acc. In C# 7 could return tuple.
	var results=WaitFor.ActiveWindow(w=>w.Name=="Name" && null!=w.Find(...));

Interactive playback. When something failed etc, let user choose what to do. Optionally modify the script.
	Examples:
	On timeout - wait more.
	If window not found but there are windows with similar name/class/etc - choose that window. Even when waiting, after eg 10 s.

Pane or dialog: Sub-functions, regions and bookmarks.

Add 'Slow' checkbox in dialogs Mouse, Find Image etc, like now is in Keys.

Pause macro.
   Hot key to pause/resume macro.
     This was in Wish List several times.
     While hold down, all threads also are paused.
     Try SuspendThread/ResumeThread.
     At least in debug mode.
   Option to auto-pause when user uses keyboard or mouse, and resume when idle.

Web recorder. Object recorder. Probably as QM extensions.

QM item type: text. Not executable. No auto coloring. Can add styles and links like in the tips pane.

Option to on error show message box without ending macro. User can end macro, retry, auto-retry n times, go to edit.
	Also ShowOnErrorMessage(). Eg on err. Or at the end, to continue creating macro.
	Allow to edit the satatement at run time, and then retry. ("Edit and Continue")
	Maybe even replace it with multiple statements.
	It is difficult inplace, but maybe use something like a temp #sub.

Menu File -> New -> File Link -> New (creates txt file in $my qm$), Open, -, About.

When reopening QM file, don't close/reopen System.qml. Then eg can use System-defined window classes in QM UI. Also add this option for any shared file.

Ctrl+drag or middle-drag QM item: if drop on code editor, show menu:
   Open in this editor (primary or secondary)
   Insert mac "Name"
   Insert Name
   ...

Double-click item except folder: open Properties.

Debug mode: step over. Because now eg can skip functions in closed folders, but cannot skip subs.

Monitor CPU usage of QM threads. Use GetThreadTimes, for qmm GetProcessTimes. Warn if high, or display in UI.

Option to see currently executing statemen (or just line number) of a thread. From Wish List.

In dialogs, on mouse over on a ontrol, show small transparent circle with ?. On click, would expand and show tooltip.

In UDF, possibility to evaluate caller's arguments by executing them in caller's context, 0 or more times.
   For example, a wait function could repeatedly evaluate caller's argument win(...) instead of just receiving it's return value.
   Someting similar like UDFs used with foreach.

Dockable UI etc: http://www.quickmacros.com/forum/viewtopic.php?f=3&t=5217&p=23809#p23809

Source control integration: http://www.quickmacros.com/forum/viewtopic.php?f=3&t=5880

Statistics.
   All thread start times, who started, end times, who ended, etc.
   Triger usage.
   Etc.
   It would help users to find some problems, such as when they don't know why certain bad things happened.
   Also some users want to know how much time they spent creating macros and how much time/times they used the macros.
   Can optionally get debug info such as number of allocations, handles (kernel/user/gdi), etc. (before and after thread).

QM activity log (suggestion of stupomer). Number of user/qm clicks, length of user/qm mouse movement, number of user/qm keys, macros that were running.

Make safe code like this: __Handle h ... h=something. Must automatically close handle etc, eg call dtor. Also __Handle etc should have function Close.

Recording:
   Detect window name parts that may change, and provide correction links in output.

Option to auto wait/retry for more functions, like now has Acc.Find etc.
   User suggestion: http://www.quickmacros.com/forum/viewtopic.php?f=3&t=5272
   Eg record clicks etc with this option.
   Maybe interactively record 'wait for object' functions.

Message recorder. Eg on click record SendMessage(WM_LBUTTONDOWN etc) or ClickControl. See how Girder does it.

opt interactive.
   On error shows dialog where user can change something and retry. The changes may be temporary or permanent.
   Eg if win() does not find window, suggests to: 1. wait (and possibly change code to wait). 2. change name etc and retry.
   Also in this mode win() etc could wait some time. Or opt winwait.

A "tiny macro" tool. Eg replace duplicate win() to variables (now only suggests).

menu Run -> Compile Options -> More Warnings. If checked, shows warnings eg on 'if(s="")' etc. It helps to learn and debug.

Export item or folder as XML. Then paste in forum. In forum, add Copy button to copy that and import into QM. With all properties, tabs, etc.

File link items: autosave to a temp file, and somehow notify to save to original file. If not saved to original file in this session, remember the temp file in next session.

File link: detect when modified externally.

Compare macros. Could simply add the existing code (Compare2macros, Xdiff class), but better to add more integration, eg sync scroll.

Add lexers: XML, VBScript, JScript, C, C#, VB.NET, maybe more. Or provide link to a freeware editor for these languages. With syntax highlighting, function popups, etc.

When thread terminated, show where hung (function and statement).

Spec folders "$system 64$" and "$program files 64$".

When current item is compiled and not dirty, somehow indicate it. Eg change Compile button image. But don't disable.

Menu and toolbar editors. For QM menus/toolbars, not for dialog menus.

Dialogs for repeating, variables, functions, etc.

Pane for failures and events like 'QM started', 'hooks restored', etc. Save to file.

TI popup: add ref option "sorted", so that qsort would not be called. For example, WINAPI2 popup with sorting delays about 0.6 s, without sorting - 0.1 s.

TI popup: make more flexible, like Visual AssistX. For example, show items that contain typed text in middle.

Type info: show type for variables and members.

In Run dialogs, if nonfs, add comments/menulabel, like now adds on drag drop.

Global and local flexible logging.

Dialog markup language.

Easy method to sync macro and function. Ie, macro runs function (mac), and function auto exits when macro ends.

Encrypt parts of macro. Eg [*XXXXXXXXXXXXXXXXXXXXXXXXXXX*], or #encrypt directive.

Dialog to create/test regular expression.

Import and replace existing folder. (http://www.quickmacros.com/forum/viewtopic.php?p=4273)
  Add " /uuid xxxxxxxxx" or " /author aaaaa" in folder description. If finds folder with that uuid, importing automatically deletes it and adds the imported folder in the same place (even if folder or some items are read-only).
  If importing as shared, move the new file from TIF/Temp to QM folder (replace existing shared file).

Add skinning component to skin toolbars, user-defined dialogs, etc.
   http://www.skincrafter.com/skbuilder.html  ($399)

Native MOD support for System functions.
   For example, functions that have "replace function" trigger and same name, would be called instead of these system functions. Or if placed in certain folder.
   Options -> Check Extensions should list replaced items.
   Also these items must be in file viewer.

QM deskband (http://www.quickmacros.com/forum/viewtopic.php?p=4386)

Use write-protected memory (VirtualAlloc) for compiled code.

Outlining:
   #region [name]
   code
   #endregion
   When user selects block of text and chooses menu item "Hide region", enclose it with #region untitled[]code[]#endregion, and hide.

QM item properties summary in one place. Can copy. Suggestion.
   Maybe can copy (eg XML) and apply to another item.

Better UI for acc triggers. Suggestion. Eg could automatically populate from log results.

Find Accessible Object dialog: use multiple tabs:
   Tab1 finds object, tab2 finds childs object of tab1, and so on.
   It would insert: Acc a.Find(w ...); Acc a2.Find(a.a ...); ...

Find dialog:
   Add Type combobox below Folders. Or option to sort results by type. User suggestion. I never needed it.
   Search in a saved search results. Specify it in folders, eg <savedsearch>.
   Use item types and other properties. Use grid for it.

Exe: option: wait for all threads. Because now users have problems with async functions, such as OnScreenDisplay.

Backup file link target files.

Detect when macro modifies constant strings, and show warning.
   Use VirtualAlloc with MEM_WRITE_WATCH, and GetWriteWatch. XP+.

Option to show dialog when macro runs.

Common password.
	Can be used to:
		As default password (if not specified) to encrypt/decrypt macros.
		Find text in macros excrypted with that password.
		To "lock file". Some QM users miss this removed QM feature.
	Saved in registry. Or in main file, but maybe more difficult to implement.

Assign to QM: app user model ID.

Exe manifest: use the default manifests as templates, and allow to add-replace tags in Make Exe dialog.

______________________________________________

LOW PRIORITY

Use sqlite FTS for 'Find Help, functions, tools'.

Use sqlite FTS in Find dialog, as 'Fulltext' or 'FTS'. Code to create FTS index is already created, in test3.cpp.

______________________________________________

REJECTED

Output: expandable sections.
   Eg: <expand "the text, possibly qm-escaped">Link text</expand>.
   Rarely used, difficult.

Record (in background; optionally) user's tasks all the time. If a task is repeated several times, add macro in Suggestions folder.
   Very difficult to implement. How to know when a task begins and ends? Also, mouse click coordinates will be different each time.

Save-load compiled modules.

Run macro from other files. Extract required functions too.

Options presets that can be assigned to items.

Network settings: list of friend IPs. Can implement in macro because it receives ip in _command.

Macro started by running a shortcut-macro (one that begins with " /Macro2") should receive shortcut-macro's id.
   For example it then could process the shortcut-macro.
   For example the shortcut-macro could contain VBScript code, and the second macro could run the code...
   _command may be already used, therefore need a new spec var.

Make exe: notify shell, because sometimes does not update folder or desktop view automatically.
   For example, on Vista, does not update icon if on Desktop. Once even did not show.
   However this would slow down. We need as fast as possible.

Add " /setup SetupFunction" in folder description. Importing prompts user, shows the function, and on OK executes it.

Options: default folder for exe.

COM server to execute QM functions from eg VBScript (from QM or other apps).
   This may be useful if QM was free.

