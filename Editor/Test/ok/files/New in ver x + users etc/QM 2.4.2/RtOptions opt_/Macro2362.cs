/exe
out
RTOPTIONS r
r.opt_macro_function.keychar=1
 r.opt_macro_function.keysync=128
 r.opt_macro_function.hungwindow=1
 r.opt_macro_function.keymark=1
 r.opt_macro_function.clip=1
 r.opt_macro_function.clip=0x80
 r.opt_menu_toolbar.clip=1
 r.opt_menu_toolbar.hungwindow=3
 r.opt_autotext.keysync=1
 r.opt_autotext.clip=1

RtOptions 64 r

RTOPTIONS rr; RtOptions 0 rr; out _s.getstruct(rr 1)

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
 sub.OM

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

#sub OM
opt waitmsg 2

 BEGIN PROJECT
 main_function  Macro2362
 exe_file  $my qm$\Macro2362.qmm
 flags  6
 guid  {1D47136F-A5CD-4D54-ADAA-B2F9F7532183}
 END PROJECT
