 Make sure that version is correct:
    Change QMVER.
    Run macro "QM version". Only when changed x.x.X, not for each x.x.x.X.
    Edit Help -> What's new.
    Make sure that year is correct in About box. There is no year in QM help.
 Make sure that:
    Must be Release version. Other projects too.
    All z, zz, zp, stop=1 and other debug code is removed.
    Obsolete code removed (search for !!rem).
    Functions are encrypted (search for !!encrypt).
    Deleted test code in test.cpp etc.
 Make sure that everything supports:
    Unicode and ANSI mode.
    Special folders.
    File functions should support "resource:name" and ":resid filename" syntax.
    LL and non LL mode (triggers, autotext, key).
    Admin, User, UIAccess mode.
    Portable QM.
    High DPI (scaled windows).
 Run macros AreAllQmExportedFunctionsInExe and AreAllSystemFunctionsAvailableInExe.
 Compile all System functions:
    Check menu Run -> Compiler options -> 'Show declarations...' to make sure that System does not load WINAPI. Also check 'Show loaded dlls...'.
    Open Main.qml and run macro "Compile All".
    Uncheck the checkboxes and open Ok.qml again.
    If there are new declarations in output, it means that WINAPI was used. Copy them to \System\Declarations\... It's OK if shows sqlite and SCI (they are not from WINAPI).
 Test:
    Tools (at least, run macro "test tools")
    Samples
    All new features and changes. Also on XP and other OS.
    All menu items, tray menu too
    Options
    All other QM dialogs (My Macros, Icons, etc)
    Recording, also in other OS
    Is "make exe" working well. What is exe size.
    New features in exe. For example, if uses a dll.
 Ocassionally:
    Use Spy++ to ensure that messages are not sent too frequently. Scintilla likes to do it. Test when QM hidden.
    Run with AppVerifier.
 Review:
    Categories
    Type-info popup (private functions must be hidden)
    Help and tips (last reviewed when releasing QM 2.4.0, partially 2.4.1)
 See what tasks are left in \Notes\Todo, \Notes\Bugs, etc
 Build all projects. Build 64-bit versions.
   Note: CLEAN/REBUILD SOLUTION, because some projects may be built in Debug config and VS sometimes does not rebuild if we don't clean.
 Test on all OS, 32-bit and 64-bit.
 Test all exe/dll etc files with all antivirus programs.
    Use www.virustotal.com (more AV) or virusscan.jotti.org (faster).
    Test only quickmac.exe, but ocassionally also test all exe/dll files (if many files, pack in zip).
 Compile help.
 Open the setup script (quickm21.iss) in Inno Setup:
    Review, add new files if need, update QM version string if need.
    Compile.
    Check setup file size. If significantly bigger than previously, maybe some dlls are Debug (see above, need to clean/recompile solution).
    Signing:
       Inno Setup is configured to run function "sign" to sign all QM exe/dll and uninstaller automatically. Also it does some other tasks.
       Make sure all QM exe/dll that must be signed are specified in function "sign". Make sure that signs without errors.
    Options -> Check extensions. Do it after creating setup, because the "sign" macro modifies some files.
 Install as upgrade. Install as new (uninstall at first). Test some features. Install on other OS.
 If not beta:
    Google for shared serials, and blacklict (add to macro "__Protect").
    Backup previous version's quickmac.exe (in qm website, rename to old_versions/quickmac-X-X-X-X.exe). Note that Filezilla moves file on Ctrl+drag.
 Upload.
 Download/install. Do it on several OS, with various web browsers.
   Observe SmartScreen/antivirus behavior. Eg SmartScreen and IE blocks quickmac.exe when my certificate is new, because need some time to gain reputation.
 Web help (run HelpMakeOnlineContents, upload (sync) all files)
 Web pages (at least update version and date in index.html and download.html).
 Announce in forum.
 Edit/upload version.php. Test (run macro "test CheckForNewQmVersion").
 Make backups.
 If not beta:
    Delete beta/quickmac.exe (links to missing files are redirected to index.html). If beta, delete alpha and dev.
    Create pdf help: 1. Run macro "Create QM Help pdf". 2. Add to "QM Help 2-X-X.zip". 3. Upload to com folder. 4. Add link in http://www.quickmacros.com/forum/viewtopic.php?f=2&t=5133. Note: cannot add as attachment because now max php upload file size is 2 MB; I can see it in CP, but cannot change.
    Update PAD file:
       Go to appvisor.com (main toolbar button), update, validate.
       Submit (optional, see below, but may be useful, eg some new download sites find software there). Later check publishing status, because it is not immediate. Note: the old xml files now are redirected to appvisor (in Control Panel).
       They respond after several days. If want faster, in appvisor.com export it, then with FileZilla upload to http://www.quickmacros.com/Quick_Macros_pad.xml, and use this url when submitting to download sites.
    After several days submit to software directories where not updated automatically.
