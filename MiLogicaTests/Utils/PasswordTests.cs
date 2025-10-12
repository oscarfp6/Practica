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
    public class PasswordTests
    {
        [TestMethod()]
        public void ValidarPasswordTest()
        {
            Assert.IsTrue(Password.ValidarPassword("@Contraseñavalida123"));
            Assert.IsFalse(Password.ValidarPassword("short1A@"));
            Assert.IsFalse(Password.ValidarPassword("nouppercase123@"));
            Assert.IsFalse(Password.ValidarPassword("NOLOWERCASE123@"));
            Assert.IsFalse(Password.ValidarPassword("NoDigitsHere!"));
            Assert.IsFalse(Password.ValidarPassword("NoSpecialChar123"));
            Assert.IsFalse(Password.ValidarPassword("     "));
            Assert.IsFalse(Password.ValidarPassword(""));
        }
    }
}