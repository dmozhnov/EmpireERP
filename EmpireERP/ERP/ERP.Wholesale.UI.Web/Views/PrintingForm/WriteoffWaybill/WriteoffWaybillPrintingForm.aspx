<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.PrintingForm.WriteoffWaybill.WriteoffWaybillPrintingFormViewModel>" %>

<!DOCTYPE html>
<html>
<head id="Head1" runat="server">
    <title>Накладная списания</title>
    <link rel="shortcut icon" href="/Content/Img/favicon.ico" />
    <meta name="SKYPE_TOOLBAR" content="SKYPE_TOOLBAR_PARSER_COMPATIBLE" />
    <link href="/Content/Style/PrintingForm.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <div id="PrintForm" class="floatingDivPortraitDefault">
        <% int i = 1; %>
        <div id="FirstPageHeader" style="font-family: Times New Roman">
            <p class="font7pt">
                <%: Html.LabelFor(x => x.OrganizationName) %>:&nbsp;<%: Model.OrganizationName %>
                <br />
                <%: Html.LabelFor( x=> x.Date) %>:&nbsp;
                <%: Model.Date %>
            </p>
            <p style="text-align: center;" class="font12pt">
                <b>
                    <%: Model.Title %></b>
            </p>
            <p class="font12pt">
                <%:Html.LabelFor(x => x.SenderStorageName) %>:&nbsp;<%: Model.SenderStorageName %>
            </p>
            <p class="font7pt">
                Операция: списание товара
                <br />
            </p>
        </div>
        <div id="MainTable">
            <table class="MainTable">
                <thead>
                    <tr>
                        <th style="width: 1%;">
                            №
                        </th>
                        <th style="width: 3%;">
                            КОД
                        </th>
                        <th style="width: 7%;">
                            АРТИ&shy;КУЛ
                        </th>
                        <th style="min-width: 12%;">
                            НАИМЕНОВАНИЕ
                        </th>
                        <th style="width: 4%;">
                            Кол-&shy;во
                        </th>
                        <th style="width: 2%;">
                            Кол-во БЕ в ЕУ
                        </th>
                        <th style="width: 2%;">
                            Кол-во ЕУ
                        </th>
                        <th style="width: 2%;">
                            Вес(кг)
                        </th>
                        <th style="width: 2%;">
                            Объем(м3)
                        </th>
                        <th style="width: 8%;">
                            Партия товара
                        </th>
                        <% if (Model.Settings.PrintAccountingPrice)
                           { %>
                        <th style="width: 5%;">
                            Учетная цена
                        </th>
                        <th style="width: 6%;">
                            Учетная сумма
                        </th>
                        <% } %>
                        <% if (Model.Settings.PrintPurchaseCost)
                           { %>
                        <th style="width: 5%;">
                            Зак. цена
                        </th>
                        <th style="width: 6%;">
                            Зак. сумма
                        </th>
                        <% } %>
                    </tr>
                </thead>
                <%
                    foreach (var row in Model.Rows)
                    {
                %>
                <tr>
                    <td align="right">
                        <%: i %>
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
                    <td align="right">
                        <%: row.PackSize %>
                    </td>
                    <td align="right">
                        <%: row.PackCount %>
                    </td>
                    <td align="right">
                        <%: row.Weight %>
                    </td>
                    <td align="right">
                        <%: row.Volume %>
                    </td>
                    <td>
                        <%: row.BatchName %>
                    </td>
                    <% if (Model.Settings.PrintAccountingPrice)
                       { %>
                    <td align="right">
                        <%: row.AccountingPrice %>
                    </td>
                    <td align="right" class="nobr">
                        <%: row.AccountingPriceSum %>
                    </td>
                    <% } %>
                    <% if (Model.Settings.PrintPurchaseCost)
                       { %>
                    <td align="right">
                        <%: row.PurchaseCost %>
                    </td>
                    <td align="right" class="nobr">
                        <%: row.PurchaseSum %>
                    </td>
                    <% } %>
                </tr>
                <% } %>
                <% if (Model.Settings.PrintAccountingPrice || Model.Settings.PrintPurchaseCost)
                   { %>
                    <tr>
                        <td align="right" colspan="10">
                        </td>
                        <% if (Model.Settings.PrintAccountingPrice)
                           { %>
                        <td align="right">
                            <b>Итого:</b>
                        </td>
                        <td align="right" class="nobr">
                            <b>
                                <%:Model.TotalAccountingPriceSum%></b>
                        </td>
                        <% } %>
                        <% if (Model.Settings.PrintPurchaseCost)
                           { %>
                        <td align="right">
                            <b>Итого:</b>
                        </td>
                        <td align="right" class="nobr">
                            <b>
                                <%:Model.TotalPurchaseSum%></b>
                        </td>
                        <% } %>
                    </tr>
                <%} %>
            </table>
        </div>
        <br />
        <br />
        <div id="FooterLastPage" style="page-break-inside: avoid;">
            Разрешил&nbsp;&nbsp;______________________
            <br />
            <br />
            Сдал&nbsp;&nbsp;&nbsp;&nbsp;_______________________
        </div>
    </div>
</body>
</html>
