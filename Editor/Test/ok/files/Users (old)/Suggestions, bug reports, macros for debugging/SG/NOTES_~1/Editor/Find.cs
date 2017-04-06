 July/March notes regarding FIND dialog
 ======================================

- Carot jumps to start of macro after last find

When doing find, when clicking on "Find Text" after last find, then close Find box,
it goes to the start of the macro, necessitating having to do a find again. 

I feel that the scrolled-to point as a result of the final find,
(and perhaps too the caret, though this may be less of a concern,)
should stay put after the last find if no more text is found

Example: search for "disabling" in this macro, hit "Find text" twice, then close find box.

It is particularily annoying if there is only one instance of the text in the macro.
Then to get to the found point again, you have to re-start Find and re-type the word and hit 
Find Text again.




- FEATURE REQUESTS: 

-Term highlighting

When doing a "Find" in help, the term should be highlighted on the page that its found on.
Without this feature, we have to scan through the whole page to find our search query.
Examples:
1) Windows 98: Start, Help-->Search...List Topics, Display. The term is then highlighted on the right side.
2) Google: Go to www.google.com, type in a query, then select "Cached" from one of the links

- Disabled Indicator in Find Dialog

In the Find dialog window, need to know if listed macros are disabled,
especially when searching for a triger match. Can you change the Find
list display so that disabled macro names (or the first letter of 
their name) appear in different color? Or have some other indication
that a macro is disabled.


Find: Name out of view
======================
Updated November 20
Note: this bug seems to only apply when "List: expand single folder" is checked in Options

I have a large number of macros, and with folders nested upto 2 levels.

When doing a Find that brings up a large number of items in the ListBox:

When i click on a name in the ListBox, Qm finds & highlights it
but sometimes doesnt auto-scroll to it if its out of the screen range
I then have to scroll down or up to reach it.

For example: Do a search for an item that will bring up a large list
of scattered items, like the word "the" with criteria "Text" or the word "macro"
with criteria "Name"
0) Check the box in Options: "List: expand single folders"
1) Click on an item near the top, then the bottom of the list, it works fine.
2) Now click on an item near the top of the list, its out of view.

I think this seemed more an issue in Qm 2.0.9, when step 1 would not work ok;
now its going back up the tree that's still a problem

screen resolution of 800x600 16-bit color on 15" monitor, later 1024x768 32-bit
screen resolution of 1024x768 32-bit on 17" monitor



Find: 
=====
- Ergonomics

1) Resizable window - any box with more text than can fit inside it should be resiziable, 
else it gets frustrating to access that information.
(Thats my personal view. Ive stopped using great applications when windows are
not resizable. But your Find as it is - with its auto-repositioning to display found
text, is fantastic, so i dont know...mabey change 1) and/or 2) here only
if others complain also...)
2) Small size of the close icon ("x") may be troublesome for some people, who may
appreciate a wider icon - theres lots of side-to-side space so it need not be taller,
only wider. (although they could easily set up a macro to close the Find dialog).

- Keyboard shortcuts:

- Find next
Find: using keyboard: no easy keyboard shortcut to do a  "Find next". Currently we 
have to do CTRL-F to get focus back to Find window, then ALT-F for next find - 
thats four keystrokes to do a "Find next". BUT even doing this doesnt alwawys work.
-->Update: never mind, i just set a trigger of F3 for it

-Locate next
Find: can u have it so using down & up arrow always selects in the right window,
regardless of where the caret is? Eg, user enters text in top left edit box, then 
hits down-arrow, and a macro then gets selected from the list.
--->Update: i also set a hot key for this too, to activate the ListBox, but i still 
think the feature should be added

-Loop
Find: right window: when using down/up arrow, when we reach end/beginning
of list, then it would be nice for it to jump to the beginning/end. Saves
us from scrolling back through the list.


- De-trigger those triggers

Find: when looking for a trigger, perhaps certain triggers - ie, those which type 
out a character - should be disabled, else we cant type in certain triggers 
to search for, without first disabling QM.
Its a minor thing though, thought up when i was ` as a trigger
	
