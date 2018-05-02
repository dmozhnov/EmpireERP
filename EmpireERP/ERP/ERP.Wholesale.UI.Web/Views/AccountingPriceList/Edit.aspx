<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.AccountingPriceList.AccountingPriceListEditViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	<%: Model.Title %>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        AccountingPriceList_Edit.Init();

        function OnBeginCreatePriceList() {
            StartButtonProgress($("#btnSaveAccountingPriceList"));
        }

        function OnFailCreatePriceList(ajaxContext) {
            AccountingPriceList_Edit.OnFailCreatePriceList(ajaxContext);
        }

        function OnSuccessCreatePriceList(ajaxContext) {
            AccountingPriceList_Edit.OnSuccessCreatePriceList(ajaxContext);
        }
    </script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <% using (Ajax.BeginForm("Edit", "AccountingPriceList", new AjaxOptions() { OnBegin = "OnBeginCreatePriceList",
           OnFailure = "OnFailCreatePriceList", OnSuccess = "OnSuccessCreatePriceList" }))
    { %>
        <%: Html.HiddenFor(model => model.AdditionalId)%>
        <%: Html.HiddenFor(model => model.AccountingPriceListId) %>
        <%: Html.HiddenFor(model => model.ReasonId) %>
        <%: Html.HiddenFor(model => model.BackURL) %>
        <%: Html.HiddenFor(model => model.AllowToEdit) %>
        <%: Html.HiddenFor(model => model.StorageIDs, new { id = "multipleSelectorStorages_selected_values"}) %>
        <%: Html.HiddenFor(model => model.AllowToEditStorages) %>

        <%= Html.PageTitle("AccountingPriceList", Model.Title, Model.Name, "/Help/GetHelp_AccountingPriceList_Edit")%>
        
        <%= Html.PageBoxTop(Model.Title)%>
        <div style="background: #fff; padding: 5px 0;">

            <div id="messageAccountingPriceListEdit"></div>
                
            <div class="group_title">Информация о реестре цен</div>
            <div class="h_delim"></div>

            <table class='editor_table' style="width: 865px">
                <tr>
                    <td class='row_title' style="width: 130px">
                        <%: Html.HelpLabelFor(model => model.Reason, "/Help/GetHelp_AccountingPriceList_Edit_Reason")%>:
                    </td>
                    <td>
                        <b><%: Model.Reason %></b>
                    </td>
                    <td class='row_title' valign="top" style="padding-top: 10px">
                    <%: Html.HelpLabelFor(model => model.StartDate, "/Help/GetHelp_AccountingPriceList_Edit_StartDate")%>: </td>
                 <td style="width: 300px">
                     <table>
                        <tr>
                            <td valign="top"><%= Html.DatePickerFor(model => model.StartDate, null, !Model.AllowToEdit, !Model.AllowToEdit)%>
                            <%: Html.ValidationMessageFor(model => model.StartDate)%></td>
                            <td valign="top"><%= Html.TimePickerFor(model => model.StartTime, null, !Model.AllowToEdit, !Model.AllowToEdit)%>
                            <%: Html.ValidationMessageFor(model => model.StartTime)%></td>
                        </tr>
                    </table>
                </td>
                </tr>
                <tr>
                    <td class='row_title'>
                        <%: Html.HelpLabelFor(model => model.Number, "/Help/GetHelp_AccountingPriceList_Edit_Number")%>:
                    </td>
                    <td align="left">
                        <%: Html.TextBoxFor(model => model.Number, !Model.AllowToEdit)%>
                        <%: Html.ValidationMessageFor(model => model.Number)%>                    
                        <%: Html.HiddenFor(model => model.NumberIsUnique) %>
                        <%: Html.ValidationMessageFor(model => model.NumberIsUnique) %>                    
                    </td>
                    <td class='row_title' valign="top" style="padding-top: 10px">
                    <%: Html.HelpLabelFor(model => model.EndDate, "/Help/GetHelp_AccountingPriceList_Edit_EndDate")%>: </td>
                    <td align="left">
                    <table>
                        <tr>
                            <td valign="top"><%= Html.DatePickerFor(model => model.EndDate, null, !Model.AllowToEdit, !Model.AllowToEdit)%>
                            <%: Html.ValidationMessageFor(model => model.EndDate)%></td>
                            <td valign="top"><%= Html.TimePickerFor(model => model.EndTime, null, !Model.AllowToEdit, !Model.AllowToEdit)%>
                            <%: Html.ValidationMessageFor(model => model.EndTime)%></td>
                        </tr>
                        
                    </table>
                </td>
                </tr>                
            </table>
            <% if (Model.AllowToEditStorages) { %>
               <br />
                <div style="width: 860px">
                    <%= Html.MultipleSelector("multipleSelectorStorages", Model.Storages, "Список доступных мест хранения", "Выбранные места хранения для распространения", null)%>
                </div>
            <% } %>

            <br />
            <div class="group_title"><%:Html.LabelFor(model => model.AccountingPriceCalcRuleType) %></div>
            <div class="h_delim"></div>
            
            <br />
        
            <div id='messagePriceCalcRule'></div>

            <!--первый блок -->
            <%: Html.RadioButtonFor(model => model.AccountingPriceCalcRuleType, 1, new { id = "rbAccountingPriceCalcRuleType_1", style = "margin-left: 15px" }, !Model.AllowToEdit || !Model.AllowToSetByPurchaseCost)%>
        
            <label for="rbAccountingPriceCalcRuleType_1"><%: Model.AccountingPriceCalcRuleType_caption1%></label>
            <br />
            <br />

            <table>
                <tr>
                    <td>
                        <div style='padding-left: 34px;' id="divPurchaseCostDeterminationRuleType">
                            <fieldset style='height: 140px;'>
                                <legend><%:Html.LabelFor(model => model.PurchaseCostDeterminationRuleType) %></legend>

                                <table class='groupBoxTable'>
                                    <tr>
                                        <td>
                                            <%: Html.RadioButtonFor(model => model.PurchaseCostDeterminationRuleType, 1, new { id = "rbPurchaseCostDeterminationRuleType_1" }, !Model.AllowToEdit)%>
                                            <label for="rbPurchaseCostDeterminationRuleType_1"><%: Model.PurchaseCostDeterminationRuleType_caption1 %></label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <%: Html.RadioButtonFor(model => model.PurchaseCostDeterminationRuleType, 2, new { id = "rbPurchaseCostDeterminationRuleType_2" }, !Model.AllowToEdit)%>
                                            <label for="rbPurchaseCostDeterminationRuleType_2"><%: Model.PurchaseCostDeterminationRuleType_caption2 %></label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <%: Html.RadioButtonFor(model => model.PurchaseCostDeterminationRuleType, 3, new { id = "rbPurchaseCostDeterminationRuleType_3" }, !Model.AllowToEdit)%>
                                            <label for="rbPurchaseCostDeterminationRuleType_3"><%: Model.PurchaseCostDeterminationRuleType_caption3 %></label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <%: Html.RadioButtonFor(model => model.PurchaseCostDeterminationRuleType, 4, new { id = "rbPurchaseCostDeterminationRuleType_4" }, !Model.AllowToEdit)%>
                                            <label for="rbPurchaseCostDeterminationRuleType_4"><%: Model.PurchaseCostDeterminationRuleType_caption4 %></label>
                                        </td>
                                    </tr>
                                </table>
                            </fieldset>
                        </div>
                    </td>
                    <td>
                        <div style='padding-left: 14px;' id="divMarkupPercentDeterminationRuleType">
                            <fieldset style='height: 140px;'>
                                <legend><%:Html.LabelFor(model => model.MarkupPercentDeterminationRuleType) %></legend>

                                <table class='groupBoxTable'>
                                    <tr>
                                        <td colspan="2">
                                            <%: Html.RadioButtonFor(model => model.MarkupPercentDeterminationRuleType, 1, new { id = "rbMarkupPercentDeterminationRuleType_1" }, !Model.AllowToEdit)%>
                                            <label for="rbMarkupPercentDeterminationRuleType_1"><%: Model.MarkupPercentDeterminationRuleType_caption1%></label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2">
                                            <%: Html.RadioButtonFor(model => model.MarkupPercentDeterminationRuleType, 2, new { id = "rbMarkupPercentDeterminationRuleType_2" }, !Model.AllowToEdit)%>
                                            <label for="rbMarkupPercentDeterminationRuleType_2"><%: Model.MarkupPercentDeterminationRuleType_caption2%></label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <%: Html.RadioButtonFor(model => model.MarkupPercentDeterminationRuleType, 3, new { id = "rbMarkupPercentDeterminationRuleType_3" }, !Model.AllowToEdit)%>
                                        </td>
                                        <td>
                                            <%: Html.TextBoxFor(model => model.CustomMarkupValue, new { size = "7" }, !Model.AllowToEdit)%>                                            
                                            <%: Model.MarkupPercentDeterminationRuleType_caption3%>
                                            <%: Html.ValidationMessageFor(x => x.CustomMarkupValue) %>
                                        </td>
                                    </tr>
                                </table>
                            </fieldset>
                        </div>
                    </td>
                </tr>
            </table>
            <br />

            <!--второй блок -->
            <%: Html.RadioButtonFor(model => model.AccountingPriceCalcRuleType, 2, new { id = "rbAccountingPriceCalcRuleType_2", style = "margin-left: 15px" }, !Model.AllowToEdit)%>
            <label for="rbAccountingPriceCalcRuleType_2"><%: Model.AccountingPriceCalcRuleType_caption2%></label>
            <br />
            <br />

            <table>
                <tr>
                    <td>
                        <div style='padding-left: 34px;' id="divAccountingPriceDeterminationRuleType" >
                            <fieldset style='height:140px;'>
                                <legend><%:Html.LabelFor(model => model.AccountingPriceDeterminationRuleType) %></legend>

                                <table class='groupBoxTable'>
                                    <tr>
                                        <td>
                                            <%: Html.RadioButtonFor(model => model.AccountingPriceDeterminationRuleType, 1, new { id = "rbAccountingPriceDeterminationRuleType_1" }, !Model.AllowToEdit)%>
                                            <label for="rbAccountingPriceDeterminationRuleType_1"><%: Model.AccountingPriceDeterminationRuleType_caption1%></label>
                                        </td>
                                        <td>
                                            <%: Html.DropDownListFor(model => model.StorageTypeId1, Model.StorageTypeList, new { id = "listAccountingPriceDeterminationRuleType1" }, !Model.AllowToEdit)%>
                                            <%:Html.ValidationMessageFor(model => model.StorageTypeId1) %>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <%: Html.RadioButtonFor(model => model.AccountingPriceDeterminationRuleType, 2, new { id = "rbAccountingPriceDeterminationRuleType_2" }, !Model.AllowToEdit)%>
                                            <label for="rbAccountingPriceDeterminationRuleType_2"><%: Model.AccountingPriceDeterminationRuleType_caption2%></label>
                                        </td>
                                        <td>
                                            <%: Html.DropDownListFor(model => model.StorageTypeId2, Model.StorageTypeList, new {id ="listAccountingPriceDeterminationRuleType2"}, !Model.AllowToEdit) %>
                                            <%:Html.ValidationMessageFor(model => model.StorageTypeId2) %>                      
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <%: Html.RadioButtonFor(model => model.AccountingPriceDeterminationRuleType, 3, new { id = "rbAccountingPriceDeterminationRuleType_3" }, !Model.AllowToEdit)%>
                                            <label for="rbAccountingPriceDeterminationRuleType_3"><%: Model.AccountingPriceDeterminationRuleType_caption3%></label>            
                                        </td>
                                        <td>
                                            <%: Html.DropDownListFor(model => model.StorageTypeId3, Model.StorageTypeList, new {id ="listAccountingPriceDeterminationRuleType3"}, !Model.AllowToEdit) %>
                                            <%:Html.ValidationMessageFor(model => model.StorageTypeId3) %>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <%: Html.RadioButtonFor(model => model.AccountingPriceDeterminationRuleType, 4, new { id = "rbAccountingPriceDeterminationRuleType_4" }, !Model.AllowToEdit)%>
                                            <label for="rbAccountingPriceDeterminationRuleType_4"><%: Model.AccountingPriceDeterminationRuleType_caption4%></label>
                                        </td>
                                        <td>
                                            <%: Html.DropDownListFor(model => model.StorageId, Model.StorageList, new {id ="listAccountingPriceDeterminationRuleType4"}, !Model.AllowToEdit) %>                                            
                                            <%:Html.ValidationMessageFor(model => model.StorageId) %>                                               
                                        </td>
                                    </tr>
                                </table>
                            </fieldset>
                        </div>
                    </td>
                    <td>
                        <div style='padding-left: 14px; width: 220px;' id="divMarkupValueRuleType">
                            <fieldset style='height:140px;'>
                                <legend><%:Html.LabelFor(model => model.MarkupValueRuleType) %></legend>

                                <table class='groupBoxTable'>
                                    <tr>
                                        <td>
                                            <%: Html.RadioButtonFor(model => model.MarkupValueRuleType, 1, new { id = "rbMarkupValueRuleType_1" }, !Model.AllowToEdit)%>
                                        </td>
                                        <td>
                                            <%: Html.TextBoxFor(model => model.MarkupValuePercent, new { size = "7", maxlength = "7"}, !Model.AllowToEdit)%>                                            
                                            <%: Model.MarkupValueRuleType_caption1%>      
                                            <%:Html.ValidationMessageFor(model => model.MarkupValuePercent) %>                      
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <%: Html.RadioButtonFor(model => model.MarkupValueRuleType, 2, new { id = "rbMarkupValueRuleType_2" }, !Model.AllowToEdit)%>
                                        </td>
                                        <td>
                                            <%: Html.TextBoxFor(model => model.DiscountValuePercent, new { size = "7", maxlength = "7"}, !Model.AllowToEdit)%>                                                                                        
                                            <%: Model.MarkupValueRuleType_caption2%>
                                            <%:Html.ValidationMessageFor(model => model.DiscountValuePercent) %>
                                        </td>
                                    </tr>
                                </table>
                            </fieldset>
                        </div>
                    </td>
                </tr>
            </table>
            <br />
        
            <!--третий блок -->
            <div class="group_title"><%:Html.LabelFor(model => model.LastDigitCalcRuleType)%></div>
            <div class="h_delim"></div>

            <table class='groupBoxTable' style="margin-left: 35px;">
                <tr>
                    <td colspan="2">
                        <%: Html.RadioButtonFor(model => model.LastDigitCalcRuleType, 1, new { id = "rbLastDigitCalcRuleType_1" }, !Model.AllowToEdit)%>
                        <label for="rbLastDigitCalcRuleType_1"><%: Model.LastDigitCalcRuleType_caption1 %></label>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <%: Html.RadioButtonFor(model => model.LastDigitCalcRuleType, 2, new { id = "rbLastDigitCalcRuleType_2" }, !Model.AllowToEdit)%>
                        <label for="rbLastDigitCalcRuleType_2"><%: Model.LastDigitCalcRuleType_caption2 %></label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <%: Html.RadioButtonFor(model => model.LastDigitCalcRuleType, 3, new { id = "rbLastDigitCalcRuleType_3" }, !Model.AllowToEdit)%>
                        <label for="rbLastDigitCalcRuleType_3"><%: Model.LastDigitCalcRuleType_caption3 %></label>
                    </td>
                    <td>
                        <%: Html.DropDownListFor(model => model.StorageTypeId4, Model.StorageList, new { id = "listLastDigitCalcRuleType" }, !Model.AllowToEdit)%>                            
                    </td>
                </tr>
                <tr>
                    <td>
                        <%:Html.RadioButtonFor(model => model.LastDigitCalcRuleType, 4, new { id = "rbLastDigitCalcRuleType_4" }, !Model.AllowToEdit)%>
                        <label for="rbLastDigitCalcRuleType_4"><%: Model.LastDigitCalcRuleType_caption4 %>:</label>
                    </td>
                    <td>
                        <table>
                            <tr>   
                                <td><%: Html.TextBoxFor(model => model.LastDigitCalcRuleNumber, new { size = "1", maxlength = "1" }, !Model.AllowToEdit)%></td>
                                <td><%: Model.LastDigitCalcRuleType_caption5%></td>
                                <td><%: Html.TextBoxFor(model => model.LastDigitCalcRulePenny, new { size = "2", maxlength = "2" }, !Model.AllowToEdit)%></td>                                
                            </tr>
                            <tr>
                                <td colspan="3" style="height:0px">
                                    <span class="field-validation-valid" id="LastDigitCalcRuleError"></span>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>                 
        
            <br />
            
            <div class="button_set">
                <%: Html.SubmitButton("btnSaveAccountingPriceList", "Сохранить", Model.AllowToEdit, Model.AllowToEdit)%>               
                <input id="btnClosePriceList" type="button" value="Назад" />               
            </div>

        </div>
        <%= Html.PageBoxBottom() %>
    <%} %>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">
    
</asp:Content>
