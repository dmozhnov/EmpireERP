<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.ReceiptWaybill.ReceiptWaybillEditViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%: Model.Title %>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        ReceiptWaybill_Edit.Init();

        function OnBeginReceiptWaybillSave() {
            StartButtonProgress($("#btnSaveReceiptWaybill"));
        }

        function OnFailReceiptWaybillSave(ajaxContext) {
            ReceiptWaybill_Edit.OnFailReceiptWaybillSave(ajaxContext);
        }

        function OnSuccessReceiptWaybillSave(ajaxContext) {
            ReceiptWaybill_Edit.OnSuccessReceiptWaybillSave(ajaxContext);
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%= Html.PageTitle("ReceiptWaybill", Model.Title, Model.Name, "/Help/GetHelp_ReceiptWaybill_Edit")%>

    <% using (Ajax.BeginForm("Save", "ReceiptWaybill", new AjaxOptions() { OnBegin = "OnBeginReceiptWaybillSave", OnSuccess = "OnSuccessReceiptWaybillSave", OnFailure = "OnFailReceiptWaybillSave" })) %>
    <%{ %>
        <%: Html.HiddenFor(model => model.BackURL) %>
        <%: Html.HiddenFor(model => model.Id) %>
        <%: Html.HiddenFor(model => model.AllowToChangeStorageAndOrganization)%>
        <%: Html.HiddenFor(model => model.IsNew) %>
        <%: Html.HiddenFor(model => model.IsCreatedFromProductionOrderBatch) %>
        <%: Html.HiddenFor(model => model.ProductionOrderBatchId) %>
        <%: Html.HiddenFor(model => model.AllowToViewPurchaseCosts)%>
        <%: Html.HiddenFor(model => model.IsCustomsDeclarationNumberFromReceiptWaybill)%>

        <%= Html.PageBoxTop(Model.Title)%>
        <div style="background: #fff; padding: 5px 0;">
            <div id="messageReceiptWaybillEdit"></div>

            <div class="group_title">Основная информация</div>
            <div class="h_delim"></div>
            <br />

            <table class='editor_table'>
                <tr>
                    <% if (Model.IsCreatedFromProductionOrderBatch) { %>
                        <td class='row_title' style="min-width: 110px">
                            <%: Html.HelpLabelFor(model => model.ProducerName, "/Help/GetHelp_ReceiptWaybill_Edit_ProducerName")%>:
                        </td>
                        <td style="width: 100%">
                            <%: Model.ProducerName %>
                        </td>
                    <% } else { %>
                        <td class='row_title' style="min-width: 110px">
                            <%: Html.HelpLabelFor(model => model.ProviderId, "/Help/GetHelp_ReceiptWaybill_Edit_Provider")%>:
                        </td>
                        <td style="width: 100%">
                            <%: Html.DropDownListFor(model => model.ProviderId, Model.ProviderList, new { style = "min-width:250px" }, !(Model.AllowToEdit && Model.AllowToChangeProvider))%>
                            <%: Html.ValidationMessageFor(model => model.ProviderId)%>
                        </td>         
                    <% } %>
                </tr>                
                <tr>
                    <% if (Model.IsCreatedFromProductionOrderBatch) { %>
                        <td class='row_title'>
                            <%: Html.HelpLabelFor(model => model.ProductionOrderName, "/Help/GetHelp_ReceiptWaybill_Edit_ProductionOrderName")%>:
                        </td>
                        <td>
                            <%: Model.ProductionOrderName%>
                        </td>                    
                    <% } else { %>
                        <td class='row_title'>
                            <%: Html.HelpLabelFor(model => model.ContractId, "/Help/GetHelp_ReceiptWaybill_Edit_Contract")%>:
                        </td>
                        <td>
                            <%:Html.DropDownListFor(model => model.ContractId, Model.ContractList, new { style = "min-width:250px" }, !(Model.AllowToEdit && Model.AllowToChangeStorageAndOrganization))%>
                            <%: Html.ValidationMessageFor(model => model.ContractId)%>
                        </td>                    
                    <% } %>
                </tr>
                <tr>
                    <td class='row_title'>
                        <%: Html.HelpLabelFor(model => model.ReceiptStorageId, "/Help/GetHelp_ReceiptWaybill_Edit_ReceiptStorage")%>:
                    </td>
                    <td>
                        <%: Html.DropDownListFor(model => model.ReceiptStorageId, Model.ReceiptStorageList, new { style = "min-width:250px" }, !(Model.AllowToEdit && Model.AllowToChangeStorageAndOrganization))%>
                        <%: Html.ValidationMessageFor(model => model.ReceiptStorageId)%>
                    </td>
                </tr>
                <tr>
                    <td class='row_title'>
                        <%: Html.HelpLabelFor(model => model.AccountOrganizationId, "/Help/GetHelp_ReceiptWaybill_Edit_AccountOrganization")%>:
                    </td>
                    <td>
                        <%: Html.DropDownListFor(model => model.AccountOrganizationId, Model.AccountOrganizationList, new { style = "min-width:250px" }, !(Model.AllowToEdit && Model.AllowToChangeStorageAndOrganization))%>
                        <%: Html.ValidationMessageFor(model => model.AccountOrganizationId)%>
                    </td>
                </tr>
                <tr>
                    <td class='row_title'>
                        <%: Html.HelpLabelFor(model => model.CuratorId, "/Help/GetHelp_ReceiptWaybill_Edit_Curator")%>:
                    </td>
                    <td>
                        <% if (Model.AllowToChangeCurator)
                           {%>
                            <span class="link" id="CuratorName"><%: Model.CuratorName%></span>
                        <%}
                           else
                           { %>
                           <%: Model.CuratorName%>
                        <% } %>
                        <%: Html.HiddenFor(model => model.CuratorId)%>
                        <%: Html.ValidationMessageFor(model => model.CuratorId)%>
                    </td>
                </tr>
            </table>
        
            <br />
        
            <div class="group_title">Информация о документах</div>
            <div class="h_delim"></div>
            <br />

            <table class='editor_table'>
                <tr>
                    <td class='row_title' style="min-width: 110px">
                        <%: Html.HelpLabelFor(model => model.Number, "/Help/GetHelp_ReceiptWaybill_Edit_Number")%>:
                    </td>
                    <td style="width: 32%; min-width: 420px;">                       
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
                    <% if (!Model.IsCreatedFromProductionOrderBatch) { %>
                        <td class='row_title'>
                            <%: Html.HelpLabelFor(model => model.ProviderNumber, "/Help/GetHelp_ReceiptWaybill_Edit_ProviderNumber")%>:
                        </td>
                        <td style="width: 33%">
                            <%: Html.TextBoxFor(model => model.ProviderNumber, new { style = "width: 90px"}, !Model.AllowToEditProviderDocuments)%>
                            <%: Html.ValidationMessageFor(model => model.ProviderNumber)%>
                        </td>
                        <td class='row_title'>
                            <%: Html.HelpLabelFor(model => model.ProviderInvoiceNumber, "/Help/GetHelp_ReceiptWaybill_Edit_ProviderInvoiceNumber")%>:
                        </td>
                        <td style="width: 35%">
                            <%: Html.TextBoxFor(model => model.ProviderInvoiceNumber, new { style = "width: 90px" }, !Model.AllowToEditProviderDocuments)%>
                            <%: Html.ValidationMessageFor(model => model.ProviderInvoiceNumber)%>
                        </td>
                    <% } else { %>
                        <td style="width: 68%"></td>
                    <% } %>
                </tr>
                <tr>
                    <td class='row_title'>
                        <%: Html.HelpLabelFor(model => model.Date, "/Help/GetHelp_ReceiptWaybill_Edit_Date")%>:
                    </td>
                    <td>
                        <%= Html.DatePickerFor(model => model.Date, isDisabled: !Model.AllowToChangeDate)%>
                        <%: Html.ValidationMessageFor(model => model.Date)%>
                    </td>
                    <% if (!Model.IsCreatedFromProductionOrderBatch) { %>
                        <td class='row_title'>
                            <%: Html.HelpLabelFor(model => model.ProviderDate, "/Help/GetHelp_ReceiptWaybill_Edit_ProviderDate")%>:
                        </td>
                        <td>
                            <%= Html.DatePickerFor(model => model.ProviderDate, null, !Model.AllowToEditProviderDocuments)%>
                            <%: Html.ValidationMessageFor(model => model.ProviderDate)%>
                        </td>
                        <td class='row_title'>
                            <%: Html.HelpLabelFor(model => model.ProviderInvoiceDate, "/Help/GetHelp_ReceiptWaybill_Edit_ProviderInvoiceDate")%>:
                        </td>
                        <td>
                            <%= Html.DatePickerFor(model => model.ProviderInvoiceDate, null, !Model.AllowToEditProviderDocuments)%>
                            <%: Html.ValidationMessageFor(model => model.ProviderInvoiceDate)%>
                        </td>
                    <% } else { %>
                        <td></td>
                    <% } %>
                </tr>
            </table>
        
            <br />
            <div class="group_title">Дополнительная информация</div>
            <div class="h_delim"></div>
            <br />

            <table class='editor_table'>
                <tr>
                    <td class='row_title' style="min-width: 110px">
                        <%: Html.HelpLabelFor(model => model.PendingSum, "/Help/GetHelp_ReceiptWaybill_Edit_PendingSum")%>:                    
                    </td>
                    <td style="width: 33%">
                        <% if (!Model.AllowToViewPurchaseCosts){%> 
                            ---&nbsp;р.
                        <%}else{%>
                            <%: Html.NumericTextBoxFor(model => model.PendingSum, new { style = "width: 80px" }, Model.IsCreatedFromProductionOrderBatch || !(Model.IsPending && Model.AllowToEdit))%>
                            <%: Html.ValidationMessageFor(model => model.PendingSum)%>
                        <% } %>
                    </td>
                    <td class='row_title'>
                        <% if (!Model.IsCreatedFromProductionOrderBatch) { %>
                            <%: Html.HelpLabelFor(model => model.DiscountPercent, "/Help/GetHelp_ReceiptWaybill_Edit_DiscountPercent")%>:
                        <% } %>
                    </td>
                    <td style="width: 33%">
                        <% if (!Model.IsCreatedFromProductionOrderBatch) { %>
                            <% if (!Model.AllowToViewPurchaseCosts){%> 
                                ---&nbsp;
                            <%} else {%>
                                <%: Html.NumericTextBoxFor(model => model.DiscountPercent, new { style = "width: 80px" }, !(Model.IsPending && Model.AllowToEdit))%>
                                <%: Html.ValidationMessageFor(model => model.DiscountPercent)%>
                           <% } %>
                        <% } %>
                    </td>
                    <td class='row_title'>
                        <%: Html.HelpLabelFor(model => model.CustomsDeclarationNumber, "/Help/GetHelp_ReceiptWaybill_Edit_CustomsDeclarationNumber")%>:
                    </td>
                    <td style="width: 33%; min-width:318px">
                        <div id="rbIsCustomsDeclarationNumberFromReceiptWaybill_false_wrapper" style="text-align: left; float: left;">
                            <%: Html.RadioButtonFor(model => model.IsCustomsDeclarationNumberFromReceiptWaybill, 0, new { id = "rbIsCustomsDeclarationNumberFromReceiptWaybill_false" }, !(Model.IsPending && Model.AllowToEdit))%>                            
                            <label for="rbIsCustomsDeclarationNumberFromReceiptWaybill_false"><%: Model.IsCustomsDeclarationNumberFromReceiptWaybill_false%></label>
                        </div>  
                    </td>     
                </tr>
                <tr>
                    <td class='row_title'>
                        <%: Html.HelpLabelFor(model => model.PendingValueAddedTaxId, "/Help/GetHelp_ReceiptWaybill_Edit_PendingValueAddedTax")%>:
                    </td>
                    <td style="width: 10%">
                        <%: Html.DropDownListFor(model => model.PendingValueAddedTaxId, Model.ValueAddedTaxList, !(Model.IsPending && Model.AllowToEdit))%>                   
                        <%: Html.ValidationMessageFor(model => model.PendingValueAddedTaxId)%>
                    </td>
                    <td class='row_title'>
                        <% if (!Model.IsCreatedFromProductionOrderBatch) { %>
                            <%: Html.HelpLabelFor(model => model.DiscountSum, "/Help/GetHelp_ReceiptWaybill_Edit_DiscountSum")%>:
                        <% } %>
                    </td>
                    <td style="width: 10%">
                        <% if (!Model.IsCreatedFromProductionOrderBatch) { %>
                            <% if (!Model.AllowToViewPurchaseCosts){%> 
                                ---&nbsp;р.
                            <%} else {%>
                                <%: Html.NumericTextBoxFor(model => model.DiscountSum, new { style = "width: 80px" }, !(Model.IsPending && Model.AllowToEdit))%>
                                <%: Html.ValidationMessageFor(model => model.DiscountSum)%>
                           <% } %>
                        <% } %>
                    </td>
                    <td>
                    </td>
                    <td id="rbIsCustomsDeclarationNumberFromReceiptWaybill_true_wrapper" style="text-align: left; float: left;">
                        <%: Html.RadioButtonFor(model => model.IsCustomsDeclarationNumberFromReceiptWaybill, 1, new { id = "rbIsCustomsDeclarationNumberFromReceiptWaybill_true" }, !(Model.IsPending && Model.AllowToEdit))%>                            
                        <label for="rbIsCustomsDeclarationNumberFromReceiptWaybill_true"><%: Model.IsCustomsDeclarationNumberFromReceiptWaybill_true%></label>
                        <%: Html.TextBoxFor(model => model.CustomsDeclarationNumber, new { style = "width: 120px", maxlength = "33" }, !(Model.IsPending && Model.AllowToEdit))%>
                        <%: Html.ValidationMessageFor(model => model.CustomsDeclarationNumber)%>
                    </td>
                </tr>
                <tr>
                    <td class="row_title">
                        <%: Html.HelpLabelFor(model => model.Comment, "/Help/GetHelp_Comment")%>:
                    </td>
                    <td colspan="5">                        
                        <%: Html.CommentFor(model => model.Comment, new { style = "width: 98%" }, !Model.AllowToEdit, rowsCount: 4)%>                        
                        <%: Html.ValidationMessageFor(model => model.Comment)%>                        
                    </td>
                </tr>
            </table>
        
            <br />

            <div class="button_set">
                <input id="btnSaveReceiptWaybill" type="submit" value="Сохранить" />
                <input type="button" id="btnBack" value="Назад" />
            </div>

        </div>
        <%= Html.PageBoxBottom() %>
    <% } %>
    <div id="curatorSelector"></div>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">
    
</asp:Content>
