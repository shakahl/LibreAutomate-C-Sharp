 Note: don't need it in QM 2.3.4 and later.

 Registers MailBee COM component (mailbee.dll) that is required for QM email functions.
 Use this function in exe. Call somewhere at the beginning of your exe code. The registration is valid while current thread is running.
 When QM is running in QM, this function does nothing. MailBee is registered on your computer.
 Error if fails.

 On Windows XP and later, registers only for use by your exe. It's clean and reliable. Does not need admin.
 On Windows 2000 tries to register as usually, unless already registered. Exe must run as admin, or fails. Does not unregister later, it's not important.

 Requires class __UseComUnregistered.
 Also you need a manifest file created by __UseComUnregistered_CreateManifest.
 When exe runs, in its folder must be these files: mailbee.dll, mailbee.X.manifest.

 EXAMPLE
 ExeEnableMailFunctions
 err mes- "Failed to register MailBee component" "Test Mailbee" "x"


#if EXE

#compile "____UseComUnregistered"
__UseComUnregistered-- x.Activate("mailbee.X.manifest")
err ;;fails on Windows 2000 and XP SP0
	if rget(_s "" "MailBee.SMTP\CLSID" HKEY_CLASSES_ROOT)!=39 ;;if not already registered
		if !RegisterComComponent("$qm$\mailbee.dll") ;;succeeds only if exe runs as admin
			end ERR_FAILED
