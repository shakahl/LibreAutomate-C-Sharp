 /
function [$threadName] [threadHandle] [flags] ;;flags: 1 thread id, 2 unique id, 4 activate "End thread" trigger, 8 synchronous

 Ends a thread (running macro or function).

 threadName - name of the function or macro that started the <help #IDP_THREADS>thread</help>.
   Also can be QM item id (use operator +, like +iid).
   If sub-function, can be like "ParentName:SubName".
 threadHandle - thread handle or id.
 flags:
   0 - threadHandle is thread handle.
   1 - threadHandle is thread id.
   2 - threadHandle is unique thread id.
     Note: Thread handle, id and unique id are not a window handle or QM item id. Read more in Remarks.
   4 - if a function has trigger "QM events -> End thread" for the thread, run the function. 
   8 - synchronous. Wait until the thread is ended. 

 REMARKS
 Use this function to end another thread. To end current thread, you can use <help>ret</help>, <help>end</help>, <help>shutdown</help> -7, etc.
 If threadName and threadHandle not used (omitted, "" or 0), ends the currently running macro.
 If only threadName used (threadHandle omitted or 0), ends all threads (all running instances of the function).
 If threadHandle used, ends only that instance. Then threadName is optional, can be "".
 Not error if the thread is not running.
 This function cannot end special threads. Cannot close toolbars (use clo instead).
 To start new thread and get its handle, use <help>mac</help>. To get thread handle, id and unique id, you can use <help>EnumQmThreads</help> or <help>GetQmThreadInfo</help>. Don't use GetCurrentThread, DuplicateHandle. A thread handle identifies thread only while it is running. Later (after > 3 seconds) the same value can be reused (assigned to a new thread). Thread id also can be reused. Unique thread id is not reused.

 Added in: QM 2.3.3. In older versions you can use <help>shutdown</help> -6.

 See also: <IsThreadRunning>.

 EXAMPLE
 mac "FunctionX" ;;run function "FunctionX" in separate thread
 wait 5
 EndThread "FunctionX"


if(threadName&0xffff0000) shutdown -6 flags threadName threadHandle
else _i=threadName; shutdown -6 flags _i threadHandle

err+ end _error
