<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.PrintingForm.ReceiptWaybill.ReceiptWaybillPrintingFormViewModel>" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Приходная накладная </title>
    <link rel="shortcut icon" href="/Content/Img/favicon.ico" />
    <meta name="SKYPE_TOOLBAR" content="SKYPE_TOOLBAR_PARSER_COMPATIBLE" />
    <link href="/Content/Style/PrintingForm.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <div id="PrintForm" class="floatingDivPortraitDefault">
        <% int i = 1; %>
        <div id="FirstPageHeader">
            <p class="font7pt">
                <%: Html.LabelFor(x => x.OrganizationName) %>:&nbsp;
                <%: Model.OrganizationName %>&nbsp;
                <br />
                <br />
                <%: Html.LabelFor( x=> x.Date) %>:&nbsp;
                <%: Model.Date %>
                <br />
            </p>
            <p style="font-size: 11pt; line-height: 1.7em;">
                <%: Html.LabelFor(x => x.ReceiptStorageName) %>:&nbsp;<%: Model.ReceiptStorageName %>
                <br />
                <% if (Model.IsCreatedFromProductionOrderBatch) {%><%: Html.LabelFor(x => x.IsCreatedFromProductionOrderBatch)%><% } else { %><%: Html.LabelFor(x => x.ContractorName)%><% } %>:&nbsp;<%: Model.ContractorName%>
            </p>
            <p align="center" class="font12pt">
                <b>
                    <%: Model.Title %></b>
            </p>
            <p class="font7pt">
                Операция: приход товара
                <br />
                Сопр док:
                <%: Model.AdditionDocs %>
                <br />
            </p>
        </div>
        <div id="MainTables">
         <p style="font-size: 11pt; line-height: 1.7em;">Совпадающие позиции:</p>
            <table class="MainTable">
                <thead>
                    <tr>
                        <th style="width: 2%;">
                            №
                        </th>
                        <th style="width: 3%;">
                            КОД
                        </th>
                        <th style="width: 13%;">
                            АРТИ&shy;КУЛ
                        </th>
                        <th style="min-width: 20%;">
                            НАИМЕНОВАНИЕ
                        </th>
                        <th style="width: 5%;">
                            Кол-&shy;во
                        </th>
                        <th style="width: 2%;">
                            ЕИ
                        </th>
                         <th style="width: 2%;">
                            Кол-во БЕ в ЕУ
                        </th>
                         <th style="width: 2%;">
                            Кол-во ЕУ
                        </th>
                        <th style="width: 3%;">
                            Вес (кг)                            
                        </th>
                        <th style="width: 3%;">
                            Объем (м3)                            
                        </th>
                        <% if (Model.Settings.PrintPurchaseCost)
                           { %>
                        <th style="width: 5%;">
                            Цена зак.                            
                        </th>
                        <th style="width: 6%;">
                            Сумма зак.                            
                        </th>
                        <% } %>
                        <% if (Model.Settings.PrintAccountingPrice)
                           { %>
                        <th style="width: 5%;">
                            Цена учет.                            
                        </th>
                        <th style="width: 6%;">
                            Сумма учет.                            
                        </th>
                        <% } %>
                        <% if (Model.Settings.PrintMarkup)
                           { %>
                        <th style="width: 5%;">
                            Наценка за ед.                            
                        </th>
                        <th style="width: 6%;">
                            Наценка сумма                            
                        </th>
                        <% } %>
                    </tr>
                </thead>
                <%
                    foreach (var row in Model.MatchRows)
                    {
                %>
                <tr>
                    <td align="right">
                        <% if (!String.IsNullOrEmpty(row.Id))
                           { %>
                        <%: i++ %>
                        <% }  %>
                    </td>
                    <td align="right" class="nobr">
                        <%: row.Id %>
                    </td>
                    <td>
                        <%: row.Number %>
                    </td>
                    <td>
                        <%: row.ArticleName %>
                    </td>
                    <td align="right">
                        <%: row.Count %>
                    </td>
                    <td align="center">
                        <%: row.MeasureUnit %>
                    </td>
                    <td align="right">
                        <%: row.PackSize %>
                    </td>
                    <td align="right">
                        <%: row.PackCount %>
                    </td>
                    <td align="right" class="nobr">
                        <%: row.Weight %>
                    </td>
                    <td align="right" class="nobr">
                        <%: row.Volume %>
                    </td>
                    <% if (Model.Settings.PrintPurchaseCost)
                       { %>
                    <td align="right">
                        <%: row.PurchaseCost %>
                    </td>
                    <td align="right" class="nobr">
                        <%: row.PurchaseSum %>
                    </td>
                    <% } %>
                    <% if (Model.Settings.PrintAccountingPrice)
                       { %>
                    <td align="right">
                        <%: row.AccountingPrice %>
                    </td>
                    <td align="right">
                        <%: row.AccountingPriceSum %>
                    </td>
                    <% } %>
                    <% if (Model.Settings.PrintMarkup)
                       { %>
                    <td align="right">
                        <%: row.MarkupCost %>
                    </td>
                    <td align="right" class="nobr">
                        <%: row.MarkupSum %>
                    </td>
                    <% } %>
                </tr>
                <% } %>
                <tr>
                    <td align="right"></td>
                    <td align="right"></td>
                    <td></td>
                    <td align="right"><b>Количество мест:</b> </td>
                    <td align="right">
                        <%: Model.TotalMatchRowsCountString %>
                    </td>
                    <td align="right"></td>
                    <td align="center"></td>
                    <td align="right"></td>
                    <td align="right"></td>
                    <td align="right"></td>
                    <% if (Model.Settings.PrintPurchaseCost)
                       { %>
                    <td align="right"><b>Итого:</b> </td>
                    <td align="right" class="nobr">
                        <%: Model.TotalPurchaseSumString %>
                    </td>
                    <% } %>
                    <% if (Model.Settings.PrintAccountingPrice)
                       { %>
                    <td align="right"><b>Итого:</b> </td>
                    <td align="right" class="nobr">
                        <%: Model.TotalAccountingPriceSumString %>
                    </td>
                    <% } %>
                    <% if (Model.Settings.PrintMarkup)
                       { %>
                    <td align="right"><b>Итого:</b> </td>
                    <td align="right" class="nobr">
                        <%: Model.TotalMarkupSumString %>
                    </td>
                    <% } %>
                </tr>
            </table>

            <%if (Model.DifRows.Any())
              { %>

                <% i = 1; %>

                <p style="font-size: 11pt; line-height: 1.7em;">Позиции с расхождениями:</p>
                <table class="MainTable">
                    <thead>
                        <tr>
                            <th style="width: 2%;">
                                №
                            </th>
                            <th style="width: 3%;">
                                КОД
                            </th>
                            <th style="width: 13%;">
                                АРТИ&shy;КУЛ                            
                            </th>
                            <th style="min-width: 20%;">
                                НАИМЕНОВАНИЕ                            
                            </th>
                            <th style="width: 5%;">
                                Кол-&shy;во ожид.
                            </th>
                            <th style="width: 5%;">
                                Кол-&shy;во принято
                            </th>
                            <th style="width: 2%;">
                                ЕИ                            
                            </th>
                            <th style="width: 2%;">
                                Кол-во БЕ в ЕУ                            
                            </th>
                            <th style="width: 2%;">
                                Кол-во ЕУ                            
                            </th>
                            <th style="width: 3%;">
                                Вес (кг)                            
                            </th>
                            <th style="width: 3%;">
                                Объем (м3)                            
                            </th>                        
                        </tr>
                    </thead>
                    <%
                   foreach (var row in Model.DifRows)
                   {
                    %>
                    <tr>
                        <td align="right">
                            <% if (!String.IsNullOrEmpty(row.Id))
                               { %>
                            <%: i++%>
                            <% }  %>
                        </td>
                        <td align="right" class="nobr">
                            <%: row.Id%>
                        </td>
                        <td>
                            <%: row.Number%>
                        </td>
                        <td>
                            <%: row.ArticleName%>
                        </td>
                        <td align="right">
                            <%: row.PendingCount%>
                        </td>
                        <td align="right">
                            <%: row.ReceiptedCount%>
                        </td>
                        <td align="center">
                            <%: row.MeasureUnit%>
                        </td>
                        <td align="right">
                            <%: row.PackSize%>
                        </td>
                        <td align="right">
                            <%: row.PackCount%>
                        </td>
                        <td align="right" class="nobr">
                            <%: row.Weight%>
                        </td>
                        <td align="right" class="nobr">
                            <%: row.Volume%>
                        </td>                    
                    </tr>
                    <% } %>
                    <tr>
                        <td align="right"></td>
                        <td align="right"></td>
                        <td></td>
                        <td align="right"><b>Количество мест:</b> </td>
                        <td align="right">
                            <%: Model.TotalDifRowsPendingCountString%>
                        </td>
                        <td align="right">
                            <%: Model.TotalDifRowsReceiptedCountString%>
                        </td>
                        <td align="center"></td>
                        <td align="right"></td>
                        <td align="right"></td>
                        <td align="right"></td>                    
                        <td align="right"></td>
                    </tr>
                </table>

            <% } %>

            <%if (Model.AddedRows.Any())
              { %>

                <% i = 1; %>

                <p style="font-size: 11pt; line-height: 1.7em;">Добавленные при приемке позиции:</p>
                <table class="MainTable">
                    <thead>
                        <tr>
                            <th style="width: 2%;">
                                №
                            </th>
                            <th style="width: 3%;">
                                КОД
                            </th>
                            <th style="width: 13%;">
                                АРТИ&shy;КУЛ                            
                            </th>
                            <th style="min-width: 20%;">
                                НАИМЕНОВАНИЕ                            
                            </th>
                            <th style="width: 5%;">
                                Кол-&shy;во принято                            
                            </th>
                            <th style="width: 2%;">
                                ЕИ                            
                            </th>
                            <th style="width: 2%;">
                                Кол-во БЕ в ЕУ                            
                            </th>
                            <th style="width: 2%;">
                                Кол-во ЕУ                            
                            </th>
                            <th style="width: 3%;">
                                Вес (кг)                            
                            </th>
                            <th style="width: 3%;">
                                Объем (м3)                            
                            </th>                        
                        </tr>
                    </thead>
                    <%
                   foreach (var row in Model.AddedRows)
                   {
                    %>
                    <tr>
                        <td align="right">
                            <% if (!String.IsNullOrEmpty(row.Id))
                               { %>
                            <%: i++%>
                            <% }  %>
                        </td>
                        <td align="right" class="nobr">
                            <%: row.Id%>
                        </td>
                        <td>
                            <%: row.Number%>
                        </td>
                        <td>
                            <%: row.ArticleName%>
                        </td>
                        <td align="right">
                            <%: row.ReceiptedCount%>
                        </td>
                        <td align="center">
                            <%: row.MeasureUnit%>
                        </td>
                        <td align="right">
                            <%: row.PackSize%>
                        </td>
                        <td align="right">
                            <%: row.PackCount%>
                        </td>
                        <td align="right" class="nobr">
                            <%: row.Weight%>
                        </td>
                        <td align="right" class="nobr">
                            <%: row.Volume%>
                        </td>                    
                    </tr>
                    <% } %>
                    <tr>
                        <td align="right"></td>
                        <td align="right"></td>
                        <td></td>
                        <td align="right"><b>Количество мест:</b> </td>
                        <td align="right">
                            <%: Model.TotalAddedRowsCountString%>
                        </td>
                        <td align="right"></td>
                        <td align="center"></td>
                        <td align="right"></td>
                        <td align="right"></td>
                        <td align="right"></td>                    
                    </tr>
                </table>
            <% } %>
        </div>
        <div id="FooterLastPage">
            <br />
            <br />
            Сдал__________________________ Принял__________________________ </div>
    </div>
</body>
</html>
