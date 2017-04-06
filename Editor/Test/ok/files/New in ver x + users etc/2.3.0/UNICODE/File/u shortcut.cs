 CreateShortcut "$desktop$\n ąč ﯔﮥ k.lnk" "$system$\notepad.exe" "/n ąč ﯔﮥ k.lnk"

SHORTCUTINFO si
 si.descr="descr ąčﯔﮥ q"
 si.iconfile="$desktop$\ąčﯔﮥ q\ąčﯔﮥ q.ico"
 si.initdir="$desktop$\ąčﯔﮥ q"
 si.param="/arg ąčﯔﮥ q"
 si.target="$desktop$\todoڳ.txt"
 out CreateShortcutEx("$desktop$\ąčﯔﮥ q\sh ąčﯔﮥ q.lnk" &si)

out GetShortcutInfoEx("$desktop$\ąčﯔﮥ q\sh ąčﯔﮥ q.lnk" &si)
out si.descr
out si.iconfile
out si.iconindex
out si.initdir
out si.param
out si.showstate
out si.target
