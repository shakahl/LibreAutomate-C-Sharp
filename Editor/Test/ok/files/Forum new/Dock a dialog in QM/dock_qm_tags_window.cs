 ^<-- Paste this in the Trigger field on the toolbar: !cv"Tags" "#32770" "" "" "" 0x2 /QM
 This function starts when the Tags window opened, and runs until closed.
 Moves the Tags window to the right-bottom of the QM window and keeps it there when you resize etc QM or resize/minimize the docked Tags.
 Tested on Windows 10 and XP.


int wTags=TriggerWindow
if(!wTags) end "To run this, set trigger 'window created & visible' for QM 'Tags' dialog of QM."

int wQM=_hwndqm
int wPane=id(2206 _hwndqm) ;;QM_Scc

SetParent wTags wQM
 SetWinStyle wTags GetWinStyle(wTags)|WS_CHILD&~WS_POPUP ;;removes menu bar

rep
	if IsWindowVisible(wQM) and !IsIconic(wQM)
		if !IsWindow(wTags) ;;closed
			SendMessageW wQM WM_SIZE 0 0 ;;restores control sizes
			break
		sub.MoveSize
	0.5


#sub MoveSize v

RECT rQM rPane rTags
GetClientRect wPane &rPane; MapWindowPoints wPane wQM +&rPane 2
GetWindowRect wTags &rTags; int cxTags=rTags.right-rTags.left
GetClientRect wQM &rQM; int xTags=rQM.right-cxTags

 MoveWindow is slow...
MapWindowPoints 0 wQM +&rTags 2
if(rTags.left=xTags and rTags.top=rPane.top and rTags.bottom=rPane.bottom and rTags.right=rQM.right and rPane.right=xTags) ret

MoveWindow wPane rPane.left rPane.top xTags-rPane.left rPane.bottom-rPane.top 1
MoveWindow wTags xTags rPane.top cxTags rPane.bottom-rPane.top 1

 adjust the Tags child controls too. Remove this code if don't need.
int c1(id(3 wTags)) c2(id(4 wTags))
GetClientRect wTags &rTags
int xSeparator=rTags.right/2
MoveWindow c1 0 0 xSeparator-8 rTags.bottom 1
MoveWindow c2 xSeparator 0 rTags.right-xSeparator rTags.bottom 1
TO_LvAdjustColumnWidth c1 1
TO_LvAdjustColumnWidth c2 1
