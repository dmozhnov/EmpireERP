using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP.Infrastructure.NHibernate.Repositories.Criteria
{
    public class OrExpression : BaseExpression,/* ISubExpression,*/ IExpression
    {
        #region Поля

        private IExpression left;
        private IExpression right;

        #endregion

        #region Конструктор

        internal OrExpression(IExpression left, IExpression right)
        {
            this.left = left;
            this.right = right;
        }

        #endregion

        #region Методы

        public void Compile(ref global::NHibernate.ICriteria iCriteria)
        {
            iCriteria.Add(Compile(ref iCriteria, String.Empty));
        }

        public global::NHibernate.Criterion.ICriterion Compile(ref global::NHibernate.ICriteria iCriteria, string alias)
        {
            var leftExpr = left.Compile(ref iCriteria, alias);
            var rightExpr = right.Compile(ref iCriteria, alias);

            return global::NHibernate.Criterion.Expression.Or(leftExpr, rightExpr);
        }

        #endregion
    }
}
