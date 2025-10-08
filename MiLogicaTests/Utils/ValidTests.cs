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
    public class ValidTests
    {
        [TestMethod()]
        public void NIFTest()
        {
            Assert.IsFalse(Valid.NIF(""));
            Assert.IsFalse(Valid.NIF("12345678"));
            Assert.IsFalse(Valid.NIF("12345678X"));
            Assert.IsTrue(Valid.NIF("71364350L"));
        }

        [TestMethod()]
        public void IBANTest()
        {
            Assert.IsFalse(Valid.IBAN("ES9121000418456172314546"));
        }

        [TestMethod()]
        public void NombreTest()
        {
            Assert.IsTrue(Valid.Nombre("Oscar"));
            Assert.IsFalse(Valid.Nombre("Oscar4"));
            Assert.IsFalse(Valid.Nombre(""));
            Assert.IsFalse(Valid.Nombre(" "));
        }
    }
}