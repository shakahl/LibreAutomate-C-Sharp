VMware Player :RunVmx "$pf$\VMware\VMware Player\vmplayer.exe" *"$pf$\VMware\VMware Player\vmplayer.exe"
VM :run "Q:\VM"
 Virtual PC :run "$program files$\Microsoft Virtual PC\Virtual PC.exe"
-
Win8.1-64 :RunVmx "%VM%\win8.1-64\Win8.1-64.vmx" *.vmx
Win7-64 :RunVmx "%VM%\Win7-64\Win7-64.vmx" *.vmx
Win7-64 Ent Eval :RunVmx "%VM%\Win7-64 Ent Eval\Win7-64 Ent Eval.vmx" *.vmx
XP SP3 :RunVmx "%VM%\XP SP3\XP SP3.vmx" *.vmx
>other
	Vista-64 (changes date) :RunVmx "%VM%\Vista_64\Vista_64.vmx" *.vmx
	 Win8-64-Lang :RunVmx "%backup%\VM\win8-64-Lang\Win8-64-Lang.vmx" *.vmx
	XP SP0, multimonitor :RunVmx "%VM%\XP SP0\XP.vmx" *.vmx
	Win10-32 (USB drive) :RunVmx "%backup%\VM\Win10-32\Win10-32.vmx" *.vmx
	Win8-64-Lang :RunVmx "Q:\VM\Win8-64\Win8-64-Lang.vmx" *.vmx
	 Win8-64 (changes date; cannot start: bad SVGA driver of tools) :RunVmx "%VM%\win8-64\Win8-64.vmx" *.vmx
	 Win2008 (changes date) :RunVmx "%VM%\Win2008\Win2008.vmx" *.vmx
	-
	Virtual PC notes :mac+ "Virtual PC notes"
	<
-
Restore date :VmwareDate "" 0 1
delete file cache of all drives :mac "delete file cache of all drives"
 -
 Enable disk2 :EnableDisk2
 Disable disk2 :EnableDisk2 1
