<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.Client.ClientMainDetailsViewModel>" %>

<table class='main_details_table'>
    <tr>
        <td class='row_title' style='min-width: 180px;'><%:Html.LabelFor(model => model.Name)%>:</td>
        <td style='width: 60%;'>
            <span id="Name"><%:Model.Name%></span>
        </td>

        <td class='row_title'><%: Html.HelpLabelFor(model => model.SaleSum, "/Help/GetHelp_Client_Details_MainDetails_SaleSum")%>:</td>
        <td style='width: 40%; min-width: 120px;'>
            <span id='TotalSales'><%:Model.SaleSum%></span> р.
        </td>
    </tr>
    <tr>
        <td class='row_title'><%: Html.LabelFor(model=>model.FactualAddress) %>:</td>
        <td><%: Model.FactualAddress %></td>
            
        <td class='row_title'><%: Html.HelpLabelFor(model => model.ShippingPendingSaleSum, "/Help/GetHelp_Client_Details_MainDetails_ShippingPendingSaleSum")%>:</td>
        <td>
            <span id='ExpectedShipment'><%:Model.ShippingPendingSaleSum%></span> р.
        </td>
    </tr>
    <tr>
        <td class='row_title'><%: Html.LabelFor(model=>model.ContactPhone) %>:</td>
        <td><%: Model.ContactPhone%></td>
            
        <td class='row_title'><%: Html.HelpLabelFor(model => model.PaymentSum, "/Help/GetHelp_Client_Details_MainDetails_PaymentSum")%>:</td>
        <td>
            <span id='PaymentSum'><%:Model.PaymentSum%></span> р.            
        </td>
    </tr>
    <tr>
        <td class='row_title'><%:Html.LabelFor(model => model.TypeName)%>:</td>
        <td><%:Model.TypeName%></td>
            
        <td class='row_title'><%: Html.HelpLabelFor(model => model.InitialBalance, "/Help/GetHelp_Client_Details_MainDetails_InitialBalance")%>:</td>
        <td>
            <span id="InitialBalance"><%:Model.InitialBalance%></span> р.
        </td>
    </tr>
    <tr>
        <td class='row_title'><%:Html.LabelFor(model => model.LoyaltyName)%>:</td>
        <td><%:Model.LoyaltyName%></td>
            
        <td class='row_title'><%: Html.HelpLabelFor(model => model.Balance, "/Help/GetHelp_Client_Details_MainDetails_Balance")%>:</td>
        <td>
            <span id='Balance'><%:Model.Balance%></span> р.
        </td>        
    </tr>
    <tr>
        <td class='row_title'><%:Html.LabelFor(model => model.RegionName)%>:</td>
        <td><%:Model.RegionName%></td>
        
        <td class='row_title'>
            <%: Html.HelpLabelFor(model => model.TotalReservedByReturnSum, "/Help/GetHelp_Client_Details_MainDetails_ReturnSum")%>:
        </td>
        <td> 
            <span id="TotalReturnedSum"><%: Model.TotalReturnedSum %></span> р. 
            &nbsp;||&nbsp;
            <span id="TotalReservedByReturnSum"><%: Model.TotalReservedByReturnSum %></span> р.
        </td>
    </tr>
    <tr> 
        <td class='row_title'><%:Html.LabelFor(model => model.ServiceProgramName)%>:</td>
        <td><%:Model.ServiceProgramName%></td>

        <td class='row_title'><%: Html.HelpLabelFor(model => model.PaymentDelayPeriod, "/Help/GetHelp_Client_Details_MainDetails_PaymentDelay")%>:</td>
        <td>
            <span id='PaymentDelayPeriod'><%:Model.PaymentDelayPeriod%></span> дн. 
            &nbsp;||&nbsp;
            <span id='PaymentDelaySum'><%:Model.PaymentDelaySum%></span> р.
        </td>
    </tr>
    <tr> 
        <td class='row_title'><%:Html.LabelFor(model => model.Rating)%>:</td>
        <td>
            <span><%:Model.Rating%></span>
        </td>
                
        <td class='row_title'><%:Html.LabelFor(model => model.IsBlockedManually)%>:</td>
        <td>
            <%:Html.YesNoToggleFor(model => model.IsBlockedManually, Model.AllowToBlock)%>
        </td>
    </tr>
    <tr>
        <td class='row_title'><%:Html.LabelFor(model => model.Comment)%>:</td>
        <td colspan='3'>
            <%: Html.CommentFor(x => x.Comment, true) %>
        </td>
    </tr>
</table>