WS_BORDER
Creates a window that has a thin-line border.

WS_CAPTION
Creates a window that has a title bar.
Note: WS_CAPTION consists of 2 bits: WS_BORDER|WS_DLGFRAME. It cannot be combined with these styles. Don't use code like <code>if(style&WS_CAPTION)</code>, use <code>if(style&WS_CAPTION=WS_CAPTION)</code>.

WS_CHILD
Creates a child window. A window with this style cannot have a menu bar. This style cannot be used with the WS_POPUP style.

WS_CLIPCHILDREN
Excludes the area occupied by child windows when drawing occurs within the parent window. This style is used when creating the parent window.

WS_CLIPSIBLINGS
Clips child windows relative to each other; that is, when a particular child window receives a WM_PAINT message, the WS_CLIPSIBLINGS style clips all other overlapping child windows out of the region of the child window to be updated. If WS_CLIPSIBLINGS is not specified and child windows overlap, it is possible, when drawing within the client area of a child window, to draw within the client area of a neighboring child window.

WS_DISABLED
Creates a window that is initially disabled. A disabled window cannot receive input from the user. To change this after a window has been created, use EnableWindow.

WS_DLGFRAME
Creates a window that has a border of a style typically used with dialog boxes. A window with this style cannot have a title bar.

WS_GROUP
Specifies the first control of a group of controls. The group consists of this first control and all controls defined after it, up to the next control with the WS_GROUP style. The first control in each group usually has the WS_TABSTOP style so that the user can move from group to group. The user can subsequently change the keyboard focus from one control in the group to the next control in the group by using the direction keys.

WS_HSCROLL
Creates a window that has a horizontal scroll bar.

WS_MAXIMIZE
Creates a window that is initially maximized.

WS_MAXIMIZEBOX
Creates a window that has a maximize button. Cannot be combined with the WS_EX_CONTEXTHELP style. The WS_SYSMENU style must also be specified.

WS_MINIMIZE
Creates a window that is initially minimized.

WS_MINIMIZEBOX
Creates a window that has a minimize button. Cannot be combined with the WS_EX_CONTEXTHELP style. The WS_SYSMENU style must also be specified.

WS_POPUP
Creates a pop-up window. This style cannot be used with the WS_CHILD style.

WS_SYSMENU
Creates a window that has a window menu on its title bar. The WS_CAPTION style must also be specified.

WS_TABSTOP
Specifies a control that can receive the keyboard focus when the user presses the TAB key. Pressing the TAB key changes the keyboard focus to the next control with the WS_TABSTOP style.

WS_THICKFRAME
Creates a window that has a sizing border.

WS_VISIBLE
Creates a window that is initially visible.
This style can be turned on and off by using ShowWindow or SetWindowPos (or hid in QM).

WS_VSCROLL
Creates a window that has a vertical scroll bar.




WS_EX_ACCEPTFILES
Specifies that a window created with this style accepts drag-drop files.

WS_EX_APPWINDOW
Forces a top-level window onto the taskbar when the window is visible.

WS_EX_CLIENTEDGE
Specifies that a window has a border with a sunken edge.

WS_EX_COMPOSITED
Paints all descendants of a window in bottom-to-top painting order using double-buffering. This cannot be used if the window has a class style of either CS_OWNDC or CS_CLASSDC.

WS_EX_CONTEXTHELP
Adds a question mark in the title bar of the window. When the user clicks the question mark, the cursor changes to a question mark with a pointer. If the user then clicks a child window, the child receives a WM_HELP message. Then it or its parent dialog can display a help pop-up window or open a help file topic.
This style cannot be used with the WS_MAXIMIZEBOX or WS_MINIMIZEBOX styles.

WS_EX_CONTROLPARENT
The window itself contains child windows that should take part in dialog box navigation. If this style is specified, the dialog manager recurses into children of this window when performing navigation operations such as handling the TAB key, an arrow key, or a keyboard mnemonic.

WS_EX_DLGMODALFRAME
Creates a window that has a double border; the window can, optionally, be created with a title bar by specifying the WS_CAPTION style.

WS_EX_LAYERED
Creates a layered window. Note that this cannot be used for child windows until Windows 8. Also, this cannot be used if the window has a class style of either CS_OWNDC or CS_CLASSDC.

WS_EX_LAYOUTRTL
Arabic and Hebrew versions of Windows: Creates a window whose horizontal origin is on the right edge. Increasing horizontal values advance to the left.

WS_EX_LEFTSCROLLBAR
If the shell language is Hebrew, Arabic, or another language that supports reading order alignment, the vertical scroll bar (if present) is to the left of the client area. For other languages, the style is ignored.

WS_EX_MDICHILD
Creates a multiple-document interface (MDI) child window.

WS_EX_NOACTIVATE
A top-level window created with this style does not become the foreground window when the user clicks it. The system does not bring this window to the foreground when the user minimizes or closes the foreground window.
The window does not appear in the taskbar, unless the WS_EX_APPWINDOW style is used.

WS_EX_NOINHERITLAYOUT
A window created with this style does not pass its window layout to its child windows.

WS_EX_NOPARENTNOTIFY
Specifies that a child window created with this style does not send the WM_PARENTNOTIFY message to its parent window when it is created or destroyed.

WS_EX_RIGHT
The window has generic "right-aligned" properties. This depends on the window class. This style has an effect only if the shell language is Hebrew, Arabic, or another language that supports reading-order alignment; otherwise, the style is ignored.
Using the WS_EX_RIGHT style for static or edit controls has the same effect as using the SS_RIGHT or ES_RIGHT style, respectively. Using this style with button controls has the same effect as using BS_RIGHT and BS_RIGHTBUTTON styles.

WS_EX_RTLREADING
If the shell language is Hebrew, Arabic, or another language that supports reading-order alignment, the window text is displayed using right-to-left reading-order properties. For other languages, the style is ignored.

WS_EX_STATICEDGE
Creates a window with a three-dimensional border style intended to be used for items that do not accept user input.

WS_EX_TOOLWINDOW
Creates a tool window; that is, a window intended to be used as a floating toolbar. The window has a smaller title bar. The window does not appear in the taskbar or in the ALT+TAB window, unless the WS_EX_APPWINDOW style is used. If a tool window has a system menu, its icon is not displayed on the title bar. However, you can display the system menu by right-clicking or by typing ALT+SPACE.

WS_EX_TOPMOST
Specifies that a window created with this style should be placed above all non-topmost windows and should stay above them, even when the window is deactivated. To add or remove this style, use the SetWindowPos function (or Zorder in QM).

