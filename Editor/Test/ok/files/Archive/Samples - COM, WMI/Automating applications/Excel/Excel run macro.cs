 Run Excel macro Macro1. Excel must be running,
 and macro Macro1 must exist in current document.
Excel.Application app._getactive
app.Run("Macro1")


 Here we don't declare Excel type library like in Word
 sample because it is already declared. You can see declared
 libraries at the bottom of the popup list when you type
 period.
