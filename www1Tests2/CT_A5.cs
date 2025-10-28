using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace SeleniumTests
{
    [TestClass]
    public class CTA5
    {
        // --- CAMPOS NO ESTÁTICOS ---
        // Estas variables ahora son únicas para cada test que se ejecute.
        private IWebDriver driver;
        private StringBuilder verificationErrors;
        private string baseURL;
        private bool acceptNextAlert = true;
        private WebDriverWait wait; // El "elemento extra", ahora tampoco es estático


        // [ClassInitialize] SE ELIMINA


        [TestInitialize] // Se ejecuta ANTES de CADA test
        public void InitializeTest()
        {
            // --- CÓDIGO MOVIDO AQUÍ ---
            // 1. Se crea un navegador NUEVO y único para este test.
            driver = new ChromeDriver();
            baseURL = "https://www.google.com/";

            // 2. Se crea el 'wait' usando el 'driver' de ESTE test.
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            // 3. Se inicializa el gestor de errores.
            verificationErrors = new StringBuilder();
        }

        [TestCleanup] // Se ejecuta DESPUÉS de CADA test
        public void CleanupTest()
        {
            // --- CÓDIGO MOVIDO AQUÍ ---
            // 1. Se cierra el navegador de ESTE test.
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

            // 2. Se comprueban los errores de ESTE test.
            Assert.AreEqual("", verificationErrors.ToString());
        }

        // [ClassCleanup] SE ELIMINA

        [TestMethod]
        public void TheCTA5Test()
        {
            // El código de tu test no necesita ningún cambio,
            // ya que ahora utiliza las variables 'driver' y 'wait'
            // que se crearon específicamente para él en [TestInitialize].

            driver.Navigate().GoToUrl("https://localhost:44367/Login.aspx");
            driver.FindElement(By.Id("tbxUsuario")).Click();
            driver.FindElement(By.Id("tbxUsuario")).Clear();
            driver.FindElement(By.Id("tbxUsuario")).SendKeys("prueba@gmail.com");
            driver.FindElement(By.Id("tbxContraseña")).Clear();
            driver.FindElement(By.Id("tbxContraseña")).SendKeys("mala");
            driver.FindElement(By.Id("btnAceptar")).Click();
            driver.FindElement(By.Id("tbxContraseña")).Click();
            driver.FindElement(By.Id("tbxContraseña")).Clear();
            driver.FindElement(By.Id("tbxContraseña")).SendKeys("mala");
            driver.FindElement(By.Id("btnAceptar")).Click();
            driver.FindElement(By.Id("tbxContraseña")).Click();
            driver.FindElement(By.Id("tbxContraseña")).Clear();
            driver.FindElement(By.Id("tbxContraseña")).SendKeys("mala");
            driver.FindElement(By.Id("btnAceptar")).Click();

            // Esperamos HASTA 10 segundos a que el label 'lblIncorrecto' se haga visible
            IWebElement lblError = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("lblIncorrecto")));

            // Verificamos que el texto contenga "Cuenta bloqueada" (ignorando mayúsculas/minúsculas)
            StringAssert.Contains(lblError.Text.ToLower(), "cuenta bloqueada",
                $"El mensaje de error ('{lblError.Text}') no indica que la cuenta esté bloqueada como se esperaba.");

            // Opcional: Verificar que tiene la clase CSS de error
            Assert.AreEqual("error-message", lblError.GetAttribute("class"),
                "El mensaje de error no tiene el estilo CSS esperado ('error-message').");

            // Si llegamos aquí, la verificación es exitosa
            Console.WriteLine("Verificación exitosa: El mensaje de cuenta bloqueada se mostró correctamente.");
        }

        // --- MÉTODOS AUXILIARES ---
        // (No cambian, ya que usan las variables 'driver' y 'acceptNextAlert'
        // que ahora son campos de instancia, lo cual es correcto)

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