WS_EX_TRANSPARENT
Specifies that a window created with this style should not be painted until siblings beneath the window (that were created by the same thread) have been painted. The window appears transparent because the bits of underlying sibling windows have already been painted.
To achieve transparency without these restrictions, use the SetWindowRgn function.

WS_EX_WINDOWEDGE
Specifies that a window has a border with a raised edge.




DS_ABSALIGN
Indicates that the coordinates of the dialog box are screen coordinates. If this style is not specified, the coordinates are client coordinates.

DS_CENTER
Centers the dialog box in the working area of the monitor that contains the owner window. If no owner window is specified, the dialog box is centered in the working area of a monitor determined by the system. The working area is the area not obscured by the taskbar or any appbars.

DS_CENTERMOUSE
Centers the dialog box on the mouse cursor.

DS_CONTROL
Creates a dialog box that works well as a child window of another dialog box, much like a page in a property sheet. This style allows the user to tab among the control windows of a child dialog box, use its accelerator keys, and so on.

DS_MODALFRAME
Creates a dialog box with a modal dialog-box frame that can be combined with a title bar and window menu by specifying the WS_CAPTION and WS_SYSMENU styles.

DS_NOFAILCREATE
Creates the dialog box even if errors occur. For example, if a child window cannot be created.

DS_NOIDLEMSG
Suppresses WM_ENTERIDLE messages that the system would otherwise send to the owner of the dialog box while the dialog box is displayed.

DS_SETFOREGROUND
Always activate the dialog box window (bring the dialog box to the foreground).

DS_SHELLFONT
Indicates that the dialog box should use the system shell font "MS Shell Dlg". The system passes font handle to the dialog box and to each control by sending them the WM_SETFONT message.

DS_SYSMODAL
Adds WS_EX_TOPMOST style.




CCS_ADJUSTABLE
Enables a toolbar's built-in customization features, which let the user to drag a button to a new position or to remove a button by dragging it off the toolbar. In addition, the user can double-click the toolbar to display the Customize Toolbar dialog box, which enables the user to add, delete, and rearrange toolbar buttons.

CCS_BOTTOM
Causes the control to position itself at the bottom of the parent window's client area and sets the width to be the same as the parent window's width. Status windows have this style by default.

CCS_LEFT
Causes the control to be displayed vertically on the left side of the parent window.

CCS_NODIVIDER
Prevents a two-pixel highlight from being drawn at the top of the control.

CCS_NOMOVEX
Causes the control to resize and move itself vertically, but not horizontally, in response to a WM_SIZE message. If CCS_NORESIZE is used, this style does not apply.

CCS_NOMOVEY
Causes the control to resize and move itself horizontally, but not vertically, in response to a WM_SIZE message. If CCS_NORESIZE is used, this style does not apply. Header windows have this style by default.

CCS_NOPARENTALIGN
Prevents the control from automatically moving to the top or bottom of the parent window. Instead, the control keeps its position within the parent window despite changes to the size of the parent. If CCS_TOP or CCS_BOTTOM is also used, the height is adjusted to the default, but the position and width remain unchanged.

CCS_NORESIZE
Prevents the control from using the default width and height when setting its initial size or a new size. Instead, the control uses the width and height specified in the request for creation or sizing.

CCS_RIGHT
Causes the control to be displayed vertically on the right side of the parent window.

CCS_TOP
Causes the control to position itself at the top of the parent window's client area and sets the width to be the same as the parent window's width. Toolbars have this style by default.

CCS_VERT
Causes the control to be displayed vertically.




ACS_AUTOPLAY
Starts playing the animation as soon as the AVI clip is opened.

ACS_CENTER
Centers the animation in the animation control's window.

ACS_TRANSPARENT
Allows you to match an animation's background color to that of the underlying window, creating a "transparent" background. The parent of the animation control must not have the WS_CLIPCHILDREN style. The control sends a WM_CTLCOLORSTATIC message to its parent. Use SetBkColor to set the background color for the device context to an appropriate value. The control interprets the upper-left pixel of the first frame as the animation's default background color. It will remap all pixels with that color to the value you supplied in response to WM_CTLCOLORSTATIC.




BS_3STATE
Creates a button that is the same as a check box, except that the box can be grayed as well as checked or cleared. Use the grayed state to show that the state of the check box is not determined.

BS_AUTO3STATE
Creates a button that is the same as a three-state check box, except that the box changes its state when the user selects it. The state cycles through checked, indeterminate, and cleared.

BS_AUTOCHECKBOX
Creates a button that is the same as a check box, except that the check state automatically toggles between checked and cleared each time the user selects the check box.

BS_AUTORADIOBUTTON
Creates a button that is the same as a radio button, except that when the user selects it, the system automatically sets the button's check state to checked and automatically sets the check state for all other buttons in the same group to cleared.

BS_CHECKBOX
Creates a small, empty check box with text. By default, the text is displayed to the right of the check box. To display the text to the left of the check box, combine this flag with the BS_LEFTTEXT style (or with the equivalent BS_RIGHTBUTTON style).

BS_DEFPUSHBUTTON
Creates a push button that behaves like a BS_PUSHBUTTON style button, but has a distinct appearance. If the button is in a dialog box, the user can select the button by pressing the ENTER key, even when the button does not have the input focus. This style is useful for enabling the user to quickly select the most likely (default) option.

BS_GROUPBOX
Creates a rectangle in which other controls can be grouped. Any text associated with this style is displayed in the rectangle's upper left corner.

BS_LEFTTEXT
Places text on the left side of the radio button or check box when combined with a radio button or check box style. Same as the BS_RIGHTBUTTON style.

BS_OWNERDRAW
Creates an owner-drawn button. The owner window receives a WM_DRAWITEM message when a visual aspect of the button has changed. Do not combine the BS_OWNERDRAW style with any other button styles.

BS_PUSHBUTTON
Creates a push button that posts a WM_COMMAND message to the owner window when the user selects the button.

BS_RADIOBUTTON
Creates a small circle with text. By default, the text is displayed to the right of the circle. To display the text to the left of the circle, combine this flag with the BS_LEFTTEXT style (or with the equivalent BS_RIGHTBUTTON style). Use radio buttons for groups of related, but mutually exclusive choices.

BS_BITMAP
Specifies that the button displays a bitmap.
Read more in BS_ICON help.

BS_BOTTOM
Places text at the bottom of the button rectangle.

BS_CENTER
Centers text horizontally in the button rectangle.

BS_ICON
Specifies that the button displays an icon.
The appearance of text or an icon or both on a button control depends on the BS_ICON and BS_BITMAP styles, and whether the BM_SETIMAGE message is called. The possible results are as follows.
BS_ICON or BS_BITMAP set?	 BM_SETIMAGE called?	 Result
Yes	 Yes	 Show icon only
No	 Yes	 Show icon and text
Yes	 No	 Show text only
No	 No	 Show text only

