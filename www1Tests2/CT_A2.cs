using System;
using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace SeleniumTests
{
    [TestClass]
    public class CTA2
    {
        private static IWebDriver driver;
        private StringBuilder verificationErrors;
        private static string baseURL;
        private bool acceptNextAlert = true;

        [ClassInitialize]
        public static void InitializeClass(TestContext testContext)
        {
            driver = new ChromeDriver();
            baseURL = "https://www.google.com/";
        }

        [ClassCleanup]
        public static void CleanupClass()
        {
            try
            {
                //driver.Quit();// quit does not close the window
                driver.Close();
                driver.Dispose();
            }
            catch (Exception)
            {
                // Ignore errors if unable to close the browser
            }
        }

        [TestInitialize]
        public void InitializeTest()
        {
            verificationErrors = new StringBuilder();
        }

        [TestCleanup]
        public void CleanupTest()
        {
            Assert.AreEqual("", verificationErrors.ToString());
        }

        [TestMethod]
        public void TheCTA2Test()
        {
            driver.Navigate().GoToUrl("https://localhost:44367/Login.aspx");
            driver.FindElement(By.Id("btnRegistrarse")).Click();
            driver.Navigate().GoToUrl("https://localhost:44367/SignUp.aspx");
            driver.FindElement(By.Id("tbxEmailRegistro")).Click();
            driver.FindElement(By.Id("tbxEmailRegistro")).Clear();
            driver.FindElement(By.Id("tbxEmailRegistro")).SendKeys("prueba@gmail.com");
            driver.FindElement(By.Id("tbxPasswordRegistro")).Clear();
            driver.FindElement(By.Id("tbxPasswordRegistro")).SendKeys("@PruebaPassword123");
            driver.FindElement(By.Id("btnConfirmar")).Click();
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

        private string CloseAlertAndGetItsText()
        {
            try
            {
                IAlert alert = driver.SwitchTo().Alert();
                string alertText = alert.Text;
                if (acceptNextAlert)
                {
                    alert.Accept();
                }
                else
                {
                    alert.Dismiss();
                }
                return alertText;
            }
            finally
            {
                acceptNextAlert = true;
            }
        }
    }
}
