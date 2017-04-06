Scope:
   Add window. Eg Trigger /EXE win("name" "class"). Or Trigger /win("name" "class" "exe").
   Add options: 1. If a window of that program is active. 2. If mouse is over a window of that program.
   Can be defined named trigger scopes (scope presets) that can be selected for various triggers.
   Computer, user, OS.
   FF etc for all trigger types.
   FF with arguments.

Trigger: on error. Or when a macro ends, regardless how.
   The function would be called on error in all macros or in all that have specified parameters, etc.
   It would receive error info, and can change something...
   http://www.quickmacros.com/forum/viewtopic.php?f=1&t=5626
   + call stack.

Trigger: Internet or certain website available/unavailable. Call InternetCheckConnectionW at user-specified interval.

Trigger: web page opened/closed. Maybe other browser events.
   Eg Firefox fires acc event, and changes acc state of DOCUMENT.
   Probably better use user-defined trigger.

Keyboard and mouse triggers: device-specific. Like now keyboard detector. Try libusb: http://www.libusb.org/.

Trigger: scheduler (internal, simple). Try SetWaitableTimer.

Triggers: PC lock, user switch, power, hibernate, standby.

Trigger: broadcasted messages, eg WM_POWERBROADCAST, WM_DEVICECHANGE, RegisterDeviceNotification.

Trigger: Drive insert/eject. use WM_DEVICECHANGE, see in New Projects.

acc triggers: support all events. Easy.

acc does not work in filter functions.
	Maybe possible to make work for window triggers. Eg make FF run async.
	But anyway often will not work because accessible objects are still not inited then.
	Also could add acc option to window triggers, but difficult because of the above problem.
	Eg in Vista message boxes, eg Notepad, text is not control but accessible object.

Make clipboard triggers intrinsic? This is from Forum Wish List.

Trigger: mouse hover. Eg if hover on a link, show menu of actions.

Triggers: tray (add, change, click, etc). Maybe by injecting code.

Triggers: add items to window system menu. Suggestion. Command handling could be in qmhook dll.

Triggers: email (maybe ext.).

Triggers: mouse gestures (like StrokeIt).

Triggers: WMI.

Trigger: mouse moved into other monitor.

Triggers: registry. Use CmRegisterCallback (documented in online MSDN (maybe in DDK help too)) in driver. See also a sample: http://www.codeproject.com/vista/vista_x64.asp

Triggers: raw input (WM_INPUT). But cannot eat.

Double click triggers: should be option to eat first button (useful if with mod keys).

Triggers: html element events.

When a file dropped on QM, show menu with various options. Add @ trigger for the menu. (pi suggestion).

Shell menu triggers: option to run macro when menu shown. Somebody in forum wanted it.

Triggers: Middle mouse button left/right.
   The middle mouse button on many mouses can pushed to the left or right...

_______________________

Window "Visible" trigger shoul work after creating too. Like "Active".

Option to not disable trigger when QM is disabled. Especially useful for toolbars.

If trigger scope ends with comma, add container scopes.

Mouse click triggers: chord. For example, right+any or middle+any would be possible.

Notify about Task Scheduler failures.

Scheduler triggers: for all users. Run as SYSTEM, and start qmcl on default desktop.

TFF should not use SendT (use events instead). Maybe then Acc functions will work.

To pass info from all sources from other processes, use single thread. Use events and SM. Including TFF.

Folder options: enable triggers only for these users ...

init should run before MakeTables. Then init2 could enable-disable folders. Eg, now is warning if file trigger path is invalid. Also, could use constants and global vars in triggers.

Check exe in trigger string. Must be max 31 char, must not contain \/etc, warning if ends with .exe.

Process triggers: same as above.

Event log triggers should work on nonadmin accounts. (low priority) (fortunately, on Vista, works on admin account under UAC)

Key triggers should work with sticky keys. Now does not work if pressed separately. Macro Express too.

Mouse wheel triggers: add option "program from mouse" (now "active program").

Trigger: horizontal wheel. Needs hardware.

__________________________________________________

	REJECTED

Triggers: certain message sent to certain window.

Triggers: performance counters. Arbitrary counters can be specified. But probably can be implemented through performance logs and alerts.

Triggers: joystick.

Triggers: serial port, IR port.

Trigger: Windows 7 gestures.
  Impossible or almost impossible.
  Don't remember exactly, but probably Windows send messages to the active window, and we cannot intercept, unless we have sendmsg hook.

__________________________________________________

	NOTES

Useful messages: WM_SETTINGCHANGE (supersedes WM_WININICHANGE), WM_POWERBROADCAST,
WM_USERCHANGED (W9x?), WM_SYSCOMMAND (screensaver, etc), WM_DISPLAYCHANGE,
WM_DEVMODECHANGE, WM_FONTCHANGE, WM_THEMECHANGED, WM_TIMECHANGE, WM_SYSCOLORCHANGE

