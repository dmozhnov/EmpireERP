<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.Settings.SettingViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Настройки
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        Setting_List.Init();
    </script>
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <%= Html.PageTitle("Setting", Model.Title, "")%>
    <br />
    <%: Html.HiddenFor(model => model.BackURL) %>

    <% using (Ajax.BeginForm("Save", "Setting", new AjaxOptions() { OnBegin = "Setting_List.OnBeginSettingSave", OnSuccess = "Setting_List.OnSuccessSettingSave", OnFailure = "Setting_List.OnFailSettingSave" })) %>
    <%{ %>
        <div id="messageSettingList"></div>

        <div class="group_title"><%: Model.GroupTitleForReadyToAcceptState %></div>
        <div class="h_delim"></div>

        <table class='editor_table'>
            <tr>
                <td class='row_title' style='min-width:150px;'><%: Html.LabelFor(model => model.UseReadyToAcceptStateForMovementWaybill)%>:</td>
                <td style='width: 50%'><%: Html.YesNoToggleFor(model => model.UseReadyToAcceptStateForMovementWaybill)%></td>
                <td class='row_title'><%: Html.LabelFor(model => model.UseReadyToAcceptStateForExpenditureWaybill)%>:</td>
                <td><%: Html.YesNoToggleFor(model => model.UseReadyToAcceptStateForExpenditureWaybill)%></td>
            </tr>
            <tr>
                <td class='row_title'><%: Html.LabelFor(model => model.UseReadyToAcceptStateForChangeOwnerWaybill)%>:</td>
                <td><%: Html.YesNoToggleFor(model => model.UseReadyToAcceptStateForChangeOwnerWaybill)%></td>
                <td class='row_title' style='min-width:150px;'><%: Html.LabelFor(model => model.UseReadyToAcceptStateForReturnFromClientWaybill)%>:</td>
                <td style='width: 50%'><%: Html.YesNoToggleFor(model => model.UseReadyToAcceptStateForReturnFromClientWaybill)%></td>                
            </tr>
            <tr>
                <td class='row_title'><%: Html.LabelFor(model => model.UseReadyToAcceptStateForWriteOffWaybill)%>:</td>
                <td><%: Html.YesNoToggleFor(model => model.UseReadyToAcceptStateForWriteOffWaybill)%></td>
                <td class='row_title'></td>
                <td></td>
            </tr>
        </table>
        
        <div class="button_set">
            <input id="btnSaveSetting" type="submit" value="Сохранить" />
            <input type="button" id="btnBack" value="Назад" />
        </div>
    <%} %>

</asp:Content>