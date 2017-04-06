 Java Access Bridge (JAB) allows Windows applications to communicate with UI objects in Java windows. Find, get properties, click, etc.
 Java accessible objects are similar to Windows accessible objects that QM uses. However need other functions to use them.
 The functions (API) are declared in macro "__jab_api" and can be used through a reference file JAB.
 Each macro that uses the API must call QM function JabInit. Read more in JabInit help. See examples in "test" folder.

 Java must be installed, and Java Access Bridge enabled.
 When you install Java, it also installs Java Access Bridge. However it is initially disabled. To enable, run the next macro.
 Old Java versions (before v7.u6) don't install JAB. You can see Java version in Control Panel -> Java. Java usually is in $program files$\Java. To install JAB, you can update Java (recommended) or download just JAB and install it manually.

 Currently tested only several JAB functions and events. Tested on Windows XP, Windows 7 32-bit, and Windows 8 64-bit with 32-bit Java. Tested with Java 7 update 21. If only 64-bit Java is installed (unlikely, not tested), probably also need to install 32-bit JAB for 64-bit Windows.

 JAB functions and callback functions use type JOBJECT64 as a reference to a Java accessible object (UI object). Also use JAB.AccessibleContext and several other types that are derived from JOBJECT64. They are actually the same as JOBJECT64 and can be assigned to each other etc.
 Be careful with JOBJECT64 (and the related types). It has different size on 32-bit and 64-bit Windows. On 32-bit it is same as int (32-bit integer). On 64-bit it is same as long (64-bit integer).
 This should not create many problems when creating macro that runs only in QM (not in exe) or in exe that runs only on the same PC. However exe created on 32-bit Windows runs only on 32-bit Windows, and created on 64-bit Windows will run only on 64-bit Windows. You may need to create 2 exe files - for 32-bit and for 64-bit Windows.
 Also don't forget to call JAB.ReleaseJavaObject for each object that JAB API or events give to you.

 JAB API use Unicode UTF-16 strings. Such strings in QM are declared as word* (or @*). In types often declared as @array[n]. See how the test macros/functions convert them to/from str.

 To receive events (callbacks), the thread should process messages (have a dialog, or wait with opt waitmsg 1, etc). To call JAB functions it is maybe not necessary, but try it if something does not work.

 Easier to use QM functions/classes probably will be added in the future. Currently there is only JabInit and maybe several other.
