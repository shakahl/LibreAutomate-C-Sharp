Run program :mac "TO_FileRun" * $qm$\run.ico
Open file :TO_Fav "TO_FileRun" 1 * $qm$\files3.ico
Open folder :TO_Fav "TO_FileRun" 2 * $qm$\folder_open.ico
-
Create folder :mac "TO_FileMkDir" * $qm$\folder.ico
Copy file :mac "TO_FileCopy" * $qm$\files2.ico
Move or rename file :TO_Fav "TO_FileCopy" 1 * $qm$\files2.ico
Delete file :mac "TO_FileDelete" * $qm$\del.ico
-
If file exists :mac "TO_FileIf" * $qm$\files.ico
Get file info :mac "TO_FileInfo" * $qm$\files.ico
Enumerate files :mac "TO_FileDir" * $qm$\files.ico
-
Read file :TO_Fav "TO_Text" 3 4 * $qm$\files3.ico
Write file :TO_Fav "TO_Text" 9 1 * $qm$\files3.ico
-
Open web page, wait :mac "TO_Web" * $qm$\web.ico
 New email message :mac "TO_EMail" * $qm$\email.ico
Send email message :mac "TO_SendMail" * $qm$\email.ico
Receive email messages :mac "TO_ReceiveMail" * $qm$\email_receive.ico
