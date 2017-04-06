Moving marcos from one end of a long tree to the other is extremely difficult
=============================================================================

I tried 3 times to move the All_keys macro (will submit this next time),
but ended up each time with Qm asking me if i wanted to delete the macro.

To move a macro, i need to drag it, but when dragging from way down below to 
way up high in the tree, have to keep moving the mouse side-to-side at the top
until i reach the destination, and this process is very unnerving. And this process
often results in the drag failing, (mouse gets unclicked somehow?), so have to repete the
operation again.

I just gave up. To move macros great distances, i instead do Select-All, Copy.
Then i create a new macro at the destination, and re-paste the text and 
re-define the triggers. Or i move a macro one screenful at a time, repeting the procedure
for each macro to be moved.


A context-menu item to select a macro, a folder, or a group of macros/folders,
to be cut/copied to elsewhere, like in Windows Explorer, would help keep me sane :).


Currently my qml file is 2 Megabytes with hundreds of items and a big mass
of folders and subfolders, so anything like this to make it easier to
navagate would be a pleasure.


Below are all other notes on Moving/Copying (from March & July mainly)


Moving/Copying items
====================


See-sawing Folders
==================

-When moving macro into folder, folder usually expands, but not always
Need to be able to have it NOT expand, else gotta close it after every move,
making macro housekeeping difficult

-When i move a folder, QM moves caret to last open (currently open?) macro. This is 
annoying & disorienting & confusing - caret should stay on the folder i think,
but i dont know since we have to account for the open macro in editor. Also when moving
a macro into a folder, some other macro (last one opened?) opens in
some other folder.

Update, November:
-----------------
Ok, i believe i figured out how this works:
-caret always returns to name of currently open macro in the List
-so if moving another macro to a different folder, that folder wont expand: the folder
the current macro is in will expand or stay expanded
-and if moving a folder, the caret again returns to name of currently open macro,
thus expanding the folder that contains it
-if moving a macro that's currently open in editor, folder it moves to gets expanded

---- all in all, a confusing experience at the start (at least for someone with a large/complex folder tree)
just gotta figure how to make it less confusing -----

One possibility:
Treat Folder name and Folder icon as separate destination points. (I think it was like this at one time?)
Folder name: if drag of mouse lands here, do the move, & expand folder
Folder icon: if drag of mouse lands here, do the move, & dont expand any folder
(or vice-versa)




- Undo move/delete

A one-level undo operation for move/delete, to undo the last
operation, would help out a lot.


Difficulty in moving/copying
-----------------------------

- Unintended deletion

When copying macro, need to use ctrl, but if the destination is far above,
the cursor has to land out of the range, and if mouse is 
somehow not kept pressed, macro gets deleted

- Misplaced move

Moving folders great distances amoungst a forest of folders: ended up moving folder to wrong folder
- gotta have great control of the mouse to accomplish the feat flawlessly. 
We need a better solution to move/copy items.

- Redundant earlier miscellany:

Need a "move to" option in list view context menu, that would
allow user to move item to one of existing folders.

Moving/copying an item from bottom of a long list to top is quite
a struggle with the mouse--have to move mouse pointer up & down instead
of holding the mouse at the top.

Need to be able to copy groups of macros and/or a folder -for example when
we want to associate a different set of keys to a group of macros, we
can quickly copy a folder and then change key definitions (instead of
changing the keys in the current folder.)

Need ability ot select multiple items for moving to a folder. (Like in Windows Explorer)
Would help when user wants to move many items into a separate folder.
