<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.PrintingForm.Common.InvoicePrintingFormViewModel>" %>

<!DOCTYPE html>
<html>
<head id="Head1">
    <title>Счет-фактура</title>
    <link rel="shortcut icon" href="/Content/Img/favicon.ico" />
    <meta name="SKYPE_TOOLBAR" content="SKYPE_TOOLBAR_PARSER_COMPATIBLE" />
    <link href="/Content/Style/PrintingForm.css" rel="stylesheet" type="text/css" />
    
    <script src="/Scripts/jquery-1.5.2.min.js" type="text/javascript"></script>    
    <script src="/Scripts/jquery.validate.min.js" type="text/javascript"></script>
    <script src="/Scripts/jquery.validate.unobtrusive.min.js" type="text/javascript"></script>
    <script src="/Scripts/modules.min.js" type="text/javascript"></script>
    <script src="/Scripts/Common.js" type="text/javascript"></script>

    <style type="text/css">
            .page{
                height: 545pt;
                margin: 0;
                padding: 0;
            }
            .page_Opera{
                height: 500pt;
                margin: 0;
                padding: 0;
            }
            .MainTable td
            {
                padding: 0 2px;
            }
            
            table.MainTable
            {
                border: none;
                width: 100%;
            }
            
            #PageHeader > table
            {
                padding-bottom:35px !important;
            }
            
            #PageFooter > table
            {
                padding-top:35px !important;
            }
            
            @media print{
                .pageBreak{
                    page-break-after: always;
                }            
            }
            .tableSummary
            {
                font-weight: bold;  
                font-size: 10pt;              
            }
            
            td.emptyCell
            {
                border: none !important;
               
            }
            
            td.PageNumberCell
            {
                font-family: Arial; 
                font-size: 8pt; 
                font-style: italic; 
                text-align: right; 
                vertical-align: top;
                border:none !important;
            }
            
            td.col1
            {
                width: 100%;
            }
            
    </style>
    
    <script type="text/javascript">
        PrintingForm_InvoicePrintingForm.Init();
    </script>
