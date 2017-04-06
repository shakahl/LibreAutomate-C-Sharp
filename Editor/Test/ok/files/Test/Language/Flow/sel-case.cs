ClearOutput

int i=7

sel i&5
	case 10 out 1
	case 5 out 2
	case [6,7] out 3
	case else out "else"


lpstr s="Combobox"

sel s 1
	case "Static" out 1
	case "Edit" out 2
	case ["Button","ComboBox"] out 3
	case else out "else"
out 111
