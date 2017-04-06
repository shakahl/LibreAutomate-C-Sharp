str s=
 Error #333: HANDLE LEAK: USER Handle 0x57160427 was opened but not closed:
 # 0 system call NtUserSetWinEventHook
 # 1 snxhk.dll!?                                    +0x0      (0x6fa814c8 <snxhk.dll+0x114c8>)
 # 2 qmhook32.dll!HookUnhook  
 # 3 qmhook32.dll!_threadstartex                     [f:\dd\vctools\crt_bld\self_x86\crt\src\threadex.c:326]
 Note: @0:01:13.186 in thread 7936
 
 Error #335: HANDLE LEAK: USER Handle 0x0a6f1327 was opened but not closed:
 # 0 system call NtUserSetWinEventHook
 # 1 snxhk.dll!?                                    +0x0      (0x6fa814c8 <snxhk.dll+0x114c8>)
 # 2 qmhook32.dll!HookUnhook  
 # 3 qmhook32.dll!_threadstartex                     [f:\dd\vctools\crt_bld\self_x86\crt\src\threadex.c:326]
 Note: @0:01:13.187 in thread 7936
 
 Error #346: HANDLE LEAK: USER Handle 0x1ae6136f was opened but not closed:
 # 0 system call NtUserSetWinEventHook
 # 1 snxhk.dll!?                                    +0x0      (0x6fa814c8 <snxhk.dll+0x114c8>)
 # 2 qmhook32.dll!HookUnhook  
 # 3 qmhook32.dll!_threadstartex                     [f:\dd\vctools\crt_bld\self_x86\crt\src\threadex.c:326]
 Note: @0:01:13.249 in thread 7936
 
 Error #347: HANDLE LEAK: USER Handle 0x87be0271 was opened but not closed:
 # 0 system call NtUserSetWinEventHook
 # 1 snxhk.dll!?                                    +0x0      (0x6fa814c8 <snxhk.dll+0x114c8>)
 # 2 qmhook32.dll!HookUnhook  
 # 3 qmhook32.dll!_threadstartex                     [f:\dd\vctools\crt_bld\self_x86\crt\src\threadex.c:326]
 Note: @0:01:13.249 in thread 7936
 
 ERRORS FOUND:
       3 unique,     5 total unaddressable access(es)
      72 unique,   252 total uninitialized access(es)
       0 unique,     0 total invalid heap argument(s)
      18 unique,    67 total GDI usage error(s)
     234 unique,   351 total handle leak(s)
       0 unique,     0 total warning(s)
      11 unique,    15 total,   9566 byte(s) of leak(s)
      16 unique,    16 total,  30480 byte(s) of possible leak(s)
 ERRORS IGNORED:
    2136 still-reachable allocation(s)
          (re-run with "-show_reachable" for details)
 Details: C:\Users\G\AppData\Roaming\Dr. Memory\DrMemory-qm.exe.6684.000\results.txt

out
DrMemoryProcessResults s 4
out s
