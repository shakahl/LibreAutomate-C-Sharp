mkdir "$desktop$\folder"
cop+ "$desktop$\forum1.txt[]$desktop$\list" "$desktop$\folder"
ChDir "$desktop$\folder"
ren "$desktop$\folder\forum1.txt" "fofo.txt"
ren+ "$desktop$\folder\fofo.txt" "$desktop$"
out CurDir
