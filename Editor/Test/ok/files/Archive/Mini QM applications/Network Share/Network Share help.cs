 PURPOSE

 Copy/paste text or files between computers.

 HOW TO USE

 To copy text on one of computers, press Ctrl+Shift+C. It stores the text
 to clipboards of all predefined computers. Use eg Ctrl+V to paste. To send
 clipboard text without copying currently selected text, use Ctrl+Alt+C.
 You can change these triggers. Note that only plain text is copied.

 To send (copy) a file or folder from one of computers to one of other computers,
 right-click the file or folder in the Windows Explorer, go to Send To submenu,
 and click 'Computer COMPUTER', where COMPUTER is the name of a computer
 specified in the 'QM Network Share Setup' dialog.

 The file will be placed in '$my qm$\received files' folder of that computer (My QM
 normally is in My Documents). Existing files are silently replaced.

 SETUP

 On all computers that will be recipients or senders, run QM and import this file
 (Network Share.qml).

 On all computers that will be recipients, run QM, open Options dialog, select
 Network tab, check 'Allow other computers ...' and enter a password. Click OK.
 If Windows or a firewall program displays a confirmation dialog, click Allow.

 On all computers that will be senders, run NS_Setup function. Add computers that
 will be recipients. Computers can be identified by name or IP. Passwords must be
 the same as set in Options -> Network of these computers. If QM port on a recipient
 computer is different than on sender computer, append it to computer name or IP,
 like COMPUTER:8178.

 Each computer can be sender and receiver. QM must be running on that computer.

 NOTES

 Uses the net function. You can read about it in QM Help.

 If used Send To menu, only single file or folder can be sent at a time.

 QM shell menu extension (available in the forum) also can be used. It allows to
 send multiple selected files or folders. See shell menu samples.

 You can use functions NS_Copy, NS_SendClipboard and NS_SendFiles in macros.