BS_FLAT
Specifies that the button is two-dimensional; it does not use the default shading to create a 3-D image.

BS_LEFT
Left-justifies the text in the button rectangle. However, if the button is a check box or radio button that does not have the BS_RIGHTBUTTON style, the text is left justified on the right side of the check box or radio button.

BS_MULTILINE
Wraps the button text to multiple lines if the text string is too long to fit on a single line in the button rectangle.

BS_NOTIFY
Enables a button to send BN_KILLFOCUS and BN_SETFOCUS notification messages to its parent window.
Note that buttons send the BN_CLICKED notification message regardless of whether it has this style. To get BN_DBLCLK notification messages, the button must have the BS_RADIOBUTTON or BS_OWNERDRAW style.

BS_PUSHLIKE
Makes a button (such as a check box, three-state check box, or radio button) look and act like a push button. The button looks raised when it isn't pushed or checked, and sunken when it is pushed or checked.

BS_RIGHT
Right-justifies text in the button rectangle. However, if the button is a check box or radio button that does not have the BS_RIGHTBUTTON style, the text is right justified on the right side of the check box or radio button.

BS_RIGHTBUTTON
Positions a radio button's circle or a check box's square on the right side of the button rectangle. Same as the BS_LEFTTEXT style.

BS_TOP
Places text at the top of the button rectangle.

BS_VCENTER
Places text in the middle (vertically) of the button rectangle.

BS_SPLITBUTTON
Microsoft Windows Vista: Creates a split button. A split button has a drop down arrow.
The arrow occupies a small rectangle on one side of the button.

BS_DEFSPLITBUTTON
Microsoft Windows Vista: Creates a split button that behaves like a BS_PUSHBUTTON style button, but also has a distinctive appearance. If the split button is in a dialog box, the user can select the split button by pressing the ENTER key, even when the split button does not have the input focus. This style is useful for enabling the user to quickly select the most likely (default) option.

BS_COMMANDLINK
Microsoft Windows Vista: Creates a command link button.
Use message BCM_SETNOTE.

BS_DEFCOMMANDLINK
Microsoft Windows Vista: Creates a command link button that behaves like a BS_PUSHBUTTON style button. If the button is in a dialog box, the user can select the command link button by pressing the ENTER key, even when the command link button does not have the input focus. This style is useful for enabling the user to quickly select the most likely (default) option.




CBS_AUTOHSCROLL
Automatically scrolls the text in an edit control to the right when the user types a character at the end of the line. If this style is not set, only text that fits within the rectangular boundary is allowed.

CBS_DISABLENOSCROLL
Shows a disabled vertical scroll bar in the list box when the box does not contain enough items to scroll. Without this style, the scroll bar is hidden when the list box does not contain enough items.

CBS_DROPDOWN
The user can edit control text or select an item from the drop-down list box.

CBS_DROPDOWNLIST
The user select an item from the drop-down list box but cannot edit control text.

CBS_HASSTRINGS
Specifies that an owner-drawn combo box contains items consisting of strings. The combo box maintains the memory and address for the strings so the application can use the CB_GETLBTEXT message to retrieve the text for a particular item.

CBS_NOINTEGRALHEIGHT
Specifies that the size of the combo box is exactly the size specified by the application when it created the combo box. Normally, the system sizes a combo box so that it does not display partial items.

CBS_OWNERDRAWFIXED
Specifies that the owner of the list box is responsible for drawing its contents and that the items in the list box are all the same height. The owner window receives a WM_MEASUREITEM message when the combo box is created and a WM_DRAWITEM message when a visual aspect of the combo box has changed.

CBS_OWNERDRAWVARIABLE
Specifies that the owner of the list box is responsible for drawing its contents and that the items in the list box are variable in height. The owner window receives a WM_MEASUREITEM message for each item in the combo box when you create the combo box and a WM_DRAWITEM message when a visual aspect of the combo box has changed.

CBS_SIMPLE
Displays the list box at all times. The user can edit control text.

CBS_SORT
Automatically sorts strings added to the list box.




DTS_APPCANPARSE
Allows the owner to parse user input and take necessary action. It enables users to edit within the client area of the control when they press the F2 key. The control sends DTN_USERSTRING notification messages when users are finished.

DTS_LONGDATEFORMAT
Displays the date in long format. The default format string for this style is defined by LOCALE_SLONGDATEFORMAT, which produces output like "Friday, April 19, 1996".

DTS_RIGHTALIGN
The drop-down month calendar will be right-aligned with the control instead of left-aligned, which is the default.

DTS_SHOWNONE
It is possible to have no date currently selected in the control. With this style, the control displays a check box that is automatically selected whenever a date is picked or entered. If the check box is subsequently deselected, the application cannot retrieve the date from the control because, in essence, the control has no date. The state of the check box can be set with the DTM_SETSYSTEMTIME message or queried with the DTM_GETSYSTEMTIME message.

DTS_SHORTDATEFORMAT
Displays the date in short format. The default format string for this style is defined by LOCALE_SSHORTDATE, which produces output like "4/19/96".

DTS_SHORTDATECENTURYFORMAT
Similar to the DTS_SHORTDATEFORMAT style, except the year is a four-digit field. The default format string for this style is based on LOCALE_SSHORTDATE. The output looks like: "4/19/1996".

DTS_TIMEFORMAT
Displays the time. The default format string for this style is defined by LOCALE_STIMEFORMAT, which produces output like "5:31:42 PM".

DTS_UPDOWN
Places an up-down control to the right of the DTP control to modify date-time values. This style can be used in place of the drop-down month calendar, which is the default style.




ES_AUTOHSCROLL
Automatically scrolls text to the right by 10 characters when the user types a character at the end of the line. When the user presses the ENTER key, the control scrolls all text back to position zero.

ES_AUTOVSCROLL
Automatically scrolls text up one page when the user presses the ENTER key on the last line.

ES_CENTER
Centers text in a single-line or multiline edit control.

