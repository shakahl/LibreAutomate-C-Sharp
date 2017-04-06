function~

 Gets text. If fails, returns empty string.

 REMARKS
 Use this function to get text of 'text' nodes (node type 3).
 Node text is unavailable for 'element' nodes (node type 1). This function returns empty string. Use HTML() instead.
 In Chrome gets only text of 'element' nodes.


if(!node) end ERR_INIT
Info(0 _s)
ret _s

err+

 info: ISimpleDOMText gets same text, but it is unavailable for script etc.
