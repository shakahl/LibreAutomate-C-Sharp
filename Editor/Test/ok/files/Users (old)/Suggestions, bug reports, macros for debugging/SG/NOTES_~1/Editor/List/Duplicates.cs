was getting this error after installing Qm 2.1.0
 Warning : can't add function Function ,  name already exists

(Where "Function" is a qm item i had with that name)
 

This because 2.1.0 doesnt allow a "function" and "macro" with same name 
like 2.0.9 allowed, and a qm template "Function" exists in System folder 2.1.0

All well and good

 Heres the problem (if it is a problem):
But i had 2 other duplicates in my non-system folders. After i got rid of my "Function" item, restarted QM, 
QM didnt give an error regarding the other duplicates: in both cases the duplicates were a macro 
and a function with same name

(If you want example of this, let me know, although you can just use Qm 2.0.9 to create some
duplicate names to test out.)


-----------------


Also, after i renamed one duplicate, then tried to rename it back to original duplicate name, 
it added a # to the name(OK)


-----------

Now i dont know if its worth mentioning, but the warning above made me think that QM
wasn't giving me the item name
i misread it as "Warning: can't add function, name already exists")
