
namespace ERP.Wholesale.UI.ViewModels.Report.Report0002
{
    /// <summary>
    /// Информация о МХ, выводимом в столбце
    /// </summary>
    public class Report0002_StorageInfoItem
    {
        #region Свойства

        /// <summary>
        /// Код МХ
        /// </summary>
        public short Id { get; set; }

        /// <summary>
        /// Название МХ
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///  Код типа МХ
        /// </summary>
        public byte TypeId { get; set; }

        #endregion

        public override bool Equals(object obj)
        {
            var right = obj as Report0002_StorageInfoItem;
            if (right != null)
                return Id == right.Id;

            return base.Equals(obj);
        }
    }
}
