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
    public class EmailTests
    {
        [TestMethod()]
        public void ValidarEmailTest()
        {
            Assert.IsFalse(Email.ValidarEmail("oscargmail.com")); // No tiene '@'
            Assert.IsFalse(Email.ValidarEmail("oscar@gmailcom")); // No tiene '.'
            Assert.IsFalse(Email.ValidarEmail("oscar@.com")); // No tiene texto antes del '.'
            Assert.IsFalse(Email.ValidarEmail("oscar@gmail.")); // No tiene texto después del '.'
            Assert.IsTrue(Email.ValidarEmail("gepeto@gmail.com"));
        }
    }
}