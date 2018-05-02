using ERP.Utils;

namespace ERP.Wholesale.Domain.Entities.Security
{
    /// <summary>
    /// Право на выполнение операции или доступ к информации
    /// </summary>
    /// <remarks>РЕЗЕРВИРУЕМ ПО 100 ДЛЯ КАЖДОЙ СУЩНОСТИ</remarks>
    public enum Permission : short
    {
        #region Общие права доступа (1 - 1000)
        
        /// <summary>
        /// Просмотр ЗЦ товаров везде
        /// </summary>
        [EnumDisplayName("Просмотр ЗЦ товаров везде")]
        [EnumDescription("Разрешение на информацию о закупочных ценах и на вычисляемые на их основе показатели.")]
        PurchaseCost_View_ForEverywhere = 1,

        /// <summary>
        /// Просмотр ЗЦ товаров в приходе
        /// </summary>
        [EnumDisplayName("Просмотр ЗЦ товаров в приходе")]
        [EnumDescription("Разрешение на информацию о закупочных ценах и на вычисляемые на их основе показатели в приходных накладных (зависит от права «Просмотр списка и деталей приходной накладной»).")]
        PurchaseCost_View_ForReceipt = 2,

        /// <summary>
        /// Просмотр учетных цен на некомандных местах хранения
        /// Запретить – если МХ не является командным, то УЦ скрываются.
        /// Все – УЦ всегда отображаются (независимо от команды и видимости).
        /// Так же распространяется на печатные формы
        /// </summary>
        [EnumDisplayName("Просмотр УЦ на некомандных МХ")]
        [EnumDescription("Разрешение на просмотр учетных цен на местах хранения, не относящихся к командным.")]
        AccountingPrice_NotCommandStorage_View = 3,
      
        #endregion
        
        #region Права доступа к операциям товародвижения (1001 - 3000)

        #region Поставщики (1001 - 1100)

        /// <summary>
        /// Просмотр списка и деталей поставщиков
        /// </summary>
        [EnumDisplayName("Просмотр списка и деталей")]
        [EnumDescription("Разрешение на просмотр списка поставщиков и деталей любого из них, а также просмотр связанных справочников.")]
        Provider_List_Details = 1001,

        /// <summary>
        /// Добавление поставщика
        /// </summary>
        [EnumDisplayName("Добавление поставщика")]
        [EnumDescription("Разрешение на добавление нового поставщика в систему. Добавление организаций поставщика не подразумевается.")]
        Provider_Create = 1002,

        /// <summary>
        /// Изменение поставщика
        /// </summary>
        [EnumDisplayName("Изменение поставщика")]
        [EnumDescription("Разрешение на изменение данных о поставщике (форма «Редактирование поставщика»).")]
        Provider_Edit = 1003,
        
        /// <summary>
        /// Добавление договора
        /// </summary>
        [EnumDisplayName("Добавление договора")]
        [EnumDescription("Разрешение на добавление нового договора с поставщиком.")]
        Provider_ProviderContract_Create = 1004,

        /// <summary>
        /// Изменение договора
        /// </summary>
        [EnumDisplayName("Изменение договора")]
        [EnumDescription("Разрешение на изменение параметров существующего договора с поставщиком.")]
        Provider_ProviderContract_Edit = 1005,

        /// <summary>
        /// Удаление договора
        /// </summary>
        [EnumDisplayName("Удаление договора")]
        [EnumDescription("Разрешение на удаление договора с поставщиком.")]
        Provider_ProviderContract_Delete = 1006,

        /// <summary>
        /// Удаление поставщика
        /// </summary>
        [EnumDisplayName("Удаление поставщика")]
        [EnumDescription("Разрешение на выполнение операции «Удаление поставщика».")]
        Provider_Delete = 1007,

        #endregion

        #region Организации поставщиков (1101 - 1200)

        /// <summary>
        /// Просмотр списка и деталей организации
        /// </summary>
        [EnumDisplayName("Просмотр списка и деталей организации")]
        [EnumDescription("Разрешение на просмотр списка связанных с поставщиком организаций, общего списка организаций поставщиков и их деталей.")]
        ProviderOrganization_List_Details = 1101,

        /// <summary>
        /// Добавление организации к поставщику
        /// </summary>
        [EnumDisplayName("Добавление организации к поставщику")]
        [EnumDescription("Разрешение на добавление связи поставщика с существующей в системе организацией. Добавление организаций не подразумевается.")]
        Provider_ProviderOrganization_Add = 1102,

        /// <summary>
        /// Удаление организации из поставщика
        /// </summary>
        [EnumDisplayName("Удаление организации из поставщика")]
        [EnumDescription("Разрешение на удаление связи между поставщиком и организацией. Удаление самой организации не определяется.")]
        Provider_ProviderOrganization_Remove = 1103,

        /// <summary>
        /// Создание новой организации поставщика
        /// </summary>
        [EnumDisplayName("Создание новой организации поставщика")]
        [EnumDescription("Разрешение на создание новой организации и автоматического связывания ее с поставщиком.")]
        ProviderOrganization_Create = 1104,

        /// <summary>
        /// Изменение организации поставщика
        /// </summary>
        [EnumDisplayName("Редактирование организации поставщика")]
        [EnumDescription("Разрешение на изменение данных об организации из деталей организации.")]
        ProviderOrganization_Edit = 1105,

        /// <summary>
        /// Добавление расчетного счета
        /// </summary>
        [EnumDisplayName("Добавление расчетного счета")]
        [EnumDescription("Разрешение на добавление нового расчетного счета организации поставщика.")]
        ProviderOrganization_BankAccount_Create = 1106,

        /// <summary>
        /// Изменение расчетного счета
        /// </summary>
        [EnumDisplayName("Изменение расчетного счета")]
        [EnumDescription("Разрешение на редактирование введенных ранее расчетных счетов организации поставщика.")]
        ProviderOrganization_BankAccount_Edit = 1107,

        /// <summary>
        /// Удаление расчетного счета
        /// </summary>
        [EnumDisplayName("Удаление расчетного счета")]
        [EnumDescription("Разрешение на удаление введенных ранее расчетных счетов организации поставщика.")]
        ProviderOrganization_BankAccount_Delete = 1108,

        /// <summary>
        /// Удаление организации поставщика
        /// </summary>
        [EnumDisplayName("Удаление организации поставщика")]
        [EnumDescription("Разрешение на выполнение операции «Удаление организации» из деталей организации.")]
        ProviderOrganization_Delete = 1109,
        
        #endregion

        #region Типы поставщиков (1201 - 1300)

        /// <summary>
        /// Добавление типа поставщика
        /// </summary>
        [EnumDisplayName("Добавление типа поставщика")]
        [EnumDescription("Разрешение на добавление нового типа поставщиков в справочник.")]
        ProviderType_Create = 1201,

        /// <summary>
        /// Изменение типа поставщика
        /// </summary>
        [EnumDisplayName("Изменение типа поставщика")]
        [EnumDescription("Разрешение на редактирование существующих типов поставщиков в справочнике.")]
        ProviderType_Edit = 1202,

        /// <summary>
        /// Удаление типа поставщика
        /// </summary>
        [EnumDisplayName("Удаление типа поставщика")]
        [EnumDescription("Разрешение на удаление типа поставщика из справочника.")]
        ProviderType_Delete = 1203,

        #endregion
        
        #region Приходные накладные (1301 - 1400)

        /// <summary>
        /// Просмотр списка и деталей приходной накладной
        /// </summary>
        [EnumDisplayName("Просмотр списка и деталей")]
        [EnumDescription("Разрешение на просмотр списка приходных накладных и деталей накладной.")]
        ReceiptWaybill_List_Details = 1301,

        /// <summary>
        /// Создание и редактирование приходной накладной
        /// </summary>
        [EnumDisplayName("Создание и редактирование")]
        [EnumDescription("Разрешение на создание новой накладной и ее редактирование, а также добавление и изменение позиций в накладной.")]
        ReceiptWaybill_Create_Edit = 1302,

        /*/// <summary>
        /// Смена поставщика и места хранения приходной накладной
        /// </summary>
        [EnumDisplayName("Смена поставщика и места хранения")]
        [EnumDescription("Разрешение на выполнение операции «Сменить поставщика и место хранения» для приходной накладной.")]
        ReceiptWaybill_Provider_Storage_Change = 1303,*/

        /// <summary>
        /// Смена куратора приходной накладной
        /// </summary>
        [EnumDisplayName("Смена куратора")]
        [EnumDescription("Разрешение на выполнение операции «Сменить куратора накладной» для приходной накладной.")]
        ReceiptWaybill_Curator_Change = 1305,

        /// <summary>
        /// Удаление приходной накладной и позиции в ней
        /// </summary>
        [EnumDisplayName("Удаление накладной и позиции в ней")]
        [EnumDescription("Разрешение на удаление приходной накладной, а также удаление позиции товара из приходной накладной.")]
        ReceiptWaybill_Delete_Row_Delete = 1306,

        /// <summary>
        /// Редактирование информации о док-ах приходной накладной
        /// </summary>
        [EnumDisplayName("Редактирование информации о док-ах")]
        [EnumDescription("Разрешение на изменение информации о документах поставщика.")]
        ReceiptWaybill_ProviderDocuments_Edit = 1307,

        /// <summary>
        /// Приемка на склад приходной накладной
        /// </summary>
        [EnumDisplayName("Приемка на склад")]
        [EnumDescription("Разрешение на выполнение операции «Принять товары по накладной на склад».")]
        ReceiptWaybill_Receipt = 1308,

        /// <summary>
        /// Отмена приемки приходной накладной
        /// </summary>
        [EnumDisplayName("Отмена приемки")]
        [EnumDescription("Разрешение на выполнение операции «Отмена приемки накладной на склад».")]
        ReceiptWaybill_Receipt_Cancel = 1309,

        /// <summary>
        /// Проводка приходной накладной
        /// </summary>
        [EnumDisplayName("Проводка")]
        [EnumDescription("Разрешение на выполнение операции «Провести».")]
        ReceiptWaybill_Accept = 1310,

        /// <summary>
        /// Отмена проводки приходной накладной
        /// </summary>
        [EnumDisplayName("Отмена проводки")]
        [EnumDescription("Разрешение на выполнение операции «Отменить проводку».")]
        ReceiptWaybill_Acceptance_Cancel = 1311,

        /// <summary>
        /// Согласование приходной накладной
        /// </summary>
        [EnumDisplayName("Согласование")]
        [EnumDescription("Разрешение на выполнение операции «Согласование расхождений накладной» для урегулирования этих расхождений.")]
        ReceiptWaybill_Approve = 1312,

        /// <summary>
        /// Отмена согласования приходной накладной
        /// </summary>
        [EnumDisplayName("Отмена согласования")]
        [EnumDescription("Разрешение на выполнение операции «Отмена согласования расхождений».")]
        ReceiptWaybill_Approvement_Cancel = 1313,

        /// <summary>
        /// Создание приходной накладной по партии заказа
        /// </summary>
        [EnumDisplayName("Создание накладной по партии заказа")]
        [EnumDescription("Разрешение на создание новой накладной, связанной с партией заказа.")]
        ReceiptWaybill_CreateFromProductionOrderBatch = 1314,

        /// <summary>
        /// Изменение даты накладной
        /// </summary>
        [EnumDisplayName("Изменение даты накладной")]
        [EnumDescription("Разрешение на изменение даты накладной.")]
        ReceiptWaybill_Date_Change = 1315,

        /// <summary>
        /// Проводка задним числом
        /// </summary>
        [EnumDisplayName("Проводка задним числом")]
        [EnumDescription("Разрешение на проводку накладной задним числом.")]
        ReceiptWaybill_Accept_Retroactively = 1316,

        /// <summary>
        /// Приемка накладной задним числом
        /// </summary>
        [EnumDisplayName("Приемка задним числом")]
        [EnumDescription("Разрешение на приемку накладной задним числом.")]
        ReceiptWaybill_Receipt_Retroactively = 1317,

