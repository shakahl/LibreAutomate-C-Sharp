 ABOUT

 This macro installs QM portable version in the specified removable drive (eg USB drive). Copies files, current settings, etc.
 If portable QM is already installed in the drive, this macro reinstalls it. It replaces existing files. You can keep or replace your portable macro list file.
 Make sure that your current QM settings (qml file, settings in Options dialog, toolbars, etc) are correct, because portable QM will use them.
 If you have PortableApps.com (http://portableapps.com) installed in the drive, Destination should be like E:\PortableApps.

 Then you can run QM on computers where it is not installed.
 To run QM there, double click qm_pe.exe. If you have PortableApps.com, you can run QM from its menu.
 Portable QM will use the copied files and settings. It will be temporarily registered using your registration code.
 Your QM data folder ($my qm$) will be "QuickMacrosPortable\Data\My QM" in the drive. Your macro list file will be Main.qml in the folder.
 If you change QM settings while running portable QM, the changes will not be saved. Changes in macros will be saved.

 ______________________

 SOME QM FEATURES WILL BE UNAVAILABLE

 Some QM features are unavailable in portable version, because QM is not properly installed.
 To install QM with all features, would need to run QM setup program as administrator, which means that QM could not be used on limited user accounts.

 These QM features don't work:

 1. Vista/7 integrity levels and other UAC-related things. On Vista/7:
       QM runs as launched (not as specified in Options). By default - as User. To run as Administartor, right click qm_pe.exe and click 'Run as Administrator'. Or Ctrl+click QM icon in PortableApps.com menu.
       Programs launched by macros (run) run as QM. Normally would run as User or as specified by flags. Not tested how behaves (error or ignores) if integrity level flags specified.
       If a macro is set to run in separate process, it can run only as QM. Not tested how behaves (error or ignores) if specified different integrity level.
       Function StartProcess() fails.
       Function web() may have problems. Also wait() with I.
 2. Process triggers.
 3. Shell menu triggers.
 4. Temporary unlock computer.
 5. At startup (and possibly later) QM output shows error messages, such as "QM service not running", "Failed to set process notifications". Ignore them.
 6. Paths of qm.exe and qmcl.exe not registered, so macros and applications that want to run them may have to use their full paths instead of file name.
 7. .qml file type not registered.
 8. No QM shortcuts in Start menu.

 Most of this is not tested.
 Possibly there are more unavailable features.

 ______________________

 NOTES

 Portable QM will not run on computers where QM is installed.

 QM must be registered on your computer (where you run this macro). On other computers QM does not have to be registered.

 Always exit portable QM before removing the drive or shutting down the computer.
 Otherwise it will leave some files and registry data on the computer. Also, some programs may crash.
 If these bad things happened, run portable QM again, and exit it properly.

 Tested only with QM 2.3.1.

 Portable QM takes about 10 MB in the drive.

 ________________________________________________

mac "QP_Setup"
