 this also can be used to create shortcuts, but some shortcuts don't work well, although have the same text

str name="yahoo2"
name+".url"
name-"$desktop$\"

str address="http://my.yahoo.com/"
address-"[InternetShortcut][]URL="
address+"[]"

out name
out address

address.setfile(name)
