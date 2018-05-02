<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.PrintingForm.ChangeOwnerWaybill.ChangeOwnerWaybillPrintingFormViewModel>" %>

<!DOCTYPE html>
<html>
<head id="Head1" runat="server">
    <title>Смена собственника</title>
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
                <%:Html.LabelFor(x => x.Storage) %>:&nbsp;<%: Model.Storage %>
            </p>
            <p class="font7pt">
                <%: Html.LabelFor(x => x.RecepientStorageOrganization)%>:&nbsp;
                <%: Model.RecepientStorageOrganization%>
                <br />
                <br />
                <%: Html.LabelFor( x=> x.Date) %>:&nbsp;
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
            <table class="MainTable" style="font-size:9pt;">
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
                        
                        <th style="width: 5%;">
                            Уч. цена 
                            <!--<%:  Html.LabelFor(x => x.Rows[0].Price) %>-->
                        </th>
                        <th style="width: 6%;">
                            Сумма в уч. ценах
                            <!--<%: Html.LabelFor(x => x.Rows[0].PriceSum)%>-->
                        </th>
                        
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
                   
                    <td align="right">
                        <%: row.Price %>
                    </td>
                    <td align="right" class="nobr">
                        <%: row.PriceSum %>
                    </td>
                    <% if (Model.Settings.DevideByBatch)
                       { %>
                    <% if (Model.Settings.PrintPurchaseCost)
                       { %>
                    <td align="right">
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
                    <td align="right">
                        <%: Model.TotalCountString %>
                    </td>
                    <td align="right"></td>
                    <td align="center"></td>
                    <td align="right"></td>
                    <td align="right"></td>
                    <td align="right"></td>

                    <td align="right"><b>Итого:</b> </td>
                    <td align="right" class="nobr">
                        <%: Model.TotalPriceSumString %>
                    </td>
                    
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
