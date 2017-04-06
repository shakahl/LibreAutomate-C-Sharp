
VERY OLD

When autocreating help index after version change, QM hang. Need to end process.
   Next time index still wasn't created. Created OK.
   The OSD was OK, ie showed for several seconds.
   It means that CHI_CreateIndex started but did not end.
   Possible reasons:
      getmacro, EnumQmFolder, etc?
      Acc triggers?
   Probably fixed: fixed bug in d.GetCurrentText, used by getmacro/qmitem: threads lock each other because use ECS1 and SendMessage.

