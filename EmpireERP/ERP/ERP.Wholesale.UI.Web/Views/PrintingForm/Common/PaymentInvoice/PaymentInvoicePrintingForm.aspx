<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.PrintingForm.ExpenditureWaybill.PaymentInvoicePrintingFormViewModel>" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <title>Счет на оплату</title>
    <link rel="shortcut icon" href="/Content/Img/favicon.ico" />
    <meta name="SKYPE_TOOLBAR" content="SKYPE_TOOLBAR_PARSER_COMPATIBLE" />
    <link href="/Content/Style/PrintingForm.css" rel="stylesheet" type="text/css" />   
</head>
<body>
    <div id="PrintForm" class="floatingDivPortraitDefault">
        <div id="HeaderFirstPage" class="font12pt" style="padding:0;margin:0">
        <div id="headerText" class="font7pt" align="center" style="width:80%; margin:0 auto">        
        Внимание! Оплата данного счета означает согласие с условиями поставки товара. Уведомление об оплате 
            обязательно, в противном случае не гарантируется наличие товара на складе. Товар отпускается по факту
                прихода денег на р/с Поставщика, самовывозом, при наличии доверенности и паспорта.
        </div>
        <br /><br />

        <table class="font11pt MainTable">
            <tr>                
                <td style="vertical-align:top;border-bottom:0" rowspan="2" colspan="4">                    
                    <%:Model.SellerBankName %>                   
                </td>
                <td style="width:10%;vertical-align:top;">
                    <%:Html.LabelFor(x => x.SellerBankBIC)%>
                </td>
                <td style="width:34%;vertical-align:top;border-bottom:0px">
                    <%:Model.SellerBankBIC%>
                </td>
            </tr>
            <tr style="height: 12px"> 
                <td style="vertical-align:top;" rowspan="2">
                    <%:Html.LabelFor(x => x.SellerBankAccountNumber)%>
                </td>
                <td style="vertical-align:top;border-top:0px;" rowspan="2">
                    <%:Model.SellerBankAccountNumber%>
                </td>
            </tr>
            <tr>                
                <td class="font11pt" style="border-top:0px" colspan="4">
                    <%:Html.LabelFor(x => x.SellerBankName)%>
                </td>
            </tr>
             <tr>               
                <td style="width:6%">
                    <%:Html.LabelFor(x => x.SellerINN)%>
                </td>
                <td style="width:22%">
                    <%:Model.SellerINN%>
                </td>
                <td style="width:6%">
                    <%:Html.LabelFor(x => x.SellerKPP)%>
                </td>
                <td style="width:22%">
                    <%:Model.SellerKPP%>
                </td>
                <td rowspan="4" style="vertical-align:top">
                    <%:Html.LabelFor(x => x.SellerAccountNumber)%>
                </td>
                <td rowspan="4" style="vertical-align:top">
                    <%:Model.SellerAccountNumber%>
                </td>
            </tr>
            <tr>              
                <td rowspan="2" colspan="4" style="border-bottom:0px;height: 24px;vertical-align:top">
                    <%:Model.SellerName%>
                </td>
            </tr>
            <tr></tr>
            <tr>           
                <td class="font11pt" colspan="4" style="border-top:0px">
                    <%:Html.LabelFor(x => x.SellerName)%>
                </td>
            </tr>           
        </table>
        <p class="font14pt" style="font-weight: bold">
                <%: Model.Title %>&nbsp;
                <%:Html.LabelFor(x => x.Number) %>
                <%:Model.Number %>&nbsp;
                <%:Html.LabelFor(x => x.Date) %>&nbsp;
                <%:Model.Date %>
        </p>

            <div class="hr"></div>

            <table>
                <tr>
                    <td style="width:85px; vertical-align: top;"><%: Html.LabelFor(x => x.SellerInfo)%>:</td>
                    <td class="font12pt"><%: Model.SellerInfo%></td>
                </tr>
                <tr>
                    <td style="width:85px; vertical-align: top;"><%: Html.LabelFor(x => x.BuyerInfo) %>:</td>
                    <td class="font12pt"><%: Model.BuyerInfo %></td>
                </tr>
            </table>

             <div id="MainTable">
            <table class="MainTable" style="width:100%">
                <thead>
                    <tr class="font10pt">
                        <th style="width:6%">
                            <%: Html.LabelFor(x => x.Rows[0].Number) %>
                        </th>
                        <th style="width:46%">
                            <%: Html.LabelFor(x => x.Rows[0].ArticleName) %>
                        </th>
                        <th style="width:10%">
                            <%: Html.LabelFor(x => x.Rows[0].Count) %>
                        </th>
                        <th style="width:7%">
                            <%: Html.LabelFor(x => x.Rows[0].MeasureUnitName) %>
                        </th>
                        <th style="width:15%">
                            <%: Html.LabelFor(x => x.Rows[0].Price) %>
                        </th>
                        <th style="width:16%">
                            <%: Html.LabelFor(x => x.Rows[0].Cost) %>
                        </th>
                    </tr>
                </thead>  
                <tbody>
                <%
                    foreach (var row in Model.Rows)
                    {
                %>
                <tr class="font9pt">
                    <td align="center">
                        <%: row.Number %>
                    </td>
                    <td>
                        <%: row.ArticleName %>
                    </td>
                    <td align="right">
                        <%: row.Count %>
                    </td>
                    <td align="center">
                        <%: row.MeasureUnitName %>
                    </td>
                    <td align="right">
                        <%: row.Price %>
                    </td>                   
                    <td align="right">
                        <%: row.Cost %>
                    </td>                    
                </tr>
                <% } %>                  
                </tbody>                        
            </table>  
            <br />          
            
            <table style="font-weight:bold; width: 100%; text-align: right;">
	            <tr>
    	            <td><%:Html.LabelFor(x => x.Total) %>:</td>
		            <td style="white-space: nowrap; width: 10px; padding-left: 10px;"><%:Model.Total %></td>
	            </tr>
	            <tr>
		            <td><%:Model.TotalValueAddedTax_caption %>:</td>
		            <td style="white-space: nowrap; padding-left: 10px;"><%:Model.TotalValueAddedTax %></td>
	            </tr>
	            <tr>
		            <td><%:Html.LabelFor(x => x.TotalToPay) %>:</td>
		            <td style="white-space: nowrap; padding-left: 10px;"><%:Model.TotalToPay %></td>
	            </tr>             
            </table>

            <%:Html.LabelFor(x => x.RowsCount) %>&nbsp;<%:Model.RowsCount %>,&nbsp;<%:Html.LabelFor(x => x.RowsSum) %>&nbsp;<%:Model.RowsSum %>&nbsp;руб.<br />
            <span style="font-weight:bold"><%:Model.RowsSumInSamples %></span>
            <div class="hr"></div>
        </div>    
          
        <div style="white-space: nowrap;width:100%;float:left;margin:5px 0px;">            
            <%:Html.LabelFor(x => x.DirectorName) %>____________________/<%:Model.DirectorName %>&nbsp;&nbsp;&nbsp;&nbsp;<%:Html.LabelFor(x => x.BookkeeperName) %>____________________/<%:Model.BookkeeperName%>
        </div>
          
    </div>        
    </div>
</body>
</html>
