<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.Producer.ProducerMainDetailsViewModel>" %>

<script type="text/javascript">
    Producer_Details_MainDetails.Init();
</script>

<%: Html.HiddenFor(x => x.CuratorId) %>
<%:Html.HiddenFor(x => x.AllowToViewCuratorDetails) %>

<table class="main_details_table">
    <tr>
        <td class='row_title' style='min-width: 130px'>
            <%: Html.LabelFor(model => model.CuratorName) %>:
        </td>
        <td style='width: 60%'>
            <a id="CuratorName"><%: Model.CuratorName%></a>
        </td>
        <td class='row_title'>
            <%: Html.LabelFor(model => model.IsManufacturerName)%>:
        </td>
        <td style='width: 40%'>
            <%: Model.IsManufacturerName%>
        </td>
    </tr>
    <tr>
        <td class='row_title'>
            <%: Html.HelpLabelFor(model => model.OrganizationName, "/Help/GetHelp_Producer_Details_MainDetails_OrganizationName")%>:
        </td>
        <td>
            <span id="OrganizationName"><%: Model.OrganizationName%></span>
        </td>
        <td class='row_title'>
            <%: Html.HelpLabelFor(model => model.OrderSum, "/Help/GetHelp_Producer_Details_MainDetails_OrderSum")%>:
        </td>
        <td>
            <span id="OrderSum"><%: Model.OrderSum%></span> р.
        </td>
    </tr>
    <tr>
        <td class='row_title'>
            <%: Html.LabelFor(model => model.Address) %>:
        </td>
        <td>
            <span id="Address"><%: Model.Address%></span>
        </td>
        <td class='row_title'>
            <%: Html.HelpLabelFor(model => model.OpenOrderSum, "/Help/GetHelp_Producer_Details_MainDetails_OpenOrderSum")%>:
        </td>
        <td>
            <span id="OpenOrderSum"><%: Model.OpenOrderSum%></span> р.
        </td>
    </tr>
    <tr>
        <td class='row_title'>
            <%: Html.LabelFor(model => model.VATNo) %>:
        </td>
        <td>
            <span id="VATNo"><%: Model.VATNo%></span>
        </td>
        <td class='row_title'>
            <%: Html.HelpLabelFor(model => model.ProductionSum, "/Help/GetHelp_Producer_Details_MainDetails_ProductionSum")%>:
        </td>
        <td>
            <span id="ProductionSum"><%: Model.ProductionSum%></span> р.
        </td>
    </tr>
    <tr>
        <td class='row_title'>
            <%: Html.LabelFor(model => model.DirectorName) %>:
        </td>
        <td>
            <span id="DirectorName"><%: Model.DirectorName%></span>
        </td>
        <td class='row_title'>
            <%: Html.HelpLabelFor(model => model.PaymentSum, "/Help/GetHelp_Producer_Details_MainDetails_PaymentSum")%>:
        </td>
        <td>
            <span id="PaymentSum"><%: Model.PaymentSum%></span> р.
        </td>
    </tr>
    <tr>
        <td class='row_title'>
            <%: Html.LabelFor(model => model.ManagerName) %>:
        </td>
        <td>
            <span id="ManagerName"><%: Model.ManagerName%></span>
        </td>
        <td class='row_title'>
            
        </td>
        <td>
            
        </td>
    </tr>
    <tr>
        <td class='row_title'>
            <%: Html.LabelFor(model => model.Contacts) %>:
        </td>        
        <td colspan="3">
            <span class="greytext"><%: Html.LabelFor(model => model.Email)%></span>: <span id="Email"><%: Model.Email%></span>,&nbsp;&nbsp;
            <span class="greytext"><%: Html.LabelFor(model => model.MobilePhone)%></span>: <span id="MobilePhone"><%: Model.MobilePhone%></span>,&nbsp;&nbsp;
            <span class="greytext"><%: Html.LabelFor(model => model.Phone)%></span>: <span id="Phone"><%: Model.Phone%></span>,&nbsp;&nbsp;
            <span class="greytext"><%: Html.LabelFor(model => model.Fax)%></span>: <span id="Fax"><%: Model.Fax%></span>,&nbsp;&nbsp;
            <span class="greytext"><%: Html.LabelFor(model => model.Skype)%></span>: <span id="Skype"><%: Model.Skype%></span>,&nbsp;&nbsp;
            <span class="greytext"><%: Html.LabelFor(model => model.MSN)%></span>: <span id="MSN"><%: Model.MSN%></span>
        </td>
    </tr>
    <tr>
        <td class='row_title'>
            <%:Html.LabelFor(model => model.Comment) %>:
        </td>
        <td colspan="3">
            <%: Html.CommentFor(x => x.Comment, true) %>
        </td>        
    </tr>
</table>