ES_MULTILINE
Designates a multiline edit control. The default is single-line edit control.
When the multiline edit control is in a dialog box, the default response to pressing the ENTER key is to activate the default button. To use the ENTER key as a carriage return, use the ES_WANTRETURN style.
When the multiline edit control is not in a dialog box and the ES_AUTOVSCROLL style is specified, the edit control shows as many lines as possible and scrolls vertically when the user presses the ENTER key. If you do not specify ES_AUTOVSCROLL, the edit control shows as many lines as possible and beeps if the user presses the ENTER key when no more lines can be displayed.
If you specify the ES_AUTOHSCROLL style, the multiline edit control automatically scrolls horizontally when the caret goes past the right edge of the control. To start a new line, the user must press the ENTER key. If you do not specify ES_AUTOHSCROLL, the control automatically wraps words to the beginning of the next line when necessary. A new line is also started if the user presses the ENTER key. The window size determines the position of the Wordwrap. If the window size changes, the Wordwrapping position changes and the text is redisplayed.
Multiline edit controls can have scroll bars. An edit control with scroll bars processes its own scroll bar messages. Note that edit controls without scroll bars scroll as described in the previous paragraphs and process any scroll messages sent by the parent window.

ES_NOHIDESEL
Negates the default behavior for an edit control. The default behavior hides the selection when the control loses the input focus and inverts the selection when the control receives the input focus. If you specify ES_NOHIDESEL, the selected text is inverted, even if the control does not have the focus.

ES_NUMBER
Allows only digits to be entered into the edit control. Note that, even with this set, it is still possible to paste non-digits into the edit control.

ES_PASSWORD
Displays an asterisk (* or ●) for each character typed into the edit control. This style is valid only for single-line edit controls.
To change the characters that is displayed, or set or clear this style, use the EM_SETPASSWORDCHAR message.

ES_READONLY
Prevents the user from typing or editing text in the edit control.
To change this style after the control has been created, use the EM_SETREADONLY message.

ES_RIGHT
Right-aligns text in a single-line or multiline edit control.

ES_WANTRETURN
Specifies that a carriage return be inserted when the user presses the ENTER key while entering text into a multiline edit control in a dialog box. If you do not specify this style, pressing the ENTER key has the same effect as pressing the dialog box's default push button. This style has no effect on a single-line edit control.




HDS_BUTTONS
Each item in the control looks and behaves like a push button. This style is useful if an application carries out a task when the user clicks an item in the header control. For example, an application could sort information in the columns differently depending on which item the user clicks.

HDS_DRAGDROP
Allows drag-and-drop reordering of header items.

HDS_FILTERBAR
Include a filter bar as part of the standard header control. This bar allows users to conveniently apply a filter to the display. Calls to HDM_LAYOUT will yield a new size for the control and cause the list view to update.

HDS_FLAT
Causes the header control to be drawn flat when Windows is running in classic mode.

HDS_FULLDRAG
Causes the header control to display column contents even while the user resizes a column.

HDS_HIDDEN
Indicates a header control that is intended to be hidden. This style does not hide the control. Instead, when you send the HDM_LAYOUT message to a header control with the HDS_HIDDEN style, the control returns zero in the cy member of the WINDOWPOS structure. You would then hide the control by setting its height to zero. This can be useful when you want to use the control as an information container instead of a visual control.

HDS_HORZ
Creates a header control with a horizontal orientation.

HDS_HOTTRACK
Enables hot tracking.

HDS_CHECKBOXES
Microsoft Windows Vista: Allows the placing of checkboxes on header items. For more information, see the fmt member of HDITEM.

HDS_NOSIZING
Microsoft Windows Vista: The user cannot drag the divider on the header control.

HDS_OVERFLOW
Microsoft Windows Vista: A button is displayed when not all items can be displayed within the header control's rectangle. When clicked, this button sends an HDN_OVERFLOWCLICK notification.




LBS_DISABLENOSCROLL
Shows a disabled vertical scroll bar for the list box when the box does not contain enough items to scroll. If you do not specify this style, the scroll bar is hidden when the list box does not contain enough items.

LBS_EXTENDEDSEL
Allows multiple items to be selected by using the SHIFT key and the mouse or special key combinations.

LBS_HASSTRINGS
Specifies that a list box contains items consisting of strings. The list box maintains the memory and addresses for the strings so that the application can use the LB_GETTEXT message to retrieve the text for a particular item. By default, all list boxes except owner-drawn list boxes have this style. You can create an owner-drawn list box either with or without this style.

LBS_MULTICOLUMN
Specifies a multi-columnn list box that is scrolled horizontally. The LB_SETCOLUMNWIDTH message sets the width of the columns.

LBS_MULTIPLESEL
Turns string selection on or off each time the user clicks or double-clicks a string in the list box. The user can select any number of strings.

LBS_NODATA
Specifies a no-data list box. Specify this style when the count of items in the list box will exceed one thousand. A no-data list box must also have the LBS_OWNERDRAWFIXED style, but must not have the LBS_SORT or LBS_HASSTRINGS style.
A no-data list box resembles an owner-drawn list box except that it contains no string or bitmap data for an item. Commands to add, insert, or delete an item always ignore any specified item data; requests to find a string within the list box always fail. The system sends the WM_DRAWITEM message to the owner window when an item must be drawn. The itemID member of the DRAWITEMSTRUCT structure passed with the WM_DRAWITEM message specifies the line number of the item to be drawn. A no-data list box does not send a WM_DELETEITEM message.

LBS_NOINTEGRALHEIGHT
Specifies that the size of the list box is exactly the size specified by the application when it created the list box. Normally, the system sizes a list box so that the list box does not display partial items.

LBS_NOREDRAW
Specifies that the list box's appearance is not updated when changes are made.
To change the redraw state of the control, use the WM_SETREDRAW message.

LBS_NOSEL
Specifies that the list box contains items that can be viewed but not selected.

LBS_NOTIFY
Notifies the parent window with an input message whenever the user clicks or double-clicks a string in the list box.

LBS_OWNERDRAWFIXED
Specifies that the owner of the list box is responsible for drawing its contents and that the items in the list box are the same height. The owner window receives a WM_MEASUREITEM message when the list box is created and a WM_DRAWITEM message when a visual aspect of the list box has changed.

LBS_OWNERDRAWVARIABLE
Specifies that the owner of the list box is responsible for drawing its contents and that the items in the list box are variable in height. The owner window receives a WM_MEASUREITEM message for each item in the combo box when the combo box is created and a WM_DRAWITEM message when a visual aspect of the combo box has changed.

LBS_SORT
Sorts strings in the list box alphabetically.

LBS_USETABSTOPS
Enables a list box to recognize and expand tab characters when drawing its strings. You can use the LB_SETTABSTOPS message to specify tab stop positions. The default tab positions are 32 dialog template units apart. Dialog template units are the device-independent units used in dialog box templates. To convert measurements from dialog template units to screen units (pixels), use the MapDialogRect function.

LBS_WANTKEYBOARDINPUT
Specifies that the owner of the list box receives WM_VKEYTOITEM messages whenever the user presses a key and the list box has the input focus. This enables an application to perform special processing on the keyboard input.




