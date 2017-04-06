 /
function ARRAY(int)&handles [$name] [$cls] [$exe] [winflags] [flags] ;;flags: 1 exclude hidden, 2 exclude without name, 4 exclude popup, 8 exclude non-popup, 16 exclude popup without caption.

 Finds all windows matching specified name and/or class and/or program.

 handles - receives window handles.
 name, cls, exe, winflags - the same as with the win function.


type __GWL2 ARRAY(int)*handles flags str'ts str'cs
__GWL g
g.ts.flags=1; g.cs.flags=1

opt hidden !(flags&1)
win "" "" exe flags&12|0x8000 &GWL_EnumProc &g
