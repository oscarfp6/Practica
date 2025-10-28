using System;
using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace SeleniumTests
{
    [TestClass]
    public class CTA2
    {

        private static string baseURL;
        private bool acceptNextAlert = true;
        private IWebDriver driver; // ¡YA NO ES ESTÁTICO!
        private StringBuilder verificationErrors;

        // (No necesitamos ClassInitialize)

        [TestInitialize] // Se ejecuta ANTES de CADA test
        public void InitializeTest()
        {
            driver = new ChromeDriver(); // ¡Crea un navegador nuevo para este test!
            verificationErrors = new StringBuilder();
        }

        [TestCleanup] // Se ejecuta DESPUÉS de CADA test
        public void CleanupTest()
        {
            try
            {
                driver.Close(); // Cierra el navegador de este test
                driver.Dispose();
            }
            catch (Exception) { /* Ignorar errores si ya estaba cerrado */ }

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
