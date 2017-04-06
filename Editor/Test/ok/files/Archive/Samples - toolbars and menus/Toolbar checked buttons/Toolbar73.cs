 /hook QmToolbar_HookCheck
Check1 :out QmToolbar_IsChecked ;;[check c1] * cut.ico
Simple button :out "simple"
 comment
Check2 :out QmToolbar_IsChecked ;;[check c2] * copy.ico
Check, don't save :out QmToolbar_Check(2) * paste.ico
Enable macro :if(QmToolbar_IsChecked) dis- "Macro1111"; else dis+ "Macro1111" ;;[check enableMacro]