LVS_ALIGNLEFT
Items are left-aligned in icon and small icon view.

LVS_AUTOARRANGE
Icons are automatically kept arranged in icon and small icon view.

LVS_EDITLABELS
Item text can be edited in place. The parent window must process the LVN_ENDLABELEDIT notification message.

LVS_ICON
This style specifies icon view.

LVS_LIST
This style specifies list view.

LVS_NOCOLUMNHEADER
Column headers are not displayed in report view. By default, columns have headers in report view.

LVS_NOLABELWRAP
Item text is displayed on a single line in icon view. By default, item text may wrap in icon view.

LVS_NOSCROLL
Scrolling is disabled. All items must be within the client area. This style is not compatible with the LVS_LIST or LVS_REPORT styles. See Knowledge Base Article Q137520 for further discussion.

LVS_NOSORTHEADER
Column headers do not work like buttons. This style can be used if clicking a column header in report view does not carry out an action, such as sorting.

LVS_OWNERDATA
This style specifies a virtual list-view control. For more information about this list control style, see About List-View Controls.

LVS_OWNERDRAWFIXED
The owner window can paint items in report view. The list-view control sends a WM_DRAWITEM message to paint each item; it does not send separate messages for each subitem. The iItemData member of the DRAWITEMSTRUCT structure contains the item data for the specified list-view item.

LVS_REPORT
This style specifies report view. When using the LVS_REPORT style with a list-view control, the first column is always left-aligned. You cannot use LVCFMT_RIGHT to change this alignment. See LVCOLUMN for further information on column alignment.

LVS_SHAREIMAGELISTS
The image list will not be deleted when the control is destroyed. This style enables the use of the same image lists with multiple list-view controls.

LVS_SHOWSELALWAYS
The selection, if any, is always shown, even if the control does not have the focus.

LVS_SINGLESEL
Only one item at a time can be selected. By default, multiple items may be selected.

LVS_SMALLICON
This style specifies small icon view.

LVS_SORTASCENDING
Item indexes are sorted based on item text in ascending order.

LVS_SORTDESCENDING
Item indexes are sorted based on item text in descending order.




MCS_DAYSTATE
The month calendar will send MCN_GETDAYSTATE notifications to request information about which days should be displayed in bold.

MCS_MULTISELECT
The month calendar will allow the user to select a range of dates within the control. By default, the maximum range is one week. You can change the maximum range that can be selected by using the MCM_SETMAXSELCOUNT message.

MCS_WEEKNUMBERS
The month calendar control will display week numbers (1-52) to the left of each row of days. Week 1 is defined as the first week that contains at least four days.

MCS_NOTODAYCIRCLE
The month calendar control will not circle the "today" date.

MCS_NOTODAY
The month calendar control will not display the "today" date at the bottom of the control.

MCS_NOTRAILINGDATES
Microsoft Windows Vista: This flag disables displaying the dates from the previous/next month in the current calendar.

MCS_SHORTDAYSOFWEEK
Microsoft Windows Vista: This flag uses the CAL_SSHORTESTDAYNAME* names to display for the day of the week column header.

MCS_NOSELCHANGEONNAV
Microsoft Windows Vista: This flag does not change the selection when the user navigates next or previous in the calendar. This allows the user to select a range larger than what they can currently see.




PGS_AUTOSCROLL
The pager control will scroll when the user hovers the mouse over one of the scroll buttons.

PGS_DRAGNDROP
The contained window can be a drag-and-drop target. The pager control will automatically scroll if an item is dragged from outside the pager over one of the scroll buttons.

PGS_HORZ
Creates a pager control that can be scrolled horizontally. This style and the PGS_VERT style are mutually exclusive and cannot be combined.

PGS_VERT
Creates a pager control that can be scrolled vertically. This is the default direction if no direction style is specified. This style and the PGS_HORZ style are mutually exclusive and cannot be combined.




PBS_MARQUEE
The progress indicator does not grow in size but instead moves repeatedly along the length of the bar, indicating activity without specifying what proportion of the progress is complete.

PBS_SMOOTH
The progress bar displays progress status in a smooth scrolling bar instead of the default segmented bar.
Note  This style is supported only in the Windows Classic theme. All other themes override this style.

PBS_SMOOTHREVERSE
Determines the animation behavior that the progress bar should use when moving backward (from a higher value to a lower value). If this is set, then a "smooth" transition will occur, otherwise the control will "jump" to the lower value.

PBS_VERTICAL
The progress bar displays progress status vertically, from bottom to top.




RBS_AUTOSIZE
The rebar control will automatically change the layout of the bands when the size or position of the control changes. An RBN_AUTOSIZE notification will be sent when this occurs.

RBS_BANDBORDERS
The rebar control displays narrow lines to separate adjacent bands.

RBS_DBLCLKTOGGLE
The rebar band will toggle its maximized or minimized state when the user double-clicks the band. Without this style, the maximized or minimized state is toggled when the user single-clicks on the band.

RBS_FIXEDORDER
The rebar control always displays bands in the same order. You can move bands to different rows, but the band order is static.

RBS_REGISTERDROP
The rebar control generates RBN_GETOBJECT notification messages when an object is dragged over a band in the control. To receive the RBN_GETOBJECT notifications, initialize OLE with a call to OleInitialize or CoInitialize.

RBS_TOOLTIPS
Not yet supported.

RBS_VARHEIGHT
The rebar control displays bands at the minimum required height, when possible. Without this style, the rebar control displays all bands at the same height, using the height of the tallest visible band to determine the height of other bands.

RBS_VERTICALGRIPPER
The size grip will be displayed vertically instead of horizontally in a vertical rebar control. This style is ignored for rebar controls that do not have the CCS_VERT style.




ES_DISABLENOSCROLL
Disables scroll bars instead of hiding them when they are not needed.

ES_SUNKEN
Displays the control with a sunken border style so that the rich edit control appears recessed into its parent window.

ES_NOOLEDRAGDROP
Disables support for drag-drop of OLE objects.

ES_SAVESEL
Preserves the selection when the control loses the focus. By default, the entire contents of the control are selected when it regains the focus.




SBS_BOTTOMALIGN
Aligns the bottom edge of the scroll bar with the bottom edge of the rectangle defined by the x, y, nWidth, and nHeight parameters of CreateWindowEx function. The scroll bar has the default height for system scroll bars. Use this style with the SBS_HORZ style.

SBS_HORZ
Designates a horizontal scroll bar. If neither the SBS_BOTTOMALIGN nor SBS_TOPALIGN style is specified, the scroll bar has the height, width, and position specified by the x, y, nWidth, and nHeight parameters of CreateWindowEx.

