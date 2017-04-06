 Although controls in .NET windows ("Windows Forms") have id, it is different
 each time you open the window. You cannot use function id() to find them.
 Sometimes you also cannot use function child() and acc(), because they may have no text.
 But these controls have a name property. It is set at development time and usually
 is unique in that window.

 This class has functions to get the name property and find controls using it.
 Use this class only with .NET windows. Controls in other windows don't have a name property.
 You can recognize .NET windows by class names. They are like "WindowsForms10.Window....".
 To discover control names in a .NET window, use function DotNetShowControlNames.

 Fails if the process belongs to another user.
 On Vista, fails if QM is running as User and the process has higher integrity level.

 See also: <DotNetFindControl>  <DotNetShowControlNames>  (they use this class)
