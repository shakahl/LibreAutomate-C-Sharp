 <for forum>

 This is similar to Accelerators in Internet Explorer 8. Works anywhere.
 Trigger: Ctrl+Shift+ right click.
 Gets selected text. If there is no selection, or selection is the same as previously, double-clicks to select word from mouse and gets the word.
 Then shows menu. You can google for the text, open as URL, translate, etc.
 Requires QM 2.3.2 or later.

 ________________________________________

spe 10
str+ g_mfst g_mfst2
str s.getsel; s.trim
if !s.len or s=g_mfst
	dou; 0.1
	s.getsel; s.trim

 g1
if(!s.len) inp- s "text" "Menu for text"
g_mfst2=g_mfst
g_mfst=s

str sEsc=s; sEsc.escape(9)
str pt=g_mfst2; if(pt.len>50) pt.fix(50); pt+"..."

 ________________________________________

 This code defines default menu items and code for them.
 You can change all it.

str menuItems=
F
 1 Google search
 6 Wikipedia
 2 URL open
 -
 9 Longman dictionary
 4 MW dictionary
 10 Wordnik dictionary
 11 Wiktionary
 3 Urban dictionary
 -
 5 Speak
 7 Map of address
 21 Google translate to EN
 -
 1000 Previous text ({pt})
 1002 Input text
 1001 Edit this menu

MenuPopup m
m.AddItems(menuItems)
 here you can add/remove/disable items depending on conditions: if(condition) m.AddItems(...).

sel m.Show()
	case 1 run F"http://www.google.com/search?q={sEsc}"
	case 6 run F"http://en.wikipedia.org/wiki/{sEsc}"
	case 2 run s
	
	case 9 run F"http://www.ldoceonline.com/search/?q={sEsc}"
	case 4 run F"http://www.merriam-webster.com/dictionary/{sEsc}"
	case 10 run F"http://www.wordnik.com/words/{sEsc}"
	case 11 run F"http://en.wiktionary.org/wiki/{sEsc}"
	case 3 run F"http://www.urbandictionary.com/define.php?term={sEsc}"
	
	case 5 Speak s
	case 7 run F"http://maps.google.com/maps?q={sEsc}"
	case 21 run F"http://translate.google.com/#auto|en|{sEsc}"
	
	case 1000 s=g_mfst2; goto g2
	case 1002 if(inp(s "" "" s)) goto g2
	case 1001 mac+ getopt(itemid)

err+
ret

 g2
DestroyMenu m; m=0
goto g1
