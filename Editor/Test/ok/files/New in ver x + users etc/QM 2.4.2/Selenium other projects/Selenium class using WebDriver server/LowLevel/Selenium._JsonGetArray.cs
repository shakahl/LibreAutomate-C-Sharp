function! $json $name ARRAY(str)&a [$getValueOf] [valueType] ;;valueType: 0 any, 1 string or null, 2 number, 3 bool, 4 object, 5 array

 Finds "name":[array] and gets array elements.
 If getValueOf, for each element calls _JsonGetValue with valueType. Else gets raw elements.
 Returns: 1 success, 0 failed or type mismatch.
 No errors.


a=0
int i
if(!_JsonGetValue(json name _s 5)) ret
tok(_s a -1 "," 0x2020)

if !empty(getValueOf)
	for i 0 a.len
		if(!_JsonGetValue(a[i] getValueOf _s valueType)) ret
		_s.swap(a[i])

ret 1
