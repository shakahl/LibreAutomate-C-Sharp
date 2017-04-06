out
rset "data ąč ﯔﮥ k" "value ąč ﯔﮥ k" "\ąč ﯔﮥ k\key ąč ﯔﮥ k"
rset "data ąč ﯔﮥ k" "value ąč ﯔﮥ k" "\ąč ﯔﮥ k\key ąč ﯔﮥ k 2"
rset "data ąč ﯔﮥ k 2" "value ąč ﯔﮥ k 2" "\ąč ﯔﮥ k\key ąč ﯔﮥ k"

 str s
 rget s "value ąč ﯔﮥ k" "\ąč ﯔﮥ k\key ąč ﯔﮥ k"
 out s

 rset "" "value ąč ﯔﮥ k" "\ąč ﯔﮥ k\key ąč ﯔﮥ k" 0 -1 ;;delete value
 rset "" "key ąč ﯔﮥ k" "\ąč ﯔﮥ k" 0 -2 ;;delete key
 rset "" "key ąč ﯔﮥ k" "\ąč ﯔﮥ k" 0 -3 ;;delete key if does not have subkeys
 rset "" "key ąč ﯔﮥ k" "\ąč ﯔﮥ k" 0 -4 ;;delete key if empty

 enum keys
 str s
 foreach s "\" FE_RegKey 0 1
  foreach s "Software\Gindi\QM2" FE_RegKey 0 1
  foreach s "\ąč ﯔﮥ k" FE_RegKey 0 1
	 out s

 enum value names
 str s
  foreach s "Software\Gindi\QM2\settings" FE_RegKey
 foreach s "\ąč ﯔﮥ k\key ąč ﯔﮥ k" FE_RegKey
	 out s

 enum values and get data
 ARRAY(str) a; int i
 RegGetValues a "\ąč ﯔﮥ k\key ąč ﯔﮥ k" 0 1
 for i 0 a.len
	 out "%s=%s" a[0 i] a[1 i]

 enum values and get data
 ARRAY(str) a; int i
 RegGetValues a "Software\Gindi\QM2\settings" 0 1
 for i 0 a.len
	 out "%s=%s" a[0 i] a[1 i]
