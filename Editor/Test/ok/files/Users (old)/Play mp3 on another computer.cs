 This macro accepts one or more mp3 filenames from
 the system to be played or enqueued on a remote computer

function $mp3 $action

out mp3

str m=mp3
 m.findreplace("\\zeners\music" "e:" 2)
 action is Play or Enqueue
str a=action

int p=net("GINTARAS" "slapta1" "Macro507" 0 a m)