SBS_LEFTALIGN
Aligns the left edge of the scroll bar with the left edge of the rectangle defined by the x, y, nWidth, and nHeight parameters of CreateWindowEx. The scroll bar has the default width for system scroll bars. Use this style with the SBS_VERT style.

SBS_RIGHTALIGN
Aligns the right edge of the scroll bar with the right edge of the rectangle defined by the x, y, nWidth, and nHeight parameters of CreateWindowEx. The scroll bar has the default width for system scroll bars. Use this style with the SBS_VERT style.

SBS_SIZEBOX
Designates a size box. If you specify neither the SBS_SIZEBOXBOTTOMRIGHTALIGN nor the SBS_SIZEBOXTOPLEFTALIGN style, the size box has the height, width, and position specified by the x, y, nWidth, and nHeight parameters of CreateWindowEx.

SBS_SIZEBOXBOTTOMRIGHTALIGN
Aligns the lower right corner of the size box with the lower right corner of the rectangle specified by the x, y, nWidth, and nHeight parameters of CreateWindowEx. The size box has the default size for system size boxes. Use this style with the SBS_SIZEBOX style.

SBS_SIZEBOXTOPLEFTALIGN
Aligns the upper left corner of the size box with the upper left corner of the rectangle specified by the x, y, nWidth, and nHeight parameters of CreateWindowEx. The size box has the default size for system size boxes. Use this style with the SBS_SIZEBOX style.

SBS_SIZEGRIP
Same as SBS_SIZEBOX, but with a raised edge.

SBS_TOPALIGN
Aligns the top edge of the scroll bar with the top edge of the rectangle defined by the x, y, nWidth, and nHeight parameters of CreateWindowEx. The scroll bar has the default height for system scroll bars. Use this style with the SBS_HORZ style.

SBS_VERT
Designates a vertical scroll bar. If you specify neither the SBS_RIGHTALIGN nor the SBS_LEFTALIGN style, the scroll bar has the height, width, and position specified by the x, y, nWidth, and nHeight parameters of CreateWindowEx.




SS_BITMAP
Specifies that a bitmap is to be displayed in the static control. The text is the name of a bitmap (not a filename) defined elsewhere in the resource file. The style ignores the nWidth and nHeight parameters; the control automatically sizes itself to accommodate the bitmap.

SS_BLACKFRAME
Specifies a box with a frame drawn in the same color as the window frames. This color is black in the default color scheme.

SS_BLACKRECT
Specifies a rectangle filled with the current window frame color. This color is black in the default color scheme.

SS_CENTER
Specifies a simple rectangle and centers the text in the rectangle. The text is formatted before it is displayed. Words that extend past the end of a line are automatically wrapped to the beginning of the next centered line. Words that are longer than the width of the control are truncated.

SS_CENTERIMAGE
If the static control contains a single line of text, the text is centered vertically in the client area of the control.
If the control contains a bitmap, the bitmap is centered in the control, and the control is not resized. 

SS_EDITCONTROL
Specifies that the static control duplicates the text-displaying characteristics of a multiline edit control. Specifically, the average character width is calculated in the same manner as with an edit control, and the function does not display a partially visible last line.

SS_ENDELLIPSIS
If the end of a string does not fit in the rectangle, it is truncated and ellipses are added. If a word that is not at the end of the string goes beyond the limits of the rectangle, it is truncated without ellipses. Compare with SS_PATHELLIPSIS and SS_WORDELLIPSIS.

SS_ENHMETAFILE
Specifies that an enhanced metafile is to be displayed in the static control. The text is the name of a metafile. An enhanced metafile static control has a fixed size; the metafile is scaled to fit the static control's client area.

SS_ETCHEDFRAME
Draws the frame of the static control using the EDGE_ETCHED edge style. For more information, see the DrawEdge function.

SS_ETCHEDHORZ
Draws the top and bottom edges of the static control using the EDGE_ETCHED edge style. For more information, see the DrawEdge function.

SS_ETCHEDVERT
Draws the left and right edges of the static control using the EDGE_ETCHED edge style. For more information, see the DrawEdge function.

SS_GRAYFRAME
Specifies a box with a frame drawn with the same color as the screen background (desktop). This color is gray in the default color scheme.

SS_GRAYRECT
Specifies a rectangle filled with the current screen background color. This color is gray in the default color scheme.

SS_ICON
Specifies an icon to be displayed in the dialog box. If the control is created as part of a dialog box, the text is the name of an icon (not a filename) defined elsewhere in the resource file. If the control is created via CreateWindowEx or a related function, the text is the name of an icon (not a filename) defined in the resource file associated with the module specified by the hInstance parameter to CreateWindowEx.
The icon can be an animated cursor.
The style ignores the CreateWindowEx parameters nWidth and nHeight; the control automatically sizes itself to accommodate the icon. As it uses the LoadIcon function, the SS_ICON style can load only icons of dimensions SM_CXICON and SM_CYICON. This restriction can be bypassed by using the SS_REALSIZEIMAGE style in addition to SS_ICON.
If an icon cannot be loaded through LoadIcon, an attempt is made to load the specified resource as a cursor using LoadCursor. If that too fails, an attempt is made to load from the device driver using LoadImage.

SS_LEFT
Specifies a simple rectangle and left-aligns the text in the rectangle. The text is formatted before it is displayed. Words that extend past the end of a line are automatically wrapped to the beginning of the next left-aligned line. Words that are longer than the width of the control are truncated.

SS_LEFTNOWORDWRAP
Specifies a simple rectangle and left-aligns the text in the rectangle. Tabs are expanded, but words are not wrapped. Text that extends past the end of a line is clipped.

SS_NOPREFIX
Prevents interpretation of any ampersand (&) characters in the control's text as accelerator prefix characters. These are displayed with the ampersand removed and the next character in the string underlined. This static control style may be included with any of the defined static controls. You can combine SS_NOPREFIX with other styles. This can be useful when filenames or other strings that may contain an ampersand (&) must be displayed in a static control in a dialog box.

SS_NOTIFY
Sends the parent window STN_CLICKED, STN_DBLCLK, STN_DISABLE, and STN_ENABLE notification messages when the user clicks or double-clicks the control.

SS_OWNERDRAW
Specifies that the owner of the static control is responsible for drawing the control. The owner window receives a WM_DRAWITEM message whenever the control needs to be drawn.

SS_PATHELLIPSIS
Replaces characters in the middle of the string with ellipses so that the result fits in the specified rectangle. If the string contains backslash (\) characters, SS_PATHELLIPSIS preserves as much as possible of the text after the last backslash. Compare with SS_ENDELLIPSIS and SS_WORDELLIPSIS.

