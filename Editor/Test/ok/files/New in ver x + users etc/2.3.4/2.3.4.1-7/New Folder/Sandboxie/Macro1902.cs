 dll "$desktop$\tcchook.dll" #Hook on
dll- "$desktop$\test_hook.dll" #Hook on

if Hook(1) ;;hook
	mes "Move/click mouse and see what comes to DebugView. Don't close this message box now.[][]At first download and run DebugView."
	Hook(0) ;;unhook
else mes "failed to hook"

UnloadDll "$desktop$\test_hook.dll"
