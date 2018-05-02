<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.PrintingForm.AccountingPriceList.AccountingPriceListPrintingFormViewModel>" %>

<!DOCTYPE html>
<html>
<head id="Head1" runat="server">
    <title>Переоценка</title>
    <link rel="shortcut icon" href="/Content/Img/favicon.ico" />
    <meta name="SKYPE_TOOLBAR" content="SKYPE_TOOLBAR_PARSER_COMPATIBLE" />
    <link href="/Content/Style/PrintingForm.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <div id="PrintForm" class="floatingDivPortraitDefault">
        <% int i = 1; %>
        <div id="FirstPageHeader" style="font-family: Times New Roman">
            <p class="font8pt">
                <!--<%: Html.LabelFor(x => x.OrganizationName) %>:&nbsp;<%: Model.OrganizationName %> <br />-->
                
                <%: Html.LabelFor( x=> x.Date) %>:&nbsp;
                <%: Model.Date %>
                <br />
                <%: Html.LabelFor( x=> x.StartDate) %>:&nbsp;
                <%: Model.StartDate %>
                <br />
                <%: Html.LabelFor( x=> x.EndDate) %>:&nbsp;
                <%: Model.EndDate %>
            </p>
            <p style="text-align: center;" class="font12pt">
                <b>
                    <%: Model.Title %></b>
            </p>
            <p class="font8pt">
                Операция: переоценка
                <br />
                <%: Html.LabelFor(x => x.ReasonDescription) %>:&nbsp;
                <%: Model.ReasonDescription%>
                <br />
                <%: Html.LabelFor(x => x.Comment) %>:&nbsp;
                <%: Model.Comment %>
            </p>
        </div>
        <div id="MainTable">
            <% if (!Model.DetailedMode)
               { %>
            <p class="font12pt">
                <b>Места хранения:</b></p>
            <% foreach (var stor in Model.Storages)
               { %>
            <p class="font12pt" style="line-height: 0.4em;">
                <%: stor.StorageName%></p>
            <% } %>
            <table class="MainTable" style="font-size:9pt;">
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
                        <th style="width: 35%;">
                            НАИМЕНОВАНИЕ
                        </th>
                        <th style="width: 5%;">
                            НАЗНАЧЕННАЯ УЧЕТНАЯ ЦЕНА
                        </th>
                    </tr>
                </thead>
                <%
                   foreach (var row in Model.Articles)
                   {
                %>
                <tr>
                    <td align="right">
                        <%: i++ %>
                    </td>
                    <td align="right">
                        <%: row.Id%>
                    </td>
                    <td>
                        <%: row.Number%>
                    </td>
                    <td>
                        <%: row.ArticleName%>
                    </td>
                    <td align="right">
                        <%: row.AccountingPrice%>
                    </td>
                </tr>
                <% } %>
                <tr>
                    <td align="right" colspan="4">
                        Сумма расцененных товаров в новых учетных ценах:
                    </td>
                    <td align="right">
                        <%: Model.NewAccountingPriceSum %>
                    </td>
                </tr>
                <tr>
                    <td align="right" colspan="4">
                        Изменения цен от старого - сумма изменения:
                    </td>
                    <td align="right">
                        <%: Model.AccountingPriceDifSum %>
                    </td>
                </tr>
                <tr>
                    <td align="right" colspan="4">
                        Сумма расцененных товаров в старых учетных ценах:
                    </td>
                    <td align="right">
                        <%: Model.OldAccountingPriceSum %>
                    </td>
                </tr>
                <tr>
                    <td align="right" colspan="4">
                        Сумма расцененных товаров в закупочных ценах:
                    </td>
                    <td align="right">
                        <%: Model.PurchaseCostSum %>
                    </td>
                </tr>
                <tr>
                    <td align="right" colspan="4">
                        Новая наценка от закупки - сумма изменения:
                    </td>
                    <td align="right">
                        <%: Model.PurchaseMarkupSum %>
                    </td>
                </tr>
            </table>
            <% }
               else
               { %>
            <p style="font-size: 10pt">
                Сумма расцененных товаров в новых учетных ценах:
                <%: Model.NewAccountingPriceSum %>
                <br />
                Изменения цен от старого - сумма изменения:
                <%: Model.AccountingPriceDifSum %>
                <br />
                Сумма расцененных товаров в старых учетных ценах:
                <%: Model.OldAccountingPriceSum %>
                <br />
                Сумма расцененных товаров в закупочных ценах:
                <%: Model.PurchaseCostSum %>
                <br />
                Новая наценка от закупки - сумма изменения:
                <%: Model.PurchaseMarkupSum %>
            </p>
            <% foreach (var stor in Model.Storages)
               {
                   i = 1;
                   %>
            <div id="storagePlace">
                <p class="font12pt">
                    <b>
                        <%: stor.StorageName%></b></p>
                <table class="MainTable" style="font-size:9pt;">
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
                            <th style="width: 25%;">
                                НАИМЕНОВАНИЕ
                            </th>
                            <th style="width: 5%;">
                                НАЗНАЧЕННАЯ УЧЕТНАЯ ЦЕНА
                            </th>
                            <th style="width: 5%;">
                                СТАРАЯ ЦЕНА
                            </th>
                            <th style="width: 6%;">
                                РАЗНИЦА
                            </th>
                        </tr>
                    </thead>
                    <%
                   foreach (var row in stor.AccountingPriceListItem)
                   {
                    %>
                    <tr>
                        <td align="right">
                            <%: i++ %>
                        </td>
                        <td align="right">
                            <%: row.AccountingPriceListArticle.Id%>
                        </td>
                        <td>
                            <%: row.AccountingPriceListArticle.Number%>
                        </td>
                        <td>
                            <%: row.AccountingPriceListArticle.ArticleName%>
                        </td>
                        <td align="right">
                            <%: row.AccountingPriceListArticle.AccountingPrice%>
                        </td>
                        <td align="right">
                            <%: row.OldAccountingPrice%>
                        </td>
                        <td align="right">
                            <%: row.DifferenceInAccountingPrice%>
                        </td>
                        <% } %>
                    </tr>
                </table>
                <% } %>
                <% } %>
            </div>
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
