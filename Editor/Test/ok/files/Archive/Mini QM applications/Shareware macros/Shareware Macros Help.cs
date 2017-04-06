	 Shareware protection system for macros, version 1.0.3.

 _____________________________________________________________

	 WHAT IS IT?

 Many applications, including Quick Macros, are distributed as shareware. That is,
 everybody can download and use the application for free for certain amount of time.
 When the evaluation period expires, the application locks itself and refuses to run.
 If the user wants to use the application beyond the evaluation period, he/she
 buys a license. The software owner generates an unique registration code and gives
 it to the user. The user uses the regcode to unlock the application.

 The same can be applied to macros. If you have created a set of macros and functions
 that can be useful to others (for some particular purpose, or universal), you
 can sell them as a shareware product. But you must somehow prevent using your macros
 beyond the evaluation period, or your bussiness will be certainly unsuccessful.

 Use functions SM_About, SM_CanRun and SM_keygen as templates to create your unique
 protection system for your product. It allows anybody to use your macros for 15 days
 (configurable). Then it instead shows a dialog where the user can enter registration
 code. To avoid regcode sharing, you can generate computer-specific regcodes.


	 ADDING THE PROTECTION SYSTEM TO YOUR PRODUCT

 1. Run sample macro SM_Macro107 to see how the shareware protection works. Run SM_About,
 try changing computer date, run SM_keygen to generate a sample regcode, enter it, etc.

 2. Rename SM_About and SM_CanRun functions. Give some unique names that would not conflict
 with functions of other authors of shareware macros (you will distribute these functions).
 For example, replace SM to your initials plus several random numbers. Example: AB79_About.

 3. Add ..._About and ..._CanRun functions (the renamed SM_About and SM_CanRun functions) to
 your distribution package. For example, move the "Distributed functions" folder to the folder
 that contains your product's macros.

 4. Edit ..._About, ..._CanRun and SM_keygen. All that you have to edit is described there.

 5. Insert the following line in one or more macros and functions (at/near to the beginning)
 to prevent running them after the evaluation period. Of course you must change SM_CanRun to
 the name of the renamed function, and 1522539484 to the value of sm_ret that you have defined
 in your ..._CanRun.
if(SM_CanRun!=1522539484) end

 6. Your shareware protection system is almost complete. Test how it works.

 7. Encrypt ..._About and ..._CanRun functions. Encrypt macros and functions of your product.


	 GENERATING REGISTRATION CODES

 To generate registration codes for your customers, run the SM_keygen function.
 Of course you don't distribute this function. Distribute only ..._About and ..._CanRun.


	 COMPUTER-SPECIFIC REGISTRATION CODES

 You can generate registration codes that work only on single computer. The About dialog shows
 computer ID (part of the serial number of drive c:). The user gives it to you. You use it when
 generating registration code for that user.

 This feature prevents regcode sharing, but can make problems. The regcode becomes invalid if
 the user changes or reformats drive c:. In such case, shows a message advising to contact you.
 Then the user should send you his/her full name and new computer ID, and you generate him/her
 new registration code.

 Of course, you can also generate simple regcodes (in the generator, leave Computer ID empty).
 Your existing customers that used previous versions of the shareware protection system don't
 need new regcodes, but their regcodes are not computer-specific.


	 FREQUENTLY ASKED QUESTIONS

 Q. Must my customers have Quick Macros license to run my macros, even if they will
 not use QM for other purposes?
 A. Yes. Also read next 3 questions and answers.

 Q. Can I purchase an unlimited site license to use Quick Macros with my product?
 A. No.

 Q. Can I purchase single QM license and give it to all my customers?
 A. No. It is illegal. You'll have problems when the registration code will be blacklisted.

 Q. Can I purchase multiple QM licenses with a discount and give them to my customers?
 A. Currently QM licenses are not sold with a discount.

 Q. Is there a QM ActiveX component or dll that I could use to run my macros or from my software?
 A. Currently no. Your customers must have QM running and registered.

 Q. What Quick Macros version is required to use this shareware protection system?
 A. You and your customers must have QM 2.1.5 (Feb 2, 2005) or later.

 Q. Must my customers have the System folder for this protection system to run?
 A. Yes. It is required to show the dialog, and for several constants. Generally, it is part of QM,
 and I don't see a reason to remove it from the list of macros.

 Q. I encrypted my macros, so they cannot be viewed and modified. Is there any way to protect them
 from deleting, moving, renaming and opening the Properties dialog?
 A. Make your product's folder read-only. Of course, the user can remove the read-only attribute,
 if only he/she discovers that it can be removed in the Folder Properties dialog.

 Q. Can my macros be decrypted without knowing the password?
 A. No.

 Q. My dialog does not run when encrypted.
 A. Place the dialog definition in a nonencrypted macro, or assign it to a variable (read QM Help).

 Q. Where the regcode and first run date are stored?
 A. In the registry, HKEY_LOCAL_MACHINE hive. You must specify the registry keys in ..._CanRun function.

 Q. I don't know how to work with the registry. How can I choose the registry keys?
 A. You can just replace Company\Product to some unique string. It will give enough protection.
 If you would like to hide the first run date somewhere deeper, read the following.
 You can view the registry with regedit. To run it, click Start -> Run, type regedit and click OK.
 Working with the registry is similar to working with Windows Explorer. Those folders are called "key".
 Root folders, eg HKEY_LOCAL_MACHINE, are called "hive". To specify some folder deep in the tree,
 specify its full path, like for a file system folder. Do not include the hive name. The key don't have
 to (and should not) exist. QM creates the specified key like a new folder.

 Q. When my product is registered on one of user accounts, is it automatically registered for all
 accounts of that computer?
 A. Yes. Also, your product must be registered on an admin or power
 user account. QM prompts to swith account if necessary.

 Q. What administrative privileges are required for the protection system to run?
 A. QM must be able to create keys and values in the registry HKEY_LOCAL_MACHINE hive.
 This requires administrator or power user privileges. QM writes to the registry
 only two times - when running your macros first time on that computer, and when the user enters the
 regcode. If QM fails to write to the registry, it prompts the user to try log in an admin account.

 Q. The SM_ functions are public. Does it mean that anybody could download them and generate regcodes
 for my product themselves?
 A. No. Because you use your unique encryption key, and nobody knows it.

 Q. Can I edit these functions more? For example, replace the regcode generation code?
 A. Yes, of course. But be careful. It is not necessary to replace the default regcode generator,
 because your unique encryption key makes your shareware protection system unique.


	 CHANGES
 _____

 1.0.1.

 Fixed bug causing the About dialog to not work when encrypted.
 _____

 1.0.2

 Supports computer-specific registration codes.

 More clear and secure.

 Changed SM_CanRun usage. In SM_About, you need to change the value that is assigned to sm_ret.
 In protected macros, you have to replace  #if SM_CanRun  to  if(SM_CanRun!=thevalue) end
 _____

 1.0.3:

 Can be used in exe too.

 Uses registry hive HKEY_CURRENT_USER instead of HKEY_LOCAL_MACHINE. Using local machine would fail on Vista or when running as a limited (non admin) user.
 Please review registry variables in your SM_About and SM_CanRun. You probably have to change the value assigned to sm_registry_key_first_run.
 _____