SS_REALSIZECONTROL
Adjusts the bitmap to fit the size of the static control. For example, changing the locale can change the system font, and thus controls might be resized. If a static control had a bitmap, the bitmap would no longer fit the control. This style bit dictates automatic redimensioning of bitmaps to fit their controls.
If SS_CENTERIMAGE is specified, the bitmap or icon is centered (and clipped if needed). If SS_CENTERIMAGE is not specified, the bitmap or icon is stretched or shrunk.
Note that the redimensioning in the two axes are independent, and the result may have a changed aspect ratio.
Compare with SS_REALSIZEIMAGE.

SS_REALSIZEIMAGE
Specifies that the actual resource width is used and the icon is loaded using LoadImage. SS_REALSIZEIMAGE is always used in conjunction with SS_ICON.

SS_REALSIZEIMAGE
uses LoadImage, overriding the process normally followed under SS_ICON. It does not load cursors; if LoadImage fails, no further attempts to load are made. It uses the actual resource width. The static control is resized accordingly, but the icon remains aligned to the originally specified left and top edges of the control.
Note that if SS_CENTERIMAGE is also specified, the icon is centered within the control's space, which was specified using the CreateWindowEx parameters nWidth and nHeight.
Compare with SS_REALSIZECONTROL.

SS_RIGHT
Specifies a simple rectangle and right-aligns the text in the rectangle. The text is formatted before it is displayed. Words that extend past the end of a line are automatically wrapped to the beginning of the next right-aligned line. Words that are longer than the width of the control are truncated.

SS_RIGHTJUST
Specifies that the lower right corner of a static control with the SS_BITMAP or SS_ICON style is to remain fixed when the control is resized. Only the top and left sides are adjusted to accommodate a new bitmap or icon.

SS_SIMPLE
Specifies a simple rectangle and displays a single line of left-aligned text in the rectangle. The text line cannot be shortened or altered in any way. Also, if the control is disabled, the control does not gray its text.

SS_SUNKEN
Draws a half-sunken border around a static control.

SS_WHITEFRAME
Specifies a box with a frame drawn with the same color as the window background. This color is white in the default color scheme.

SS_WHITERECT
Specifies a rectangle filled with the current window background color. This color is white in the default color scheme.

SS_WORDELLIPSIS
Truncates any word that does not fit in the rectangle and adds ellipses.
Compare with SS_ENDELLIPSIS and SS_PATHELLIPSIS.




SBARS_SIZEGRIP
The status bar control will include a sizing grip at the right end of the status bar. A sizing grip is similar to a sizing border; it is a rectangular area that the user can click and drag to resize the parent window.

SBARS_TOOLTIPS
Use this style to enable ToolTips.




TCS_BUTTONS
Tabs appear as buttons, and no border is drawn around the display area.

TCS_FIXEDWIDTH
All tabs are the same width. This style cannot be combined with the TCS_RIGHTJUSTIFY style.

TCS_FLATBUTTONS
Selected tabs appear as being indented into the background while other tabs appear as being on the same plane as the background. This style only affects tab controls with the TCS_BUTTONS style.

TCS_FOCUSNEVER
The tab control does not receive the input focus when clicked.

TCS_FOCUSONBUTTONDOWN
The tab control receives the input focus when clicked.

TCS_FORCEICONLEFT
Icons are aligned with the left edge of each fixed-width tab. This style can only be used with the TCS_FIXEDWIDTH style.

TCS_FORCELABELLEFT
Labels are aligned with the left edge of each fixed-width tab; that is, the label is displayed immediately to the right of the icon instead of being centered. This style can only be used with the TCS_FIXEDWIDTH style, and it implies the TCS_FORCEICONLEFT style.

TCS_HOTTRACK
Items under the pointer are automatically highlighted. You can check whether or not hot tracking is enabled by calling SystemParametersInfo.

TCS_MULTILINE
Multiple rows of tabs are displayed, if necessary, so all tabs are visible at once.

TCS_MULTISELECT
Multiple tabs can be selected by holding down the CTRL key when clicking. This style must be used with the TCS_BUTTONS style.

TCS_OWNERDRAWFIXED
The parent window is responsible for drawing tabs.

TCS_RAGGEDRIGHT
Rows of tabs will not be stretched to fill the entire width of the control. This style is the default.

TCS_RIGHTJUSTIFY
The width of each tab is increased, if necessary, so that each row of tabs fills the entire width of the tab control. This window style is ignored unless the TCS_MULTILINE style is also specified.

TCS_SCROLLOPPOSITE
Unneeded tabs scroll to the opposite side of the control when a tab is selected.

TCS_SINGLELINE
Only one row of tabs is displayed. The user can scroll to see more tabs, if necessary. This style is the default.

TCS_TABS
Tabs appear as tabs, and a border is drawn around the display area. This style is the default.

TCS_TOOLTIPS
The tab control has a ToolTip control associated with it.




TBSTYLE_ALTDRAG
Allows users to change a toolbar button's position by dragging it while holding down the ALT key. If this style is not specified, the user must hold down the SHIFT key while dragging a button. Note that the CCS_ADJUSTABLE style must be specified to enable toolbar buttons to be dragged.

TBSTYLE_CUSTOMERASE
Generates NM_CUSTOMDRAW notification messages when the toolbar processes WM_ERASEBKGND messages.

TBSTYLE_FLAT
Creates a flat toolbar. In a flat toolbar, both the toolbar and the buttons are transparent and hot-tracking is enabled. Button text appears under button bitmaps. To prevent repainting problems, this style should be set before the toolbar control becomes visible.

TBSTYLE_LIST
Creates a flat toolbar with button text to the right of the bitmap. Otherwise, this style is identical to TBSTYLE_FLAT. To prevent repainting problems, this style should be set before the toolbar control becomes visible.

TBSTYLE_REGISTERDROP
Generates TBN_GETOBJECT notification messages to request drop target objects when the cursor passes over toolbar buttons.

TBSTYLE_TOOLTIPS
Creates a ToolTip control that an application can use to display descriptive text for the buttons in the toolbar.

TBSTYLE_TRANSPARENT
Creates a transparent toolbar. In a transparent toolbar, the toolbar is transparent but the buttons are not. Button text appears under button bitmaps. To prevent repainting problems, this style should be set before the toolbar control becomes visible.

TBSTYLE_WRAPABLE
Creates a toolbar that can have multiple lines of buttons. Toolbar buttons can "wrap" to the next line when the toolbar becomes too narrow to include all buttons on the same line. When the toolbar is wrapped, the break will occur on either the rightmost separator or the rightmost button if there are no separators on the bar. This style must be set to display a vertical toolbar control when the toolbar is part of a vertical rebar control. This style cannot be combined with CCS_VERT.




