 ABOUT

 Starts macro PM_AfterStandbyHibernate after Windows returns
 from hibernate or standby mode. Can be extended for other
 system events.


 SETUP

 Run PM_Main or this macro. Function PM_Main must be running
 all the time. To run automatically at startup, insert this
 in function init2 (if init2 does not exist, create):

mac "PM_Main"

 If you ever want to end PM_Main, run it again and click Exit.


 USAGE

 Edit PM_AfterStandbyHibernate to do what you need.


 EXTENDING

 On some system events (power management, system settings
 change, etc), certain messages are broadcasted to all
 windows. Then Windows calls function PM_Dialog. By default,
 it processes only power management messages. You can edit
 PM_Dialog to intercept other messages. You can edit
 PM_PowerManagement to intercept other power management
 events.

 These messages, documented in the MSDN Library, are
 broadcasted to all windows:
 WM_SETTINGCHANGE (supersedes WM_WININICHANGE),
 WM_POWERBROADCAST, WM_USERCHANGED (W9x?), WM_SYSCOMMAND
 (screensaver, etc), WM_DISPLAYCHANGE, WM_DEVMODECHANGE,
 WM_FONTCHANGE, WM_THEMECHANGED, WM_TIMECHANGE,
 WM_SYSCOLORCHANGE.
