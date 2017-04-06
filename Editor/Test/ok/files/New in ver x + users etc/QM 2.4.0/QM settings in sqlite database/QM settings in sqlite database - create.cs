Sqlite x.Open("$my qm$\settings.db3")
str sql=
 BEGIN;
 CREATE TABLE qm(name TEXT UNIQUE,value);
 INSERT INTO qm VALUES('unicode',1);
 INSERT INTO qm VALUES('acclog flags',2);
 INSERT INTO qm VALUES('file','Q:\app\ok.qml');
 INSERT INTO qm VALUES('backups','5m 15m 1h 2h 3h 6h 12h 1d 2d 3d 5d 7d 10d 14d 30d 60d');
 END;
x.Exec(sql)
