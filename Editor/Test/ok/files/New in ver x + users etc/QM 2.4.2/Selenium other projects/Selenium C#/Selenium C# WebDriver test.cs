 out
PerfFirst
CsScript x
str so=
 searchDirs=Q:\Downloads\selenium-dotnet-2.42.0\net40
 references=WebDriver;WebDriver.Support;ThoughtWorks.Selenium.Core;Selenium.WebDriverBackedSelenium
x.SetOptions(so)
x.AddCode("")
PerfNext
IDispatch test=x.CreateObject("SeleniumTests.Case")
PerfNext
test.SetupTest
PerfNext
test.TheCaseTest
PerfNext;PerfOut
mes 1
test.TeardownTest


#ret
//C# code
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
//using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Support.UI;

namespace SeleniumTests
{
    //[TestFixture]
    public class Case
    {
        private IWebDriver driver;
        private StringBuilder verificationErrors;
        private string baseURL;
        private bool acceptNextAlert = true;
        
        //[SetUp]
        public void SetupTest()
        {
            //driver = new FirefoxDriver();
            driver = new ChromeDriver();
            //driver = new InternetExplorerDriver();
            baseURL = "http://www.quickmacros.com";
            verificationErrors = new StringBuilder();
        }
        
        //[TearDown]
        public void TeardownTest()
        {
            try
            {
                driver.Quit();
            }
            catch (Exception)
            {
                // Ignore errors if unable to close the browser
            }
            //Assert.AreEqual("", verificationErrors.ToString());
        }
        
        //[Test]
        public void TheCaseTest()
        {
            driver.Navigate().GoToUrl(baseURL + "/index.html");
            driver.FindElement(By.Id("m_download")).Click();
            //driver.FindElement(By.LinkText("Forum/Resources")).Click();
            //driver.FindElement(By.LinkText("Collected QM apps, functions, samples")).Click();
        }
        private bool IsElementPresent(By by)
        {
            try
            {
                driver.FindElement(by);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }
        
        private bool IsAlertPresent()
        {
            try
            {
                driver.SwitchTo().Alert();
                return true;
            }
            catch (NoAlertPresentException)
            {
                return false;
            }
        }
        
        private string CloseAlertAndGetItsText() {
            try {
                IAlert alert = driver.SwitchTo().Alert();
                string alertText = alert.Text;
                if (acceptNextAlert) {
                    alert.Accept();
                } else {
                    alert.Dismiss();
                }
                return alertText;
            } finally {
                acceptNextAlert = true;
            }
        }
    }
}
