function! IVirtualDesktopManager_Raw&m

IVirtualDesktopManager_Raw- __t_vdeskman._create("{aa509086-5ca9-4c25-8f95-589d3c07b48a}"); err
m=__t_vdeskman
ret __t_vdeskman!=0

 #opt nowarnings 1
 IVirtualDesktopManager_Raw+ __vdeskman
 if !__vdeskman
	 lock
	 if(!__vdeskman) __vdeskman._create("{aa509086-5ca9-4c25-8f95-589d3c07b48a}"); err ret
	 lock-
 m=__vdeskman
 ret 1

 Possibly unsafe to use global. Anyway, even then the first method call is quite slow first time in thread.
