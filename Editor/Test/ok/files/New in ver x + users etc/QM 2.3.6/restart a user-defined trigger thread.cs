shutdown -6 0 "clipboard_copy_triggers"
0.5
 mac "clipboard_copy_triggers" ;;will not work. The thread must be started by the trigger management function, which must be called by QM; QM passes trigger data to it.
 Need to restart QM, or reload file.
 Or add a trigger of this type:
int iid=newitem("" "" "" "^Clipboard_copy ``" "\User\Temp" 128)
newitem iid "" "" "" "\User\Temp" 32 ;;delete
