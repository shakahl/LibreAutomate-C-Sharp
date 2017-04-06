	 SpamFilter

 SpamFilter is QM extension application that checks email in POP3
 mailbox(es) and deletes spam. To launch it, run SpamFilter function.
 To launch automatically when QM starts, insert this in init2 function:

mac "SpamFilter"

 SpamFilter adds tray icon, which changes when checking and when
 there are messages waiting in server. Click it to show window.
 Right click to show menu.

 Initially, SpamFilter does not recognize spam. To make it work,
 you have to create your own filter function(s) (read below).
 _______________________________________________________

	 Main window

 The first list displays messages waiting in POP3 server(s).
 The second list displays messages that are deleted from server
 and stored locally, in spam folder, as eml files.
 The third list displays errors that might be during last check.
 It is automatically shown on error.

 In first two lists, you can:
 Double click a message to view it. Right-click to view menu that
 allows to view message, source, delete or restore selected messages.
 You can use the following keystrokes: Delete (delete), Ctrl+A (select all),
 Ctrl+click (select more messages), Shift+click (extend selection).
 When you delete a message from the first list, it is actually deleted
 during next check. When you restore a message from the second list, it
 is sent back to server. Note: if original message was bigger than
 specified in Options -> Retrieve only headers + x lines, message is
 restored partially (remaining part is lost).

 Buttons:
  Filters: edit filter function (read below).
  Options: show Options dialog.
  Edit mode: check this when you are editing, testing, adding or
   deleting filter functions. Normally, it should be unchecked.
   When unchecked, deletes spam. Also, if SpamFilter main window
   is hidden, downloads/processes only new messages. 
   When checked, spam is not deleted, but only marked as Spam.
   Also, messages are always downloaded/processed. Also, if window
   is visible, does not check mail automatically, and you have to
   click 'Check now' button.
  Check now: check mail now.
  Email app: launch email program specified in Options.
 _______________________________________________________
  
	 Filter functions

 Initially, SpamFilter does not recognize spam. To make it work, you have
 to create a filter function (or several). Filter functions must be
 specified in SpamFilter Options. To create new filter function click New.

 When checking mail, each downloaded message is passed to filter functions,
 in order they are specified in Options. A filter function can check
 messages's properties, and determine whether it is spam. If some function
 returns -1 or -2, message is considered spam, and other functions are not called.
 If some function returns 1, message is considered good, and other functions
 are not called. If all functions return 0, message is considered good.

	              Message
	                 |
	                 v
	             Function1
	      <--   -1   0   1   -->
	                 |
 S	                 v                            G
 P	      <--       ...      -->                  O
 A	                 |                            O
 M	                 v                            D
	             FunctionN
	      <--   -1   0   1   -->
	                 |
	                  --------->

 Normally, if message is considered spam, it is deleted from server and
 saved to local spam folder. If a filter function returns -2, the message
 is deleted and not saved. If 'Edit mode' button is checked, messages
 are not deleted/saved, but only marked as Spam in the Mailbox list. Then
 you can safely experiment with filter functions.
 _______________________________________________________

	 Options

 If you connect to the Internet not through Windows Dial-Up Networking,
 don't check "Detect connection" checkbox, because QM will fail to detect
 Internet connection, and SpamFilter will not work. Also, "Check after
 connected" will not work. If you connect through Dial-Up networking,
 and SpamFilter in some circumstances does not respond during 10 - 60 s,
 check "Detect connection".

