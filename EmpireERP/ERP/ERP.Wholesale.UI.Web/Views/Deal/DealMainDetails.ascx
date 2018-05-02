<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.Deal.DealMainDetailsViewModel>" %>

<script type="text/javascript">
    Deal_Details_MainDetails.Init();
</script>

<%: Html.HiddenFor(model => model.StageId) %>
<%: Html.HiddenFor(model => model.ClientContractId) %>

<%: Html.HiddenFor(x => x.AllowToViewCuratorDetails) %>
<%: Html.HiddenFor(x => x.AllowToViewClientDetails) %>
<%: Html.HiddenFor(x => x.AllowToViewClientOrganizationDetails) %>

<table class='main_details_table'>
    <tr>
        <td class="row_title" style='min-width: 150px'>
            <%:Html.LabelFor(model => model.Name) %>:
        </td>
        <td style='width: 65%'>
            <span id="Name"><%:Model.Name %></span>
        </td>
                        
        <td class="row_title">
            <%:Html.LabelFor(model => model.ExpectedBudget) %>:
        </td>
        <td style='width: 35%'>
            <span id="ExpectedBudget"><%:Model.ExpectedBudget %></span>&nbsp;р.
        </td>
    </tr>
    <tr>
        <td class="row_title">
            <%:Html.LabelFor(model => model.StartDate) %>:
        </td>
        <td>
            <span id="StartDate"><%:Model.StartDate%></span>
        </td>
        
        <td class="row_title">
            <%: Html.HelpLabelFor(model => model.SaleSum, "/Help/GetHelp_Deal_Details_MainDetails_SaleSum")%>:
        </td>
        <td>
            <span id="SaleSum"><%:Model.SaleSum %></span>&nbsp;р.
        </td>
    </tr>
    <tr>
        <td class="row_title">
            <%:Html.LabelFor(model => model.CuratorName) %>:
        </td>
        <td>
            <a id="CuratorName"><%:Model.CuratorName %></a>
            <%:Html.HiddenFor(model => model.CuratorId) %>
        </td>        
        
        <td class="row_title">
            <%: Html.HelpLabelFor(model => model.ShippingPendingSaleSum, "/Help/GetHelp_Deal_Details_MainDetails_ShippingPendingSaleSum")%>:
        </td>
        <td>
            <span id="ShippingPendingSaleSum"><%:Model.ShippingPendingSaleSum %></span>&nbsp;р.
        </td>
    </tr>
    <tr>
        <td class="row_title">
            <%:Html.LabelFor(model => model.ClientName) %>:
        </td>
        <td>
            <%: Html.HiddenFor(model => model.ClientId) %>
            <a id="ClientName"><%:Model.ClientName %></a>
        </td>
                        
        <td class="row_title">
            <%: Html.HelpLabelFor(model => model.PaymentSum, "/Help/GetHelp_Deal_Details_MainDetails_PaymentSum")%>:
        </td>
        <td>
            <span id="PaymentSum"><%:Model.PaymentSum %></span>&nbsp;р.
        </td>
    </tr>
    <tr>
        <td class="row_title">
            <%: Html.HelpLabelFor(model => model.StageName, "/Help/GetHelp_Deal_Details_MainDetails_Stage")%>:
        </td>
        <td>
            <span id="StageName"><%:Model.StageName %></span>

            <% string allowToChangeStageDisplay = (Model.AllowToChangeStage ? "inline" : "none"); %>

            <span id='linkChangeStage' class="main_details_action" style="display:<%= allowToChangeStageDisplay %>">[ Изменить ]</span>
        </td>
                        
        <td class='row_title'><%: Html.HelpLabelFor(model => model.InitialBalance, "/Help/GetHelp_Deal_Details_MainDetails_InitialBalance")%>:</td>
        <td>
            <span id="InitialBalance"><%: Model.InitialBalance %></span>&nbsp;р.
        </td>
    </tr>
    <tr>
        <td class="row_title">
            <%: Html.HelpLabelFor(model => model.StageStartDate, "/Help/GetHelp_Deal_Details_MainDetails_StageStartDate")%>:
        </td>
        <td>
            <span id="StageStartDate"><%:Model.StageStartDate %></span>&nbsp;&nbsp;||&nbsp;&nbsp;<span id="StageDuration"><%:Model.StageDuration %></span>&nbsp;дн.
        </td>
                
        <td class="row_title">
            <%: Html.HelpLabelFor(model => model.Balance, "/Help/GetHelp_Deal_Details_MainDetails_Balance")%>:
        </td>
        <td>
            <span id="Balance"><%:Model.Balance%></span>&nbsp;р.
        </td>  
    </tr>
    <tr>
        <td class="row_title">
            <%: Html.HelpLabelFor(model => model.ClientContractName, "/Help/GetHelp_Deal_Details_MainDetails_ClientContract")%>:
        </td>
        <td>
            <span id="ClientContractName"><%:Model.ClientContractName %></span>

            <%  string allowToAddContractDisplay = (Model.AllowToAddContract ? "inline" : "none");
                string allowToChangeContractDisplay = (Model.AllowToChangeContract ? "inline" : "none");
            %>

            <span id='linkAddContract' class="main_details_action" style="display:<%= allowToAddContractDisplay %>">[ Выбрать ]</span>            
            <span id='linkChangeContract' class="main_details_action" style="display:<%= allowToChangeContractDisplay %>">[ Изменить ]</span>            
        </td>
                        
        <td class="row_title">
            <%: Html.HelpLabelFor(model => model.MaxPaymentDelayDuration, "/Help/GetHelp_Deal_Details_MainDetails_MaxPaymentDelayDuration")%>:
        </td>
        <td>
            <span id="MaxPaymentDelayDuration"><%:Model.MaxPaymentDelayDuration %></span>&nbsp;дн. &nbsp;||&nbsp; 
            <span id="PaymentDelaySum"><%:Model.PaymentDelaySum %></span>&nbsp;р.
        </td>        
    </tr>
    <tr>
        <td class="row_title">
            <%:Html.LabelFor(model => model.ClientOrganizationName)%>:
        </td>
        <td>
            <span id="ClientOrganizationLink">
            <%if(Model.ClientOrganizationId != "") { %>
                <%: Html.HiddenFor(model => model.ClientOrganizationId)%>
                <a id="ClientOrganizationName"><%:Model.ClientOrganizationName %></a> 
            <% } else { %> 
                ---
            <% } %>        
            </span>
                &nbsp;&nbsp;||&nbsp;&nbsp;
            <span id="AccountOrganizationLink">
            <%if(Model.AccountOrganizationId != ""){ %>
                <%: Html.HiddenFor(model => model.AccountOrganizationId)%>
                <a id="AccountOrganizationName"><%:Model.AccountOrganizationName %></a>  
            <% } else { %> 
                ---
            <% } %>           
            </span>          
        </td>
        
        <td class='row_title'>
            <%: Html.HelpLabelFor(model => model.TotalReservedByReturnSum, "/Help/GetHelp_Deal_Details_MainDetails_ReturnSum")%>:
        </td>
        <td> 
            <span id="TotalReturnedSum"><%: Model.TotalReturnedSum %></span> р.
            &nbsp;||&nbsp; 
            <span id="TotalReservedByReturnSum"><%: Model.TotalReservedByReturnSum %></span> р.
        </td>
    </tr>
    <tr>
        <td class="row_title">
            <%:Html.LabelFor(model => model.Comment)%>:
        </td>
        <td colspan='3'>
            <%: Html.CommentFor(x => x.Comment, true) %>
        </td>
    </tr>
</table>


