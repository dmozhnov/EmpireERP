<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.PrintingForm.MovementWaybill.MovementWaybillPrintingFormViewModel>" %>

<!DOCTYPE html>
<html>
<head id="Head1" runat="server">
    <title>Внутреннее перемещение</title>
    <link rel="shortcut icon" href="/Content/Img/favicon.ico" />
    <meta name="SKYPE_TOOLBAR" content="SKYPE_TOOLBAR_PARSER_COMPATIBLE" />
    <link href="/Content/Style/PrintingForm.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <div id="PrintForm" class="floatingDivPortraitDefault">
        <% int i = 1; %>
        <div id="FirstPageHeader" style="font-family: Times New Roman">
            <p style="font-size: 11pt; line-height: 1.7em;">
                <%: Html.LabelFor(x => x.OrganizationName) %>:&nbsp;<%: Model.OrganizationName %>,&nbsp;
                <%:Html.LabelFor(x => x.INN) %>:&nbsp;<%: Model.INN %>,&nbsp;
                <%:Html.LabelFor(x => x.KPP) %>:&nbsp;<%: Model.KPP %>,&nbsp;
                <%:Html.LabelFor(x => x.Address) %>:&nbsp;<%: Model.Address %>
                <br />
                <%:Html.LabelFor(x => x.SenderStorage) %>:&nbsp;<%: Model.SenderStorage %>
                <br />
                <%:Html.LabelFor(x => x.RecepientStorage) %>:&nbsp;<%: Model.RecepientStorage%>
            </p>
            <p class="font7pt">
                <%: Html.LabelFor(x => x.RecepientStorageOrganization)%>:&nbsp;
                <%: Model.RecepientStorageOrganization%>
                <br />
                <br />
                <%: Html.LabelFor(x => x.Date) %>:&nbsp;
                <%: Model.Date %>
                <br />
            </p>
            <p style="text-align: center;" class="font12pt">
                <b>
                    <%: Model.Title %></b>
            </p>
            <p class="font7pt">
                Операция: расход при внутреннем перемещении
                <br />
            </p>
        </div>
        <div id="MainTable">
            <table class="MainTable">
                <thead>
                    <tr>
                        <th style="width: 2%;">
                            №
                        </th>
                        <th style="width: 3%;">
                            КОД<!--<%: Html.LabelFor(x => x.Rows[0].Id)%>-->
                        </th>
                        <th style="width: 7%;">
                            АРТИ&shy;КУЛ
                            <!-- АРТИ&shy;КУЛ<%: Html.LabelFor(x => x.Rows[0].Number)%> -->
                        </th>
                        <th style="width: 15%;">
                            НАИМЕНОВАНИЕ
                            <!--НАИМЕ&shy;НОВАНИЕ<%: Html.LabelFor(x => x.Rows[0].ArticleName)%>-->
                        </th>
                        <th style="width: 5%;">
                            Кол-&shy;во
                            <!--<%: Html.LabelFor(x => x.Rows[0].Count)%>-->
                        </th>
                        <th style="width: 2%;">
                            ЕИ
                            <!--<%: Html.LabelFor(x => x.Rows[0].MeasureUnit)%>-->
                        </th>
                        <th style="width: 2%;">
                            Кол-во БЕ в ЕУ
                            <!--<%: Html.LabelFor(x => x.Rows[0].PackSize)%>-->
                        </th>
                        <th style="width: 2%;">
                            Кол-во ЕУ
                            <!--<%: Html.LabelFor(x => x.Rows[0].PackCount)%>-->
                        </th>
                        <th style="width: 3%;">
                            Вес (кг)
                            <!--<%: Html.LabelFor(x => x.Rows[0].Weight)%>-->
                        </th>
                        <th style="width: 3%;">
                            Объем (м3)
                            <!--<%: Html.LabelFor(x => x.Rows[0].Volume)%>-->
                        </th>
                        <% if (Model.Settings.PrintSenderPrice)
                           { %>
                        <th style="width: 5%;">
                            Уч цена отпра&shy;вителя
                            <!--<%:  Html.LabelFor(x => x.Rows[0].SenderPrice) %>-->
                        </th>
                        <th style="width: 6%;">
                            Сумма отправ&shy;ителя
                            <!--<%: Html.LabelFor(x => x.Rows[0].SenderPriceSum)%>-->
                        </th>
                        <% } %>
                        <% if (Model.Settings.PrintRecepientPrice)
                           { %>
                        <th style="width: 5%;">
                            Цена
                            <!--<%: Html.LabelFor(x => x.Rows[0].RecepientPrice)%>-->
                        </th>
                        <th style="width: 6%;">
                            Сумма
                            <!--<%: Html.LabelFor(x => x.Rows[0].RecepientPriceSum)%>-->
                        </th>
                        <% } %>
                        <% if (Model.Settings.PrintMarkup)
                           { %>
                        <th style="width: 6%;">
                            Прибыль
                            <!--<%: Html.LabelFor(x => x.Rows[0].Markup)%>-->
                        </th>
                        <% } %>
                        <% if (Model.Settings.DevideByBatch)
                           { %>
                        <% if (Model.Settings.PrintPurchaseCost)
                           { %>
                        <th style="width: 5%;">
                            Цена зак. по партии
                            <!--<%: Html.LabelFor(x => x.Rows[0].PurchaseCost)%>-->
                        </th>
                        <th style="width: 6%;">
                            Сумма зак. по партии
                            <!--<%: Html.LabelFor(x => x.Rows[0].PurchaseSum)%>-->
                        </th>
                        <% } %>
                        <th style="width: 6%;">
                            Поставщик / Производитель
                        </th>
                        <th style="width: 10%;">
                            Партия
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
                    <td align="right">
                        <%: row.Weight %>
                    </td>
                    <td align="right">
                        <%: row.Volume %>
                    </td>
                    <% if (Model.Settings.PrintSenderPrice)
                       { %>
                    <td align="right">
                        <%: row.SenderPrice %>
                    </td>
                    <td align="right" class="nobr">
                        <%: row.SenderPriceSum %>
                    </td>
                    <% } %>
                    <% if (Model.Settings.PrintRecepientPrice)
                       { %>
                    <td align="right">
                        <%: row.RecepientPrice %>
                    </td>
                    <td align="right" class="nobr">
                        <%: row.RecepientPriceSum %>
                    </td>
                    <% } %>
                    <% if (Model.Settings.PrintMarkup)
                       { %>
                    <td align="right">
                        <%: row.Markup %>
                    </td>
                    <% } %>
                    <% if (Model.Settings.DevideByBatch)
                       { %>
                    <% if (Model.Settings.PrintPurchaseCost)
                       { %>
                    <td align="right" class="nobr">
                        <%: row.PurchaseCost %>
                    </td>
                    <td align="right" class="nobr">
                        <%: row.PurchaseSum %>
                    </td>
                    <% } %>
                    <td>
                        <%: row.ContractorName%>
                    </td>
                    <td>
                        <%: row.BatchName %>
                    </td>
                    <% } %>
                </tr>
                <% } %>
                <tr>
                    <td align="right"></td>
                    <td align="right"></td>
                    <td></td>
                    <td align="right"><b>Количество мест:</b> </td>
                    <td align="right" class="nobr">
                        <%: Model.TotalCountString %>
                    </td>
                    <td align="right"></td>
                    <td align="center"></td>
                    <td align="right"></td>
                    <td align="right"></td>
                    <td align="right"></td>
                    <% if (Model.Settings.PrintSenderPrice)
                       { %>
                    <td align="right"><b>Итого:</b> </td>
                    <td align="right" class="nobr">
                        <%: Model.TotalSenderPriceSumString %>
                    </td>
                    <% } %>
                    <% if (Model.Settings.PrintRecepientPrice)
                       { %>
                    <td align="right"><b>Итого:</b> </td>
                    <td align="right" class="nobr">
                        <%: Model.TotalRecepientPriceSumString %>
                    </td>
                    <% } %>
                    <% if (Model.Settings.PrintMarkup)
                       { %>
                    <td align="right" class="nobr">
                        <%: Model.TotalMarkupString %>
                    </td>
                    <% } %>
                    <% if (Model.Settings.DevideByBatch)
                       { %>
                    <% if (Model.Settings.PrintPurchaseCost)
                       { %>
                    <td align="right"><b>Итого:</b> </td>
                    <td align="right" class="nobr">
                        <%: Model.TotalPurchaseSumString %>
                    </td>
                    <% } %>
                    <td></td>
                    <td></td>
                    <% } %>
                </tr>
            </table>
        </div>
        <br />
        <br />
        <div id="FooterLastPage" style="page-break-inside: avoid;">Сдал &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            ____________________________________________
            <br />
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            должность&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; подпись&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; расшифровка подписи
            <br />
            <br />
            Принял &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp; ____________________________________________
            <br />
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            должность&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; подпись&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; расшифровка подписи </div>
    </div>
</body>
</html>
