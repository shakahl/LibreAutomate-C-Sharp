 Wanted to add keyword 'clear' that clears any variable.
 Frees everything (calls dtor if exists) and zero mem.
 Implemented, tested (now using keyword __test) with all types. Everything works.
 Still need to decide how to clear str. Better don't clear flags. But then difficult to do it with str that are class members.
 Decided not to add the keyword. Not hard to find info how to free str, ARRAY, interface. Rarely need to clear other types.
