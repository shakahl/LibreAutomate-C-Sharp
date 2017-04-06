All guest OS are unregistered. For Win7-64-RC/Vista/2008 need to change date, or will stop working.

All the guest OS are without SP, except XP SP3 and 7 SP1.

Office 2010 requires Win7, Vista_sp1 or XP_sp3.

After installing an OS with changed date, turn off internet time updating.

To install new OS, use one of:
1. Player UI.
2. Edit template PC (see notes.txt in the folder).
3. Use easyvmx. Did not work last time.

If have Windows setup iso file, use it as secondary CD. Can set its path in virtual PC settings.

Windows 8 (8.1 OK): Installing tools makes screen black (OS unusable). Fix: When installing, choose Custom and uncheck SVGA driver.

For Windows 8/10 install Classic Shell. It adds Start menu etc.

On Windows 7, Windows Update hangs. To fix it, install latest KB: http://superuser.com/questions/944574/windows-update-doesnt-work-and-consumes-100-of-cpu-win7-sp1.
__________________________________________________________________

INSTALLING QM ON A GUEST PC AND SHARING FILES
To share files, use network (if works) or wmware shared folders (creates problems with locked files).
If using network, in virtual PC map \\Q7c(host PC)\Q to Q:.
Else:
	Shutdown PC (not sure, maybe don't need).
	In PC settings click Options tab, Shared Folders. Check "always enabled". Add Q:\ as Q. Not read-only (will set later).
	Run PC.
	Map "\\vmware-host\Shared Folders\Q" to drive letter Q. Instead could check "map as a network drive" in PC settings, but then app path would be "Z:\Q\app".
Create desktop shortcut to app (Q:\app) and qmsetup (Q:\app\qmsetup.exe /silent).
Run qmsetup. Let QM run to make sure that $myqm$ folder exists. Exit QM.
Copy "Q:\app\app_plus\QM in app.exe" to guest desktop. Or create shortcut to QM (Q:\app\qm.exe v).
Copy "Q:\app\app_plus\Main.qml" to guest $myqm$ (replace the default Main.qml). It is configured for guest PCs and has ok.qml as shared file. Instead could import ok.qml as shared file etc, but more work and cannot change System.qml props to load read-only copy.
If using wmware shared folders: In PC settings make the shared folder read-only. It is not necessary, but just for security.
Run QM in app. Make sure that opens (local copies of) shared files.
Note: admin processes cannot use mapped drives by default. It can be fixed, but I did not test: https://technet.microsoft.com/en-us/library/ee844140%28v=ws.10%29.aspx

__________________________________________________________________

2014-07-24
Newest QM installed on:
Win8.1
Win7-64
Vista-64
XP multimon

Also keep_time.exe is scheduled on all where need.
