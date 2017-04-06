 #opt dispatch 1
typelib Project2 "C:\DevStudio\VB98\ax.dll"
Project2.Class1 c._create
 c.propput=10
 out c.propput

c.propput2(5)=10
out c.propput2(1 0)

ARRAY(VARIANT) a.create(1)
c.propput3(&a)=10
out c.propput3(&a)
#opt dispatch 1
c.propput3(1 2 3)=10
out c.propput3(1 2 3)
