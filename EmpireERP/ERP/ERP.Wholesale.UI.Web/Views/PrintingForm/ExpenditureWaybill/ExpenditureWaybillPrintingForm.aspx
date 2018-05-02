<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.PrintingForm.ExpenditureWaybill.ExpenditureWaybillPrintingFormViewModel>" %>

<%@ Import Namespace="ERP.Utils" %>
<!DOCTYPE html>
<html>
<head id="Head1" runat="server">
    <title>Расходная накладная</title>
    <link rel="shortcut icon" href="/Content/Img/favicon.ico" />
    <meta name="SKYPE_TOOLBAR" content="SKYPE_TOOLBAR_PARSER_COMPATIBLE" />
    <link href="/Content/Style/PrintingForm.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <div id="PrintForm" class="floatingDivPortraitDefault">
        <div id="FirstPageHeader" style="font-family: Times New Roman">
            <p class="font9pt">
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
                <%:Html.LabelFor(x => x.RecepientName) %>:&nbsp;<%: Model.RecepientName%>
            </p>
            <p class="font9pt">
                Операция: реализация за безналичный расчет
                <br />
            </p>
        </div>
        <div id="MainTable">
            <table class="MainTable" style="font-size: 10pt;">
                <thead>
                    <tr>
                        <th style="width: 1%; white-space: nowrap;">№</th>
                        <th style="width: 2%; white-space: nowrap;">КОД</th>
                        <th style="width: 2%; white-space: nowrap;">АРТИ&shy;КУЛ</th>
                        <th style="width: 80%; white-space: nowrap;">НАИМЕНОВАНИЕ</th>
                        <th style="min-width: 48px;">Кол-&shy;во</th>
                        <th style="min-width: 48px;">Кол-&shy;во БЕ в ЕУ</th>
                        <th style="min-width: 48px;">Кол-&shy;во ЕУ</th>
                        <th style="min-width: 48px;">Вес(кг)</th>
                        <th style="min-width: 48px;">Объем(м3)</th>
                        <th style="min-width: 55px;">Отпускная цена</th>
                        <th style="min-width: 75px;">Сумма</th>
                    </tr>
                </thead>
                <%
                    var counter = 1;
                    foreach (var row in Model.Rows)
                    {
                %>
                <tr>
                    <td align="right" style="white-space: nowrap;">
                        <%: counter %>
                    </td>
                    <td align="right" style="white-space: nowrap;">
                        <%: row.Id %>
                    </td>
                    <td style="white-space: nowrap;">
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
                    <td align="right">
                        <%: row.SalePrice %>
                    </td>
                    <td align="right">
                        <%: row.SalePriceSum %>
                    </td>
                </tr>
                <% 
                        counter++;
                    } %>
                <tr style="font-weight: bold">
                    <td colspan="10" align="right">Итого: </td>
                    <td>
                        <%: Model.TotalSalePrice.ForDisplay(ValueDisplayType.Money)%>
                    </td>
                </tr>
                <tr style="font-weight: bold">
                    <td colspan="10" align="right">В том числе НДС: </td>
                    <td>
                        <%: Model.ValueAddedTaxSum %></td>
                </tr>
                <tr style="font-weight: bold">
                    <td colspan="10" align="right">Сумма без НДС: </td>
                    <td>
                        <%: Model.TotalSumWithoutValueAddedTax %></td>
                </tr>
                <tr style="font-weight: bold">
                    <td colspan="10" align="right">Итого в ЕИ по всем позициям:</td>
                    <td>
                        <%: Model.TotalCount.ForDisplay() %></td>
                </tr>
            </table>
            Сумма к оплате:
            <%: Model.TotalSalePrice.ForDisplay(ValueDisplayType.Money)%>
        </div>
        <br />
        <br />
        <div id="FooterLastPage" style="page-break-inside: avoid;">
            <table>
                <tr>
                    <td>Разрешил&nbsp;&nbsp;______________________
                        
                    <td></td>
                    <td></td>
                </tr>
                <tr>
                    <td></td>
                    <td style="width:30px;"></td>
                    <td>Товар в полной комплектности и исправности получил.</td>
                </tr>

                <tr>
                    <td>Выдал&nbsp;&nbsp;&nbsp;&nbsp;_______________________ </td>
                    <td>&nbsp;</td>
                    <td>Претензий по качеству и количеству нет&nbsp;____________ </td>
                </tr>
            </table>
        </div>
    </div>
</body>
</html>
