selenium.AddLocationStrategy("a", "b");
selenium.AddScript("a", "b");
selenium.AddSelection("a", "b");
selenium.AllowNativeXpath("a");
selenium.AltKeyDown();
selenium.AltKeyUp();
selenium.AnswerOnNextPrompt("a");
Assert.AreEqual("a", selenium.GetAlert());
Assert.IsFalse(selenium.IsAlertPresent());
Assert.IsTrue(selenium.IsAlertPresent());
Assert.AreEqual("a", String.Join(",", selenium.GetAllButtons()));
Assert.AreEqual("a", String.Join(",", selenium.GetAllFields()));
Assert.AreEqual("a", String.Join(",", selenium.GetAllLinks()));
Assert.AreEqual("a", String.Join(",", selenium.GetAllWindowIds()));
Assert.AreEqual("a", String.Join(",", selenium.GetAllWindowNames()));
Assert.AreEqual("a", String.Join(",", selenium.GetAllWindowTitles()));
Assert.AreEqual("b", selenium.GetAttribute("a"));
Assert.AreEqual("b", String.Join(",", selenium.GetAttributeFromAllWindows("a")));
Assert.AreEqual("a", selenium.GetBodyText());
Assert.IsTrue(selenium.IsChecked("a"));
Assert.AreEqual("a", selenium.GetConfirmation());
Assert.IsFalse(selenium.IsConfirmationPresent());
Assert.IsTrue(selenium.IsConfirmationPresent());
Assert.AreEqual("a", selenium.GetCookie());
Assert.AreEqual("b", selenium.GetCookieByName("a"));
Assert.IsFalse(selenium.IsCookiePresent("a"));
Assert.IsTrue(selenium.IsCookiePresent("a"));
Assert.AreEqual("b", selenium.GetCssCount("a"));
Assert.AreEqual("b", selenium.GetCursorPosition("a"));
Assert.IsTrue(selenium.IsEditable("a"));
Assert.AreEqual("b", selenium.GetElementHeight("a"));
Assert.AreEqual("b", selenium.GetElementIndex("a"));
Assert.IsFalse(selenium.IsElementPresent("a"));
Assert.AreEqual("b", selenium.GetElementPositionLeft("a"));
Assert.AreEqual("b", selenium.GetElementPositionTop("a"));
Assert.IsTrue(selenium.IsElementPresent("a"));
Assert.AreEqual("b", selenium.GetElementWidth("a"));
Assert.AreEqual("b", selenium.GetEval("a"));
Assert.AreEqual("b", selenium.GetExpression("a"));
Assert.AreEqual("a", selenium.GetHtmlSource());
Assert.AreEqual("a", selenium.GetLocation());
Assert.AreEqual("a", selenium.GetMouseSpeed());
Assert.AreNotEqual("a", selenium.GetAlert());
Assert.AreNotEqual("a", String.Join(",", selenium.GetAllButtons()));
Assert.AreNotEqual("a", String.Join(",", selenium.GetAllFields()));
Assert.AreNotEqual("a", String.Join(",", selenium.GetAllLinks()));
Assert.AreNotEqual("a", String.Join(",", selenium.GetAllWindowIds()));
Assert.AreNotEqual("a", String.Join(",", selenium.GetAllWindowNames()));
Assert.AreNotEqual("a", String.Join(",", selenium.GetAllWindowTitles()));
Assert.AreNotEqual("b", selenium.GetAttribute("a"));
Assert.AreNotEqual("b", String.Join(",", selenium.GetAttributeFromAllWindows("a")));
Assert.AreNotEqual("a", selenium.GetBodyText());
Assert.IsFalse(selenium.IsChecked("a"));
Assert.AreNotEqual("a", selenium.GetConfirmation());
Assert.AreNotEqual("a", selenium.GetCookie());
Assert.AreNotEqual("b", selenium.GetCookieByName("a"));
Assert.AreNotEqual("b", selenium.GetCssCount("a"));
Assert.AreNotEqual("b", selenium.GetCursorPosition("a"));
Assert.IsFalse(selenium.IsEditable("a"));
Assert.AreNotEqual("b", selenium.GetElementHeight("a"));
Assert.AreNotEqual("b", selenium.GetElementIndex("a"));
Assert.AreNotEqual("b", selenium.GetElementPositionLeft("a"));
Assert.AreNotEqual("b", selenium.GetElementPositionTop("a"));
Assert.AreNotEqual("b", selenium.GetElementWidth("a"));
Assert.AreNotEqual("b", selenium.GetEval("a"));
Assert.AreNotEqual("b", selenium.GetExpression("a"));
Assert.AreNotEqual("a", selenium.GetHtmlSource());
Assert.AreNotEqual("a", selenium.GetLocation());
Assert.AreNotEqual("a", selenium.GetMouseSpeed());
Assert.IsFalse(selenium.IsOrdered("a", "b"));
Assert.AreNotEqual("a", selenium.GetPrompt());
Assert.AreNotEqual("b", String.Join(",", selenium.GetSelectOptions("a")));
Assert.AreNotEqual("b", selenium.GetSelectedId("a"));
Assert.AreNotEqual("b", String.Join(",", selenium.GetSelectedIds("a")));
Assert.AreNotEqual("b", selenium.GetSelectedIndex("a"));
Assert.AreNotEqual("b", String.Join(",", selenium.GetSelectedIndexes("a")));
Assert.AreNotEqual("b", selenium.GetSelectedLabel("a"));
Assert.AreNotEqual("b", String.Join(",", selenium.GetSelectedLabels("a")));
Assert.AreNotEqual("b", selenium.GetSelectedValue("a"));
Assert.AreNotEqual("b", String.Join(",", selenium.GetSelectedValues("a")));
Assert.IsFalse(selenium.IsSomethingSelected("a"));
Assert.AreNotEqual("a", selenium.GetSpeed());
Assert.AreNotEqual("b", selenium.GetTable("a"));
Assert.AreNotEqual("b", selenium.GetText("a"));
Assert.AreNotEqual("a", selenium.GetTitle());
Assert.AreNotEqual("b", selenium.GetValue("a"));
Assert.IsFalse(selenium.IsVisible("a"));
Assert.AreNotEqual("b", selenium.GetXpathCount("a"));
Assert.IsTrue(selenium.IsOrdered("a", "b"));
Assert.AreEqual("a", selenium.GetPrompt());
Assert.IsFalse(selenium.IsPromptPresent());
Assert.IsTrue(selenium.IsPromptPresent());
Assert.AreEqual("b", String.Join(",", selenium.GetSelectOptions("a")));
Assert.AreEqual("b", selenium.GetSelectedId("a"));
Assert.AreEqual("b", String.Join(",", selenium.GetSelectedIds("a")));
Assert.AreEqual("b", selenium.GetSelectedIndex("a"));
Assert.AreEqual("b", String.Join(",", selenium.GetSelectedIndexes("a")));
Assert.AreEqual("b", selenium.GetSelectedLabel("a"));
Assert.AreEqual("b", String.Join(",", selenium.GetSelectedLabels("a")));
Assert.AreEqual("b", selenium.GetSelectedValue("a"));
Assert.AreEqual("b", String.Join(",", selenium.GetSelectedValues("a")));
Assert.IsTrue(selenium.IsSomethingSelected("a"));
Assert.AreEqual("a", selenium.GetSpeed());
Assert.AreEqual("b", selenium.GetTable("a"));
Assert.AreEqual("b", selenium.GetText("a"));
Assert.IsFalse(selenium.IsTextPresent("a"));
Assert.IsTrue(selenium.IsTextPresent("a"));
Assert.AreEqual("a", selenium.GetTitle());
Assert.AreEqual("b", selenium.GetValue("a"));
Assert.IsTrue(selenium.IsVisible("a"));
Assert.AreEqual("b", selenium.GetXpathCount("a"));
selenium.AssignId("a", "b");
selenium.Break();
selenium.CaptureEntirePageScreenshot("a", "b");
selenium.Check("a");
selenium.ChooseCancelOnNextConfirmation();
selenium.ChooseOkOnNextConfirmation();
selenium.Click("a");
selenium.ClickAt("a", "b");
selenium.Close();
selenium.ContextMenu("a");
selenium.ContextMenuAt("a", "b");
selenium.ControlKeyDown();
selenium.ControlKeyUp();
selenium.CreateCookie("a", "b");
selenium.DeleteAllVisibleCookies();
selenium.DeleteCookie("a", "b");
selenium.DeselectPopUp();
selenium.DoubleClick("a");
selenium.DoubleClickAt("a", "b");
selenium.DragAndDrop("a", "b");
selenium.DragAndDropToObject("a", "b");
selenium.Dragdrop("a", "b");
Console.WriteLine("a");
selenium.FireEvent("a", "b");
selenium.Focus("a");
selenium.GoBack();
selenium.Highlight("a");
selenium.IgnoreAttributesWithoutValue("a");
selenium.KeyDown("a", "b");
selenium.KeyPress("a", "b");
selenium.KeyUp("a", "b");
selenium.MetaKeyDown();
selenium.MetaKeyUp();
selenium.MouseDown("a");
selenium.MouseDownAt("a", "b");
selenium.MouseDownRight("a");
selenium.MouseDownRightAt("a", "b");
selenium.MouseMove("a");
selenium.MouseMoveAt("a", "b");
selenium.MouseOut("a");
selenium.MouseOver("a");
selenium.MouseUp("a");
selenium.MouseUpAt("a", "b");
selenium.MouseUpRight("a");
selenium.MouseUpRightAt("a", "b");
selenium.Open("a");
selenium.OpenWindow("a", "b");
Thread.Sleep(2500);
selenium.Refresh();
selenium.RemoveAllSelections("a");
selenium.RemoveScript("a");
selenium.RemoveSelection("a", "b");
selenium.Rollup("a", "b");
selenium.RunScript("a");
selenium.Select("a", "b");
selenium.SelectFrame("a");
selenium.SelectPopUp("a");
selenium.SelectWindow("a");
selenium.SendKeys("a", "b");
selenium.SetBrowserLogLevel("a");
selenium.SetCursorPosition("a", "b");
selenium.SetMouseSpeed("a");
selenium.SetSpeed("a");
selenium.SetTimeout("a");
selenium.ShiftKeyDown();
selenium.ShiftKeyUp();
String b = "a";
String a = selenium.GetAlert();
Boolean a = selenium.IsAlertPresent();
String[] a = selenium.GetAllButtons();
String[] a = selenium.GetAllFields();
String[] a = selenium.GetAllLinks();
String[] a = selenium.GetAllWindowIds();
String[] a = selenium.GetAllWindowNames();
String[] a = selenium.GetAllWindowTitles();
String b = selenium.GetAttribute("a");
String[] b = selenium.GetAttributeFromAllWindows("a");
String a = selenium.GetBodyText();
Boolean b = selenium.IsChecked("a");
String a = selenium.GetConfirmation();
Boolean a = selenium.IsConfirmationPresent();
String a = selenium.GetCookie();
String b = selenium.GetCookieByName("a");
Boolean b = selenium.IsCookiePresent("a");
Number b = selenium.GetCssCount("a");
Number b = selenium.GetCursorPosition("a");
Boolean b = selenium.IsEditable("a");
Number b = selenium.GetElementHeight("a");
Number b = selenium.GetElementIndex("a");
Number b = selenium.GetElementPositionLeft("a");
Number b = selenium.GetElementPositionTop("a");
Boolean b = selenium.IsElementPresent("a");
Number b = selenium.GetElementWidth("a");
String b = selenium.GetEval("a");
String b = selenium.GetExpression("a");
String a = selenium.GetHtmlSource();
String a = selenium.GetLocation();
Number a = selenium.GetMouseSpeed();
Boolean null = selenium.IsOrdered("a", "b");
String a = selenium.GetPrompt();
Boolean a = selenium.IsPromptPresent();
String[] b = selenium.GetSelectOptions("a");
String b = selenium.GetSelectedId("a");
String[] b = selenium.GetSelectedIds("a");
String b = selenium.GetSelectedIndex("a");
String[] b = selenium.GetSelectedIndexes("a");
String b = selenium.GetSelectedLabel("a");
String[] b = selenium.GetSelectedLabels("a");
String b = selenium.GetSelectedValue("a");
String[] b = selenium.GetSelectedValues("a");
Boolean b = selenium.IsSomethingSelected("a");
String a = selenium.GetSpeed();
String b = selenium.GetTable("a");
String b = selenium.GetText("a");
Boolean b = selenium.IsTextPresent("a");
String a = selenium.GetTitle();
String b = selenium.GetValue("a");
Boolean b = selenium.IsVisible("a");
Boolean null = selenium.GetWhetherThisFrameMatchFrameExpression("a", "b");
Boolean null = selenium.GetWhetherThisWindowMatchWindowExpression("a", "b");
Number b = selenium.GetXpathCount("a");
selenium.Submit("a");
selenium.Type("a", "b");
selenium.TypeKeys("a", "b");
selenium.Uncheck("a");
selenium.UseXpathLibrary("a");
try { Assert.AreEqual("a", selenium.GetAlert()); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.IsFalse(selenium.IsAlertPresent()); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.IsTrue(selenium.IsAlertPresent()); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.AreEqual("a", String.Join(",", selenium.GetAllButtons())); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.AreEqual("a", String.Join(",", selenium.GetAllFields())); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.AreEqual("a", String.Join(",", selenium.GetAllLinks())); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.AreEqual("a", String.Join(",", selenium.GetAllWindowIds())); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.AreEqual("a", String.Join(",", selenium.GetAllWindowNames())); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.AreEqual("a", String.Join(",", selenium.GetAllWindowTitles())); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.AreEqual("b", selenium.GetAttribute("a")); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.AreEqual("b", String.Join(",", selenium.GetAttributeFromAllWindows("a"))); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.AreEqual("a", selenium.GetBodyText()); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.IsTrue(selenium.IsChecked("a")); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.AreEqual("a", selenium.GetConfirmation()); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.IsFalse(selenium.IsConfirmationPresent()); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.IsTrue(selenium.IsConfirmationPresent()); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.AreEqual("a", selenium.GetCookie()); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.AreEqual("b", selenium.GetCookieByName("a")); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.IsFalse(selenium.IsCookiePresent("a")); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.IsTrue(selenium.IsCookiePresent("a")); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.AreEqual("b", selenium.GetCssCount("a")); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.AreEqual("b", selenium.GetCursorPosition("a")); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.IsTrue(selenium.IsEditable("a")); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.AreEqual("b", selenium.GetElementHeight("a")); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.AreEqual("b", selenium.GetElementIndex("a")); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.IsFalse(selenium.IsElementPresent("a")); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.AreEqual("b", selenium.GetElementPositionLeft("a")); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.AreEqual("b", selenium.GetElementPositionTop("a")); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.IsTrue(selenium.IsElementPresent("a")); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.AreEqual("b", selenium.GetElementWidth("a")); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.AreEqual("b", selenium.GetEval("a")); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.AreEqual("b", selenium.GetExpression("a")); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.AreEqual("a", selenium.GetHtmlSource()); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.AreEqual("a", selenium.GetLocation()); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.AreEqual("a", selenium.GetMouseSpeed()); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.AreNotEqual("a", selenium.GetAlert()); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.AreNotEqual("a", String.Join(",", selenium.GetAllButtons())); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.AreNotEqual("a", String.Join(",", selenium.GetAllFields())); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.AreNotEqual("a", String.Join(",", selenium.GetAllLinks())); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.AreNotEqual("a", String.Join(",", selenium.GetAllWindowIds())); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.AreNotEqual("a", String.Join(",", selenium.GetAllWindowNames())); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.AreNotEqual("a", String.Join(",", selenium.GetAllWindowTitles())); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.AreNotEqual("b", selenium.GetAttribute("a")); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.AreNotEqual("b", String.Join(",", selenium.GetAttributeFromAllWindows("a"))); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.AreNotEqual("a", selenium.GetBodyText()); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.IsFalse(selenium.IsChecked("a")); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.AreNotEqual("a", selenium.GetConfirmation()); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.AreNotEqual("a", selenium.GetCookie()); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.AreNotEqual("b", selenium.GetCookieByName("a")); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.AreNotEqual("b", selenium.GetCssCount("a")); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.AreNotEqual("b", selenium.GetCursorPosition("a")); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.IsFalse(selenium.IsEditable("a")); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.AreNotEqual("b", selenium.GetElementHeight("a")); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.AreNotEqual("b", selenium.GetElementIndex("a")); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.AreNotEqual("b", selenium.GetElementPositionLeft("a")); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.AreNotEqual("b", selenium.GetElementPositionTop("a")); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.AreNotEqual("b", selenium.GetElementWidth("a")); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.AreNotEqual("b", selenium.GetEval("a")); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.AreNotEqual("b", selenium.GetExpression("a")); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.AreNotEqual("a", selenium.GetHtmlSource()); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.AreNotEqual("a", selenium.GetLocation()); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.AreNotEqual("a", selenium.GetMouseSpeed()); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.IsFalse(selenium.IsOrdered("a", "b")); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.AreNotEqual("a", selenium.GetPrompt()); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.AreNotEqual("b", String.Join(",", selenium.GetSelectOptions("a"))); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.AreNotEqual("b", selenium.GetSelectedId("a")); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.AreNotEqual("b", String.Join(",", selenium.GetSelectedIds("a"))); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.AreNotEqual("b", selenium.GetSelectedIndex("a")); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.AreNotEqual("b", String.Join(",", selenium.GetSelectedIndexes("a"))); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.AreNotEqual("b", selenium.GetSelectedLabel("a")); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.AreNotEqual("b", String.Join(",", selenium.GetSelectedLabels("a"))); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.AreNotEqual("b", selenium.GetSelectedValue("a")); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.AreNotEqual("b", String.Join(",", selenium.GetSelectedValues("a"))); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.IsFalse(selenium.IsSomethingSelected("a")); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.AreNotEqual("a", selenium.GetSpeed()); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.AreNotEqual("b", selenium.GetTable("a")); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.AreNotEqual("b", selenium.GetText("a")); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.AreNotEqual("a", selenium.GetTitle()); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.AreNotEqual("b", selenium.GetValue("a")); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.IsFalse(selenium.IsVisible("a")); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.AreNotEqual("b", selenium.GetXpathCount("a")); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.IsTrue(selenium.IsOrdered("a", "b")); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.AreEqual("a", selenium.GetPrompt()); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.IsFalse(selenium.IsPromptPresent()); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.IsTrue(selenium.IsPromptPresent()); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.AreEqual("b", String.Join(",", selenium.GetSelectOptions("a"))); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.AreEqual("b", selenium.GetSelectedId("a")); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.AreEqual("b", String.Join(",", selenium.GetSelectedIds("a"))); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.AreEqual("b", selenium.GetSelectedIndex("a")); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.AreEqual("b", String.Join(",", selenium.GetSelectedIndexes("a"))); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.AreEqual("b", selenium.GetSelectedLabel("a")); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.AreEqual("b", String.Join(",", selenium.GetSelectedLabels("a"))); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.AreEqual("b", selenium.GetSelectedValue("a")); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.AreEqual("b", String.Join(",", selenium.GetSelectedValues("a"))); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.IsTrue(selenium.IsSomethingSelected("a")); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.AreEqual("a", selenium.GetSpeed()); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.AreEqual("b", selenium.GetTable("a")); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.AreEqual("b", selenium.GetText("a")); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.IsFalse(selenium.IsTextPresent("a")); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.IsTrue(selenium.IsTextPresent("a")); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.AreEqual("a", selenium.GetTitle()); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.AreEqual("b", selenium.GetValue("a")); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.IsTrue(selenium.IsVisible("a")); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
try { Assert.AreEqual("b", selenium.GetXpathCount("a")); } catch (AssertionException e) { verificationErrors.Append(e.Message); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if ("a" == selenium.GetAlert()) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if (!selenium.IsAlertPresent()) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if (selenium.IsAlertPresent()) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if ("a" == String.Join(",", selenium.GetAllButtons())) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if ("a" == String.Join(",", selenium.GetAllFields())) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if ("a" == String.Join(",", selenium.GetAllLinks())) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if ("a" == String.Join(",", selenium.GetAllWindowIds())) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if ("a" == String.Join(",", selenium.GetAllWindowNames())) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if ("a" == String.Join(",", selenium.GetAllWindowTitles())) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if ("b" == selenium.GetAttribute("a")) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if ("b" == String.Join(",", selenium.GetAttributeFromAllWindows("a"))) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if ("a" == selenium.GetBodyText()) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if (selenium.IsChecked("a")) break; } catch (Exception) {  }  Thread.Sleep(1000); }
selenium.WaitForCondition("a", "b");
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if ("a" == selenium.GetConfirmation()) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if (!selenium.IsConfirmationPresent()) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if (selenium.IsConfirmationPresent()) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if ("a" == selenium.GetCookie()) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if ("b" == selenium.GetCookieByName("a")) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if (!selenium.IsCookiePresent("a")) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if (selenium.IsCookiePresent("a")) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if ("b" == selenium.GetCssCount("a")) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if ("b" == selenium.GetCursorPosition("a")) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if (selenium.IsEditable("a")) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if ("b" == selenium.GetElementHeight("a")) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if ("b" == selenium.GetElementIndex("a")) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if (!selenium.IsElementPresent("a")) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if ("b" == selenium.GetElementPositionLeft("a")) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if ("b" == selenium.GetElementPositionTop("a")) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if (selenium.IsElementPresent("a")) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if ("b" == selenium.GetElementWidth("a")) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if ("b" == selenium.GetEval("a")) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if ("b" == selenium.GetExpression("a")) break; } catch (Exception) {  }  Thread.Sleep(1000); }
selenium.WaitForFrameToLoad("a", "b");
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if ("a" == selenium.GetHtmlSource()) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if ("a" == selenium.GetLocation()) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if ("a" == selenium.GetMouseSpeed()) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if ("a" != selenium.GetAlert()) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if ("a" != String.Join(",", selenium.GetAllButtons())) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if ("a" != String.Join(",", selenium.GetAllFields())) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if ("a" != String.Join(",", selenium.GetAllLinks())) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if ("a" != String.Join(",", selenium.GetAllWindowIds())) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if ("a" != String.Join(",", selenium.GetAllWindowNames())) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if ("a" != String.Join(",", selenium.GetAllWindowTitles())) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if ("b" != selenium.GetAttribute("a")) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if ("b" != String.Join(",", selenium.GetAttributeFromAllWindows("a"))) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if ("a" != selenium.GetBodyText()) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if (!selenium.IsChecked("a")) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if ("a" != selenium.GetConfirmation()) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if ("a" != selenium.GetCookie()) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if ("b" != selenium.GetCookieByName("a")) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if ("b" != selenium.GetCssCount("a")) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if ("b" != selenium.GetCursorPosition("a")) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if (!selenium.IsEditable("a")) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if ("b" != selenium.GetElementHeight("a")) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if ("b" != selenium.GetElementIndex("a")) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if ("b" != selenium.GetElementPositionLeft("a")) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if ("b" != selenium.GetElementPositionTop("a")) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if ("b" != selenium.GetElementWidth("a")) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if ("b" != selenium.GetEval("a")) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if ("b" != selenium.GetExpression("a")) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if ("a" != selenium.GetHtmlSource()) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if ("a" != selenium.GetLocation()) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if ("a" != selenium.GetMouseSpeed()) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if (!selenium.IsOrdered("a", "b")) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if ("a" != selenium.GetPrompt()) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if ("b" != String.Join(",", selenium.GetSelectOptions("a"))) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if ("b" != selenium.GetSelectedId("a")) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if ("b" != String.Join(",", selenium.GetSelectedIds("a"))) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if ("b" != selenium.GetSelectedIndex("a")) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if ("b" != String.Join(",", selenium.GetSelectedIndexes("a"))) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if ("b" != selenium.GetSelectedLabel("a")) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if ("b" != String.Join(",", selenium.GetSelectedLabels("a"))) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if ("b" != selenium.GetSelectedValue("a")) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if ("b" != String.Join(",", selenium.GetSelectedValues("a"))) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if (!selenium.IsSomethingSelected("a")) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if ("a" != selenium.GetSpeed()) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if ("b" != selenium.GetTable("a")) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if ("b" != selenium.GetText("a")) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if ("a" != selenium.GetTitle()) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if ("b" != selenium.GetValue("a")) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if (!selenium.IsVisible("a")) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if ("b" != selenium.GetXpathCount("a")) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if (selenium.IsOrdered("a", "b")) break; } catch (Exception) {  }  Thread.Sleep(1000); }
selenium.WaitForPageToLoad("a");
selenium.WaitForPopUp("a", "b");
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if ("a" == selenium.GetPrompt()) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if (!selenium.IsPromptPresent()) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if (selenium.IsPromptPresent()) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if ("b" == String.Join(",", selenium.GetSelectOptions("a"))) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if ("b" == selenium.GetSelectedId("a")) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if ("b" == String.Join(",", selenium.GetSelectedIds("a"))) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if ("b" == selenium.GetSelectedIndex("a")) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if ("b" == String.Join(",", selenium.GetSelectedIndexes("a"))) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if ("b" == selenium.GetSelectedLabel("a")) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if ("b" == String.Join(",", selenium.GetSelectedLabels("a"))) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if ("b" == selenium.GetSelectedValue("a")) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if ("b" == String.Join(",", selenium.GetSelectedValues("a"))) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if (selenium.IsSomethingSelected("a")) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if ("a" == selenium.GetSpeed()) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if ("b" == selenium.GetTable("a")) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if ("b" == selenium.GetText("a")) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if (!selenium.IsTextPresent("a")) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if (selenium.IsTextPresent("a")) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if ("a" == selenium.GetTitle()) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if ("b" == selenium.GetValue("a")) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if (selenium.IsVisible("a")) break; } catch (Exception) {  }  Thread.Sleep(1000); }
for(int second = 0;; second++) { if (second >= 60) Assert.Fail("timeout");  try { if ("b" == selenium.GetXpathCount("a")) break; } catch (Exception) {  }  Thread.Sleep(1000); }
selenium.WindowFocus();
selenium.WindowMaximize();