        /// <summary>
        /// Согласование накладной задним числом
        /// </summary>
        [EnumDisplayName("Согласование задним числом")]
        [EnumDescription("Разрешение на согласование накладной задним числом.")]
        ReceiptWaybill_Approve_Retroactively = 1318,
        #endregion

        #region Накладные перемещения (1401 - 1500)

        /// <summary>
        /// Просмотр списка и деталей накладной перемещения
        /// </summary>
        [EnumDisplayName("Просмотр списка и деталей")]
        [EnumDescription("Разрешение на просмотр списка накладных перемещения и деталей накладной.")]
        MovementWaybill_List_Details = 1401,

        /// <summary>
        /// Создание и редактирование накладной перемещения
        /// </summary>
        [EnumDisplayName("Создание и редактирование")]
        [EnumDescription("Разрешение на создание новой накладной и ее редактирование, а также добавление и изменение позиций в накладной.")]
        MovementWaybill_Create_Edit = 1402,

        /*/// <summary>
        /// Смена получателя накладной перемещения
        /// </summary>
        [EnumDisplayName("Смена получателя")]
        [EnumDescription("Разрешение на выполнение операции «Сменить получателя» для накладной перемещения.")]
        MovementWaybill_RecipientStorage_Change = 1403,*/

        /// <summary>
        /// Смена куратора накладной перемещения
        /// </summary>
        [EnumDisplayName("Смена куратора")]
        [EnumDescription("Разрешение на выполнение операции «Сменить куратора накладной» для накладной перемещения.")]
        MovementWaybill_Curator_Change = 1404,

        /// <summary>
        /// Удаление накладной перемещения и позиции в ней
        /// </summary>
        [EnumDisplayName("Удаление накладной и позиции в ней")]
        [EnumDescription("Разрешение на удаление накладной перемещения, а также удаление позиции товара из накладной перемещения.")]
        MovementWaybill_Delete_Row_Delete = 1405,

        /// <summary>
        /// Проводка накладной перемещения
        /// </summary>
        [EnumDisplayName("Проводка накладной")]
        [EnumDescription("Разрешение на выполнение операции «Провести накладную перемещения».")]
        MovementWaybill_Accept = 1406,
        
        /// <summary>
        /// Отмена проводки накладной перемещения
        /// </summary>
        [EnumDisplayName("Отмена проводки")]
        [EnumDescription("Разрешение на выполнение операции «Отменить проводку накладной перемещения».")]
        MovementWaybill_Acceptance_Cancel = 1407,

        /// <summary>
        /// Отгрузка накладной перемещения
        /// </summary>
        [EnumDisplayName("Отгрузка накладной")]
        [EnumDescription("Разрешение на выполнение операции «Отгрузить со склада».")]
        MovementWaybill_Ship = 1408,

        /// <summary>
        /// Отмена отгрузки накладной перемещения
        /// </summary>
        [EnumDisplayName("Отмена отгрузки")]
        [EnumDescription("Разрешение на выполнение операции «Отменить отгрузку».")]
        MovementWaybill_Shipping_Cancel = 1409,

        /// <summary>
        /// Приемка на склад накладной перемещения
        /// </summary>
        [EnumDisplayName("Приемка на склад")]
        [EnumDescription("Разрешение на выполнение операции «Принять товары по накладной на склад».")]
        MovementWaybill_Receipt = 1410,

        /// <summary>
        /// Отмена приемки накладной перемещения
        /// </summary>
        [EnumDisplayName("Отмена приемки")]
        [EnumDescription("Разрешение на выполнение операции «Отменить приемку».")]
        MovementWaybill_Receipt_Cancel = 1411,

        #endregion

        #region Списание товаров (1501 - 1600)

        /// <summary>
        /// Просмотр списка и деталей накладных списания
        /// </summary>
        [EnumDisplayName("Просмотр списка и деталей")]
        [EnumDescription("Разрешение на просмотр списка накладных списания, деталей накладной и справочника «Основания для списания».")]
        WriteoffWaybill_List_Details = 1501,

        /// <summary>
        /// Создание и редактирование
        /// </summary>
        [EnumDisplayName("Создание и редактирование")]
        [EnumDescription("Разрешение на редактирование накладных списания, а также добавление и изменение позиций накладной списания.")]
        WriteoffWaybill_Create_Edit = 1502,

        /// <summary>
        /// Смена куратора
        /// </summary>
        [EnumDisplayName("Смена куратора")]
        [EnumDescription("Разрешение на выполнение операции «Сменить куратора накладной» для накладной списания.")]
        WriteoffWaybill_Curator_Change = 1503,

        /// <summary>
        /// Удаление накладной и ее позиций
        /// </summary>
        [EnumDisplayName("Удаление накладной и ее позиций")]
        [EnumDescription("Разрешение на выполнение операции «Удаление накладной» и/или позиции накладной.")]
        WriteoffWaybill_Delete_Row_Delete = 1504,

        /// <summary>
        /// Списание накладной
        /// </summary>
        [EnumDisplayName("Списание накладной")]
        [EnumDescription("Разрешение на выполнение операции «Списать».")]
        WriteoffWaybill_Writeoff = 1505,

        /// <summary>
        /// Отмена списания
        /// </summary>
        [EnumDisplayName("Отмена списания")]
        [EnumDescription("Разрешение на выполнение операции «Отменить списание».")]
        WriteoffWaybill_Writeoff_Cancel = 1506,

        /// <summary>
        /// Проводка накладной
        /// </summary>
        [EnumDisplayName("Проводка накладной")]
        [EnumDescription("Разрешение на выполнение операции «Провести».")]
        WriteoffWaybill_Accept = 1507,

        /// <summary>
        /// Отмена проводки
        /// </summary>
        [EnumDisplayName("Отмена проводки")]
        [EnumDescription("Разрешение на выполнение операции «Отменить проводку».")]
        WriteoffWaybill_Acceptance_Cancel = 1508,


        #endregion

        #region Основания для списания (1601 - 1700)

        /// <summary>
        /// Добавление основания для списания
        /// </summary>
        [EnumDisplayName("Добавление основания для списания")]
        [EnumDescription("Разрешение на добавление основания для списания в справочник.")]
        WriteoffReason_Create = 1601,

        /// <summary>
        /// Редактирование основания для списания
        /// </summary>
        [EnumDisplayName("Редактирование основания для списания")]
        [EnumDescription("Разрешение на редактирование оснований для списания в справочнике.")]
        WriteoffReason_Edit = 1602,

        /// <summary>
        /// Удаление основания для списания
        /// </summary>
        [EnumDisplayName("Удаление основания для списания")]
        [EnumDescription("Разрешение на выполнение операции «Удаление основания для списания».")]
        WriteoffReason_Delete = 1603,

        #endregion

        #region Реестры цен (1701 - 1800)

        /// <summary>
        /// Просмотр списка и деталей реестров цен
        /// </summary>
        [EnumDisplayName("Просмотр списка и деталей")]
        [EnumDescription("Разрешение на просмотр списка РЦ и деталей любого из них.")]
        AccountingPriceList_List_Details = 1701,

        /// <summary>
        /// Создание
        /// </summary>
        [EnumDisplayName("Создание")]
        [EnumDescription("Разрешение на создание РЦ на любом основании (по приходу, по месту хранения или нового).")]
        AccountingPriceList_Create = 1702,

        /// <summary>
        /// Редактирование
        /// </summary>
        [EnumDisplayName("Редактирование")]
        [EnumDescription("Разрешение на редактирование шапки РЦ и изменение правил расчета УЦ по умолчанию.")]
        AccountingPriceList_Edit = 1703,

        /// <summary>
        /// Изменение позиций
        /// </summary>
        [EnumDisplayName("Изменение позиций")]
        [EnumDescription("Разрешение на добавление/изменение позиций в РЦ. Не включает изменение УЦ по-умолчанию.")]
        AccountingPriceList_ArticleAccountingPrice_Create_Edit = 1704,

        /// <summary>
        /// Изменение УЦ по умолчанию
        /// </summary>
        [EnumDisplayName("Изменение УЦ по умолчанию")]
        [EnumDescription("Разрешение на изменение рассчитанной по правилам новой УЦ для позиции РЦ.")]
        AccountingPriceList_DefaultAccountingPrice_Edit = 1705,

        /// <summary>
        /// Добавление МХ в распространение
        /// </summary>
        [EnumDisplayName("Добавление МХ в распространение")]
        [EnumDescription("Разрешение на добавление мест хранения в распространение РЦ.")]
        AccountingPriceList_Storage_Add = 1706,

        /// <summary>
        /// Удаление МХ из распространения
        /// </summary>
        [EnumDisplayName("Удаление МХ из распространения")]
        [EnumDescription("Разрешение на удаление мест хранения из распространение РЦ.")]
        AccountingPriceList_Storage_Remove = 1707,

        /// <summary>
        /// Удаление РЦ
        /// </summary>
        [EnumDisplayName("Удаление РЦ")]
        [EnumDescription("Разрешение на удаление РЦ.")]        
        AccountingPriceList_Delete = 1708,

        /// <summary>
        /// Проводка реестра цен
        /// </summary>
        [EnumDisplayName("Проводка реестра цен")]
        [EnumDescription("Разрешение на выполнение операции «Провести РЦ».")]
        AccountingPriceList_Accept = 1709,

        /// <summary>
        /// Отмена проводки
        /// </summary>
        [EnumDisplayName("Отмена проводки")]
        [EnumDescription("Разрешение на выполнение операции «Отменить проводку РЦ».")]
        AccountingPriceList_Acceptance_Cancel = 1710,
        
        #endregion

        #region Собственные организации (1801 - 1900)

        /// <summary>
        /// Создание новой собственной организации
        /// </summary>
        [EnumDisplayName("Создание новой организации")]
        [EnumDescription("Разрешение на создание новой организации.")]
        AccountOrganization_Create = 1801,

        /// <summary>
        /// Изменение организации
        /// </summary>
        [EnumDisplayName("Изменение организации")]
        [EnumDescription("Разрешение на изменение данных об организации из деталей организации.")]
        AccountOrganization_Edit = 1802,

        /// <summary>
        /// Добавление расчетного счета
        /// </summary>
        [EnumDisplayName("Добавление расчетного счета")]
        [EnumDescription("Разрешение на добавление нового расчетного счета собственной организации.")]
        AccountOrganization_BankAccount_Create = 1803,

        /// <summary>
        /// Изменение расчетного счета
        /// </summary>
        [EnumDisplayName("Изменение расчетного счета")]
        [EnumDescription("Разрешение на редактирование введенных ранее расчетных счетов собственной организации.")]
        AccountOrganization_BankAccount_Edit = 1804,

        /// <summary>
        /// Удаление расчетного счета
        /// </summary>
        [EnumDisplayName("Удаление расчетного счета")]
        [EnumDescription("Разрешение на удаление введенных ранее расчетных счетов собственной организации.")]
        AccountOrganization_BankAccount_Delete = 1805,

        /// <summary>
        /// Удаление организации
        /// </summary>
        [EnumDisplayName("Удаление организации")]
        [EnumDescription("Разрешение на выполнение операции «Удаление организации» из деталей организации.")]
        AccountOrganization_Delete = 1806,

        #endregion

        #region Места хранения (1901 - 2000)

        /// <summary>
        /// Просмотр списка и деталей мест хранения
        /// </summary>
        [EnumDisplayName("Просмотр списка и деталей")]
        [EnumDescription("Разрешение на просмотр списка мест хранения и деталей одного места хранения.")]
        Storage_List_Details = 1901,

        /// <summary>
        /// Добавление места хранения
        /// </summary>
        [EnumDisplayName("Добавление места хранения")]
        [EnumDescription("Разрешение на добавление нового места хранения.")]
        Storage_Create = 1902,

        /// <summary>
        /// Редактирование места хранения
        /// </summary>
        [EnumDisplayName("Редактирование места хранения")]
        [EnumDescription("Разрешение на редактирование места хранения.")]
        Storage_Edit = 1903,

        /// <summary>
        /// Добавление связанной организации
        /// </summary>
        [EnumDisplayName("Добавление связанной организации")]
        [EnumDescription("Разрешение на добавление связи между организацией и местом хранения.")]
        Storage_AccountOrganization_Add = 1904,

