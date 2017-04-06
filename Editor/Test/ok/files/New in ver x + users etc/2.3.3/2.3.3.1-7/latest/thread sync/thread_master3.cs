 Need to run these functions in separate threads.

int+ g_state_of_thread_slave ;;0 normal, 1 pause

 start 3 threads
mac("thread_slave")
mac("thread_slave")
mac("thread_slave")

mes "The main macro is running.[][]To test other threads, run or activate Notepad." ;;show message box; it waits until you close

 end all threads
shutdown -6 0 "thread_slave"
