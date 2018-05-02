<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.ProductionOrder.ProductionOrderEditViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%: Model.Title %>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        ProductionOrder_Edit.Init();

        function OnProducerSelectLinkClick(producerId, producerName) {
            ProductionOrder_Edit.OnProducerSelectLinkClick(producerId, producerName);
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%= Html.PageTitle("ProductionOrder", Model.Title, Model.Name, "/Help/GetHelp_ProductionOrder_Edit")%>

    <% using (Ajax.BeginForm("Save", "ProductionOrder", new AjaxOptions() { OnBegin = "ProductionOrder_Edit.OnBeginProductionOrderSave", OnSuccess = "ProductionOrder_Edit.OnSuccessProductionOrderEdit", OnFailure = "ProductionOrder_Edit.OnFailProductionOrderEdit" })) %>
    <%{ %>
        <%: Html.HiddenFor(model => model.Id) %>
        <%: Html.HiddenFor(model => model.BackUrl) %>
        <%: Html.HiddenFor(model => model.CuratorId) %>
        <%: Html.HiddenFor(model => model.ProducerId) %>
        <%: Html.HiddenFor(model => model.AllowToChangeProducer) %>

        <%= Html.PageBoxTop(Model.Title)%>
        <div style="background: #fff; padding: 5px 0;">

            <div id="messageProductionOrderEdit"></div>

            <table class="editor_table">
                <tr>
                    <td colspan="4">
                        <div class="group_title">Общая информация о заказе</div>
                        <div class="h_delim"></div>
                    </td>
                </tr>
                <tr>
                    <td class="row_title" style="min-width: 110px;">
                        <%: Html.HelpLabelFor(model => model.Name, "/Help/GetHelp_ProductionOrder_Edit_Name")%>:
                    </td>
                    <td style="width: 50%">
                        <%: Html.TextBoxFor(model => model.Name, new { style="width: 300px", maxlength = 200, size = 40 }, !Model.AllowToEdit)%>
                        <%: Html.ValidationMessageFor(model => model.Name) %>
                    </td>
                    <td class="row_title">
                        <%: Html.HelpLabelFor(model => model.CuratorId, "/Help/GetHelp_ProductionOrder_Edit_Curator")%>:
                    </td>
                    <td style="width: 50%">
                        <%: Model.CuratorName %>
                    </td>
                </tr>
                <tr>                    
                    <td class="row_title">
                        <%: Html.HelpLabelFor(model => model.ProducerId, "/Help/GetHelp_ProductionOrder_Edit_Producer")%>:
                    </td>
                    <td style="width: 50%">
                        <% if (Model.AllowToChangeProducer && Model.AllowToEdit)
                           { %><span id="ProducerName" class="select_link"><% } %>
                        <%: Model.ProducerName %>
                        <% if (Model.AllowToChangeProducer && Model.AllowToEdit)
                           { %></span><% } %>
                        <%: Html.ValidationMessageFor(model => model.ProducerId)%>
                    </td>
                    <td class="row_title">
                        <%: Html.HelpLabelFor(model => model.Date, "/Help/GetHelp_ProductionOrder_Edit_Date")%>:
                    </td>
                    <td>
                        <%= Html.DatePickerFor(model => model.Date, isDisabled: true)%>
                        <%: Html.ValidationMessageFor(model => model.Date)%>
                    </td>
                </tr>
                <tr>
                <% if (Model.ShowCurrentStageName) { %>
                    <td class="row_title">
                        <%: Html.HelpLabelFor(model => model.CurrentStageName, "/Help/GetHelp_ProductionOrder_Edit_CurrentStageName")%>:
                    </td>
                    <td>
                        <%: Model.CurrentStageName %>
                    </td>
                <% } else { %>
                    <td class="row_title">
                    </td>
                    <td>
                    </td>
                <% } %>

                    <td class="row_title">
                        <%:Html.HelpLabelFor(model => model.CurrencyId, "/Help/GetHelp_ProductionOrder_Edit_Currency")%>:
                    </td>
                    <td>
                        <%: Html.DropDownListFor(model => model.CurrencyId, Model.CurrencyList, !Model.AllowToChangeCurrency ||  !Model.AllowToEdit)%>
                        <%: Html.ValidationMessageFor(model => model.CurrencyId)%>
                    </td>
                </tr>
                <tr>                    
                    <td class="row_title">
                        <%: Html.HelpLabelFor(model => model.StorageId, "/Help/GetHelp_ProductionOrder_Edit_Storage")%>:
                    </td>
                    <td style="width: 50%">
                        <%: Html.DropDownListFor(model => model.StorageId, Model.StorageList, !Model.AllowToChangeStorage || !Model.AllowToEdit)%>
                        <%: Html.ValidationMessageFor(model => model.StorageId)%>
                    </td>
                    <td class="row_title">
                        <%:Html.HelpLabelFor(model => model.ArticleTransportingPrimeCostCalculationType, "/Help/GetHelp_ProductionOrder_Edit_ArticleTransportingPrimeCostCalculationType")%><br />
                        <%:Html.LabelFor(model => model.ArticleTransportingPrimeCostCalculationTypeList)%>:
                    </td>
                    <td>
                        <%: Html.DropDownListFor(model => model.ArticleTransportingPrimeCostCalculationType, Model.ArticleTransportingPrimeCostCalculationTypeList,
                            !Model.AllowToChangeArticleTransportingPrimeCostCalculationType || !Model.AllowToEdit)%>
                        <%: Html.ValidationMessageFor(model => model.ArticleTransportingPrimeCostCalculationType)%>
                    </td>
                </tr>

            <% if (Model.AllowToEditSystemStage && Model.AllowToEdit) { %>

                <tr>
                    <td colspan="4">
                        <br />

                        <div class="group_title">План исполнения этапа «<%: Model.CurrentStageName %>»</div>
                        <div class="h_delim"></div>
                    </td>
                </tr>
                <tr>
                    <td class="row_title">
                        <%: Html.HelpLabelFor(model => model.SystemStagePlannedDuration, "/Help/GetHelp_ProductionOrder_Edit_SystemStagePlannedDuration")%>:
                    </td>
                    <td>
                        <%: Html.TextBoxFor(model => model.SystemStagePlannedDuration) %>&nbsp;&nbsp;дн.
                        <%: Html.ValidationMessageFor(model => model.SystemStagePlannedDuration)%>
                    </td>
                    <td class="row_title">
                        <%: Html.HelpLabelFor(model => model.SystemStagePlannedEndDate, "/Help/GetHelp_ProductionOrder_Edit_SystemStagePlannedEndDate")%>:
                    </td>
                    <td>
                        <%= Html.DatePickerFor(model => model.SystemStagePlannedEndDate) %>
                        <%: Html.ValidationMessageFor(model => model.SystemStagePlannedEndDate)%>
                    </td>
                </tr>
            <% } else { %>
                <%: Html.HiddenFor(model => model.SystemStagePlannedDuration) %>
                <%: Html.HiddenFor(model => model.SystemStagePlannedEndDate) %>
            <% } %>

                <tr>
                    <td colspan="4">
                        <table class="editor_table" style="width: 600px;">
                            <tr>
                                <td style="width: 105px;"></td>
                                <td style="width: 30px;">ПН</td>
                                <td style="width: 30px;">ВТ</td>
                                <td style="width: 30px;">СР</td>
                                <td style="width: 30px;">ЧТ</td>
                                <td style="width: 30px;">ПТ</td>
                                <td style="width: 30px;">СБ</td>
                                <td style="width: 30px;">ВС</td>
                                <td></td>
                            </tr>
                            <tr>
                                <td class="row_title">
                                    <%: Html.HelpLabelFor(model => model.MondayIsWorkDay, "/Help/GetHelp_ProductionOrder_Edit_MondayIsWorkDay")%>:
                                </td>
                                <td>
                                    <%: Html.CheckBoxFor(model => model.MondayIsWorkDay, !Model.AllowToEditWorkDaysPlan)%>
                                </td>
                                <td>
                                    <%: Html.CheckBoxFor(model => model.TuesdayIsWorkDay, !Model.AllowToEditWorkDaysPlan)%>
                                </td>
                                <td>
                                    <%: Html.CheckBoxFor(model => model.WednesdayIsWorkDay, !Model.AllowToEditWorkDaysPlan)%>
                                </td>
                                <td>
                                    <%: Html.CheckBoxFor(model => model.ThursdayIsWorkDay, !Model.AllowToEditWorkDaysPlan)%>
                                </td>
                                <td>
                                    <%: Html.CheckBoxFor(model => model.FridayIsWorkDay, !Model.AllowToEditWorkDaysPlan)%>
                                </td>
                                <td>
                                    <%: Html.CheckBoxFor(model => model.SaturdayIsWorkDay, !Model.AllowToEditWorkDaysPlan)%>
                                </td>
                                <td>
                                    <%: Html.CheckBoxFor(model => model.SundayIsWorkDay, !Model.AllowToEditWorkDaysPlan)%>
                                </td>
                                <td></td>
                            </tr>
                        </table>
                    </td>
                </tr>

                <tr>
                    <td class="row_title">
                        <%: Html.HelpLabelFor(model => model.Comment, "/Help/GetHelp_Comment")%>:
                    </td>
                    <td colspan="3">                        
                        <%: Html.CommentFor(model => model.Comment, new { style = "width: 98%" }, !Model.AllowToEdit, rowsCount: 4)%>
                        <%: Html.ValidationMessageFor(model => model.Comment)%>                        
                    </td>
                </tr>
            </table>

            <div class="button_set">
                <%= Html.SubmitButton("btnSave", "Сохранить", Model.AllowToEdit, Model.AllowToEdit)%>
                <%= Html.Button("btnBack", "Назад")%>
            </div>
        </div>
        <%= Html.PageBoxBottom() %>
    <% } %>

    <div id="producerSelector"></div>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">
</asp:Content>
