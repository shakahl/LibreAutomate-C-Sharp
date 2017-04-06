 MS-DOS name appears in title bar
 ================================

Have 3 computers: 1) Cyrix M2 @ 266Mhz  2) AMD K6-2 @ 300Mz  3) P2 @ 333Mz

All 3 computers are now running Qm 2.1.0 November release.

Computer #2, which was running Qm 2.0.9, today had this problem, which went away after upgrading to 2.1.0.
I cant recall Computer #3 ever having this problem.
The following issue only applies to computer #1 now
----------------------------------------------------------------------------------------

1) Clicking on a qml file in Windows Explorer brings up the Ms-Dos name, instead
of the long file name, in the Qm Title Bar at the top.

2) The only way to get the long file name back in the title bar seems to be to:
	- in Qm: File, Open, then browse to the file, and re-open it

3) If i dont do the above, then exiting and restaring Qm normally (using the
Quick Launch icon) will still bring up the Ms-dos name

4) If i re-open the file using browse, then launching Qm through the Quick Launch
icon works ok after that.

5) But if i exit Qm and start restart it by clicking the Qm file, instead of using
Quick Launch icon, we are back to step 1)


Some considerations:
--------------------

#s 1&2 have Qm 1.3.3, 2.0.8, 2.0.9, and #1 has the latest 2.1.0 (Update: #2 has latest as well, problem solved for it)
The P2 only has the latest 2.0.9 & the latest two releases of 2.1.0

#s 1&2 have had a lot of software installed on them (though with < 2 Gig drives).
The P2 has had very little.

All computers are running Win98SE with plenty of RAM


Speculation:
============
Could it be that a prior install of Qm 1.3.3 or Qm 2.0.8 in the slower
computers is causing the long name to not appear when a qml file is clicked on?

Or maybe some other software is conflicting.

Test
----
Just now, I installed Qm 1.3.3 onto the P2, started it, rebooted; no problem
So maybe Qm 2.0.8 is causing the trouble.

Let me know if you want me to run more tests - may have to start with a fresh
install of the Operating System? I got spare parts for another computer anyway that
i was planning to load up.

