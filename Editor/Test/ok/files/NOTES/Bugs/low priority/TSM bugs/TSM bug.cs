/exe
act "Notepad"
0.5
Key 'A'
Key 'S'
 Key 'T'
 0.01
Key 'T'

 Assume we have a TSM that triggers an item when you type "as". It then uses key or outp.
 If keys are typed very fast, eg by the above macro, two bad things can happen:
 1. If fastest (no delay between S and T), T arrives before erasing, and is typed. But erased are only the number of chars typed before. That is, T is erases and A is not erased.
 2. If there is some delay between S and T, T is inserted between text typed by the menu item. Maybe it is not filtered/retyped because arrives to the LL kbd hook before the menu is activated.

 It is difficult to solve these problems when using kbd trigger, especially if LL.
 Possible solution - use text trigger. It would watch for WM_CHAR...


 BEGIN PROJECT
 main_function  Macro541
 exe_file  $my qm$\Macro541.exe
 icon  $qm$\macro.ico
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {CAB87572-B014-4734-BEFC-6AB04F610490}
 END PROJECT

#if 0