</head>
<body>
     <div style="display:none">
        <%: Html.HiddenFor(x => x.RowsContentURL) %>
        <%: Html.HiddenFor(x => x.WaybillId) %>
        <%: Html.HiddenFor(x => x.PriceTypeId) %>
        <%: Html.HiddenFor(model => model.ConsiderReturns)%>
   
        <div id="PageHeader" style="padding: 0; margin: 0">
            <div style="height: 1%">
                <div class="font10pt" style="white-space: nowrap; text-align: right; float: right;width:100%">Приложение № 1<br />
                    к постановлению Правительства<br />                    
                    Российской Федерации<br />                    
                    от 26 декабря 2011 г. № 1137</div>
            </div>
            <table style="width: 80%" class="font10pt">
                <col width="25%">
                <col width="70%">
                <col width="5%">
                <tr>
                    <td></td>
                    <td align="right" rowspan="2">
                        <table style="width: 90%;">
                            <col width="40%">
                            <col width="25%">
                            <col width="5%">
                            <col width="30%">
                            <tr>
                                <td style="font-weight: bold" class="font12pt">
                                    <%: Model.Title %>&nbsp;&nbsp;<%:Html.LabelFor(x => x.Number) %>&nbsp;&nbsp;</td>
                                <td align="left" class="underline font12pt" style="font-weight: bold; white-space: nowrap;">
                                    <%:Model.Number %></td>
                                <td style="font-weight: bold" class="font12pt">&nbsp;&nbsp;<%:Html.LabelFor(x => x.Date) %>&nbsp;&nbsp;</td>
                                <td class="underline font12pt" style="font-weight: bold">
                                    <%:Model.Date %></td>
                            </tr>
                            <tr>
                                <td style="white-space: nowrap" class="font12pt">
                                    &nbsp;&nbsp;<%:Html.LabelFor(x => x.CorrectionNumber) %>&nbsp;&nbsp;</td>
                                <td align="center" class="underline font12pt" style="white-space: nowrap;">
                                    <%:Model.CorrectionNumber %></td>
                                <td class="font12pt">&nbsp;&nbsp;<%:Html.LabelFor(x => x.CorrectionDate) %>&nbsp;&nbsp;</td>
                                <td align="center" class="underline font12pt">
                                    <%:Model.CorrectionDate %></td>
                            </tr>
                        </table>
                    </td>
                    <td style="width: 1%">&nbsp;(1)</td>
                </tr>
                <tr>
                    <td></td>                    
                    <td style="width: 1%">&nbsp;(1а)</td>
                </tr>
                <tr>
                    <td style="width: 1%;padding-top:20px">
                        <%:Html.LabelFor(x => x.SellerName) %></td>
                    <td class="underline" style="padding-top:20px">
                        <%:Model.SellerName %></td>
                    <td style="width: 1%;padding-top:20px">&nbsp;(2)</td>
                </tr>
                <tr>
                    <td>
                        <%:Html.LabelFor(x => x.SellerAddress) %></td>
                    <td class="underline">
                        <%:Model.SellerAddress %></td>
                    <td style="width: 1%">&nbsp;(2а)</td>
                </tr>
                <tr>
                    <td>
                        <%:Html.LabelFor(x => x.SellerINN_KPP) %></td>
                    <td class="underline">
                        <%:Model.SellerINN_KPP %></td>
                    <td style="width: 1%">&nbsp;(2б)</td>
                </tr>
                <tr>
                    <td>
                        <%:Html.LabelFor(x => x.ArticleSenderInfo) %></td>
                    <td class="underline">
                        <%:Model.ArticleSenderInfo %></td>
                    <td style="width: 1%">&nbsp;(3)</td>
                </tr>
                <tr>
                    <td>
                        <%:Html.LabelFor(x => x.ArticleRecipientInfo) %></td>
                    <td class="underline">
                        <%:Model.ArticleRecipientInfo %></td>
                    <td style="width: 1%">&nbsp;(4)</td>
                </tr>
                <tr>
                    <td style="white-space: nowrap;">
                        <%:Html.LabelFor(x => x.PaymentDocumentNumber) %></td>
                    <td> 
                        <div style="position:relative;overflow:auto">
                            <div class="underline" style="min-width:39%;float:left">
                                <%:Model.PaymentDocumentNumber %>
                            </div>
                            <div style="min-width:2%;float:left">от</div>
                            <div class="underline" style="min-width:59%;position:absolute;bottom:0;right:0">
                            </div>
                        </div>
                    </td>
                    <td style="width: 1%">&nbsp;(5)</td>
                </tr>
                <tr>
                    <td>
                        <%:Html.LabelFor(x => x.BuyerName) %></td>
                    <td class="underline">
                        <%:Model.BuyerName %></td>
                    <td style="width: 1%">&nbsp;(6)</td>
                </tr>
                <tr>
                    <td>
                        <%:Html.LabelFor(x => x.BuyerAddress) %></td>
                    <td class="underline">
                        <%:Model.BuyerAddress %></td>
                    <td style="width: 1%">&nbsp;(6а)</td>
                </tr>
                <tr>
                    <td>
                        <%:Html.LabelFor(x => x.BuyerINN_KPP) %></td>
                    <td class="underline">
                        <%:Model.BuyerINN_KPP %></td>
                    <td style="width: 1%">&nbsp;(6б)</td>
                </tr>
                <tr>
                    <td>
                        <%:Html.LabelFor(x => x.CurrencyInfo) %></td>
                    <td class="underline">
                        <%:Model.CurrencyInfo%></td>
                    <td style="width: 1%">&nbsp;(7)</td>
                </tr>
            </table>
        </div>
        <div id="MainTable">
            <table style="width: 0px; height: 0px;" cellspacing="0">
                <thead>
                    <tr class="R18">
                        <td class="PageNumberCell" colspan="13"><div style="white-space: nowrap" class="PageNumber">Страница 1</div></td>                    
                    </tr>
                    <tr>
                        <th style="min-width: 30%; vertical-align: top" rowspan="2">
                            Наименование товара (описание выполненных работ, оказанных услуг), имущественного права
                        </th>
                        <th colspan="2">
                           Единица измерения
                        </th>                        
                        <th style="min-width: 5%; vertical-align: top" rowspan="2">
                           Количество (объем)
                        </th>                        
                        <th style="min-width: 7%; vertical-align: top" rowspan="2">
                           Цена (тариф) за единицу измерения
                        </th>
                        <th style="min-width: 10%; vertical-align: top" rowspan="2">
                           Стоимость товаров (работ, услуг), имущественных прав без налога - всего
                        </th>
                        <th style="min-width: 4%; vertical-align: top" rowspan="2">
                            В том числе сумма акциза
                        </th>
                        <th style="min-width: 4%; vertical-align: top" rowspan="2">
                            Налоговая ставка
                        </th>
                        <th style="min-width: 7%; vertical-align: top" rowspan="2">
                            Сумма налога, предъявляемая покупателю
                        </th>
                        <th style="min-width: 10%; vertical-align: top" rowspan="2">
                            Стоимость товаров (работ, услуг), имущественных прав с налогом - всего
                        </th>
                        <th colspan="2">
                            Страна происхождения товара
                        </th>                           
                        <th style="min-width: 8%; vertical-align: top" rowspan="2">
                           Номер таможенной декларации
                        </th>
                    </tr>
                    <tr>
                        <th style="min-width: 2%; vertical-align: top">
                            Код
                        </th>
                        <th style="min-width: 4%; vertical-align: top">
                            Условное обозначение (национальное)
                        </th>
                        <th style="min-width: 2%; vertical-align: top">
                            Цифровой код
                        </th>
                        <th style="min-width: 4%; vertical-align: top">
                            Краткое наименование
                        </th>
                    </tr>
                    <tr>
                        <th>1</th>
                        <th>2</th>
                        <th>2а</th>
                        <th>3</th>
                        <th>4</th>
                        <th>5</th>
                        <th>6</th>
                        <th>7</th>
                        <th>8</th>
                        <th>9</th>
                        <th>10</th>
                        <th>10а</th>
                        <th>11</th>
                    </tr>
                </thead>
            </table>
        </div>   
        <div id="PageFooter" class="font8pt">
            <table style='width: 100%'>
                <col width="17%">
                <col width="13%">
                <col width="2%">
                <col width="15%">
                <col width="2%">
                <col width="19%">
                <col width="15%">
                <col width="2%">
                <col width="15%">
                <tr>
                    <td align="left" style="white-space:nowrap;padding-right:1px;">Руководитель организации</td>
                    <td class="underline"></td>
                    <td></td>
                    <td class="underline"></td>
                    <td></td>                    
                    <td align="right" style="white-space:nowrap;padding-right:1px;">Главный бухгалтер</td>
                    <td class="underline"></td>
                    <td></td> 
                    <td class="underline"></td>                    
                </tr>
                <tr>
                    <td align="left" style="vertical-align: top">или иное уполномоченное лицо</td>
                    <td style="vertical-align: top" align="center">(подпись)</td>
                    <td></td>
                    <td style="vertical-align: top" align="center">(ФИО)</td>
                    <td></td>
                    <td align="right">или иное уполномоченное лицо</td>
                    <td style="vertical-align: top" align="center">(подпись)</td>
                    <td></td>
                    <td style="vertical-align: top" align="center">(ФИО)</td>
                </tr>                
                <tr style="margin-top:10px">
                    <td align="left" style="vertical-align:bottom;white-space:nowrap">Индивидуальный предприниматель</td>
                    <td style="padding-top:20px" class="underline"></td>
                    <td></td>
                    <td style="padding-top:20px" align="left" class="underline"></td>
                    <td></td>
                    <td colspan="4" align="right" style="padding-top:20px;width:1%" class="underline"></td>
                </tr>
                <tr>
                    <td align="right" style="vertical-align: top"></td>
                    <td style="vertical-align: top" align="center">(подпись)</td>
                    <td></td>
                    <td style="vertical-align: top" align="center">(ФИО)</td>
                    <td></td>
                    <td align="center" colspan="4" style="width:1%;">(реквизиты&nbsp;свидетельства&nbsp;о&nbsp;государственной регистрации&nbsp;индивидуального&nbsp;предпринимателя)</td>
                </tr>
            </table>
            <br />
            <br />
            Примечание 1. Первый экземпляр счета-фактуры, составленного на бумажном носителе - покупателю, второй экземпляр - продавцу.<br />
            2. При составлении организацией счета-фактуры в электронном виде показатель "Главный бухгалтер (подпись)(ФИО)" не формируется. </div>            
    </div>
    <div id="mainContentPrintingForm" style="width: 1070px;border:none"></div>
</body>
</html>