        /// <summary>
        /// Удаление связанной организации
        /// </summary>
        [EnumDisplayName("Удаление связанной организации")]
        [EnumDescription("Разрешение на удаление связи между организацией и местом хранения.")]
        Storage_AccountOrganization_Remove = 1905,

        /// <summary>
        /// Добавление и редактирование секции места хранения
        /// </summary>
        [EnumDisplayName("Добавление и редактирование секции")]
        [EnumDescription("Разрешение на добавление новой секции и/или редактирование существующей.")]
        Storage_Section_Create_Edit = 1906,

        /// <summary>
        /// Удаление секции места хранения
        /// </summary>
        [EnumDisplayName("Удаление секции")]
        [EnumDescription("Разрешение на удаление секции из места хранения.")]
        Storage_Section_Delete = 1907,

        /// <summary>
        /// Удаление места хранения
        /// </summary>
        [EnumDisplayName("Удаление места хранения")]
        [EnumDescription("Разрешение на удаление места хранения.")]
        Storage_Delete = 1908,

        #endregion

        #region Накладные смены собственника (2001 - 2100)

        /// <summary>
        /// Просмотр списка и деталей
        /// </summary>
        [EnumDisplayName("Просмотр списка и деталей")]
        [EnumDescription("Разрешение на просмотр списка накладных смены собственника и деталей накладной.")]
        ChangeOwnerWaybill_List_Details = 2001,

        /// <summary>
        /// Создание и редактирование
        /// </summary>
        [EnumDisplayName("Создание и редактирование")]
        [EnumDescription("Разрешение на создание новой накладной и ее редактирование, а также добавление и изменение позиций в накладной.")]
        ChangeOwnerWaybill_Create_Edit = 2002,

        /// <summary>
        /// Смена получателя
        /// </summary>
        [EnumDisplayName("Смена получателя")]
        [EnumDescription("Разрешение на выполнение операции «Сменить организацию получателя» для накладной смены собственника.")]
        ChangeOwnerWaybill_Recipient_Change = 2003,

        /// <summary>
        /// Смена куратора
        /// </summary>
        [EnumDisplayName("Смена куратора")]
        [EnumDescription("Разрешение на выполнение операции «Сменить куратора накладной» для накладной смены собственника.")]
        ChangeOwnerWaybill_Curator_Change = 2004,

        /// <summary>
        /// Удаление накладной и позиции в ней
        /// </summary>
        [EnumDisplayName("Удаление накладной и позиции в ней")]
        [EnumDescription("Разрешение на удаление накладной смены собственника, а также удаление позиции товара из накладной смены собственника.")]
        ChangeOwnerWaybill_Delete_Row_Delete = 2005,

        /// <summary>
        /// Проводка накладной
        /// </summary>
        [EnumDisplayName("Проводка накладной")]
        [EnumDescription("Разрешение на выполнение операции «Провести накладную смены собственника».")]
        ChangeOwnerWaybill_Accept = 2006,

        /// <summary>
        /// Отмена проводки
        /// </summary>
        [EnumDisplayName("Отмена проводки")]
        [EnumDescription("Разрешение на выполнение операции «Отменить проводку накладной смены собственника».")]
        ChangeOwnerWaybill_Acceptance_Cancel = 2007,

        #endregion

        #endregion

        #region Права доступа к операциям выполнения продаж (3001 - 5000)

        #region Клиенты (3001 - 3100)

        /// <summary>
        /// Просмотр списка и деталей клиента
        /// </summary>
        [EnumDisplayName("Просмотр списка и деталей")]
        [EnumDescription("Разрешение на просмотр списка клиентов и деталей любого из них, а также просмотр связанных справочников.")]
        Client_List_Details = 3001,

        /// <summary>
        /// Добавление клиента
        /// </summary>
        [EnumDisplayName("Добавление клиента")]
        [EnumDescription("Разрешение на добавление нового клиента в систему. Добавление организаций не подразумевается.")]
        Client_Create = 3002,

        /// <summary>
        /// Изменение клиента
        /// </summary>
        [EnumDisplayName("Изменение клиента")]
        [EnumDescription("Разрешение на изменение данных о клиенте (страница «Редактирование клиента»).")]
        Client_Edit = 3003,

        /// <summary>
        /// Удаление клиента
        /// </summary>
        [EnumDisplayName("Удаление клиента")]
        [EnumDescription("Разрешение на выполнение операции «Удаление клиента».")]
        Client_Delete = 3006,

        /// <summary>
        /// Блокирование клиента
        /// </summary>
        [EnumDisplayName("Блокирование клиента")]
        [EnumDescription("Разрешение заблокировать клиента (запретить оформление продаж клиенту).")]
        Client_Block = 3007,

        #endregion

        #region Организации клиентов (3101 - 3200)

        /// <summary>
        /// Просмотр списка и деталей организации клиента
        /// </summary>
        [EnumDisplayName("Просмотр списка и деталей организации")]
        [EnumDescription("Разрешение на просмотр списка связанных с клиентом организаций, общего списка организаций клиентов и их деталей.")]
        ClientOrganization_List_Details = 3101,

        /// <summary>
        /// Добавление организации к клиенту
        /// </summary>
        [EnumDisplayName("Добавление организации к клиенту")]
        [EnumDescription("Разрешение на добавление связи клиента с существующей в системе организацией. Добавление организаций не подразумевается.")]
        Client_ClientOrganization_Add = 3102,

        /// <summary>
        /// Удаление организации из клиента
        /// </summary>
        [EnumDisplayName("Удаление организации из клиента")]
        [EnumDescription("Разрешение на удаление связи между клиентом и организацией. Удаление самой организации не определяется.")]
        Client_ClientOrganization_Remove = 3103,

        /// <summary>
        /// Создание новой организации клиента
        /// </summary>
        [EnumDisplayName("Создание новой организации клиента")]
        [EnumDescription("Разрешение на создание новой организации и автоматическое связывание ее с клиентом.")]
        ClientOrganization_Create = 3104,

        /// <summary>
        /// Редактирование организации клиента
        /// </summary>
        [EnumDisplayName("Редактирование организации клиента")]
        [EnumDescription("Разрешение на изменение данных об организации из деталей организации.")]
        ClientOrganization_Edit = 3105,

        /// <summary>
        /// Добавление расчетного счета
        /// </summary>
        [EnumDisplayName("Добавление расчетного счета")]
        [EnumDescription("Разрешение на добавление нового расчетного счета организации клиента.")]
        ClientOrganization_BankAccount_Create = 3106,

        /// <summary>
        /// Изменение расчетного счета
        /// </summary>
        [EnumDisplayName("Изменение расчетного счета")]
        [EnumDescription("Разрешение на редактирование введенных ранее расчетных счетов организации клиента.")]
        ClientOrganization_BankAccount_Edit = 3107,

        /// <summary>
        /// Удаление расчетного счета
        /// </summary>
        [EnumDisplayName("Удаление расчетного счета")]
        [EnumDescription("Разрешение на удаление введенных ранее расчетных счетов организации клиента.")]
        ClientOrganization_BankAccount_Delete = 3108,

        /// <summary>
        /// Удаление организации клиента
        /// </summary>
        [EnumDisplayName("Удаление организации клиента")]
        [EnumDescription("Разрешение на выполнение операции «Удаление организации» из деталей организации.")]
        ClientOrganization_Delete = 3109,

        #endregion

        #region Типы клиентов (3201 - 3300)

        /// <summary>
        /// Добавление типа клиентов
        /// </summary>
        [EnumDisplayName("Добавление типа клиентов")]
        [EnumDescription("Разрешение на добавление нового типа клиентов в справочник типов клиентов.")]
        ClientType_Create = 3201,

        /// <summary>
        /// Редактирование типа клиентов
        /// </summary>
        [EnumDisplayName("Редактирование типа клиентов")]
        [EnumDescription("Разрешение на редактирование существующих типов клиентов в справочнике типов клиентов.")]
        ClientType_Edit = 3202,

        /// <summary>
        /// Удаление типа клиентов
        /// </summary>
        [EnumDisplayName("Удаление типа клиентов")]
        [EnumDescription("Разрешение на удаление типов клиентов из справочника типов клиентов.")]
        ClientType_Delete = 3203,

        #endregion

        #region Программы обслуживания клиентов (3301 - 3400)

        /// <summary>
        /// Добавление программы обслуживания
        /// </summary>
        [EnumDisplayName("Добавление программы обслуживания")]
        [EnumDescription("Разрешение на добавление новой программы обслуживания клиентов в соответствующий справочник.")]
        ClientServiceProgram_Create = 3301,

        /// <summary>
        /// Редактирование программы обслуживания
        /// </summary>
        [EnumDisplayName("Редактирование программы обслуживания")]
        [EnumDescription("Разрешение на редактирование существующих программ обслуживания в соответствующем справочнике.")]
        ClientServiceProgram_Edit = 3302,

        /// <summary>
        /// Удаление программы обслуживания
        /// </summary>
        [EnumDisplayName("Удаление программы обслуживания")]
        [EnumDescription("Разрешение на удаление программ обслуживания из соответствующего справочника.")]
        ClientServiceProgram_Delete = 3303,

        #endregion

        #region Регионы клиентов (3401 - 3500)

        /// <summary>
        /// Добавление региона клиента
        /// </summary>
        [EnumDisplayName("Добавление региона клиента")]
        [EnumDescription("Разрешение на добавление нового региона клиентов в соответствующий справочник.")]
        ClientRegion_Create = 3401,

        /// <summary>
        /// Изменение региона клиента
        /// </summary>
        [EnumDisplayName("Изменение региона клиента")]
        [EnumDescription("Разрешение на редактирование существующих регионов клиентов в соответствующем справочнике.")]
        ClientRegion_Edit = 3402,

        /// <summary>
        /// Удаление региона клиента
        /// </summary>
        [EnumDisplayName("Удаление региона клиента")]
        [EnumDescription("Разрешение на удаление регионов клиентов из соответствующего справочника.")]
        ClientRegion_Delete = 3403,

        #endregion

        #region Сделки (3501 - 3600)

        /// <summary>
        /// Просмотр списка и деталей сделок
        /// </summary>
        [EnumDisplayName("Просмотр списка и деталей")]
        [EnumDescription("Разрешение на просмотр списка сделок и деталей любой из них, а также общих показателей.")]
        Deal_List_Details = 3501,

        /// <summary>
        /// Создание и редактирование сделки
        /// </summary>
        [EnumDisplayName("Создание и редактирование")]
        [EnumDescription("Разрешение на создание новой сделки и редактирование информации о сделке.")]
        Deal_Create_Edit = 3502,

        /// <summary>
        /// Изменение этапа сделки
        /// </summary>
        [EnumDisplayName("Изменение этапа сделки")]
        [EnumDescription("Разрешение на выполнение операции «Изменить этап сделки».")]
        Deal_Stage_Change = 3505,

        /// <summary>
        /// Установка договора по сделке
        /// </summary>
        [EnumDisplayName("Установка договора по сделке")]
        [EnumDescription("Разрешение на выполнение операции «Установить договор по сделке».")]
        Deal_Contract_Set = 3506,

        /// <summary>
        /// Смена куратора сделки
        /// </summary>
        [EnumDisplayName("Смена куратора сделки")]
        [EnumDescription("Разрешение на выполнение операции «Изменить куратора сделки».")]
        Deal_Curator_Change = 3508,

        /// <summary>
        /// Просмотр квот сделки
        /// </summary>
        [EnumDisplayName("Просмотр квот сделки")]
        [EnumDescription("Разрешение на просмотр квот сделки.")]
        Deal_Quota_List = 3509,

        /// <summary>
        /// Добавление квоты в сделку
        /// </summary>
        [EnumDisplayName("Добавление квоты в сделку")]
        [EnumDescription("Разрешение на выполнение операции «Добавить квоту в сделку».")]
        Deal_Quota_Add = 3510,

        /// <summary>
        /// Удаление квоты из сделки
        /// </summary>
        [EnumDisplayName("Удаление квоты из сделки")]
        [EnumDescription("Разрешение на выполнение операции «Удалить квоту из сделки».")]
        Deal_Quota_Remove = 3511,

