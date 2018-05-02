<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.PrintingForm.ReceiptWaybill.DivergenceAct.DivergenceActPrintingFormViewModel>" %>

<!DOCTYPE html>
<html>
<head id="Head1" runat="server">
    <title>Акт расхождений</title>
    <link rel="shortcut icon" href="/Content/Img/favicon.ico" />
    <meta name="SKYPE_TOOLBAR" content="SKYPE_TOOLBAR_PARSER_COMPATIBLE" />
    <link href="/Content/Style/PrintingForm.css" rel="stylesheet" type="text/css" />    
</head>
<body>
    <div id="PrintForm" class="floatingDivPortraitDefault">
        <div id="HeaderFirstPage" style="padding:0;margin:0">
            <p align="center" class="font12pt" style="font-weight: bold">
                <%: Model.Title %>&nbsp;
                <%:Html.LabelFor(x => x.ActNumber) %>&nbsp;
                <%:Model.ActNumber %>&nbsp;
                <%:Html.LabelFor(x => x.ActDate) %>&nbsp;
                <%:Model.ActDate %>
            </p>
            <br />
            <div class="font9pt">
            Настоящий акт составлен о том, что при приходовании товара по накладной №<%:Model.WaybillNumber%> от <%:Model.WaybillDate %>г.,
            от <% if (Model.IsCreatedFromProductionOrderBatch) { %>производителя<% } else { %>поставщика<% } %>: <% if (!String.IsNullOrEmpty(Model.ContractorName)) { %><%:Model.ContractorName %><% } else { %>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;---&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<% } %> на склад <%:Model.StorageName %> (<%: Model.OrganizationName %>) обнаружены следующие расхождения между фактической поставкой (хранением) и количеством товара, указанным в накладных:
            </div>
            <br />
        </div>
        <div id="MainTable">
            <p style="font-size: 11pt; line-height: 1.7em;">Расхождения по количеству</p>
            <table class="MainTable" style="width:100%">
                <thead>
                    <tr>
                        <th style="width:4%">
                            <%: Html.LabelFor(x => x.CountDivergenceRows[0].Number) %>
                        </th>
                        <th style="width:60%">
                            <%: Html.LabelFor(x => x.CountDivergenceRows[0].ArticleName) %>
                        </th>
                        <th style="width:9%">                            
                            <%: Html.LabelFor(x => x.CountDivergenceRows[0].PendingCount)%>
                        </th>
                        <th style="width:9%">                            
                            <%: Html.LabelFor(x => x.CountDivergenceRows[0].ProviderCount)%>
                        </th>
                        <th style="width:9%">                            
                            <%: Html.LabelFor(x => x.CountDivergenceRows[0].ReceiptedCount)%>
                        </th>
                        <th style="width:9%">                            
                            <%: Html.LabelFor(x => x.CountDivergenceRows[0].ShortageCount)%>
                        </th>
                        <th style="width:9%">                            
                            <%: Html.LabelFor(x => x.CountDivergenceRows[0].ExcessCount)%>
                        </th>
                    </tr>
                </thead>  
                <tbody>
                <%
                    foreach (var row in Model.CountDivergenceRows)
                    {
                %>
                <tr>                   
                    <td align="center">
                        <%: row.Number %>
                    </td>
                    <td>
                        <%: row.ArticleName %>
                    </td>
                    <td align="right">
                        <%: row.PendingCount %>
                    </td>
                    <td align="right">
                        <%: row.ProviderCount %>
                    </td>
                    <td align="right">
                        <%: row.ReceiptedCount %>
                    </td>
                    <td align="right">
                        <%: row.ShortageCount %>
                    </td>
                    <td align="right">
                        <%: row.ExcessCount %>
                    </td>
                </tr>
                <% } %>  
                </tbody>                        
            </table>

            <p style="font-size: 11pt; line-height: 1.7em;">Расхождения по сумме</p>
            <table class="MainTable" style="width:100%">
                <thead>
                    <tr>
                        <th style="width:4%">
                            <%: Html.LabelFor(x => x.CountDivergenceRows[0].Number) %>
                        </th>
                        <th style="width:60%">
                            <%: Html.LabelFor(x => x.CountDivergenceRows[0].ArticleName) %>
                        </th>
                        <th style="width:9%">
                            <%: Html.LabelFor(x => x.CountDivergenceRows[0].PendingSum)%>
                        </th>
                        <th style="width:9%">
                            <%: Html.LabelFor(x => x.CountDivergenceRows[0].ProviderSum)%>
                        </th>
                    </tr>
                </thead>  
                <tbody>
                <%
                    foreach (var row in Model.SumDivergenceRows)
                    {
                %>
                <tr>
                    <td align="center">
                        <%: row.Number %>
                    </td>
                    <td>
                        <%: row.ArticleName %>
                    </td>
                    <td align="right">
                        <%: row.PendingSum %>
                    </td>
                    <td align="right">
                        <%: row.ProviderSum %>
                    </td>
                </tr>
                <% } %>  
                </tbody>
            </table>
        </div>
        
        <div id="FooterLastPage" class="font12pt">
            <br />
            <br />
            Комментарий:&nbsp;_____________________________________________________________________
            __________________________________________________________________________________
            __________________________________________________________________________________
            __________________________________________________________________________________
            <br />
            <br />
            Акт&nbsp;составил:&nbsp;___________________________________/__________________________________
            <br />
            <br />
            Дата&nbsp;составления:&nbsp;«__»&nbsp;__________&nbsp;20___г.
        </div>
    </div>
</body>
</html>
