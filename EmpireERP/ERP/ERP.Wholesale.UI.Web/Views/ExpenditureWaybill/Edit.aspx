<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.ExpenditureWaybill.ExpenditureWaybillEditViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%: Model.Title %>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        ExpenditureWaybill_Edit.Init();

        function OnSuccessExpenditureWaybillEdit(ajaxContext) {
            ExpenditureWaybill_Edit.OnSuccessExpenditureWaybillEdit(ajaxContext);
        }

        function OnBeginExpenditureWaybillEdit() {
            StartButtonProgress($("#btnSaveExpenditureWaybill"));
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%= Html.PageTitle("ExpenditureWaybill", Model.Title, Model.Name, "/Help/GetHelp_ExpenditureWaybill_Edit")%>

    <% using (Ajax.BeginForm("Save", "ExpenditureWaybill", new AjaxOptions() { OnBegin = "OnBeginExpenditureWaybillEdit",
           OnSuccess = "OnSuccessExpenditureWaybillEdit", OnFailure = "ExpenditureWaybill_Edit.OnFailExpenditureWaybillEdit" })) %>
    <%{ %>
        <%: Html.HiddenFor(model => model.Id) %>
        <%: Html.HiddenFor(model => model.BackURL) %>               
        <%: Html.HiddenFor(model => model.DealId) %>
        <%: Html.HiddenFor(model => model.DealQuotaId) %>
        <% if (Model.AllowToChangePaymentType) { %>
            <%: Html.HiddenFor(model => model.IsPrepayment, new { disabled = "disabled" })%>
        <% } else { %>
            <%: Html.HiddenFor(model => model.IsPrepayment)%>
        <% } %>
        <%: Html.HiddenFor(model => model.ClientDeliveryAddress) %>
        <%: Html.HiddenFor(model => model.OrganizationDeliveryAddress) %>

        <%= Html.PageBoxTop(Model.Title)%>
        <div style="background: #fff; padding: 5px 0;">

            <div id="messageExpenditureWaybillEdit"></div>

            <table class="editor_table">
                <tr>
                    <td class="row_title" style="min-width: 110px;">
                        <%: Html.HelpLabelFor(model => model.Number, "/Help/GetHelp_ExpenditureWaybill_Edit_Number")%>:
                    </td>
                    <td style="width: 50%; min-width: 420px;">                       
                        <%: Html.HiddenFor(model => model.AllowToGenerateNumber)%>
                        
                        <div id="rbIsAutoNumber_true_wrapper" style="text-align: left; float: left; margin-right: 10px;">
                            <%: Html.RadioButtonFor(model => model.IsAutoNumber, 1, new { id = "rbIsAutoNumber_true" })%>                            
                            <label for="rbIsAutoNumber_true"><%: Model.IsAutoNumber_true%></label>
                        </div>                        
                        <div id="rbIsAutoNumber_false_wrapper" style="text-align: left; float: left; margin-right: 5px; ">
                            <%: Html.RadioButtonFor(model => model.IsAutoNumber, 0, new { id = "rbIsAutoNumber_false" })%>                            
                            <label for="rbIsAutoNumber_false"><%: Model.IsAutoNumber_false%></label>
                        </div>
                        <div class="row_title" style="text-align: left; float: left">
                            <%: Html.TextBoxFor(model => model.Number, new { maxlength = "25", style = "width: 120px" } ) %>
                            <%: Html.ValidationMessageFor(model => model.Number) %>
                        </div>
                    </td>
                    <td class="row_title">
                        <% if(!String.IsNullOrEmpty(Model.ClientOrganizationId)) {%>
                            <%: Html.HelpLabelFor(model => model.ClientOrganizationId, "/Help/GetHelp_ExpenditureWaybill_Edit_ClientOrganization") %>:
                        <%} else {%>
                            <%: Html.HelpLabelFor(model => model.ClientId, "/Help/GetHelp_ExpenditureWaybill_Edit_Client")%>:
                        <%} %>
                    </td>
                    <td style="width: 50%; min-width: 150px;">
                        
                        <% if(!String.IsNullOrEmpty(Model.ClientOrganizationId)) 
                        {%>
                            <%: Model.ClientOrganizationName %>
                            <%: Html.HiddenFor(model => model.ClientOrganizationId) %>
                        <%} 
                          else if(!String.IsNullOrEmpty(Model.ClientId)) 
                        {%>
                            <%: Model.ClientName %>
                            <%: Html.HiddenFor(model => model.ClientId) %> 
                        <%} else 
                        {%>
                            <span class="select_link" id="ClientName"><%: Model.ClientName %></span>
                            <%: Html.HiddenFor(model => model.ClientId) %> 
                            <%: Html.ValidationMessageFor(model => model.ClientId)%>
                        <%} %>
                    </td>
                </tr>
                <tr>                    
                    <td class="row_title">
                        <%: Html.HelpLabelFor(model => model.Date, "/Help/GetHelp_ExpenditureWaybill_Edit_Date")%>:
                    </td>
                    <td>
                        <%= Html.DatePickerFor(model => model.Date, isDisabled: !Model.AllowToChangeDate)%>
                        <%: Html.ValidationMessageFor(model => model.Date)%>
                    </td>
                    <td class="row_title">
                        <%: Html.HelpLabelFor(model => model.DealId, "/Help/GetHelp_ExpenditureWaybill_Edit_Deal")%>:
                    </td>
                    <td>
                        <% if (String.IsNullOrEmpty(Model.DealId))
                           {%>
                            <span class="select_link no_auto_progress" id="DealName"><%: Model.DealName %></span>
                        <%}
                           else
                           { %>
                            <%: Model.DealName %>
                        <%} %>
                        <%: Html.ValidationMessageFor(model => model.DealId)%>
                        <div id="DealContractCashPaymentSumDiv" style="color:#48BB07; text-align: left; font-size: 11px; padding-top: 5px">
                            <%: Html.LabelFor(model => model.DealContractCashPaymentSum)%>
                            <span id="DealContractCashPaymentSum"><%: Html.TextBoxFor(model => model.DealContractCashPaymentSum, true) %></span> руб.
                        </div>
                    </td>
                </tr>
                <tr>
                    <td class="row_title">
                        <%: Html.HelpLabelFor(model => model.SenderStorageId, "/Help/GetHelp_ExpenditureWaybill_Edit_SenderStorage")%>:
                    </td>
                    <td>                        
                        <%: Html.DropDownListFor(model => model.SenderStorageId, Model.StorageList, !Model.AllowToEdit)%>
                        <%: Html.ValidationMessageFor(model => model.SenderStorageId) %>
                    </td>
                    <td class="row_title">
                        <%: Html.HelpLabelFor(model => model.DealQuotaId, "/Help/GetHelp_ExpenditureWaybill_Edit_DealQuota")%>:
                    </td>
                    <td>
                        <span class="select_link no_auto_progress" id="DealQuotaName"><%: Model.DealQuotaName %></span>
                        <%: Html.ValidationMessageFor(model => model.DealQuotaId)%>
                    </td>
                </tr>
                <tr>
                    <td class="row_title">
                        <%: Html.HelpLabelFor(model => model.CustomDeliveryAddress, "/Help/GetHelp_ExpenditureWaybill_Edit_CustomDeliveryAddress")%>:
                    </td>
                    <td valign="top">
                    <div style="vertical-align:top;">
                        <%: Html.DropDownListFor(model => model.DeliveryAddressTypeId, Model.DeliveryAddressTypeList) %>
                        <%: Html.ValidationMessageFor(model => model.DeliveryAddressTypeId) %>
                    </div>
                    <div style="margin-top:5px;" id="divSelectedDeliveryAddress"></div>
                    <div style="margin-top:5px;" id="divCustomDeliveryAddress">
                        <%= Html.TextBoxFor(model => model.CustomDeliveryAddress, new { maxlength="250", size="60" })%>
                        <%= Html.ValidationMessageFor(model => model.CustomDeliveryAddress)%>
                    </div>
                    
                    </td>
                    <td class="row_title">
                        <%: Html.HelpLabelFor(model => model.ValueAddedTaxId, "/Help/GetHelp_ExpenditureWaybill_Edit_ValueAddedTax")%>: </td>
                    <td>
                        <%: Html.DropDownListFor(model => model.ValueAddedTaxId, Model.ValueAddedTaxList)%>
                        <%: Html.ValidationMessageFor(model => model.ValueAddedTaxId)%>
                    </td>
                </tr>
                <tr>    
                    <td class="row_title">
                        <%:Html.HelpLabelFor(model => model.IsPrepayment, "/Help/GetHelp_ExpenditureWaybill_Edit_IsPrepayment")%>:
                    </td>
                    <td>
                        <div class="row_title" style="text-align: left; width: 120px; float: left;">
                            <% if (Model.AllowToChangePaymentType) { %>
                                <%: Html.RadioButtonFor(model => model.IsPrepayment, 1, new { id = "rbIsPrepayment_true" })%>
                            <% } else { %>
                                <%: Html.RadioButtonFor(model => model.IsPrepayment, 1, new { id = "rbIsPrepayment_true", disabled = "disabled" })%>
                            <% } %>
                            <label for="rbIsPrepayment_true"><%: Model.IsPrepayment_true%></label>
                        </div>
                        <div class="row_title" style="text-align: left; width: 190px; float: left;">
                            <% if (Model.AllowToChangePaymentType) { %>
                                <%: Html.RadioButtonFor(model => model.IsPrepayment, 0, new { id = "rbIsPrepayment_false" })%>
                            <% } else { %>
                                <%: Html.RadioButtonFor(model => model.IsPrepayment, 0, new { id = "rbIsPrepayment_false", disabled = "disabled" })%>
                            <% } %>
                            <label for="rbIsPrepayment_false"><%: Model.IsPrepayment_false%></label>
                        </div>
                    </td>
                    <td class='row_title'>
                        <%: Html.HelpLabelFor(model => model.CuratorId, "/Help/GetHelp_ExpenditureWaybill_Edit_Curator")%>:
                    </td>
                    <td>
                        <% if (Model.AllowToChangeCurator)
                           { %>
                            <span id="CuratorName" class="link"><%: Model.CuratorName%></span>
                        <% }
                           else
                           { %>
                            <%: Model.CuratorName%>
                        <% } %>
                        <%: Html.HiddenFor(model => model.CuratorId)%>
                        <%: Html.ValidationMessageFor(model => model.CuratorId)%>
                    </td>
                </tr>
                <tr>    
                    <td class="row_title">
                        <%: Html.HelpLabelFor(model => model.RoundSalePrice, "/Help/GetHelp_ExpenditureWaybill_Edit_RoundSalePrice")%>:
                    </td>
                    <td>
                        <%: Html.YesNoToggleFor(model => model.RoundSalePrice) %>
                    </td>
                    <td class="row_title">
                        <%: Html.HelpLabelFor(model => model.TeamId, "/Help/GetHelp_ExpenditureWaybill_Edit_Team")%>:
                    </td>
                    <td>
                       <%: Html.DropDownListFor(model => model.TeamId, Model.TeamList, !Model.AllowToEditTeam)%>
                       <%: Html.ValidationMessageFor(model=>model.TeamId) %>
                    </td>  
                </tr>
                <tr>
                    <td class="row_title">
                        <%: Html.HelpLabelFor(model => model.Comment, "/Help/GetHelp_Comment")%>:
                    </td>
                    <td colspan="3">
                        <%: Html.CommentFor(model => model.Comment, new { style = "width: 98%" }, rowsCount: 4)%>
                        <%: Html.ValidationMessageFor(model => model.Comment)%>
                    </td>
                </tr>
            </table>

            <div class="button_set">
                <input id="btnSaveExpenditureWaybill" type="submit" value="Сохранить" />
                <input type="button" id="btnBack" value="Назад" />
            </div>
        </div>
        <%= Html.PageBoxBottom() %>
    <% } %>

    <div id="dealQuotaSelector"></div>
    <div id="dealSelector"></div>
    <div id="clientSelector"></div>
    <div id="curatorSelector"></div>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">
</asp:Content>