        /// <summary>
        /// Просмотр суммы общего сальдо
        /// </summary>
        [EnumDisplayName("Просмотр суммы общего сальдо")]
        [EnumDescription("Разрешение на просмотр общего сальдо по сделке.")]
        Deal_Balance_View = 3512,

        #endregion

        #region Накладные реализации товаров (3601 - 3700)

        /// <summary>
        /// Просмотр списка и деталей накладных реализации товаров
        /// </summary>
        [EnumDisplayName("Просмотр списка и деталей")]
        [EnumDescription("Разрешение на просмотр списка накладных реализации товаров и деталей накладной.")]
        ExpenditureWaybill_List_Details = 3601,

        /// <summary>
        /// Создание и редактирование
        /// </summary>
        [EnumDisplayName("Создание и редактирование")]
        [EnumDescription("Разрешение на создание новой накладной и ее редактирование, а также добавление и изменение позиций в накладной.")]
        ExpenditureWaybill_Create_Edit = 3602,

        /// <summary>
        /// Смена куратора
        /// </summary>
        [EnumDisplayName("Смена куратора")]
        [EnumDescription("Разрешение на выполнение операции «Сменить куратора накладной» для реализации товаров.")]
        ExpenditureWaybill_Curator_Change = 3603,

        /// <summary>
        /// Удаление накладной и позиции в ней
        /// </summary>
        [EnumDisplayName("Удаление накладной и позиции в ней")]
        [EnumDescription("Разрешение на удаление накладной реализации товаров, а также удаление позиции товара из накладной.")]
        ExpenditureWaybill_Delete_Row_Delete = 3604,

        /// <summary>
        /// Сделки для проводки накладной
        /// </summary>
        [EnumDisplayName("Сделки для проводки накладной")]
        [EnumDescription("Список сделок, по которым можно провести накладную реализации товаров.")]
        ExpenditureWaybill_Accept_Deal_List = 3605,

        /// <summary>
        /// Сделки для отмены проводки накладной
        /// </summary>
        [EnumDisplayName("Сделки для отмены проводки накладной")]
        [EnumDescription("Список сделок, по которым можно отменить проводку накладной реализации товаров.")]
        ExpenditureWaybill_Cancel_Acceptance_Deal_List = 3606,

        /// <summary>
        /// Сделки для отгрузки накладной со склада
        /// </summary>
        [EnumDisplayName("Сделки для отгрузки накладной со склада")]
        [EnumDescription("Список сделок, по которым можно отгрузить накладную реализации товаров со склада.")]
        ExpenditureWaybill_Ship_Deal_List = 3607,

        /// <summary>
        /// Сделки для отмены отгрузки накладной со склада
        /// </summary>
        [EnumDisplayName("Сделки для отмены отгрузки")]
        [EnumDescription("Список сделок, по которым можно отменить отгрузку накладной реализации товаров со склада.")]
        ExpenditureWaybill_Cancel_Shipping_Deal_List = 3608,

        /// <summary>
        /// Места хранения для проводки накладной
        /// </summary>
        [EnumDisplayName("Места хранения для проводки накладной")]
        [EnumDescription("Список мест хранения, по которым можно провести накладную реализации товаров.")]
        ExpenditureWaybill_Accept_Storage_List = 3609,

        /// <summary>
        /// Места хранения для отмены проводки накладной
        /// </summary>
        [EnumDisplayName("Места хранения для отмены проводки")]
        [EnumDescription("Список мест хранения, по которым можно отменить проводку накладной реализации товаров.")]
        ExpenditureWaybill_Cancel_Acceptance_Storage_List = 3610,

        /// <summary>
        /// Места хранения для отгрузки накладной со склада
        /// </summary>
        [EnumDisplayName("Места хранения для отгрузки накладной")]
        [EnumDescription("Список мест хранения, по которым можно отгрузить накладную реализации товаров со склада.")]
        ExpenditureWaybill_Ship_Storage_List = 3611,

        /// <summary>
        /// Места хранения для отмены отгрузки накладной со склада
        /// </summary>
        [EnumDisplayName("Места хранения для отмены отгрузки")]
        [EnumDescription("Список мест хранения, по которым можно отменить отгрузку накладной реализации товаров со склада.")]
        ExpenditureWaybill_Cancel_Shipping_Storage_List = 3612,

        /// <summary>
        /// Изменение даты накладной
        /// </summary>
        [EnumDisplayName("Изменение даты накладной")]
        [EnumDescription("Разрешение на изменение даты накладной.")]
        ExpenditureWaybill_Date_Change = 3613,

        /// <summary>
        /// Проводка задним числом
        /// </summary>
        [EnumDisplayName("Проводка задним числом")]
        [EnumDescription("Разрешение на проводку накладной задним числом.")]
        ExpenditureWaybill_Accept_Retroactively = 3614,

        /// <summary>
        /// Отгрузка задним числом
        /// </summary>
        [EnumDisplayName("Отгрузка задним числом")]
        [EnumDescription("Разрешение на отгрузку накладной задним числом.")]
        ExpenditureWaybill_Ship_Retroactively = 3615,

        #endregion

        #region Оплаты (3701 - 3800)

        /// <summary>
        /// Просмотр списка и деталей оплат
        /// </summary>
        [EnumDisplayName("Просмотр списка и деталей")]
        [EnumDescription("Разрешение на просмотр списка оплат, деталей оплаты и общей суммы оплат (в клиенте или сделке).")]
        DealPayment_List_Details = 3701,

        /// <summary>
        /// Создание и редактирование оплаты от клиента
        /// </summary>
        [EnumDisplayName("Создание и редактирование оплаты от клиента")]
        [EnumDescription("Разрешение на создание новой оплаты от клиента и переразнесение существующей. Разнесение оплаты можно выполнить на «видимые» накладные.")]
        DealPaymentFromClient_Create_Edit = 3702,

        /// <summary>
        /// Создание оплаты «возврат денег клиенту»
        /// </summary>
        [EnumDisplayName("Создание оплаты «возврат денег клиенту»")]
        [EnumDescription("Разрешение на создание нового возврата оплаты клиенту. Разносится автоматически.")]
        DealPaymentToClient_Create = 3703,
        
        /// <summary>
        /// Удаление оплаты от клиента
        /// </summary>
        [EnumDisplayName("Удаление оплаты от клиента")]
        [EnumDescription("Разрешение на удаление оплаты от клиента.")]
        DealPaymentFromClient_Delete = 3704,

        /// <summary>
        /// Удаление возврата оплаты клиенту
        /// </summary>
        [EnumDisplayName("Удаление возврата оплаты клиенту")]
        [EnumDescription("Разрешение на удаление возврата оплаты клиенту.")]
        DealPaymentToClient_Delete = 3705,

        /// <summary>
        /// Смена пользователя в оплате
        /// </summary>
        [EnumDisplayName("Смена пользователя в оплате")]
        [EnumDescription("Разрешение на смену пользователя в оплатах от клиента и в возвратах оплаты клиенту.")]
        DealPayment_User_Change = 3706,

        /// <summary>
        /// Изменение даты оплаты
        /// </summary>
        [EnumDisplayName("Изменение даты")]
        [EnumDescription("Разрешение на изменение даты оплаты.")]
        DealPayment_Date_Change = 3707,

        #endregion

        #region Квоты (3801 - 3900)

        /// <summary>
        /// Просмотр списка и деталей квот
        /// </summary>
        [EnumDisplayName("Просмотр списка и деталей")]
        [EnumDescription("Разрешение на просмотр списка и деталей общих квот.")]
        DealQuota_List_Details = 3801,

        /// <summary>
        /// Добавление квоты
        /// </summary>
        [EnumDisplayName("Создание квоты")]
        [EnumDescription("Разрешение на создание новой квоты.")]
        DealQuota_Create = 3802,

        /// <summary>
        /// Редактирование квоты
        /// </summary>
        [EnumDisplayName("Редактирование квоты")]
        [EnumDescription("Разрешение на редактирование квоты (когда это разрешено бизнес-логикой).")]
        DealQuota_Edit = 3803,

        /// <summary>
        /// Удаление квоты
        /// </summary>
        [EnumDisplayName("Удаление квоты")]
        [EnumDescription("Разрешение на удаление квоты (когда это разрешено бизнес-логикой).")]
        DealQuota_Delete = 3804,

        #endregion

        #region Возвраты от клиентов (3901 - 4000)

        /// <summary>
        /// Просмотр списка и деталей
        /// </summary>
        [EnumDisplayName("Просмотр списка и деталей")]
        [EnumDescription("Разрешение на просмотр списка накладных возврата от клиентов и деталей накладной. Право определяется позициями возврата.")]
        ReturnFromClientWaybill_List_Details = 3901,

        /// <summary>
        /// Создание и редактирование
        /// </summary>
        [EnumDisplayName("Создание и редактирование")]
        [EnumDescription("Разрешение на создание новой накладной возврата и ее редактирование, а также добавление и изменение позиций в накладной.")]
        ReturnFromClientWaybill_Create_Edit = 3902,

        /// <summary>
        /// Смена куратора
        /// </summary>
        [EnumDisplayName("Смена куратора")]
        [EnumDescription("Разрешение на выполнение операции «Сменить куратора накладной» для накладной возврата.")]
        ReturnFromClientWaybill_Curator_Change = 3903,

        /// <summary>
        /// Удаление накладной и ее позиции
        /// </summary>
        [EnumDisplayName("Удаление накладной и ее позиции")]
        [EnumDescription("Разрешение на удаление накладной возврата от клиента, а также удаление позиции товара из накладной.")]
        ReturnFromClientWaybill_Delete_Row_Delete = 3904,

        /// <summary>
        /// Сделки для проводки накладной
        /// </summary>
        [EnumDisplayName("Сделки для проводки накладной")]
        [EnumDescription("Список сделок, по которым можно провести накладную возврата от клиента.")]
        ReturnFromClientWaybill_Accept_Deal_List = 3905,

        /// <summary>
        /// Сделки для отмены проводки накладной
        /// </summary>
        [EnumDisplayName("Сделки для отмены проводки накладной")]
        [EnumDescription("Список сделок, по которым можно отменить проводку накладной возврата от клиента.")]
        ReturnFromClientWaybill_Acceptance_Cancel_Deal_List = 3906,

        /// <summary>
        /// Сделки для приемки товаров по накладной на склад
        /// </summary>
        [EnumDisplayName("Сделки для приемки на склад")]
        [EnumDescription("Список сделок, по которым можно принять на склад накладную возврата от клиента.")]
        ReturnFromClientWaybill_Receipt_Deal_List = 3907,

        /// <summary>
        /// Сделки для отмены приемки товаров по накладной на склад
        /// </summary>
        [EnumDisplayName("Сделки для отмены приемки на склад")]
        [EnumDescription("Список сделок, по которым можно отменить приемку на склад накладной возврата от клиента.")]
        ReturnFromClientWaybill_Receipt_Cancel_Deal_List = 3908,

        /// <summary>
        /// Места хранения для проводки накладной
        /// </summary>
        [EnumDisplayName("Места хранения для проводки накладной")]
        [EnumDescription("Список мест хранения, по которым можно провести накладную возврата от клиента.")]
        ReturnFromClientWaybill_Accept_Storage_List = 3909,

        /// <summary>
        /// Места хранения для отмены проводки накладной
        /// </summary>
        [EnumDisplayName("Места хранения для отмены проводки")]
        [EnumDescription("Список мест хранения, по которым можно отменить проводку накладной возврата от клиента.")]
        ReturnFromClientWaybill_Acceptance_Cancel_Storage_List = 3910,

        /// <summary>
        /// Места хранения для приемки товаров по накладной на склад
        /// </summary>
        [EnumDisplayName("Места хранения для приемки на склад")]
        [EnumDescription("Список мест хранения, по которым можно принять на склад накладную возврата от клиента.")]
        ReturnFromClientWaybill_Receipt_Storage_List = 3911,

