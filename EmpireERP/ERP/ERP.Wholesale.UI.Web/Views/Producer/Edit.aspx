<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.Producer.ProducerEditViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%: Model.Title %>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        Producer_Edit.Init();

        function OnBeginProducerSave() {
            StartButtonProgress($("#btnSaveProducer"));
        }

        function OnSuccessProducerSave(ajaxContext) {
            Producer_Edit.OnSuccessProducerSave(ajaxContext);
        }

        function OnFailProducerSave(ajaxContext) {
            Producer_Edit.OnFailProducerSave(ajaxContext);
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<%= Html.PageTitle("Producer", Model.Title, Model.Name, "/Help/GetHelp_Producer_Edit")%>

    <% using (Ajax.BeginForm("Save", "Producer", new AjaxOptions() { OnBegin = "OnBeginProducerSave", OnSuccess = "OnSuccessProducerSave", OnFailure = "OnFailProducerSave" })) %>
    <%{ %>
        <%: Html.HiddenFor(model => model.Id) %>
        <%: Html.HiddenFor(model => model.BackURL) %>
        <%: Html.HiddenFor(model => model.CuratorId) %>

        <%= Html.PageBoxTop(Model.Title)%>
        
        <div style="background: #fff; padding: 10px 0;">

            <div id="messageProducerEdit"></div>

            <table class="editor_table">
                <tr>
                    <td class="row_title" style="min-width: 155px;">
                        <%: Html.HelpLabelFor(model => model.CuratorId, "/Help/GetHelp_Producer_Edit_Curator")%>:
                    </td>
                    <td colspan="3">
                        <%: Model.CuratorName %>
                        <%: Html.ValidationMessageFor(model => model.CuratorId)%>
                    </td>
                </tr>
                <tr>
                    <td class="row_title">
                        <%: Html.HelpLabelFor(model => model.Name, "/Help/GetHelp_Producer_Edit_Name")%>:
                    </td>
                    <td colspan="3">
                        <%: Html.TextBoxFor(model => model.Name, new { style="width: 524px", maxlength="100"})%>
                        <%: Html.ValidationMessageFor(model => model.Name)%>
                    </td>
                </tr>
                <tr>
                    <td class="row_title">
                        <%: Html.HelpLabelFor(model => model.OrganizationName, "/Help/GetHelp_Producer_Edit_OrganizationName")%>:
                    </td>
                    <td colspan="3">
                        <%: Html.TextBoxFor(model => model.OrganizationName, new { style = "width: 524px", maxlength = "100" })%>
                        <%: Html.ValidationMessageFor(model => model.OrganizationName)%>
                    </td>
                </tr>
                <tr>
                    <td class="row_title">
                        <%: Html.HelpLabelFor(model => model.Address, "/Help/GetHelp_Producer_Edit_Address")%>:
                    </td>
                    <td>
                        <%: Html.TextBoxFor(model => model.Address, new { style = "width: 188px", maxlength = "100" })%>
                    </td>                
                    <td class="row_title">
                        <%: Html.HelpLabelFor(model => model.VATNo, "/Help/GetHelp_Producer_Edit_VATNo")%>:
                    </td>
                    <td style="width: 100%">
                        <%: Html.TextBoxFor(model => model.VATNo, new { style = "width: 188px", maxlength = "100" })%>
                        <%: Html.ValidationMessageFor(model => model.VATNo)%>
                    </td>
                </tr>
                <tr>
                    <td class="row_title">
                        <%: Html.HelpLabelFor(model => model.DirectorName, "/Help/GetHelp_Producer_Edit_DirectorName")%>:
                    </td>
                    <td>
                        <%: Html.TextBoxFor(model => model.DirectorName, new { style = "width: 188px", maxlength = "100" })%>
                    </td>
                    <td class="row_title">
                        <%: Html.HelpLabelFor(model => model.ManagerName, "/Help/GetHelp_Producer_Edit_ManagerName")%>:
                    </td>
                    <td style="width: 100%">
                        <%: Html.TextBoxFor(model => model.ManagerName, new { style = "width: 188px", maxlength = "100" })%>
                    </td>
                </tr>
                <tr>
                    <td class="row_title">
                        <%: Html.HelpLabelFor(model => model.Contacts, "/Help/GetHelp_Producer_Edit_Contacts")%>:
                    </td>
                    <td colspan="3">
                        <table>
                            <tr>
                                <td class="row_title"><%: Html.LabelFor(model => model.Email)%>:</td>
                                <td>
                                    <%: Html.TextBoxFor(model => model.Email, new { style = "width: 134px", maxlength = "100" })%>
                                    <%: Html.ValidationMessageFor(model => model.Email)%>
                                </td>
                                <td class="row_title" style="min-width: 118px"><%: Html.LabelFor(model => model.MobilePhone)%>:</td>
                                <td><%: Html.TextBoxFor(model => model.MobilePhone, new { style = "width: 134px", maxlength = "100" })%></td>                                
                            </tr>
                            <tr>
                                <td class="row_title"><%: Html.LabelFor(model => model.Phone)%>:</td>
                                <td><%: Html.TextBoxFor(model => model.Phone, new { style = "width: 134px", maxlength = "100" })%></td>
                                <td class="row_title"><%: Html.LabelFor(model => model.Fax)%>:</td>
                                <td><%: Html.TextBoxFor(model => model.Fax, new { style = "width: 134px", maxlength = "100" })%></td>
                            </tr>
                            <tr>                                
                                <td class="row_title"><%: Html.LabelFor(model => model.Skype)%>:</td>
                                <td><%: Html.TextBoxFor(model => model.Skype, new { style = "width: 134px", maxlength = "100" })%></td>
                                <td class="row_title"><%: Html.LabelFor(model => model.MSN)%>:</td>
                                <td><%: Html.TextBoxFor(model => model.MSN, new { style = "width: 134px", maxlength = "100" })%></td>
                            </tr>                            
                        </table>
                    </td>
                </tr>                
                <tr>
                    <td class="row_title">
                        <%: Html.HelpLabelFor(model => model.IsManufacturer, "/Help/GetHelp_Producer_Edit_IsManufacturer")%>:
                    </td>
                    <td>
                        <%: Html.YesNoToggleFor(model => model.IsManufacturer)%>
                        <%: Html.ValidationMessageFor(model => model.IsManufacturer)%>
                    </td>
                    <td class="row_title">
                        <%:Html.HelpLabelFor(model => model.Rating, "/Help/GetHelp_Producer_Edit_Rating")%>:
                    </td>
                    <td>
                        <%:Html.DropDownListFor(model => model.Rating, Model.RatingList)%>
                        <%:Html.ValidationMessageFor(model => model.Rating)%>
                    </td>
                </tr>
                <tr>
                    <td class="row_title">
                        <%: Html.HelpLabelFor(model => model.Comment, "/Help/GetHelp_Comment")%>:
                    </td>
                    <td colspan="3">
                        <%: Html.CommentFor(model => model.Comment, new { style = "width: 524px" }, rowsCount: 4)%>
                        <%: Html.ValidationMessageFor(model => model.Comment)%>
                    </td>
                </tr>
                
            </table>

            <div class="button_set">
                <input id="btnSaveProducer" type="submit" value="Сохранить" />
                <input type="button" id="btnBack" value="Назад" />
            </div>
        </div>
        <%= Html.PageBoxBottom() %>
    <% } %>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">
</asp:Content>
