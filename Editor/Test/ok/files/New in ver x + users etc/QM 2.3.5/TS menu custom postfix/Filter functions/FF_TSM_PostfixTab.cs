 /
function# iid FILTER&f

 Assign this filter function to a TS menu to make it work only if the user-typed postfix is Tab.


int ch; TriggerInfoTsMenu 0 ch 0 0 f
sel(ch) case [0,9] ret iid
ret -2
