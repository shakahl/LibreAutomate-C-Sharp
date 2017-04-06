 Mouse Chords configuration. Runs automatically.
 You can change some values. To apply changes, run this function manually.

type MCHORDS !Initialized @onDoubleClick tStart @tWait @tDoubleClick
MCHORDS+ g_mc.Initialized=1
g_mc.onDoubleClick=0

 You can change the following values
g_mc.tWait=1000 ;;time (milliseconds) during which must be pressed second button
g_mc.onDoubleClick=qmitem("MC_Middle") ;;macro that runs when you double click the first button. Disable this line if you don't want it.
g_mc.tDoubleClick=350 ;;double-click time of the first button
