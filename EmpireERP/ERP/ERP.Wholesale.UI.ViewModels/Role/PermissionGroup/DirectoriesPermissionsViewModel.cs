
namespace ERP.Wholesale.UI.ViewModels.Role
{
    public class DirectoriesPermissionsViewModel
    {
        public short RoleId { get; set; }

        public bool AllowToEdit { get; set; }
                
        // товары
        public byte Article_List_Details { get; set; }
        public PermissionViewModel Article_List_Details_ViewModel { get; set; }
        
        public byte Article_Create { get; set; }
        public PermissionViewModel Article_Create_ViewModel { get; set; }
        
        public byte Article_Edit { get; set; }
        public PermissionViewModel Article_Edit_ViewModel { get; set; }
        
        public byte Article_Delete { get; set; }
        public PermissionViewModel Article_Delete_ViewModel { get; set; }
        
        // группы товаров
        public byte ArticleGroup_Create { get; set; }
        public PermissionViewModel ArticleGroup_Create_ViewModel { get; set; }
        
        public byte ArticleGroup_Edit { get; set; }
        public PermissionViewModel ArticleGroup_Edit_ViewModel { get; set; }
        
        public byte ArticleGroup_Delete { get; set; }
        public PermissionViewModel ArticleGroup_Delete_ViewModel { get; set; }
        
        // торговая марка
        public byte Trademark_Create { get; set; }
        public PermissionViewModel Trademark_Create_ViewModel { get; set; }
        
        public byte Trademark_Edit { get; set; }
        public PermissionViewModel Trademark_Edit_ViewModel { get; set; }
        
        public byte Trademark_Delete { get; set; }
        public PermissionViewModel Trademark_Delete_ViewModel { get; set; }
        
        // изготовитель
        public byte Manufacturer_Create { get; set; }
        public PermissionViewModel Manufacturer_Create_ViewModel { get; set; }
        
        public byte Manufacturer_Edit { get; set; }
        public PermissionViewModel Manufacturer_Edit_ViewModel { get; set; }
        
        public byte Manufacturer_Delete { get; set; }
        public PermissionViewModel Manufacturer_Delete_ViewModel { get; set; }
        
        // страна
        public byte Country_Create { get; set; }
        public PermissionViewModel Country_Create_ViewModel { get; set; }
        
        public byte Country_Edit { get; set; }
        public PermissionViewModel Country_Edit_ViewModel { get; set; }
        
        public byte Country_Delete { get; set; }
        public PermissionViewModel Country_Delete_ViewModel { get; set; }
        
        // единица измерения
        public byte MeasureUnit_Create { get; set; }
        public PermissionViewModel MeasureUnit_Create_ViewModel { get; set; }
        
        public byte MeasureUnit_Edit { get; set; }
        public PermissionViewModel MeasureUnit_Edit_ViewModel { get; set; }
        
        public byte MeasureUnit_Delete { get; set; }
        public PermissionViewModel MeasureUnit_Delete_ViewModel { get; set; }

        // банки
        public byte Bank_Create { get; set; }
        public PermissionViewModel Bank_Create_ViewModel { get; set; }

        public byte Bank_Edit { get; set; }
        public PermissionViewModel Bank_Edit_ViewModel { get; set; }

        public byte Bank_Delete { get; set; }
        public PermissionViewModel Bank_Delete_ViewModel { get; set; }

        // валюты
        public byte Currency_Create { get; set; }
        public PermissionViewModel Currency_Create_ViewModel { get; set; }

        public byte Currency_Edit { get; set; }
        public PermissionViewModel Currency_Edit_ViewModel { get; set; }

        public byte Currency_AddRate { get; set; }
        public PermissionViewModel Currency_AddRate_ViewModel { get; set; }

        public byte Currency_Delete { get; set; }
        public PermissionViewModel Currency_Delete_ViewModel { get; set; }

        // сертификат товара
        public byte ArticleCertificate_Create { get; set; }
        public PermissionViewModel ArticleCertificate_Create_ViewModel { get; set; }

        public byte ArticleCertificate_Edit { get; set; }
        public PermissionViewModel ArticleCertificate_Edit_ViewModel { get; set; }

        public byte ArticleCertificate_Delete { get; set; }
        public PermissionViewModel ArticleCertificate_Delete_ViewModel { get; set; }

        // Ставки НДС
        public byte ValueAddedTax_Create { get; set; }
        public PermissionViewModel ValueAddedTax_Create_ViewModel { get; set; }

        public byte ValueAddedTax_Edit { get; set; }
        public PermissionViewModel ValueAddedTax_Edit_ViewModel { get; set; }

        public byte ValueAddedTax_Delete { get; set; }
        public PermissionViewModel ValueAddedTax_Delete_ViewModel { get; set; }

        // Организационно-правовые формы
        public byte LegalForm_Create { get; set; }
        public PermissionViewModel LegalForm_Create_ViewModel { get; set; }

        public byte LegalForm_Edit { get; set; }
        public PermissionViewModel LegalForm_Edit_ViewModel { get; set; }

        public byte LegalForm_Delete { get; set; }
        public PermissionViewModel LegalForm_Delete_ViewModel { get; set; }
    }
}
