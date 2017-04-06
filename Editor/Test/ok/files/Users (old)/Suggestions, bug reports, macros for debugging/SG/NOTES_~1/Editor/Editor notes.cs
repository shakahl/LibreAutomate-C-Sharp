 Notes, mostly from July or March

List
====

For option checked: List: expand single folders
-----------------------------------------------

- Expanded option to have certain folders left open:

Subfolders opened with CTRL should not be closed when we go to a different folder,
its very annoying to not be able to have a particular folder open by choice. 

(The only other option is to uncheck the option altogether,
but thats just going from one extreme to another - need something in the middle,
so i think user should be able to keep certain folders open by using a certain
key combination.)

Currently, folders opened with CTRL may or may not stay open, depending on where and 
how you click.

Eg. CLRL-Click ONE top level folder, then click, without using CTRL, a top level macro:
the folder closes, so that it makes it difficult to go back and forth between a
folder-level macro and a top level macro.

If instead you CTRL-Click TWO top level folders before clicking a top level macro, one
of the folders does stay open.

There are other situations i wont point out here. 
But we need consistant predictable results on this issue.


- Possible accidental renaming

When closing a folder by clicking on it,
QM goes into rename-mode for the folder. This may lead
to unintentional renaming of folder?



For option not checked: List: expand single folders
---------------------------------------------------

- Annoying folder expansion 

When moving folders that are not expanded, it expands them - gets to be annoying when we 
explictly closed them and expect them to stay closed til we open them



For both - option checked or not checked
----------------------------------------

- New Folder being created but where?

Sometimes File, New, New Folder wont work (either that, or
the folder is being created somplace unknown), for example when
you have only folders in the list view, or when caret is on a folder.
However right-click, New, New Folder works ok.

- Case of the shifting focus

When hitting Del key, focus then jumps to Right pane, instead
of staying in left pane; works ok when doing Ctrl-Del though.

- Case of the jumping caret - my rabbit's going crazy! lol

:When deleting a macro above a folder, some other macro opens up-
caret should stay in place (like at the next/previous item/folder.)
						
:When ctrl-del a macro that is right above a folder, the caret jumps to the top. 
Its annoying to lose my position when i'm cleaning up my list of numerous test macros.

- Duplicate top-level folders?

QM lets you have 2 or more top-level folders with the same name. 
Are u sure u want to allow this?


Editor
======

- Vanishing space

When carot is at beginning of a line that starts with a space, the space is lost when you hit Enter

- More keyboard menu shortcuts

Qm main menu bar (File, Edit, Tools, Help):
When a drop-down menu is activated (using Alt-E for example), 
Left & Right arrow keys should go to previous/next menu


---
- Maintaining scroll point during a commercial (haha)

If u scroll down using the editor scroll bar,
then Ctrl-Tab to another program,
then come back,
the spot reverts to where the caret is, not to where you had scrolled to
so u are forced to scroll back to where you were. 

This gets annoying after a while.
I can click in the editor window b4 going
to another program, but that adds an extra step.

QM apparently keeps track of the caret position in the editor;
can it keep track of the scrolled-to-point also?

Update, November: I now work knowing what can happen, so its less of a hassle now,
but still yet a hassle. Is there an easily workable solution?
---


Add macros to external file
===========================
Need an "Export to" option to add macros to an existing qml file on disk.


Qml Files: Drag & drop option and/or Recent files history
=========================================================
Need to be able to drag a file into the qm window and have it close
current file & load the dragged file. When im switching between a
network file & a local file, it gets tiring to have to go through
the "Look in" procedure, having to navigate through many paths to 
get to my file every time. The Open/New file dialog has no 
"Bookmark" icon...but instead i can keep an explorer window open
from which i can drag a file...

Perhaps: Drag to Open file; Ctrl-drag to Import it

And/or a "recent files" or "recent directories" ("MRUDs") menu under 
the "File" menu, such as implemented by other programs (eg. the 
File-->MRUDs item in Irfanview, the fabulous free fast and 
keyboard-friendly image viewer & multimedia player, www.irfanview.com)

 Update, November: I no longer like to edit across the network, instead 
 i copy the file locally and work on that.
 However a recent files list may still be of use (though for me, not lately)



NAVAGATION:


Macro Navigation
================

- Need easy access to last edited macro

Optional: [need to get to last edited macro, eg after doing a find - the drop down box
wont show last edited macro, only marcos we were at last or that we ran last]

- Would wacro bookmarks be good?

Need a "Bookmarks" icon to be able to quickly go edit macros i'm currently 
working on. Or, can i do this by listing macro bookmarks in a Toolbar or Popup Menu?

- How about <-- & --> buttons 

Along with a bookmark icon, maybe also a 'true' back & forward button that will 
keep taking you back or forward instead of alternating between two macros. 
I dont know, im just making notes as i use QM...


Settings Navigation			
===================
- Qm Options dialog - keyboard shortcuts

Tab & Ctrl-Tab currently work the same way
Suggestion:
Ctrl-Tab 		- takes you to next button. ie General-->Trigger-->Run time-->etc
Shift-Ctrl-Tab	- takes you back one button. ie General-->Files-->Security--->etc

Ctrl-tab (forward) & Shift-Ctrl-Tab (to go backwards) is used
in Windows 98 windows also, eg in the Control Panel items (Mouse dialog etc)


Scroll in Editor
================

Ctrl-Left, Ctrl-Right, Ctrl-Up, Ctrl-Down:  Works in the List frame, for scrolling

Doesnt work in the Editor frame. 

Ok, in editor frame it does it differently: Ctrl-Right goes to next word etc., but
perhaps you could set hot keys for the other type scroll as well 
(perhaps Alt-Right...? or, as in Adobe Acrobat Reader, Shift-Right...)





Output
======

- Quick copy of output text

Need a quick way to copy all text in output window to clipboard - 
presently we are forced to drag and select using  mouse or several key strokes
I realize i could set a hot key to get the output text, but i feel it should be built into
the program
Perhaps just have the context menu 'Copy' command copy all the output text
if no text is selected, as is the case in Qm Help

- Output without CRLF

Really need a way to not have output of out command not insert a carriage return


- Adjustable output limit

 Recent note:
Need to be able to specify size limit of output

Sometimes i need to reference and/or analyze pages and pages of constant output, and 
the only ways i implement it now is to manually hit a key to save the output to a file,
or i set a timer function to save the output to a file whenever the output lenght
reaches just below the maximum allowed.

Would be nice if i could specify my own limit: for example a 2 meg limit would 
give me far more than enough output.

Practical usage:
I have macros set up to record progress of a game.
Everytime a loop in a certain macro runs, it produces output to
indicate current status of the game.

If something goes wrong, or if theres a particular point in the game i want
to analyze, then i want to be able to go back in that display which typically
spans thousands of bytes per minute.




