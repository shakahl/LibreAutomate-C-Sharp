 Thoughts on Backups
 ===================
 Notes from March 2003
 ---------------------
Auto backup: Here is a list of my backup directory from one afternoon:
506,685  03-30-03  4:29a ~~back~~.qml
506,685  03-30-03 12:12p QM main_h.qml
502,449  03-30-03 12:12p QM main_d.qml
523,812  03-30-03  1:52a QM main_w.qml
 and a minute later:
506,704  03-30-03 12:12p ~~back~~.qml
506,685  03-30-03 12:12p QM main_h.qml
502,449  03-30-03 12:12p QM main_d.qml
523,812  03-30-03  1:52a QM main_w.qml

Sometimes i make a change and want to go back to a point in time
of 1, 2, or 3 hours ago, to go back to a prior version of a macro,
for example when current version is failing. And though i havent found the need, 
there may (or may not) be a need to go back in time some days or
week(s) before.

Suggestions: 
1) Weekely backup: save at least 1 previous weekely b4 replacing it with new weekely
2) Daily backup: save at least 1 previous instance of daily backup, up to 6 (7=weekly)
3) Hourly backup: save at least 1 previous instance of hourly backup, up to lets say 3 (can of course be up to 23, & 24=daily)

 ----Another note from March:

Should be more than one hourly backup - at least two, as much as 5
(perhaps have it as option). Some times we need to go back to a state
that may be 3 hours prior, when we make some changes and want to revert
to a previous condition. For example, u make a lot of changes and discover
a bug, so u want to compare current macro with a prior version that worked 
fine before the current one got buggy.

 Notes from October 2003
 -----------------------
I went to restore a macro i was editing (decided i wanted to save the old version 
for reference) So i went to restore from backup qml file, but discovered this:

10/29/03 3:33AM Hourly 
(^Was saved as soon as i went to open another qml file 
from backup), thus overwriting the previous backup i needed, 
thus defeating the purpose of the hourly backup)
10/29/03 3:30AM ~~back~~.qml
10/28/03 5:19AM daily
10/26/03 5:59AM weekly


What would be desired is to have, in my opionion:
3 Backups each at least an hour apart
3 Backups each at least a day apart
1 Backup at least one week apart from the oldest daily backup
