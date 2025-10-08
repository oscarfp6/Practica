using Microsoft.VisualStudio.TestTools.UnitTesting;
using MiLogica.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiLogica.Utils.Tests
{
    [TestClass()]
    public class EncriptarTests
    {
        [TestMethod()]
        public void EncriptarPasswordSHA256Test()
        {
            string password1 = "hola1234";
            string password2 = "hola1234";
            string password1Encriptada = Encriptar.EncriptarPasswordSHA256(password1);
            string password2Encriptada = Encriptar.EncriptarPasswordSHA256(password2);
            Assert.AreEqual(password1Encriptada, password2Encriptada);
        }
    }
}