 Spelling checker using Aspell.
 Requires QM 2.3.0 or later.

 SETUP

 Download Aspell and one or more dictionaries from http://aspell.net/win32/.
 Install Aspell and dictionaries.
 After installing a new dictionary, restart QM.
 It is possible to use Aspell on computers where it is not installed. Copy Aspell folder from computer where it is installed to computer where it is not installed, to QM folder (or to your exe folder). Before #compile Caspell insert: SetEnvVar "aspell.dll" "$qm$\aspell\bin\aspell-15.dll". Or use some other folder.

 EXAMPLE

str w
inp- w "some misspelled or correct word" "" "mispeled"

#compile Caspell
Caspell k.Init("" "" "$my qm$\Aspell") ;;use "en_US" language, default Aspell options, and set folder of wordlists
 Caspell k.Init("" "ignore=2[]sug-mode=bad-spellers") ;;example of setting Aspell options
 Caspell k.Init("ru") ;;Russian
 out k.GetOption("repl") ;;you can see Aspell options

if(k.Check(w))
	mes "Correct." "" "i"
else
	ARRAY(str) a
	k.Suggest(w a)
	str s=a
	s-"[]"; s-"<it is correct word>"
	int i=list(s _s.format("%s is incorrect.[]Suggestions:" w))
	if(i=1)
		k.AddWord(w)
		k.SaveWordLists
	else if(i)
		k.AddReplacement(w a[i-2])
		k.SaveWordLists
