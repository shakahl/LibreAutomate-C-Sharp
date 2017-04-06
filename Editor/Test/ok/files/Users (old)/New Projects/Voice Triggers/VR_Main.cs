
 Main function of Voice Triggers. Run it.
 Creates hidden dialog and tray icon. Click it.
 List of commands is in 'Voice Commands' menu. Edit it.
 Each item must be name of existing QM item (macro,
 menu, etc). Speek its name to execute.

typelib HSRLib {60462311-3373-11D1-8C43-0060081841DE} 1.0

int+ _hwndvr; if(_hwndvr and IsWindow(_hwndvr)) ret
int active=win
_hwndvr=ShowDialog("VR_Dlg" &VR_Dlg 0 0 17 WS_VISIBLE|DS_SETFOREGROUND)
act active
if(!_hwndvr) ret
MessageLoop
