 A variable of this class manages a control of "QM_Tools" window class. Don't use this class directly; use controls.

 ---- About "QM_Tools" window class ----

 Controls of this class can be used to create QM code to add to current macro (InsertStatement).
 It is a QM-registered class (in TO_InitToolsControl). In exe not registed and not tested.

 Designed to support various tools. Currently there is only single tool - window/control selector.

 ---- About window/control selector tool ----

 Allows user to select and edit a window or control, or just a point in screen.
 Craete this control with text "1". Also can optionally pass flags, eg "1 1" and lock, eg "1 0 1".

 Has several child controls.
 When need, parent dialog can hide some of them, change text, select etc.
 Use WS_EX_CONTROLPARENT to include them in dialog keyboard navigation.

 To get results, can be used dialog variables or str.getwintext.
   Format: "type[]handle[]winStatement[]comments"
     type - 1 window, 2 control.
     handle - an expression that returns handle of the window or control. Handle, child, id or win.
     winStatement - int var=win(...). Can be empty line.
     comments - comments. Can be empty line. Used if control selected. If flag 0x400, also if window.
   All is validated, escaped, etc. Don't need more processing, just get lines.
   Can use __strt.Win to parse into variables.
   Empty if screen/none. If flag 0x800, can be "0[]0[][]comments" if screen.

 You can optionally set initial content with a dialog variable or str.setwintext. Format:
   Same as you see in the window and control fields after capturing.
   To specify and select just window, use single line. To specify and select control, add second line for control. To select screen, use empty string.
   In control string use {window} as a placeholder for window handle.

 flags:
   1 - if 'Window' selected, get text as single line, usually "win(...)". Does not declare a handle variable.
   2 - if 'Window' or 'Control' selected, get text as single line, eg "win(...)" or "id(... win(...))".
   16 - hide 'Screen'.
   32 - select 'Window'.
   64 - show only the window combo and small edit button. Autosize. Lock to Window.
   128 - if window field empty, get empty text, like 'Screen' would be selected.
   0x100 - window cannot be handle. Used on get text.
   0x200 - 'test' mode. Used on get text. Uses _i for window handle instead of declaring var. This flag is temporary; unsets after getting text.
   0x400 - also get comments if window selected. Also get comments of accessible object from mouse. If not set, gets only control comments.
   0x800 - get comments if screen too.
   0x1000 - hide the Drag control when 'Screen' selected.
 flags 16, 32 and 64 used only when creating control.

 lock
   1 - select 'Window' and don't allow to select others.
   2 - select 'Control' and don't allow to select others.
   3 - select 'Screen' and don't allow to select others.
   0 - can select any.

 Messages:
 __TWM_DRAGDROP - can be used with drag-drop. Read <help>__ToolsControl._WinCapture</help>.
 __TWM_SETFLAGS - changes flags. wParam is flags. lParam is how to set flags: 0 set, 1 add, 2 remove.
 __TWM_SETLOCK - changes lock. wParam is lock.
 __TWM_SELECT - selects window type. wParam: 0,3 screen, 1 window, 2 control, -1 none.
 __TWM_GETSELECTED - returns selected window type: 0,3 screen, 1 window, 2 control, -1 none.
 __TWM_GETCAPTUREDHWND - returns captured/selected window or control handle or 0.

 Notifications:
 __TWN_DRAGBEGIN and __TWN_DRAGEND - used with drag-drop. Read <help>__ToolsControl._WinCapture</help>.
 __TWN_WINDOWCHANGED - window edit control text changed. Does not send for control edit.
   wParam - selected window handle if selected from list or with Drag. 0 if user typed.
   lParam - Window edit control handle.