TBS_AUTOTICKS
The trackbar control has a tick mark for each increment in its range of values.

TBS_VERT
The trackbar control is oriented vertically.

TBS_HORZ
The trackbar control is oriented horizontally. This is the default orientation.

TBS_TOP
The trackbar control displays tick marks above the control. This style is valid only with TBS_HORZ.

TBS_LEFT
The trackbar control displays tick marks to the left of the control. This style is valid only with TBS_VERT.

TBS_BOTH
The trackbar control displays tick marks on both sides of the control. This will be both top and bottom when used with TBS_HORZ or both left and right if used with TBS_VERT.

TBS_NOTICKS
The trackbar control does not display any tick marks.

TBS_ENABLESELRANGE
The trackbar control displays a selection range only. The tick marks at the starting and ending positions of a selection range are displayed as triangles (instead of vertical dashes), and the selection range is highlighted.

TBS_FIXEDLENGTH
The trackbar control allows the size of the slider to be changed with the TBM_SETTHUMBLENGTH message.

TBS_NOTHUMB
The trackbar control does not display a slider.

TBS_TOOLTIPS
The trackbar control supports ToolTips. When a trackbar control is created using this style, it automatically creates a default ToolTip control that displays the slider's current position. You can change where the ToolTips are displayed by using the TBM_SETTIPSIDE message.

TBS_REVERSED
This style bit is used for "reversed" trackbars, where a smaller number indicates "higher" and a larger number indicates "lower." It has no effect on the control; it is simply a label that can be checked to determine whether a trackbar is normal or reversed.

TBS_DOWNISLEFT
By default, the trackbar control uses down equal to right and up equal to left. Use the TBS_DOWNISLEFT style to reverse the default, making down equal left and up equal right.

TBS_NOTIFYBEFOREMOVE
Trackbar should notify parent before repositioning the slider due to user action (enables snapping).

TBS_TRANSPARENTBKGND
Background is painted by the parent via the WM_PRINTCLIENT message.




TVS_CHECKBOXES
Enables check boxes for items in a tree-view control. A check box is displayed only if an image is associated with the item. When set to this style, the control effectively uses DrawFrameControl to create and set a state image list containing two images. State image 1 is the unchecked box and state image 2 is the checked box. Setting the state image to zero removes the check box altogether. For more information, see Working with state image indexes.
Once a tree-view control is created with this style, the style cannot be removed. Instead, you must destroy the control and create a new one in its place. Destroying the tree-view control does not destroy the check box state image list. You must destroy it explicitly. Get the handle to the state image list by sending the tree-view control a TVM_GETIMAGELIST message. Then destroy the image list with ImageList_Destroy.
If you want to use this style, you must set the TVS_CHECKBOXES style with SetWindowLong after you create the treeview control, and before you populate the tree. Otherwise, the checkboxes might appear unchecked, depending on timing issues.

TVS_DISABLEDRAGDROP
Prevents the tree-view control from sending TVN_BEGINDRAG notification messages.

TVS_EDITLABELS
Allows the user to edit the labels of tree-view items.

TVS_FULLROWSELECT
Enables full-row selection in the tree view. The entire row of the selected item is highlighted, and clicking anywhere on an item's row causes it to be selected. This style cannot be used in conjunction with the TVS_HASLINES style.

TVS_HASBUTTONS
Displays plus (+) and minus (-) buttons next to parent items. The user clicks the buttons to expand or collapse a parent item's list of child items. To include buttons with items at the root of the tree view, TVS_LINESATROOT must also be specified.

TVS_HASLINES
Uses lines to show the hierarchy of items.

TVS_INFOTIP
Obtains ToolTip information by sending the TVN_GETINFOTIP notification.

TVS_LINESATROOT
Uses lines to link items at the root of the tree-view control. This value is ignored if TVS_HASLINES is not also specified.

TVS_NOHSCROLL
Disables horizontal scrolling in the control. The control will not display any horizontal scroll bars.

TVS_NONEVENHEIGHT
Sets the height of the items to an odd height with the TVM_SETITEMHEIGHT message. By default, the height of items must be an even value.

TVS_NOSCROLL
Disables both horizontal and vertical scrolling in the control. The control will not display any scroll bars.

TVS_NOTOOLTIPS
Disables ToolTips.

TVS_RTLREADING
Causes text to be displayed from right-to-left (RTL). Usually, windows display text left-to-right (LTR). Windows can be mirrored to display languages such as Hebrew or Arabic that read RTL. Typically, tree-view text is displayed in the same direction as the text in its parent window. If TVS_RTLREADING is set, tree-view text reads in the opposite direction from the text in the parent window.

TVS_SHOWSELALWAYS
Causes a selected item to remain selected when the tree-view control loses focus.

TVS_SINGLEEXPAND
Causes the item being selected to expand and the item being unselected to collapse upon selection in the tree view. If the mouse is used to single-click the selected item and that item is closed, it will be expanded. If the user holds down the CTRL key while selecting an item, the item being unselected will not be collapsed.
Causes the item being selected to expand and the item being unselected to collapse upon selection in the tree view. If the user holds down the CTRL key while selecting an item, the item being unselected will not be collapsed.

TVS_TRACKSELECT
Enables hot tracking in a tree-view control.




UDS_ALIGNLEFT
Positions the up-down control next to the left edge of the buddy window. The buddy window is moved to the right, and its width is decreased to accommodate the width of the up-down control.

UDS_ALIGNRIGHT
Positions the up-down control next to the right edge of the buddy window. The width of the buddy window is decreased to accommodate the width of the up-down control.

UDS_ARROWKEYS
Causes the up-down control to increment and decrement the position when the UP ARROW and DOWN ARROW keys are pressed.

UDS_AUTOBUDDY
Automatically selects the previous window in the z-order as the up-down control's buddy window.

UDS_HORZ
Causes the up-down control's arrows to point left and right instead of up and down.

UDS_HOTTRACK
Causes the control to exhibit "hot tracking" behavior. That is, it highlights the UP ARROW and DOWN ARROW on the control as the pointer passes over them.

UDS_NOTHOUSANDS
Does not insert a thousands separator between every three decimal digits.

UDS_SETBUDDYINT
Causes the up-down control to set the text of the buddy window (using the WM_SETTEXT message) when the position changes. The text consists of the position formatted as a decimal or hexadecimal string.

UDS_WRAP
Causes the position to "wrap" if it is incremented or decremented beyond the ending or beginning of the range.
