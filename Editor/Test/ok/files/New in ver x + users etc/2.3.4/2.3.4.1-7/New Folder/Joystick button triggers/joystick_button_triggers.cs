/exe 1

 Runs a macro when a joystick button pressed. Also supports gamepad and other similar devices. Max 32 buttons.

 SETUP
 1. Test if this function is working:
    Run this function and press joystick buttons. It should show some debug info in QM output. If not, does not work.
    If works, set the debugMode variable to 0. Initially it is 1, which disables macros and instead shows debug info.

 2. Create list of macros here:
    Edit the example list below. Replace with names of your macros.
    Can be max 32 lines, one for a button. For buttons without macros, use ; like in the example.

 3. Run this function and test again whether the macros run when you press a button. If does not run, try to set debugMode=1 and see debug info.

 May not work if QM is running as admin. Try to run this function in separate process as user: add 1 space before /exe 1. Then after editing the list will need to end old thread.
 This function must be running all the time. Recommended trigger: 'QM events / QM file loaded'; don't check synchronous.
 You can edit the list later, at any time. After editing run this function again to apply changes.


 list of macros to assign to joystick buttons
str macros=
 button 1 macro
 button 2 macro
;
;
 button 5 macro
 button 6 macro


 change these variables if need
int debugMode=1 ;;if nonzero, does not run macros, but shows pressed button etc in QM output
int joystickID=0 ;;can be from 0 to number of attached joysticks - 1
double pollInterval=0.02 ;;if sometimes skips button, try 0.01. Smaller values are bad for your computer performance.

 don't change code below
 ____________________________________

int+ __jt_stop=1; rep() if(getopt(nthreads)>1) 0.1; else __jt_stop=0; break ;;stop trigger thread if already running. An easy way to restart after editing this macro.

JOYINFOEX j.dwSize=sizeof(j); j.dwFlags=JOY_RETURNBUTTONS
if(joyGetPosEx(joystickID &j)) out "joystick_button_triggers: joystick %i is not attached" joystickID; ret

int buttons i mm
ARRAY(str) a=macros; if(a.len>=32) err end "max 32 buttons supported"
for(i 0 a.len) if(a[i].len) mm|1<<i ;;create mask of buttons with assigned macros

rep
	wait pollInterval
	if(__jt_stop) ret
	if(joyGetPosEx(joystickID &j)) continue
	int k=j.dwButtons^buttons&j.dwButtons; buttons=j.dwButtons; if(!k) continue
	if(debugMode) outx k
	if(k&mm=0)
		if(debugMode) out "no macros are assigned to this button"
		continue
	for(i 0 a.len) if(k>>i&1) break
	if(debugMode) out "macro '%s' is assigned to button %i" a[i] i+1
#if !EXE
	int iid=qmitem(a[i])
	if(!iid) out "joystick_button_triggers: macro '%s' not found" a[i]; continue
	if(dis(iid) or dis&0x90)
		if(debugMode) out "the macro or QM is disabled, or user-defined triggers disabled in Options"
		continue
	mac iid; err out _error.description
#else
	str cl=F"M ''{a[i]}''"
	int w=win("" "QM_Editor")
	if(w) SendMessageW w WM_SETTEXT 1 @cl; else run "qmcl.exe" cl
#endif

 BEGIN PROJECT
 guid  {6721B8CA-2E6C-450C-B917-45AE01AAF8D4}
 END PROJECT
