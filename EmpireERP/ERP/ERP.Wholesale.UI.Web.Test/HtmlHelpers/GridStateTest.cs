using ERP.UI.ViewModels.Grid;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ERP.Wholesale.UI.Web.Test.HtmlHelpers
{
    /// <summary>
    /// Summary description for GridStateTest
    /// </summary>
    [TestClass]
    public class GridStateTest
    {
        public GridStateTest()
        {
        }

        [TestMethod]
        public void GridState_Initial_Parameters_Must_Be_Set()
        {
            var state = new GridState();

            Assert.AreEqual(1, state.CurrentPage);
            Assert.AreEqual(10, state.PageSize);
            Assert.AreEqual(0, state.TotalRow);
        }

        [TestMethod]
        public void GridState_Data_Is_Correct1()
        {
            var state = new GridState();
            state.CurrentPage = 1;
            state.PageSize = 10;
            state.TotalRow = 20;
            state.CheckAndCorrectCurrentPage();

            Assert.AreEqual(1, state.CurrentPage);
        }

        [TestMethod]
        public void GridState_Data_Is_Correct2()
        {
            var state = new GridState();
            state.CurrentPage = 3;
            state.PageSize = 10;
            state.TotalRow = 25;
            state.CheckAndCorrectCurrentPage();

            Assert.AreEqual(3, state.CurrentPage);
        }

        [TestMethod]
        public void GridState_CurrentPage_Is_Too_Big()
        {
            var state = new GridState();
            state.CurrentPage = 30;
            state.PageSize = 10;
            state.TotalRow = 25;
            state.CheckAndCorrectCurrentPage();

            Assert.AreEqual(3, state.CurrentPage);
        }

        [TestMethod]
        public void GridState_CurrentPage_Is_Too_Small()
        {
            var state = new GridState();
            state.CurrentPage = 0;
            state.PageSize = 10;
            state.TotalRow = 25;
            state.CheckAndCorrectCurrentPage();

            Assert.AreEqual(1, state.CurrentPage);
        }
    }
}
