 str s
 out s.DateInFilename("file-{D}.txt" "$desktop$")

str s
 s.DateInFilename("qm {D}.log" "$temp$")
 s.DateInFilename("qm {MM/dd/yyyy}.log" "$temp$")

 s.DateInFilename("qm {D}.log" "$temp$" "07/07/2000")
DATE d.getclock; d=d-1
s.DateInFilename("qm {D}.log" "$temp$" d)
 s.DateInFilename("qm {D}.log" "$temp$" 0)
out s

