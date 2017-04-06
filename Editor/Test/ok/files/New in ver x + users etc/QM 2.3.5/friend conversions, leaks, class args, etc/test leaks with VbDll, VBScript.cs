str code=
 Set x=CreateObject("Lietuviškas.Class1")
 for i=0 to 20000
 Set d=x.RetObject
 x.ArgObject d 'leak too
 next

VbsExec code
