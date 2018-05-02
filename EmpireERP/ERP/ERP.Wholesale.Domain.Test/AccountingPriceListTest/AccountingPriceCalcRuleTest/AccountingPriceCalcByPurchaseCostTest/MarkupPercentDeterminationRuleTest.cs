using ERP.Wholesale.Domain.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ERP.Wholesale.Domain.Test
{
    [TestClass]
    public class MarkupPercentDeterminationRuleTest
    {
        [TestMethod]
        public void MarkupPercentDeterminationRule_Initial_parameters_Must_Be_Set()
        {
            var mpRule = new MarkupPercentDeterminationRule(MarkupPercentDeterminationRuleType.ByArticle);

            Assert.AreEqual(MarkupPercentDeterminationRuleType.ByArticle, mpRule.Type);
            Assert.IsNull(mpRule.MarkupPercentValue);
        }

        
        [TestMethod]
        public void MarkupPercentDeterminationRule_Must_Set_Type_To_Custom_When_MarkupPercentValue_Is_Set_In_Constructor()
        {
            var mpRule = new MarkupPercentDeterminationRule(75);

            Assert.AreEqual(MarkupPercentDeterminationRuleType.Custom, mpRule.Type);
            Assert.AreEqual(75, mpRule.MarkupPercentValue);
        }
    }
}
