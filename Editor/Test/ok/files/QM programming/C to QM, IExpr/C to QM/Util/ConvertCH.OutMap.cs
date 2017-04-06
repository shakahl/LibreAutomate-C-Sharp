function IStringMap&m

 Displays map content. Can be used to debug.

str s k v
m.EnumBegin
rep
	if(!m.EnumNext(k v)) break
	s.formata("%s: %s[]" k v)

out s
