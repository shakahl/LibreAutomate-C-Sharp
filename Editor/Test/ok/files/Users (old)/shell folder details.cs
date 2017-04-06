out
str folder.expandpath("$my pictures$\Screensaver")

int iDetails ;;different on all OS. See http://www.kixtart.org/forums/ubbthreads.php?ubb=showflat&Number=160880&page=1
if(_winver>=0x600) iDetails=12 ;;Vista
else if(_winver>=0x501) iDetails=25 ;;XP/2003. Not tested
else end "Date Taken not available"

Shell32.Shell shell._create
Shell32.Folder sf=shell.NameSpace(folder)
VARIANT filen
foreach filen sf.Items
	out filen
	out sf.GetDetailsOf(filen iDetails)
