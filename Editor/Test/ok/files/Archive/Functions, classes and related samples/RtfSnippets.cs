 /
function~ action $databaseFile $snippetName ;;action: 0 copy/save, 1 paste, 2 delete, 3 get list

 Manages RTF snippets.
 Error if fails.
 With action 0 and 1, fails if the active window does not support RTF clipboard format.

 action:
   0 - gets selected text as RTF (copies through clipboard) and saves in database.
     Error if there is no text selected.
   1 - gets saved snippet and pastes as RTF.
     Error if the snippet (snippetName) does not exist, or if fails to paste.
   2 - deletes snippet. Not error if does not exist.
   3 - returns list of snippet names. For example, can be used to fill a combo box in a dialog. snippetName not used and can be "".
 databaseFile - SQLite database file.
 snippetName - snippet name in the file. Can be any string.

 Stores snippets in the database file, in table 'rtf', which has 2 columns: 'name' and 'data'.
 Creates the file and the table, if don't exist.
 If the snippet (snippetName) already exists in the table, updates.

 EXAMPLES
 RtfSnippets 0 "$my qm$\snippets.db3" "test1" ;;copy selected text and save as RTF snippet with name 'test1'
 err out _error.description

 RtfSnippets 1 "$my qm$\snippets.db3" "test1" ;;paste snippet 'test1'
 err


str s sn(snippetName)
sn.SqlEscape
Sqlite x.Open(databaseFile action&1)
ARRAY(str) a
int i

sel action
	case 0
	s.getsel(0 "Rich Text Format")
	if(!s.len) end "failed to copy text in RTF format"
	x.Exec("CREATE TABLE IF NOT EXISTS rtf (name TEXT PRIMARY KEY ON CONFLICT REPLACE, data TEXT)")
	x.Exec(F"INSERT INTO rtf VALUES ('{sn}', '{s.SqlEscape}')")
	
	case 1
	x.Exec(F"SELECT data FROM rtf WHERE name='{sn}' COLLATE NOCASE" a)
	if(!a.len) end F"snippet '{snippetName}' not found"
	a[0 0].setsel("Rich Text Format")
	
	case 2
	x.Exec(F"DELETE FROM rtf WHERE name='{sn}' COLLATE NOCASE" a)
	
	case 3
	x.Exec(F"SELECT name FROM rtf ORDER BY 1 COLLATE NOCASE ASC" a)
	for(i 0 a.len) s.addline(a[0 i])
	ret s
	
	case else
	end ES_BADARG

err+ end _error