        /// <summary>
        /// Места хранения для отмены приемки товаров по накладной на склад
        /// </summary>
        [EnumDisplayName("Места хранения для отмены приемки")]
        [EnumDescription("Список мест хранения, по которым можно отменить приемку на склад накладной возврата от клиента.")]
        ReturnFromClientWaybill_Receipt_Cancel_Storage_List = 3912,

        #endregion

        #region Основание для возврата (4001 - 4100)

        /// <summary>
        /// Добавление основания для возврата
        /// </summary>
        [EnumDisplayName("Добавление основания для возврата")]
        [EnumDescription("Разрешение на добавление основания для возврата в справочник.")]
        ReturnFromClientReason_Create = 4001,

        /// <summary>
        /// Редактирование основания для возврата
        /// </summary>
        [EnumDisplayName("Редактирование основания для возврата")]
        [EnumDescription("Разрешение на редактирование оснований для возврата в справочнике.")]
        ReturnFromClientReason_Edit = 4002,

        /// <summary>
        /// Удаление основания для возврата
        /// </summary>
        [EnumDisplayName("Удаление основания для возврата")]
        [EnumDescription("Разрешение на выполнение операции «Удаление основания для возврата».")]
        ReturnFromClientReason_Delete = 4003,

        #endregion

        #region Корректировки сальдо (4101 - 4200)

        /// <summary>
        /// Просмотр списка и деталей корректировок сальдо
        /// </summary>
        [EnumDisplayName("Просмотр списка и деталей")]
        [EnumDescription("Разрешение на просмотр списка корректировок сальдо и деталей корректировки (в клиенте или сделке).")]
        DealInitialBalanceCorrection_List_Details = 4101,

        /// <summary>
        /// Создание и редактирование кредитовой корректировки сальдо по сделке
        /// </summary>
        [EnumDisplayName("Создание и редактирование кредитовой корректировки")]
        [EnumDescription("Разрешение на создание новой кредитовой корректировки сальдо и переразнесение существующей. Разнесение корректировки можно выполнить на «видимые» накладные.")]
        DealCreditInitialBalanceCorrection_Create_Edit = 4102,

        /// <summary>
        /// Создание дебетовой корректировки сальдо по сделке
        /// </summary>
        [EnumDisplayName("Создание дебетовой корректировки")]
        [EnumDescription("Разрешение на создание новой дебетовой корректировки сальдо. Разносится автоматически.")]
        DealDebitInitialBalanceCorrection_Create = 4103,

        /// <summary>
        /// Удаление кредитовой корректировки сальдо по сделке
        /// </summary>
        [EnumDisplayName("Удаление кредитовой корректировки")]
        [EnumDescription("Разрешение на удаление кредитовой корректировки сальдо.")]
        DealCreditInitialBalanceCorrection_Delete = 4104,

        /// <summary>
        /// Удаление дебетовой корректировки сальдо по сделке
        /// </summary>
        [EnumDisplayName("Удаление дебетовой корректировки")]
        [EnumDescription("Разрешение на удаление дебетовой корректировки сальдо.")]
        DealDebitInitialBalanceCorrection_Delete = 4105,

        /// <summary>
        /// Изменение даты корректировки сальдо
        /// </summary>
        [EnumDisplayName("Изменение даты")]
        [EnumDescription("Разрешение на изменение даты корректировки сальдо.")]
        DealInitialBalanceCorrection_Date_Change = 4106,

        #endregion

        #region Договоры по сделке (4201 - 4300)

        /// <summary>
        /// Создание договора по сделке
        /// </summary>
        [EnumDisplayName("Создание договора")]
        [EnumDescription("Разрешение на выполнение операции «Создание договора по сделке».")]
        ClientContract_Create = 4201,

        /// <summary>
        /// Редактирование договора по сделке
        /// </summary>
        [EnumDisplayName("Редактирование договора")]
        [EnumDescription("Разрешение на выполнение операции «Редактирование договора по сделке».")]
        ClientContract_Edit = 4202,

        #endregion

        #endregion

        #region Права доступа к операциям работы с пользователями (5001 - 7000)

        #region Пользователи (5001 - 5100)

        /// <summary>
        /// Просмотр списка и деталей пользователей
        /// </summary>
        [EnumDisplayName("Просмотр списка и деталей")]
        [EnumDescription("Разрешение на просмотр списка пользователей и деталей любого из них.")]
        User_List_Details = 5001,

        /// <summary>
        /// Добавление пользователя
        /// </summary>
        [EnumDisplayName("Добавление пользователя")]
        [EnumDescription("Разрешение на создание нового пользователя (ввод данных о пользователе).")]
        User_Create = 5002,

        /// <summary>
        /// Редактирование пользователя
        /// </summary>
        [EnumDisplayName("Редактирование пользователя")]
        [EnumDescription("Разрешение на редактирование информации о пользователе (форма редактирования).")]
        User_Edit = 5003,

        /// <summary>
        /// Добавление роли пользователю
        /// </summary>
        [EnumDisplayName("Добавление роли пользователю")]
        [EnumDescription("Разрешение назначить роль пользователю. Просмотр ролей правом не определяется.")]
        User_Role_Add = 5004,

        /// <summary>
        /// Исключение роли у пользователя
        /// </summary>
        [EnumDisplayName("Исключение роли у пользователя")]
        [EnumDescription("Разрешение на выполнение операции «Исключить роль».")]
        User_Role_Remove = 5005,

        /// <summary>
        /// Удаление пользователя
        /// </summary>
        [EnumDisplayName("Удаление пользователя")]
        [EnumDescription("Разрешение на выполнение операции «Удаление пользователя».")]
        User_Delete = 5006,

        #endregion

        #region Команды (5101 - 5200)

        /// <summary>
        /// Просмотр списка и деталей команд
        /// </summary>
        [EnumDisplayName("Просмотр списка и деталей")]
        [EnumDescription("Разрешение на просмотр списка команд и деталей любой из просматриваемых в списке.")]
        Team_List_Details = 5101,

        /// <summary>
        /// Добавление команды
        /// </summary>
        [EnumDisplayName("Добавление команды")]
        [EnumDescription("Разрешение на добавление новой команды.")]
        Team_Create = 5102,

        /// <summary>
        /// Редактирование команды
        /// </summary>
        [EnumDisplayName("Редактирование команды")]
        [EnumDescription("Разрешение на редактирование информации о команде.")]
        Team_Edit = 5103,

        /// <summary>
        /// Добавление пользователя в команду
        /// </summary>
        [EnumDisplayName("Добавление пользователя в команду")]
        [EnumDescription("Разрешение на добавление пользователя в команду. Видимые пользователи правом не определяются.")]
        Team_User_Add = 5104,

        /// <summary>
        /// Исключение пользователя из команды
        /// </summary>
        [EnumDisplayName("Исключение пользователя из команды")]
        [EnumDescription("Разрешение на исключение пользователя из команды. Список видимых членов команды данным правом не определяется.")]
        Team_User_Remove = 5105,

        /// <summary>
        /// Удаление команды
        /// </summary>
        [EnumDisplayName("Удаление команды")]
        [EnumDescription("Разрешение на удаление команды.")]
        Team_Delete = 5106,

        /// <summary>
        /// Добавление места хранения в команду
        /// </summary>
        [EnumDisplayName("Добавление места хранения в команду")]
        [EnumDescription("Разрешение на добавление места хранения в команду. Видимые места хранения правом не определяются.")]
        Team_Storage_Add = 5107,

        /// <summary>
        /// Исключение места хранения из команды
        /// </summary>
        [EnumDisplayName("Исключение места хранения из команды")]
        [EnumDescription("Разрешение на исключение места хранения из команды. Список видимых мест хранения данным правом не определяется.")]
        Team_Storage_Remove = 5108,

        /// <summary>
        /// Добавление заказа на производство в команду
        /// </summary>
        [EnumDisplayName("Добавление заказа на производство в команду")]
        [EnumDescription("Разрешение на добавление заказа на производство в команду. Видимые заказы на производство правом не определяются.")]
        Team_ProductionOrder_Add = 5109,

        /// <summary>
        /// Исключение заказа на производство из команды
        /// </summary>
        [EnumDisplayName("Исключение заказа на производство из команды")]
        [EnumDescription("Разрешение на исключение заказа на производство из команды. Список видимых заказов на производство данным правом не определяется.")]
        Team_ProductionOrder_Remove = 5110,

        /// <summary>
        /// Добавление сделки в команду
        /// </summary>
        [EnumDisplayName("Добавление сделки в команду")]
        [EnumDescription("Разрешение на добавление сделки в команду. Видимые сделки правом не определяются.")]
        Team_Deal_Add = 5111,

        /// <summary>
        /// Исключение сделки из команды
        /// </summary>
        [EnumDisplayName("Исключение сделки из команды")]
        [EnumDescription("Разрешение на исключение сделки из команды. Список видимых сделок данным правом не определяется.")]
        Team_Deal_Remove = 5112,

        #endregion

        #region Роли (5201 - 5300)

        /// <summary>
        /// Просмотр списка и деталей
        /// </summary>
        [EnumDisplayName("Просмотр списка и деталей")]
        [EnumDescription("Разрешение на просмотр списка ролей и просмотр деталей любой из них.")]
        Role_List_Details = 5201,

        /// <summary>
        /// Добавление роли
        /// </summary>
        [EnumDisplayName("Добавление роли")]
        [EnumDescription("Разрешение на добавление новой роли.")]
        Role_Create = 5202,

        /// <summary>
        /// Редактирование роли
        /// </summary>
        [EnumDisplayName("Редактирование роли")]
        [EnumDescription("Разрешение на редактирование роли (настройку прав доступа).")]
        Role_Edit = 5203,

        /// <summary>
        /// Удаление роли
        /// </summary>
        [EnumDisplayName("Удаление роли")]
        [EnumDescription("Разрешение на удаление роли.")]
        Role_Delete = 5204,

        #endregion

        #region Должности пользователей (5301 - 5400)

        /// <summary>
        /// Добавление должности пользователя
        /// </summary>
        [EnumDisplayName("Добавление должности пользователя")]
        [EnumDescription("Разрешение на добавление должности пользователя.")]
        EmployeePost_Create = 5301,

        /// <summary>
        /// Редактирование должности пользователя
        /// </summary>
        [EnumDisplayName("Редактирование должности пользователя")]
        [EnumDescription("Разрешение на редактирование должности пользователя.")]
        EmployeePost_Edit = 5302,

        /// <summary>
        /// Удаление должности пользователя
        /// </summary>
        [EnumDisplayName("Удаление должности пользователя")]
        [EnumDescription("Разрешение на удаление должности пользователя.")]
        EmployeePost_Delete = 5303,

        #endregion

        #endregion

        #region Права доступа к операциям работы со справочниками (7001 - 20000)

        #region Товары (7201 - 7300)

        /// <summary>
        /// Просмотр списка и деталей товаров
        /// </summary>
        [EnumDisplayName("Просмотр списка и деталей")]
        [EnumDescription("Разрешение на просмотр номенклатур товаров.")]
        Article_List_Details = 7201,

        /// <summary>
        /// Добавление товара
        /// </summary>
        [EnumDisplayName("Добавление")]
        [EnumDescription("Разрешение на добавление новой номенклатуры товара.")]
        Article_Create = 7202,

        /// <summary>
        /// Изменение товара
        /// </summary>
        [EnumDisplayName("Изменение")]
        [EnumDescription("Разрешение на изменение информации в номенклатуре товара (включая перенос между группами).")]
        Article_Edit = 7203,

        /// <summary>
        /// Удаление товара
        /// </summary>
        [EnumDisplayName("Удаление")]
        [EnumDescription("Разрешение на удаление товара из справочника (если удаление допустимо).")]
        Article_Delete = 7204,

        #endregion

        #region Группы товаров (7301 - 7400)

        /// <summary>
        /// Добавление
        /// </summary>
        [EnumDisplayName("Добавление")]
        [EnumDescription("Разрешение на добавление новой группы товаров.")]
        ArticleGroup_Create = 7301, 

        /// <summary>
        /// Изменение
        /// </summary>
        [EnumDisplayName("Изменение")]
        [EnumDescription("Разрешение на изменение группы товаров в справочнике.")]
        ArticleGroup_Edit = 7302, 

