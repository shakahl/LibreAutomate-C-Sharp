 \
function hwnd [flags] ;;flags: 1 also hide window, 2 also remove from Alt+Tab

 Hides the taskbar button and optionally the window.


#compile MTB_Main
if(!IsFunctionRunning("MTB_Main")) mac "MTB_Main"; 1
g_taskbar.HideButton(hwnd flags)
