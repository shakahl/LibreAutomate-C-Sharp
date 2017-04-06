Windows 10: after sleep/hibernation, LL mouse hooks don't receive button messages in admin windows.

Windows 10: in DPI-scaled windows, LL mouse hooks receive wrong (logical) coordinates on button messages. Good on mouse move messages.

Windows 8.1+: in DPI-scaled windows: wrong locations of many acc objects. But good of others. UI Automation (UI Spy) gets correct locations.

Windows 8.1+: in DPI-scaled windows in non-primary monitor: RealChildWindowFromPoint and other "ChildWindowFromPointX" API use wrong coord. These API are used in TopChildWindowFromPoint (also in hok).

Visual C++: in some areas mouse hooks (nonLL) don't work. When moving mouse to the right, status bar stops on 1021 and does not show 1022 and 1023.

Mouse triggers do not work over some windows if they are inactive (Avant).

Vista: record macro that pastes in notepad and then closes notepad. When running, clicking on Close button does not work after pasting. Needs to insert eg 0.3 s delay.

Vista: if CPU is VERY busy (eg 8 busy threads), when you Ctrl+click QM tray icon, it does not close QM. It is because QM tray icon then does not receive mouse click messages.

Vista: if CPU is VERY busy (eg 4 busy threads), and a LL mouse hook is installed, mouse sticks.
   Cannot fix it.
   It happens when QM is being shut down because of illegal operation.
   Sometimes it also happens in stress conditions, eg when minimizing all windows, but not sure that it is related to LL hooks.
   Tried to set thread priority.
   Tested when QM thread is busy (using for loop) but then mouse works well.

Acc.Location, Htm.Location: with some web pages, eg forumas.draugas.lt, if scrolled not at the top, BODY y is negative, and height is height of visible part. This also makes Htm.Mouse to scroll to view. Acc may think that BODY is invisible, and then also does not find descendant objects, unless searches for invisible objects.

InternetSetOption: INTERNET_OPTION_CONNECT_TIMEOUT does not work. Workaround: close one of internet handles.

In WPF (windows presentation foundation) windiws, accessible navigation does not work. Works only navigation to parent.

Empty taskbar area returns hittest HTCAPTION. Tested on win7 and xp. Hit testing is used in mouse triggers. Same with LL and nonLL. Don't apply corrections (fbc).

Dreamweaver: cannot drag-move floating panes if QM toolbar is attached.
   QM toolbar does not receive any messages while trying to move a pane.
   While trying to move, DW drag-pane transparent windows are repeatedly created-destroyed.

Firefox hangs when capturing accessible object.
   Conditions:
      One of tabs is http://layout.osenkov.com/ (silverlight app).
      Filter: Show invisible. Ok if only visible. But in the latest test not ok too.
   Same with some other silverlight apps. OK with others (eg http://gallery.expression.microsoft.com/en-us/TextboxFocusStates?SRC=Home).
   'Find acc obj' dialog also hangs. Firefox remains hung even after killing the dialog.
   Does not eat CPU.

IE 9 beta. Cannot drag drop links to other apps. Only to Windows Explorer. IE 7 would show message box...

acc dialog does not show objects in Office dialogs and some Dreamweaver panes (eg formatting).
   But acc finds them when using child window, because the child window is inside the subtree.
   The last shown nodes are simple elements, not full objects. They should contain children, but acc_getChild fails.

Avast 7 false positive for some QM programs, especially in notebook, especially from app folder.
   Depends on folder, program version, etc.

Firefox: if zoomed, bad acc locations.

Inno Setup: non-LL mouse hook does not work if main window is disabled because of a modal dialog, eg config sign tools.

Windows bug: If an admin or uiAccess process has a WH_KEYBOARD hook, it breaks such hooks of standard user processes (set before) in 64-bit windows.

If high DPI, QM web browser control gives incorrect location of elements.

Vista 64 on vmware: cannot load ADO and some other typelibs. Reason: there are no environment variables %CommonProgramFiles(x86)% etc.

end() in macro, on WM_CREATE: Windows 8 and 10 throws "exception in callback" and QM crashes. Then how to end macro?
