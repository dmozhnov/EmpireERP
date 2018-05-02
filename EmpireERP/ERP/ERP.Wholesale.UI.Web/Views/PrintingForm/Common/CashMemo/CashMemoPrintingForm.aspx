<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.PrintingForm.Common.CashMemoPrintingFormViewModel>" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Товарный чек</title>
    <link rel="shortcut icon" href="/Content/Img/favicon.ico" />
    <meta name="SKYPE_TOOLBAR" content="SKYPE_TOOLBAR_PARSER_COMPATIBLE" />
    <link href="/Content/Style/PrintingForm.css" rel="stylesheet" type="text/css" />    
</head>
<body>
    <div id="PrintForm" class="floatingDivPortraitDefault">        
        <div id="HeaderFirstPage" style="padding:0;margin:0">
            <p align="center" class="font12pt" style="font-weight: bold">
                <%: Model.Title %>&nbsp;
                <%:Html.LabelFor(x => x.Number) %>&nbsp;
                <%:Model.Number %>&nbsp;
                <%:Html.LabelFor(x => x.Date) %>&nbsp;
                <%:Model.Date %>
            </p>

            <p>
                <%: Model.OrganizationName %>&nbsp;
                <br />                

                <%: Model.OGRN_Caption %>:&nbsp;
                <%: Model.OGRN %>
                <br />   
            </p>           
        </div>
        <div id="MainTable">
            <table class="MainTable" style="width:100%;font-size:9pt;">
                <thead>
                    <tr>
                        <th style="width:46%">
                            <%: Html.LabelFor(x => x.Rows[0].ArticleName) %>
                        </th>
                        <th style="width:11%">
                            <%: Html.LabelFor(x => x.Rows[0].PackSize) %>
                        </th>
                        <th style="width:11%">                            
                            <%: Html.LabelFor(x => x.Rows[0].Count)%>
                        </th>
                        <th style="width:14%">                            
                            <%: Html.LabelFor(x => x.Rows[0].Price)%>
                        </th>
                        <th style="width:18%">                            
                            <%: Html.LabelFor(x => x.Rows[0].Sum)%>
                        </th>
                    </tr>
                </thead>  
                <tbody>
                <%
                    foreach (var row in Model.Rows)
                    {
                %>
                <tr>                   
                    <td>
                        <%: row.ArticleName %>
                    </td>
                    <td align="right">
                        <%: row.PackSize %>
                    </td>
                    <td align="right">
                        <%: row.Count %>
                    </td>
                    <td align="right">
                        <%: row.Price %>
                    </td>
                    <td align="right">
                        <%: row.Sum %>
                    </td>                   
                </tr>
                <% } %>  
                </tbody>                        
            </table>
        </div>
        <div style="text-align:right; margin-right: 3px; font-size: 8pt">
            <b><%:Html.LabelFor(x => x.TotalSum) %>:&nbsp; <%: Model.TotalSum %></b>
        </div>
        
        <div id="FooterLastPage" class="font7pt">
            <br />
            <br />
            Товар получил в полной комплектности
            <br />
            без внешних повреждений ___________________________/___________________________
            <br />
            <br />
            Покупатель информирован:<br />
            &ndash; об основных потребительских свойствах приобретаемого товара;<br />
            &ndash; о правилах и условиях эффективного и безопасного использоваия товара;<br />
            &ndash; о сроке службы (сроке годности) товара;<br />
            &ndash; о предприятиях сервисного обслуживания, уполномоченных на проведение гарантийного ремонта;<br />
            &ndash; об устранении недостатков в течении 45-ти календарных дней со дня обращения;<br />
            <br />
            Покупатель:___________________________________/_______________________________<br />
            <br />
            Покупателем товар получен в полной комплектации, с приложением гарантийного талона и технической документации. Претензий к внешнему виду товара не имею. С условиями продажи и сроком устранения недостатков согласен. <br />
            <br />
            «__»&nbsp; __________ &nbsp; 20___г.&nbsp;&nbsp;&nbsp;&nbsp; Подпись ___________________________________
        </div>
    </div>
</body>
</html>
