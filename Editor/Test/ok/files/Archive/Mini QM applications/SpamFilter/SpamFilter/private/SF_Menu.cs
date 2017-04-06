sel PopupMenu("Check now[]Email program[]-[]Exit")
	case 1 SendMessage __sfmain WM_USER 1 0
	case 2 SendMessage __sfmain WM_USER 2 0
	case 4 SendMessage __sfmain WM_CLOSE 1 0
