 Macro wont run for created Internet Explorer windows
 ====================================================
 1) For example, go to www.google.com
 2) Right click on "Groups" button, and click "Open in New Window"
 3) This macro, with activatin set as !Google Groups - Microsoft Internet /IEXPLORE
 	wont work
 3b) In the Qm output it shows "!Microsoft Internet Explorer"
     instead of "Google Groups - Microsoft Internet Explorer"
 4) Change the activation to @, and it all works as expected

 int w=val(command)
int w=win("Google Groups")
max w
