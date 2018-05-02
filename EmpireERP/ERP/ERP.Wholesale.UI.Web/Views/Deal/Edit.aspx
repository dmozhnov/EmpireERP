<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.Deal.DealEditViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%:Model.Title %>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        Deal_Edit.Init();

        function OnFailDealEdit(ajaxContext) {
            Deal_Edit.OnFailDealEdit(ajaxContext);
        }

        function OnSuccessDealEdit(ajaxContext) {
            Deal_Edit.OnSuccessDealEdit(ajaxContext);
        }

        function OnBeginDealEdit() {
            StartButtonProgress($("#btnSave"));
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<% using (Ajax.BeginForm("Edit", "Deal", new AjaxOptions() { OnBegin = "OnBeginDealEdit",
       OnFailure = "OnFailDealEdit", OnSuccess = "OnSuccessDealEdit" }))%>
<%{ %>
    <%:Html.HiddenFor(model => model.BackURL) %>
    <%:Html.HiddenFor(model => model.Id) %>
    <%:Html.HiddenFor(model => model.ClientId) %>
    
    <%=Html.PageTitle("Deal", Model.Title, Model.Name, "/Help/GetHelp_Deal_Edit")%>

    <%=Html.PageBoxTop(Model.Title)%>

    <div style="background: #fff; padding: 10px 5px 5px;">
            <div id='messageDealEdit'></div>

            <table class='editor_table'>
                <tr>
                    <td class="row_title" style="min-width: 110px;">
                        <%:Html.HelpLabelFor(model => model.Name, "/Help/GetHelp_Deal_Edit_Name")%>:
                        </td>
                    <td style="width: 50%">
                        <%:Html.TextBoxFor(model => model.Name, new { maxlength = 200, size = 60 })%>
                        <%:Html.ValidationMessageFor(model => model.Name)%>
                    </td>

                    <td class="row_title"><%:Html.HelpLabelFor(model => model.ClientName, "/Help/GetHelp_Deal_Edit_ClientName")%>:</td>
                    <td style="width: 50%">
                        <% if(!String.IsNullOrEmpty(Model.ClientId))
                           {%>
                            <%:Model.ClientName %>
                        <%} 
                          else 
                          { %>
                            <span class="select_link" id="ClientName"><%:Model.ClientName %></span>
                        <%} %>
                        <%: Html.ValidationMessageFor(model => model.ClientId) %>
                    </td>
                </tr>

                <tr>
                    <td class="row_title"><%:Html.HelpLabelFor(model => model.ExpectedBudget, "/Help/GetHelp_Deal_Edit_ExpectedBudget")%>:</td>
                    <td>
                        <%:Html.TextBoxFor(model => model.ExpectedBudget, new { maxlength = 19, size = 25 })%>&nbsp;р.
                        <%:Html.ValidationMessageFor(model => model.ExpectedBudget)%>
                    </td>

                    <td class="row_title"><%:Html.HelpLabelFor(model => model.StageName, "/Help/GetHelp_Deal_Edit_StageName")%>:</td>
                    <td>
                        <%:Model.StageName %>
                    </td>
                </tr>
                <tr>
                    <td colspan="2"></td>                    
                    <td class="row_title"><%:Html.HelpLabelFor(model => model.CuratorName, "/Help/GetHelp_Deal_Edit_CuratorName")%>:</td>
                    <td>
                        <%: Model.CuratorName %>
                        <%:Html.HiddenFor(model => model.CuratorId)%>
                        <%:Html.ValidationMessageFor(model => model.CuratorId)%>
                    </td>
                </tr>                
                <tr>
                    <td class="row_title">
						<%: Html.HelpLabelFor(model => model.Comment, "/Help/GetHelp_Comment")%>:
					</td>
                    <td colspan='3'>
                        <%:Html.CommentFor(model => model.Comment, new { style = "width: 98%" }, rowsCount: 5)%>
                        <%:Html.ValidationMessageFor(model => model.Comment)%>
                    </td>
                </tr>
            </table>

            <div class='button_set'>
                <%: Html.SubmitButton("btnSave", "Сохранить")%>
                <input id="btnBack" type="button" value="Назад" />                
            </div>
        </div>

    <%=Html.PageBoxBottom()%>

<%} %>

    <div id="clientSelector"></div>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">
</asp:Content>