using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ERP.Utils.Test
{
    [TestClass]
    public class ParameterStringTest
    {
        [TestMethod]
        public void ReadString()
        {
            var ps = new ParameterString("Key=qwerty;");

            Assert.AreEqual(1, ps.Keys.Count());
            Assert.AreEqual("Key", ps.Keys.ElementAt(0));
            Assert.AreEqual("qwerty", ps["Key"].Value as string);
        }

        [TestMethod]
        public void MergeWithOneIntersectionItem()
        {
            var ps1 = new ParameterString("Key=1,2,3;");
            var ps2 = new ParameterString("Key=3,4,5;");

            ps1.MergeWith(ps2);

            Assert.AreEqual(1, ps1.Keys.Count());
            Assert.AreEqual("Key", ps1.Keys.ElementAt(0));
            Assert.AreEqual("3", (ps1["Key"].Value as IList<string>)[0]);
        }

        [TestMethod]
        public void MergeWithManyIntersectionItem()
        {
            var ps1 = new ParameterString("Key=1,2,3,4,5;");
            var ps2 = new ParameterString("Key=3,4,5,6,7;");

            ps1.MergeWith(ps2);

            Assert.AreEqual(1, ps1.Keys.Count());
            Assert.AreEqual("Key", ps1.Keys.ElementAt(0));
            Assert.AreEqual("3", (ps1["Key"].Value as IList<string>)[0]);
            Assert.AreEqual("4", (ps1["Key"].Value as IList<string>)[1]);
            Assert.AreEqual("5", (ps1["Key"].Value as IList<string>)[2]);
        }
        
        [TestMethod]
        public void MergeWithManyIntersectionItemAndOtherKeys()
        {
            var ps1 = new ParameterString("Key=1,2,3,4,5;");
            var ps2 = new ParameterString("Key=3,4,5,6,7;Param=qwerty;");

            ps1.MergeWith(ps2);

            Assert.AreEqual(2, ps1.Keys.Count());
            Assert.AreEqual(true, ps1.Keys.Contains("Key"));
            Assert.AreEqual(true, ps1.Keys.Contains("Param"));
        }

        [TestMethod]
        public void MergeWithNullIntersectionItem()
        {
            var ps1 = new ParameterString("Key=1,2,3,4,5;");
            var ps2 = new ParameterString("Key=6,7,8,9,10;");

            ps1.MergeWith(ps2);

            Assert.AreEqual(1, ps1.Keys.Count());
            Assert.AreEqual("0", (ps1["Key"].Value as IList<string>)[0]);
        }

        [TestMethod]
        public void MergeWithNotEqualKeys_Empty_Value_Mast_Be_Null()
        {
            var ps1 = new ParameterString("Key=;");
            var ps2 = new ParameterString("Param=;");

            ps1.MergeWith(ps2);

            Assert.AreEqual(null, ps1["Param"].Value);
        }
    }
}