        /// <summary>
        /// Удаление
        /// </summary>
        [EnumDisplayName("Удаление")]
        [EnumDescription("Разрешение на удаление группы товаров из справочника.")]
        ArticleGroup_Delete = 7303, 

        #endregion

        #region Торговые марки (7401 - 7500)

        /// <summary>
        /// Добавление
        /// </summary>
        [EnumDisplayName("Добавление")]
        [EnumDescription("Разрешение на добавление новой торговой марки.")]
        Trademark_Create = 7401,

        /// <summary>
        /// Изменение
        /// </summary>
        [EnumDisplayName("Изменение")]
        [EnumDescription("Разрешение на изменение торговой марки в справочнике.")]
        Trademark_Edit = 7402,

        /// <summary>
        /// Удаление
        /// </summary>
        [EnumDisplayName("Удаление")]
        [EnumDescription("Разрешение на удаление торговой марки из справочника.")]
        Trademark_Delete = 7403,

        #endregion

        #region Изготовители (7501 - 7600)

        /// <summary>
        /// Добавление
        /// </summary>
        [EnumDisplayName("Добавление")]
        [EnumDescription("Разрешение на добавление нового изготовителя.")]
        Manufacturer_Create = 7501,

        /// <summary>
        /// Изменение
        /// </summary>
        [EnumDisplayName("Изменение")]
        [EnumDescription("Разрешение на изменение изготовителя в справочнике.")]
        Manufacturer_Edit = 7502,

        /// <summary>
        /// Удаление
        /// </summary>
        [EnumDisplayName("Удаление")]
        [EnumDescription("Разрешение на удаление изготовителя из справочника.")]
        Manufacturer_Delete = 7503,

        #endregion

        #region Страны (7601 - 7700)

        /// <summary>
        /// Добавление
        /// </summary>
        [EnumDisplayName("Добавление")]
        [EnumDescription("Разрешение на добавление новой страны.")]
        Country_Create = 7601,

        /// <summary>
        /// Изменение
        /// </summary>
        [EnumDisplayName("Изменение")]
        [EnumDescription("Разрешение на изменение страны в справочнике.")]
        Country_Edit = 7602,

        /// <summary>
        /// Удаление
        /// </summary>
        [EnumDisplayName("Удаление")]
        [EnumDescription("Разрешение на удаление страны из справочника.")]
        Country_Delete = 7603,

        #endregion

        #region Единицы измерения (7701 - 7800)

        /// <summary>
        /// Добавление
        /// </summary>
        [EnumDisplayName("Добавление")]
        [EnumDescription("Разрешение на добавление новой единицы измерения.")]
        MeasureUnit_Create = 7701,

        /// <summary>
        /// Изменение
        /// </summary>
        [EnumDisplayName("Изменение")]
        [EnumDescription("Разрешение на изменение единицы измерения в справочнике.")]
        MeasureUnit_Edit = 7702,

        /// <summary>
        /// Удаление
        /// </summary>
        [EnumDisplayName("Удаление")]
        [EnumDescription("Разрешение на удаление единицы измерения из справочника.")]
        MeasureUnit_Delete = 7703,

        #endregion

        #region Банки (7801 - 7900)

        /// <summary>
        /// Добавление
        /// </summary>
        [EnumDisplayName("Добавление")]
        [EnumDescription("Разрешение на добавление нового банка.")]
        Bank_Create = 7801,

        /// <summary>
        /// Изменение
        /// </summary>
        [EnumDisplayName("Изменение")]
        [EnumDescription("Разрешение на изменение банка в справочнике.")]
        Bank_Edit = 7802,

        /// <summary>
        /// Удаление
        /// </summary>
        [EnumDisplayName("Удаление")]
        [EnumDescription("Разрешение на удаление банка из справочника.")]
        Bank_Delete = 7803,

        #endregion

        #region Валюты (7901 - 8000)

        /// <summary>
        /// Добавление
        /// </summary>
        [EnumDisplayName("Добавление")]
        [EnumDescription("Разрешение на добавление новой валюты.")]
        Currency_Create = 7901,

        /// <summary>
        /// Изменение
        /// </summary>
        [EnumDisplayName("Изменение")]
        [EnumDescription("Разрешение на изменение параметров валюты в справочнике.")]
        Currency_Edit = 7902,

        /// <summary>
        /// Добавление курса валюты
        /// </summary>
        [EnumDisplayName("Добавление курса валюты")]
        [EnumDescription("Разрешение на добавление нового курса валюты для существующей валюты в справочнике.")]
        Currency_AddRate = 7903,

        /// <summary>
        /// Удаление
        /// </summary>
        [EnumDisplayName("Удаление")]
        [EnumDescription("Разрешение на удаление валюты из справочника.")]
        Currency_Delete = 7904,

        #endregion

        #region Сертификаты товаров (8001 - 8100)

        /// <summary>
        /// Добавление
        /// </summary>
        [EnumDisplayName("Добавление")]
        [EnumDescription("Разрешение на добавление нового сертификата товара.")]
        ArticleCertificate_Create = 8001,

        /// <summary>
        /// Изменение
        /// </summary>
        [EnumDisplayName("Изменение")]
        [EnumDescription("Разрешение на изменение сертификата товара в справочнике.")]
        ArticleCertificate_Edit = 8002,

        /// <summary>
        /// Удаление
        /// </summary>
        [EnumDisplayName("Удаление")]
        [EnumDescription("Разрешение на удаление сертификата товара из справочника.")]
        ArticleCertificate_Delete = 8003,

        #endregion

        #region Ставки НДС (8101 - 8200)

        /// <summary>
        /// Добавление
        /// </summary>
        [EnumDisplayName("Добавление")]
        [EnumDescription("Разрешение на добавление новой ставки НДС.")]
        ValueAddedTax_Create = 8101,

        /// <summary>
        /// Изменение
        /// </summary>
        [EnumDisplayName("Изменение")]
        [EnumDescription("Разрешение на изменение ставки НДС.")]
        ValueAddedTax_Edit = 8102,

        /// <summary>
        /// Удаление
        /// </summary>
        [EnumDisplayName("Удаление")]
        [EnumDescription("Разрешение на удаление ставки НДС из справочника.")]
        ValueAddedTax_Delete = 8103,

        #endregion

        #region Организационно-правовые формы (8201 - 8300)

        /// <summary>
        /// Добавление
        /// </summary>
        [EnumDisplayName("Добавление")]
        [EnumDescription("Разрешение на добавление новой организационно-правовой формы.")]
        LegalForm_Create = 8201,

        /// <summary>
        /// Изменение
        /// </summary>
        [EnumDisplayName("Изменение")]
        [EnumDescription("Разрешение на изменение организационно-правовой формы.")]
        LegalForm_Edit = 8202,

        /// <summary>
        /// Удаление
        /// </summary>
        [EnumDisplayName("Удаление")]
        [EnumDescription("Разрешение на удаление организационно-правовой формы из справочника.")]
        LegalForm_Delete = 8203,

        #endregion

        #endregion

        #region Права доступа к операциям производства (20001 - 24000)

        #region Заказы на производство (20001 - 20100)

        /// <summary>
        /// Просмотр списка и деталей
        /// </summary>
        [EnumDisplayName("Просмотр списка и деталей")]
        [EnumDescription("Включает возможность просмотра плана этапов и финансового плана.")]
        ProductionOrder_List_Details = 20001,

        /// <summary>
        /// Создание и редактирование
        /// </summary>
        [EnumDisplayName("Создание и редактирование")]
        [EnumDescription("Разрешение на создание нового заказа, его редактирование (форма «Редактирование заказа»).")]
        ProductionOrder_Create_Edit = 20002,

        /// <summary>
        /// Изменение куратора заказа
        /// </summary>
        [EnumDisplayName("Изменение куратора заказа")]
        [EnumDescription("Разрешение на изменение куратора заказа.")]
        ProductionOrder_Curator_Change = 20005,

        /// <summary>
        /// Изменение курса валюты заказа
        /// </summary>
        [EnumDisplayName("Изменение курса валюты заказа")]
        [EnumDescription("Разрешение на изменение куратора заказа.")]
        ProductionOrder_CurrencyRate_Change = 20006,

        /// <summary>
        /// Добавление и редактирование контракта
        /// </summary>
        [EnumDisplayName("Добавление и редактирование контракта")]
        [EnumDescription("Разрешение на добавление и изменение информации о контракте в заказе.")]
        ProductionOrder_Contract_Change = 20007,

        /// <summary>
        /// Просмотр печатной формы "Себестоимость"
        /// </summary>
        [EnumDisplayName("Расчет себестоимости")]
        [EnumDescription("Разрешение на просмотр формы «Себестоимость».")]
        ProductionOrder_ArticlePrimeCostPrintingForm_View = 20008,


        #endregion

        #region Производители (20101 - 20200)

        /// <summary>
        /// Просмотр списка и деталей
        /// </summary>
        [EnumDisplayName("Просмотр списка и деталей")]
        [EnumDescription("Разрешение на просмотр списка производителей и деталей любого из них, а также просмотр связанных справочников.")]
        Producer_List_Details = 20101,

        /// <summary>
        /// Добавление производителя
        /// </summary>
        [EnumDisplayName("Добавление производителя")]
        [EnumDescription("Разрешение на добавление нового производителя в систему.")]
        Producer_Create = 20102,

        /// <summary>
        /// Изменение производителя
        /// </summary>
        [EnumDisplayName("Изменение производителя")]
        [EnumDescription("Разрешение на изменение данных о производителе.")]
        Producer_Edit = 20103,

        /// <summary>
        /// Создание банковского счета
        /// </summary>
        [EnumDisplayName("Создание банковского счета")]
        [EnumDescription("Разрешение на создание нового банковского счета производителю.")]
        Producer_BankAccount_Create = 20104,

        /// <summary>
        /// Изменение банковского счета
        /// </summary>
        [EnumDisplayName("Изменение банковского счета")]
        [EnumDescription("Разрешение на редактирование введенных ранее банковских счетов для производителя.")]
        Producer_BankAccount_Edit = 20105,

        /// <summary>
        /// Удаление банковского счета
        /// </summary>
        [EnumDisplayName("Удаление банковского счета")]
        [EnumDescription("Разрешение на удаление введенных ранее банковских счетов для производителя.")]
        Producer_BankAccount_Delete = 20106,

        /// <summary>
        /// Удаление производителя
        /// </summary>
        [EnumDisplayName("Удаление производителя")]
        [EnumDescription("Разрешение на выполнение операции «Удаление производителя».")]
        Producer_Delete = 20107,

        #endregion

        #region Партии заказа (20201 - 20300)

        /// <summary>
        /// Просмотр сводной информации о партиях
        /// </summary>
        [EnumDisplayName("Просмотр сводной информации о партиях")]
        [EnumDescription("Разрешение на просмотр сводной информации о партиях заказа (на стр. «Детали заказа»).")]
        ProductionOrderBatch_List = 20201,

        /// <summary>
        /// Просмотр деталей партии заказа
        /// </summary>
        [EnumDisplayName("Просмотр деталей партии заказа")]
        [EnumDescription("Разрешение на просмотр деталей партий заказа (стр. «Детали партии заказа»).")]
        ProductionOrderBatch_Details = 20202,

        /// <summary>
        /// Добавление и редактирование позиции заказа
        /// </summary>
        [EnumDisplayName("Добавление и редактирование позиции заказа")]
        [EnumDescription("Разрешение на добавление и редактирование позиции в партии заказа.")]
        ProductionOrderBatch_Row_Create_Edit = 20203,

        /// <summary>
        /// Удаление позиции партии заказа
        /// </summary>
        [EnumDisplayName("Удаление позиции партии заказа")]
        [EnumDescription("Разрешение на удаление позиции из партии заказа.")]
        ProductionOrderBatch_Row_Delete = 20204,

        /// <summary>
        /// Проводка партии заказа
        /// </summary>
        [EnumDisplayName("Проводка партии заказа")]
        [EnumDescription("Разрешение на выполнение операции «Провести партию заказа».")]
        ProductionOrderBatch_Accept = 20205,

