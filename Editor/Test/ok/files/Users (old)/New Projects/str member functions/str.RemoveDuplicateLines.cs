function [flags] ;;flags: 1 case insensitive

 EXAMPLE
 str s=
  apple
  apple
  orange
  apple
  grape
  orange
 
 s.RemoveDuplicateLines


if(!this.end("[]")) this+"[]"

str s ss
int i j
for i 0 1000000000
	j=s.getl(this i)
	if(j<0) break
	
	ss.from("[]" s "[]")
	this.findreplace(ss "[]" 8|(flags&1) "" j+s.len)
