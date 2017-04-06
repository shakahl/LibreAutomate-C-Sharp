
 Restores broken keyboard, mouse and autotext triggers.

 REMARKS
 For triggers QM uses hooks. It is a Windows programming interface. It is not 100% reliable, and sometimes some triggers may stop working.
 Possible reasons of broken hooks:
   1. Some other program uses hooks incorrectly, or its hooks interfere with QM. Can be even a QM macro that uses SetWindowsHookEx incorrectly.
   2. Small LowLevelHooksTimeout value in registry HKEY_CURRENT_USER\Control Panel\Desktop. Should be >=5000 (5000 is default). You can find more info on the Internet.
 If you call this function periodically, don't do it too frequently. Every >=1 minute should be OK.


men 33019 _hwndqm
