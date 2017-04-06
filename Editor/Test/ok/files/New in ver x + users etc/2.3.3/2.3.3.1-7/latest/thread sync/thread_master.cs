 start 3 threads
mac("thread_slave")
mac("thread_slave")
mac("thread_slave")

mes "The main macro is running.[][]To test other threads, run or activate Notepad.[][]Click OK to end all." ;;show message box; it waits until you close

 end all threads
shutdown -6 0 "thread_slave"
