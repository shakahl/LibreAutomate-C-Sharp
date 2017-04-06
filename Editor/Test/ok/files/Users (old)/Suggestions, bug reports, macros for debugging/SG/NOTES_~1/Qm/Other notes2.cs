
Qm Installation:	Need option to have icon (or updating of existing icon) in Quick Launch toolbar

Operators:			Need logical XOR

Constants:			Constant arrays possible?

Timer functions:	Where are the running timer functions listed? How to get a list?

---


ifi : what does the i stand for? it made me think it stood 
for "inactive"; it may be more understandable to have it be "ife"

----

Ck starts up Recorder, is it supposed to?

---


winapiQM.txt
When i upgraded QM, i found that the Reference file setting
in QM settings pointed to the one in an older directory,
not to the winapiQM.txt file in the newer directory (i upgraded
to a different directory. I dont know, but u may want to
post a message asking if user wants reference to point to new
file.

---

Can you stop other hard disks from spinning up when overwriting file?
---------------------------------------------------------------------
QM appears to access other drive(s) when overwriting a flie. I exported a folder named 
555 to be overwriten to another file called 555.qml. Suddenly my other hardisk(s)
started up. 

Ok i found out why - because it sent the file to the recycle bin, and windows has
a nasty habit of starting up all drives if any file is sent to recycle bin.
---


Case of the mysterious "Modified from outside" due to corrupt qml file sent in last email
=========================================================================================

Another note below, from March, that was attached to the above note, 
i cant remember the circumstances, other than that that file you last 
sent me had a "bad file format" error so i had to extract all the macros
you sent me using Metapad. I then placed them into a folder called "55".

(Ive put the file, sent to me on Mon, March 3, 2003, at: 
www.geocities.com/rsrapr/not_working.rar.zip
if you want to analyse it. Open archive with winrar: www.rarlab.com.
Qm still wont import it.)

Note from March: 
"That file: sais its a "QM: bad file format". then a message pops up saying the file has
been modified from outside and asks if i want to reload. But no other computer
is connected to my file at the time."


UPDATE: November 20 - i duplicated the above scenario:
1) I tried to import that corrupt file 
2) Renamed it, tried to import it again
3) Soon i was presented with the message that my main qml file has been modified from outside
4) I went to Windows Explorer and saved a copy of the main qml file
5) Then, to avoid possibly corrupting my main qml file from the "modified" one
I chose to Overwrite, rather than reload the file 
(could my main file now be corrupt? should i revert to a backup?)

6) Did a ASCII file compare on the two files, and only lines 3&4 show a difference*
7) Tried to do a BINARY file compare, but that was taking forever so i gave up


* ASCII compare of copies of main qml file:
First file is the one that was currently loaded into qm
Second one is the one that was mysteriously reported as having been "modified from outside"
 - the second one is the copy i saved before overwriting it using "Overwrite" option in the dialog

Comparing files Over written QM Main 2.qml and modifi~1.qml
****** Over written QM Main 2.qml
     2:  1715    
     3:  ÙÏÝúÏs
     4:  ñ¬Ù$ ©¤Áq¸ŒûÈlX_%®
     5:  
****** modifi~1.qml
     2:  1715    
     3:  9ÛßÉµ-¬i¯× ñ­b;·]ŒK½rÑ9¬Ð­>X#
     4:  
******

