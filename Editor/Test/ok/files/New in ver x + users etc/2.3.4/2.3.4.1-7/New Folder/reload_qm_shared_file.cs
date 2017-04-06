function event $name [$newname]
 event: 1 added, 2 removed, 4 renamed, 8 modified
 out F"{event} {name}"

MES m.style="YNn"; m.x=-1
sel mes("Shared file modified on server computer. Update it here in QM? It will reload your main QM file.[][]%s" "Quick Macros" m name)
	case 'Y'
	shutdown -5


 Also check 'Allow single instance' in Properties -> Function properties.
