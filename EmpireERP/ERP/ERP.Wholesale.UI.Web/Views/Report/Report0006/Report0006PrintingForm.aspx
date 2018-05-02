<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.Report.Report0006.Report0006PrintingFormListViewModel>" %>

<!DOCTYPE html>
<html>
<head id="Head1" runat="server">
    <title>Акт сверки взаиморасчетов</title>
    <link href="/Content/Style/PrintingForm.css" rel="stylesheet" type="text/css" />    
</head>
<body>

<% if (Model.Report0006PrintingFormList.Count == 0) { %>
Данные для печати отсутствуют.
<% } %>

<% bool isFirstDocument = true; %>
<% foreach(var form in Model.Report0006PrintingFormList) { %>

    <% if (!isFirstDocument) { %>
        <div class="font6pt" style="width: 100%; page-break-before: always;">&nbsp;</div>
    <% } %>

    <% isFirstDocument = false; %>

    <div class="floatingDivPortraitDefault">
        <div style="padding:0; margin:0">
            <div align="center" class="font14pt" style="font-weight: bold">
                Акт сверки взаиморасчетов
            </div>
            <div align="center" class="font12pt">
                по состоянию на
                <%:Model.Date%>
            </div>
            <div align="center" class="font12pt">
                между «<%:form.AccountOrganizationName%>» и «<%:Model.ClientOrClientOrganizationName%>»
            </div>
            <div align="center" class="font6pt">&nbsp;</div>
            <div align="center" class="font12pt">
                за период с <%:Model.Settings.StartDate%> по <%:Model.Settings.EndDate%>
            </div>
            <div align="center" class="font6pt">&nbsp;</div>
            <div class="font11pt">
            <div>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Настоящий акт составлен о том, что состояние взаимных расчетов по данным учета «<%:form.AccountOrganizationName%>» следующее:</div>
            </div>
            <div align="center" class="font6pt">&nbsp;</div>
        </div>

        <div>
            <table class="MainTable" style="width:100%; font-size:11pt">
                <thead>
                    <tr>
                        <th style="width:6%; font-size:11pt">
                            №<br />п/п
                        </th>
                        <th style="width:64%; font-size:11pt">
                            Наименование операции
                        </th>
                        <th style="width:15%; font-size:11pt">
                            Дебет
                        </th>
                        <th style="width:15%; font-size:11pt">
                            Кредит
                        </th>
                    </tr>
                </thead>
                <tbody>
                    <% foreach (var balanceItem in form.BalanceDocumentSummary) { %>
                    <tr>
                        <td>
                            <%:balanceItem.Number%>
                        </td>
                        <td <% if (balanceItem.IsHeader) { %>align="right" style="font-weight: bold"<% } %>>
                            <%:balanceItem.Name%>
                        </td>
                        <td align="right" <% if (balanceItem.IsHeader) { %>style="font-weight: bold"<% } %>>
                            <%:balanceItem.Debit%>
                        </td>
                        <td align="right" <% if (balanceItem.IsHeader) { %>style="font-weight: bold"<% } %>>
                            <%:balanceItem.Credit%>
                        </td>
                    </tr>
                    <% } %>
                </tbody>
            </table>
        </div>

        <div align="center" class="font6pt">&nbsp;</div>
        <div class="font11pt">
        <div>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Развернутая информация по документам учета следующая:</div>
        </div>
        <div align="center" class="font6pt">&nbsp;</div>
        
        <div>
            <table class="MainTable" style="width:100%; font-size:11pt">
                <thead>
                    <tr>
                        <th style="width:6%; font-size:11pt">
                            №<br />п/п
                        </th>
                        <th style="width:64%; font-size:11pt">
                            Наименование операции
                        </th>
                        <th style="width:15%; font-size:11pt">
                            Дебет
                        </th>
                        <th style="width:15%; font-size:11pt">
                            Кредит
                        </th>
                    </tr>
                </thead>
                <tbody>
                    <% foreach (var item in form.BalanceDocumentFullInfo) { %>
                    <tr>
                        <td>
                            <%:item.Number%>
                        </td>
                        <td <% if (item.IsHeader) { %>align="right" style="font-weight: bold"<% } %>>
                            <%:item.Name%>
                            <%if (!String.IsNullOrEmpty(item.AdditionalInfo1)) { %>
                                <br /><%:item.AdditionalInfo1%>
                            <% } %>
                            <%if (!String.IsNullOrEmpty(item.AdditionalInfo2)) { %>
                                <br /><%:item.AdditionalInfo2%>
                            <% } %>
                        </td>
                        <td align="right" <% if (item.IsHeader) { %>style="font-weight: bold"<% } %>>
                            <%:item.Debit%>
                        </td>
                        <td align="right" <% if (item.IsHeader) { %>style="font-weight: bold"<% } %>>
                            <%:item.Credit%>
                        </td>
                    </tr>
                    <% } %>
                </tbody>
            </table>
        </div>

        <div style="width:100%; height: 1%;">
            <table style="width:100%;">
            <tr>
                <td width="55%"></td>
                <td width="45%">
                    <div class="font10pt" style="white-space: nowrap; text-align: right;">
                        <div style="display: block; text-align: left;">
                            <div align="center" class="font11pt">&nbsp;</div>
                            <%:form.AccountOrganizationName%><br />
                            <%if (form.IsJuridicalPerson) { %>
                                <%if (!String.IsNullOrEmpty(form.DirectorPost)) { %>
                                    <%:form.DirectorPost%>:<br />
                                <% } else { %>
                                   <br />__________________________________ <br />
                                <% } %>
                                <br />
                                <%if (!String.IsNullOrEmpty(form.DirectorName)) { %>
                                    <%:form.DirectorName%> /&nbsp;__________________________________ <br />
                                <% } else { %>
                                    <br />__________________________________ <br />
                                <% } %>
                                <br />
                                Бухгалтер:<br />
                                <br />
                                <%if (!String.IsNullOrEmpty(form.MainBookkeeperName)) { %>
                                    <%:form.MainBookkeeperName%> /&nbsp;__________________________________ <br />
                                <% } else { %>
                                    __________________________________ <br />
                                <% } %>
                            <% } else { %>
                                <br />
                                <%:form.OwnerName%> /&nbsp;__________________________________ <br />
                            <% } %>
                                <br />
                                <br />
                        </div>
                    </div>
                </td>
            </tr>
            </table>
        </div>
    </div>

<% } %>

</body>
</html>