        /// <summary>
        /// Отмена проводки партии заказа
        /// </summary>
        [EnumDisplayName("Отмена проводки партии заказа")]
        [EnumDescription("Разрешение на выполнение операции «Отменить проводку партии заказа».")]
        ProductionOrderBatch_Cancel_Acceptance = 20206,

        /// <summary>
        /// Утверждение партии от руководителя
        /// </summary>
        [EnumDisplayName("Утверждение партии от руководителя")]
        [EnumDescription("Разрешение на выполнение операции «Утвердить: руководитель» в партии заказа.")]
        ProductionOrderBatch_Approve_By_LineManager = 20207,

        /// <summary>
        /// Отмена утверждения партии от руководителя
        /// </summary>
        [EnumDisplayName("Отмена утверждения партии от руководителя")]
        [EnumDescription("Разрешение на выполнение операции «Отменить утверждение: руководитель» в партии заказа.")]
        ProductionOrderBatch_Cancel_Approvement_By_LineManager = 20208,

        /// <summary>
        /// Утверждение партии от фин. отдела
        /// </summary>
        [EnumDisplayName("Утверждение партии от фин. отдела")]
        [EnumDescription("Разрешение на выполнение операции «Утвердить: фин. отдел» в партии заказа.")]
        ProductionOrderBatch_Approve_By_FinancialDepartment = 20209,

        /// <summary>
        /// Отмена утверждения партии от фин. отдела
        /// </summary>
        [EnumDisplayName("Отмена утверждения партии от фин. отдела")]
        [EnumDescription("Разрешение на выполнение операции «Отменить утверждение: фин. отдел» в партии заказа.")]
        ProductionOrderBatch_Cancel_Approvement_By_FinancialDepartment = 20210,

        /// <summary>
        /// Утверждение партии от отдела продаж
        /// </summary>
        [EnumDisplayName("Утверждение партии от отдела продаж")]
        [EnumDescription("Разрешение на выполнение операции «Утвердить: отдел продаж» в партии заказа.")]
        ProductionOrderBatch_Approve_By_SalesDepartment = 20211,

        /// <summary>
        /// Отмена утверждения партии от отдела продаж
        /// </summary>
        [EnumDisplayName("Отмена утверждения партии от отдела продаж")]
        [EnumDescription("Разрешение на выполнение операции «Отменить утверждение: отдел продаж» в партии заказа.")]
        ProductionOrderBatch_Cancel_Approvement_By_SalesDepartment = 20212,

        /// <summary>
        /// Утверждение партии от аналитического отдела
        /// </summary>
        [EnumDisplayName("Утверждение партии от аналитического отдела")]
        [EnumDescription("Разрешение на выполнение операции «Утвердить: аналит. отдел» в партии заказа.")]
        ProductionOrderBatch_Approve_By_AnalyticalDepartment = 20213,

        /// <summary>
        /// Отмена утверждения партии от аналитического отдела
        /// </summary>
        [EnumDisplayName("Отмена утверждения партии от аналитического отдела")]
        [EnumDescription("Разрешение на выполнение операции «Отменить утверждение: аналит. отдел» в партии заказа.")]
        ProductionOrderBatch_Cancel_Approvement_By_AnalyticalDepartment = 20214,

        /// <summary>
        /// Утверждение партии от руководителя проекта
        /// </summary>
        [EnumDisplayName("Утверждение партии от руководителя проекта")]
        [EnumDescription("Разрешение на выполнение операции «Утвердить: РП» в партии заказа.")]
        ProductionOrderBatch_Approve_By_ProjectManager = 20215,

        /// <summary>
        /// Отмена утверждения партии от руководителя проекта
        /// </summary>
        [EnumDisplayName("Отмена утверждения партии от руководителя проекта")]
        [EnumDescription("Разрешение на выполнение операции «Отменить утверждение: РП» в партии заказа.")]
        ProductionOrderBatch_Cancel_Approvement_By_ProjectManager = 20216,

        /// <summary>
        /// Перевод партии в "Готово"
        /// </summary>
        [EnumDisplayName("Перевод партии в «Готово»")]
        [EnumDescription("Разрешение на перевод партии в состояние «Готово».")]
        ProductionOrderBatch_Approve = 20217,

        /// <summary>
        /// Отмена готовности партии
        /// </summary>
        [EnumDisplayName("Отмена готовности партии")]
        [EnumDescription("Разрешение на отмену состояния готовности партии заказа.")]
        ProductionOrderBatch_Cancel_Approvement = 20218,

        /// <summary>
        /// Разделение партии заказа
        /// </summary>
        [EnumDisplayName("Разделение партии заказа")]
        [EnumDescription("Разрешение на выполнение операции «Разделить партию».")]
        ProductionOrderBatch_Split = 20219,

        /// <summary>
        /// Склеивание партии заказа
        /// </summary>
        [EnumDisplayName("Склеивание партии заказа")]
        [EnumDescription("Разрешение на выполнение операции «Склеить партию».")]
        ProductionOrderBatch_Join = 20220,

        /// <summary>
        /// Изменение информации для размещения в контейнерах
        /// </summary>
        [EnumDisplayName("Изменение информации для размещения в контейнерах")]
        [EnumDescription("Разрешение на ввод и изменение информации о контейнерах для расчета размещения.")]
        ProductionOrderBatch_Edit_Placement_In_Containers = 20221,

        /// <summary>
        /// Создание партии заказа
        /// </summary>
        [EnumDisplayName("Создание партии заказа")]
        [EnumDescription("Разрешение на выполнение операции «Создать партию».")]
        ProductionOrderBatch_Create = 20222,

        /// <summary>
        /// Удаление партии заказа
        /// </summary>
        [EnumDisplayName("Удаление партии заказа")]
        [EnumDescription("Разрешение на выполнение операции «Удалить партию».")]
        ProductionOrderBatch_Delete = 20223,

        #endregion

        #region Транспортные листы (20301-20400)

        /// <summary>
        /// Просмотр списка и деталей
        /// </summary>
        [EnumDisplayName("Просмотр списка и деталей")]
        [EnumDescription("Просмотр списка и деталей.")]
        ProductionOrderTransportSheet_List_Details = 20301,

        /// <summary>
        /// Создание и редактирование
        /// </summary>
        [EnumDisplayName("Создание и редактирование")]
        [EnumDescription("Создание и редактирование.")]
        ProductionOrderTransportSheet_Create_Edit = 20302,

        /// <summary>
        /// Удаление
        /// </summary>
        [EnumDisplayName("Удаление")]
        [EnumDescription("Удаление.")]
        ProductionOrderTransportSheet_Delete = 20303,

        #endregion

        #region Листы дополнительных расходов (20401-20500)

        /// <summary>
        /// Просмотр списка и деталей
        /// </summary>
        [EnumDisplayName("Просмотр списка и деталей")]
        [EnumDescription("Просмотр списка и деталей.")]
        ProductionOrderExtraExpensesSheet_List_Details = 20401,

        /// <summary>
        /// Создание и редактирование
        /// </summary>
        [EnumDisplayName("Создание и редактирование")]
        [EnumDescription("Создание и редактирование.")]
        ProductionOrderExtraExpensesSheet_Create_Edit = 20402,

        /// <summary>
        /// Удаление
        /// </summary>
        [EnumDisplayName("Удаление")]
        [EnumDescription("Удаление.")]
        ProductionOrderExtraExpensesSheet_Delete = 20403,

        #endregion

        #region Таможенные листы (20501-20600)

        /// <summary>
        /// Просмотр списка и деталей
        /// </summary>
        [EnumDisplayName("Просмотр списка и деталей")]
        [EnumDescription("Просмотр списка и деталей.")]
        ProductionOrderCustomsDeclaration_List_Details = 20501,

        /// <summary>
        /// Создание и редактирование
        /// </summary>
        [EnumDisplayName("Создание и редактирование")]
        [EnumDescription("Создание и редактирование.")]
        ProductionOrderCustomsDeclaration_Create_Edit = 20502,

        /// <summary>
        /// Удаление
        /// </summary>
        [EnumDisplayName("Удаление")]
        [EnumDescription("Удаление.")]
        ProductionOrderCustomsDeclaration_Delete = 20503,

        #endregion

        #region Оплаты в заказах (20601-20700)

        /// <summary>
        /// Просмотр списка и деталей
        /// </summary>
        [EnumDisplayName("Просмотр списка и деталей")]
        [EnumDescription("Просмотр списка и деталей.")]
        ProductionOrderPayment_List_Details = 20601,

        /// <summary>
        /// Создание и редактирование
        /// </summary>
        [EnumDisplayName("Создание и редактирование")]
        [EnumDescription("Создание и редактирование.")]
        ProductionOrderPayment_Create_Edit = 20602,

        /// <summary>
        /// Удаление
        /// </summary>
        [EnumDisplayName("Удаление")]
        [EnumDescription("Удаление.")]
        ProductionOrderPayment_Delete = 20603,

        #endregion

        #region Пакеты материалов (20701-20800)

        /// <summary>
        /// Просмотр списка и деталей
        /// </summary>
        [EnumDisplayName("Просмотр списка и деталей")]
        [EnumDescription("Просмотр списка и деталей.")]
        ProductionOrderMaterialsPackage_List_Details = 20701,

        /// <summary>
        /// Создание и редактирование
        /// </summary>
        [EnumDisplayName("Создание и редактирование")]
        [EnumDescription("Создание и редактирование.")]
        ProductionOrderMaterialsPackage_Create_Edit = 20702,

        /// <summary>
        /// Удаление
        /// </summary>
        [EnumDisplayName("Удаление")]
        [EnumDescription("Удаление.")]
        ProductionOrderMaterialsPackage_Delete = 20703,

        #endregion

        #region Шаблоны этапов (20801-20900)

        /// <summary>
        /// Просмотр списка и деталей
        /// </summary>
        [EnumDisplayName("Просмотр списка и деталей")]
        [EnumDescription("Разрешение на просмотр списка шаблонов и деталей любого из них.")]
        ProductionOrderBatchLifeCycleTemplate_List_Details = 20801,

        /// <summary>
        /// Создание и редактирование
        /// </summary>
        [EnumDisplayName("Создание и редактирование")]
        [EnumDescription("Разрешение на создание и редактирование шаблона (включая изменение этапов в шаблоне).")]
        ProductionOrderBatchLifeCycleTemplate_Create_Edit = 20802,

        /// <summary>
        /// Удаление
        /// </summary>
        [EnumDisplayName("Удаление")]
        [EnumDescription("Разрешение на удаление шаблона.")]
        ProductionOrderBatchLifeCycleTemplate_Delete = 20803,

        #endregion

        #region План исполнения заказа (этапы) (20901-21000)

        /// <summary>
        /// Просмотр списка и деталей
        /// </summary>
        [EnumDisplayName("Просмотр списка и деталей")]
        [EnumDescription("Разрешение на просмотр операционного плана и деталей этапа.")]
        ProductionOrder_Stage_List_Details = 20901,

        /// <summary>
        /// Ввод и редактирование этапов
        /// </summary>
        [EnumDisplayName("Ввод и редактирование этапов")]
        [EnumDescription("Разрешение на изменение операционного плана партии.")]
        ProductionOrder_Stage_Create_Edit = 20902,

        /// <summary>
        /// Перевод партии на следующий этап
        /// </summary>
        [EnumDisplayName("Перевод партии на следующий этап")]
        [EnumDescription("Разрешение перевести партию на следующий по плану этап.")]
        ProductionOrder_Stage_MoveToNext = 20903,

        /// <summary>
        /// Перевод партии на предыдущий этап
        /// </summary>
        [EnumDisplayName("Перевод партии на предыдущий этап")]
        [EnumDescription("Разрешение перевести партию на предыдущий по плану этап.")]
        ProductionOrder_Stage_MoveToPrevious = 20904,

        /// <summary>
        /// Перевод партии на этап "Неуспешное закрытие"
        /// </summary>
        [EnumDisplayName("Перевод партии на этап «Неуспешное закрытие»")]
        [EnumDescription("Разрешение перевести партию на этап «Неуспешное закрытие».")]
        ProductionOrder_Stage_MoveToUnsuccessfulClosing = 20905,

