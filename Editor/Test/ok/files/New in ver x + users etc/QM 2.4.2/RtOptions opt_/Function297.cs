out

 spe 20
 opt keysync 1
 opt keymark 1
 opt keychar 1
 opt hungwindow 1
 opt clip 1
 opt err 1
 opt end 1
 opt waitmsg 1
 opt waitcpu 1
 opt slowmouse 1
 opt slowkeys 1
 opt hidden 1
 opt nowarnings 1
 opt nowarningshere 1
 opt noerrorshere 1

out spe
out "keysync %i" getopt(keysync)
out "keymark %i" getopt(keymark)
out "keychar %i" getopt(keychar)
out "hungwindow %i" getopt(hungwindow)
out "clip %i" getopt(clip)
out "err %i" getopt(err)
out "end %i" getopt(end)
out "waitmsg %i" getopt(waitmsg)
out "waitcpu %i" getopt(waitcpu)
out "slowmouse %i" getopt(slowmouse)
out "slowkeys %i" getopt(slowkeys)
out "hidden %i" getopt(hidden)
out "nowarnings %i" getopt(nowarnings)
out "nowarningshere %i" getopt(nowarningshere)
out "noerrorshere %i" getopt(noerrorshere)