        #endregion

        #region Финансовый план заказа (21001-21100)

        /// <summary>
        /// Просмотр плана оплат
        /// </summary>
        [EnumDisplayName("Просмотр плана оплат")]
        [EnumDescription("Разрешение на просмотр плана оплат заказа (плановые оплаты) и деталей любой из них.")]
        ProductionOrder_PlannedPayments_List_Details = 21001,

        /// <summary>
        /// Добавление платежа в план оплат
        /// </summary>
        [EnumDisplayName("Добавление платежа в план")]
        [EnumDescription("Разрешение на добавление платежа в план оплат заказа.")]
        ProductionOrder_PlannedPayments_Create = 21002,

        /// <summary>
        /// Редактирование платежа в плане оплат
        /// </summary>
        [EnumDisplayName("Редактирование платежа в плане")]
        [EnumDescription("Разрешение на редактирование платежа в плане оплат заказа.")]
        ProductionOrder_PlannedPayments_Edit = 21003,

        /// <summary>
        /// Удаление платежа в плане оплат
        /// </summary>
        [EnumDisplayName("Удаление платежа из плана")]
        [EnumDescription("Разрешение на удаление платежа из плана оплат заказа.")]
        ProductionOrder_PlannedPayments_Delete = 21004,

        /// <summary>
        /// Просмотр финансового плана
        /// </summary>
        [EnumDisplayName("Просмотр финансового плана")]
        [EnumDescription("Разрешение на просмотр финансового плана заказа (плановые затраты).")]
        ProductionOrder_PlannedExpenses_List_Details = 21005,

        /// <summary>
        /// Ввод и редактирование финансового плана
        /// </summary>
        [EnumDisplayName("Ввод и редактирование финансового плана")]
        [EnumDescription("Разрешение на изменение финансового плана заказа (плановые затраты).")]
        ProductionOrder_PlannedExpenses_Create_Edit = 21006,

        #endregion

        #endregion

        #region Права доступа к отчетам (24001 - 30000)

        #region Наличие товаров на местах хранения (24001 - 24100)

        /// <summary>
        /// Просмотр отчета «Наличие товаров на местах хранения»
        /// </summary>
        [EnumDisplayName("Просмотр отчета")]
        [EnumDescription("Разрешение на построение отчетов группы Report0001.")]
        Report0001_View = 24001,

        /// <summary>
        /// Места хранения в отчете «Наличие товаров на местах хранения»
        /// </summary>
        [EnumDisplayName("Места хранения в отчете")]
        [EnumDescription("Список мест хранения для построения отчетов группы Report0001.")]
        Report0001_Storage_List = 24002,

        #endregion

        #region Реализация товаров (24101 - 24200)
        
        /// <summary>
        /// Просмотр отчета «Реализация товаров»
        /// </summary>
        [EnumDisplayName("Просмотр отчета")]
        [EnumDescription("Разрешение на просмотр отчетов группы Report0002.")]
        Report0002_View = 24101,

        /// <summary>
        /// Места хранения в отчете «Реализация товаров»
        /// </summary>
        [EnumDisplayName("Места хранения в отчете")]
        [EnumDescription("Список мест хранения для построения отчетов группы Report0002.")]
        Report0002_Storage_List = 24102,

        /// <summary>
        /// Пользователи в отчете «Реализация товаров»
        /// </summary>
        [EnumDisplayName("Пользователи в отчете")]
        [EnumDescription("Список пользователей для построения отчетов группы Report0002.")]
        Report0002_User_List = 24103,

        #endregion

        #region Финансовый отчет (24201 - 24300)

        /// <summary>
        /// Просмотр отчета «Финансовый отчет»
        /// </summary>
        [EnumDisplayName("Просмотр отчета")]
        [EnumDescription("Разрешение на просмотр отчетов группы Report0003.")]
        Report0003_View = 24201,

        /// <summary>
        /// Места хранения в отчете «Финансовый отчет»
        /// </summary>
        [EnumDisplayName("Места хранения в отчете")]
        [EnumDescription("Список мест хранения для построения отчетов группы Report0003.")]
        Report0003_Storage_List = 24202,

        #endregion

        #region Отчет "Движение товара за период" (24301 - 24400)

        /// <summary>
        /// Просмотр отчета «Движение товара за период»
        /// </summary>
        [EnumDisplayName("Просмотр отчета")]
        [EnumDescription("Разрешение на просмотр отчетов группы Report0004.")]
        Report0004_View = 24301,

        /// <summary>
        /// Места хранения в отчете «Движение товара за период»
        /// </summary>
        [EnumDisplayName("Места хранения в отчете")]
        [EnumDescription("Список мест хранения для построения отчетов группы Report0004.")]
        Report0004_Storage_List = 24302,

        #endregion

        #region Карта движения товара (24401 - 24500)

        /// <summary>
        /// Просмотр отчета «Карта движения товара»
        /// </summary>
        [EnumDisplayName("Просмотр отчета")]
        [EnumDescription("Разрешение на просмотр отчетов группы Report0005.")]
        Report0005_View = 24401,

        /// <summary>
        /// Места хранения в отчете «Карта движения товара»
        /// </summary>
        [EnumDisplayName("Места хранения в отчете")]
        [EnumDescription("Список мест хранения для построения отчетов группы Report0005.")]
        Report0005_Storage_List = 24402,

        #endregion

        #region Отчет по взаиморасчетам с клиентами (24501 - 24600)

        /// <summary>
        /// Просмотр отчета «Отчет по взаиморасчетам с клиентами»
        /// </summary>
        [EnumDisplayName("Просмотр отчета")]
        [EnumDescription("Разрешение на просмотр отчетов группы Report0006.")]
        Report0006_View = 24501,

        #endregion

        #region Отчет по взаиморасчетам по реализациям (24601-24700)

        /// <summary>
        /// Просмотр отчета «Отчет по взаиморасчетам по реализациям»
        /// </summary>
        [EnumDisplayName("Просмотр отчета")]
        [EnumDescription("Разрешение на просмотр отчетов группы Report0007.")]
        Report0007_View = 24601,

        /// <summary>
        /// Места хранения в отчете «Отчет по взаиморасчетам по реализациям»
        /// </summary>
        [EnumDisplayName("Места хранения в отчете")]
        [EnumDescription("Список мест хранения для построения отчетов группы Report0007.")]
        Report0007_Storage_List = 24602,

        /// <summary>
        /// Дата построения отчета «Отчет по взаиморасчетам по реализациям»
        /// </summary>
        [EnumDisplayName("Выбор даты построения отчета")]
        [EnumDescription("Выбор даты, на которую строятся отчеты группы Report0007.")]
        Report0007_Date_Change = 24603,

        #region Отчет «Реестр накладных» (24701-24800)

        /// <summary>
        /// Просмотр отчета «Реестр накладных»
        /// </summary>
        [EnumDisplayName("Просмотр отчета")]
        [EnumDescription("Разрешение на просмотр отчетов группы Report0008.")]
        Report0008_View = 24701,

        /// <summary>
        /// Места хранения в отчете «Реестр накладных»
        /// </summary>
        [EnumDisplayName("Места хранения в отчете")]
        [EnumDescription("Список мест хранения для построения отчетов группы Report0008.")]
        Report0008_Storage_List = 24702,

        #endregion

        #endregion

        #region Отчет по поставкам (24801-24900)

        /// <summary>
        /// Просмотр отчета «Отчет по поставкам»
        /// </summary>
        [EnumDisplayName("Просмотр отчета")]
        [EnumDescription("Разрешение на просмотр отчетов группы Report0009.")]
        Report0009_View = 24801,

        /// <summary>
        /// Пользователи в отчете «Отчет по поставкам»
        /// </summary>
        [EnumDisplayName("Пользователи в отчете")]
        [EnumDescription("Список пользователей для построения отчетов группы Report0009.")]
        Report0009_User_List = 24802,

        /// <summary>
        /// Места хранения в отчете «Отчет по поставкам»
        /// </summary>
        [EnumDisplayName("Места хранения в отчете")]
        [EnumDescription("Список мест хранения для построения отчетов группы Report0009.")]
        Report0009_Storage_List = 24803,

		#endregion

        #region Принятые платежи (24901-25000)

        /// <summary>
        /// Просмотр отчета «Принятые платежи»
        /// </summary>
        [EnumDisplayName("Просмотр отчета")]
        [EnumDescription("Разрешение на просмотр отчетов группы Report0010.")]
        Report0010_View = 24901,

        #endregion

        #endregion

        #region Права доступа к задачам (30001 - 30200)

        #region Исполнение задачи (30001 - 30100)

        /// <summary>
        /// Создание исполнения задачи
        /// </summary>
        [EnumDisplayName("Создание исполнения задачи")]
        [EnumDescription("Разрешение на создание исполнения задачи (по исполнителю задачи).")]
        TaskExecutionItem_Create = 30001,

        /// <summary>
        /// Редактирование исполнения задачи
        /// </summary>
        [EnumDisplayName("Редактирование исполнения задачи")]
        [EnumDescription("Разрешение на редактирование исполнения задачи (по автору исполнения).")]
        TaskExecutionItem_Edit= 30002,

        /// <summary>
        /// Удаление исполнения задачи
        /// </summary>
        [EnumDisplayName("Удаление исполнения задачи")]
        [EnumDescription("Разрешение на удаление исполнения задачи (по автору исполнения).")]
        TaskExecutionItem_Delete = 30003,

        /// <summary>
        /// Редаутирвоание даты изменения исполнения
        /// </summary>
        [EnumDisplayName("Радактирование даты исполнения задачи")]
        [EnumDescription("Разрешение на редактирование даты исполнения задачи (по автору исполнения).")]
        TaskExecutionItem_EditExecutionDate= 30004,

        #endregion

        #region Задача (30101 - 30200)

        /// <summary>
        /// Просмотр авторских задач
        /// </summary>
        [EnumDisplayName("Просмотр авторских задач")]
        [EnumDescription("Разрешение на просмотр задачи в зависимости от пользователя, создавшего эту задачу (по автору задачи).")]
        Task_CreatedBy_List_Details = 30101,

        /// <summary>
        /// Просмотр назначенных задач
        /// </summary>
        [EnumDisplayName("Просмотр назначенных задач")]
        [EnumDescription("Разрешение на просмотр задачи в зависимости от пользователя, которому она назначена (по исполнителю задачи).")]
        Task_ExecutedBy_List_Details = 30102,

        /// <summary>
        /// Создание задачи
        /// </summary>
        [EnumDisplayName("Создание задачи")]
        [EnumDescription("Разрешение на создание новой задачи (по исполнителю задачи).")]
        Task_Create = 30103,

        /// <summary>
        /// Редактирование задачи
        /// </summary>
        [EnumDisplayName("Редактирование задачи")]
        [EnumDescription("Разрешение на изменение существующей задачи (по автору задачи).")]
        Task_Edit = 30104,

        /// <summary>
        /// Задачи для редактирования/удаления исполнения
        /// </summary>
        [EnumDisplayName("Задачи для редактирования и удаления исполнения")]
        [EnumDescription("Разрешение на редактирование и удаление исполнения в задаче (по автору задачи).")]
        Task_TaskExecutionItem_Edit_Delete = 30105,

        /// <summary>
        /// Удаление задачи
        /// </summary>
        [EnumDisplayName("Удаление задачи")]
        [EnumDescription("Разрешение на удаление задачи (по автору задачи).")]
        Task_Delete = 30106,

        #endregion

        #endregion

        #region Права доступа на выгрузку в 1С (30201 - 30300)

        /// <summary>
        /// Экспорт в 1С
        /// </summary>
        [EnumDisplayName("Экспорт в 1С")]
        [EnumDescription("Разрешение на осуществление выгрузки данных в 1С.")]
        ExportTo1C = 30201

        #endregion


        /*
        /// <summary>
        /// 
        /// </summary>
        [EnumDisplayName("")]
        [EnumDescription(".")]
         = ,
        */

    }
}
