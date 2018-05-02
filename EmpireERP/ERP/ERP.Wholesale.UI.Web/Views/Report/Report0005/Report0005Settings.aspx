<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.Report.Report0005.Report0005SettingsViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Настройки отчета Report0005
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        Report0005_Settings.Init();
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <% using (Ajax.BeginForm("Report0005", "Report0005", new AjaxOptions())) %>
    <%{ %>
        <%= Html.PageTitle("Report", "Настройки отчета Report0005", "Карта движения товара")%>
        <%: Html.HiddenFor(model => model.BackURL)%>    
        <%= Html.PageBoxTop("Настраиваемые параметры отчета")%>
        <div style="background: #fff; padding: 10px 0;">
            <div id="messageReport0005Settings"></div>
            <div class="group_title">Настройка детализации отчета</div>
            <div class="h_delim"></div>
        
            <br />
        
            <div style="max-width: 650px; min-width: 450px; padding-left: 10px;">
            
                <table class="editor_table">
                    <tr>
                        <td class='row_title' style="width:40%"><%:Html.LabelFor(x => x.ArticleName)%>:</td>
                        <td>
                            <span id="ArticleName" class="select_link"><%: Model.ArticleName%></span>                            
                            <%: Html.HiddenFor(model => model.ArticleId)%>
                            <%: Html.ValidationMessageFor(x => x.ArticleId)%>
                        </td>     
                    </tr>                    
                </table>
                <br />

            <table>
            <tr>
            <td>

                <div>
                    <table class="editor_table" style="width:250px"> 
                        <tr>
                            <td>
                                <%: Html.RadioButtonFor(model => model.ReportSourceType, 1, new { id = "rbReportSourceType_1" })%>
                                <label for="rbReportSourceType_1"><%: Model.ReportSourceType_caption1 %></label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <%: Html.RadioButtonFor(model => model.ReportSourceType, 2, new { id = "rbReportSourceType_2" })%>
                                <label for="rbReportSourceType_2"><%: Model.ReportSourceType_caption2%></label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <%: Html.RadioButtonFor(model => model.ReportSourceType, 3, new { id = "rbReportSourceType_3" })%>
                                <label for="rbReportSourceType_3"><%: Model.ReportSourceType_caption3%></label>
                            </td>
                        </tr>
                    </table>
                </div>

                </td>
                <td>

                    <table class="editor_table" id="ReportSourceType_Waybill" style="display:none">
                        <tr>
                            <td class='row_title'>
                                <%: Html.LabelFor(x => x.IncomingWaybillTypeId) %>:
                            </td>                                
                            <td>
                                <%: Html.DropDownListFor(x => x.IncomingWaybillTypeId, Model.IncomingWaybillTypeList) %>
                                <%: Html.ValidationMessageFor(x => x.IncomingWaybillTypeId) %>
                            </td>
                        </tr>
                        <tr>
                            <td>
                            </td>
                        </tr>
                        <tr>
                            <td class='row_title'>
                                <%: Html.LabelFor(x => x.IncomingWaybillName) %>:
                            </td>
                            <td>                                
                                <span id="IncomingWaybillName" class="link"><%: Model.IncomingWaybillName%></span>                            
                                <%: Html.HiddenFor(model => model.IncomingWaybillId)%>
                                <%: Html.ValidationMessageFor(x => x.IncomingWaybillId)%>
                            </td> 
                        </tr>
                    </table>

                    <table class="editor_table" id="ReportSourceType_Period">
                        <tr>
                            <td class='row_title'><%:Html.LabelFor(x => x.StartDate)%>:</td>
                            <td>
                                <%= Html.DatePickerFor(model => model.StartDate)%>
                                <%: Html.ValidationMessageFor(model => model.StartDate)%></td>                                    
                            <td class='row_title'><%:Html.LabelFor(x => x.EndDate)%>:</td>
                            <td>
                                <%= Html.DatePickerFor(model => model.EndDate)%>
                                <%: Html.ValidationMessageFor(model => model.EndDate)%></td>
                        </tr>
                    </table>

                </td>
                </tr>
                </table>             

            </div>
            <br />

            <div class="group_title">Места хранения, по которым построить движение товара</div>
            <div class="h_delim"></div>
            <br />
            <%= Html.MultipleSelector("multipleSelectorStorages", Model.Storages, "Список доступных мест хранения", "Выбранные места хранения для отчета")%>
            <br />
            <br />
            <div class="button_set">               
                <input id="btnRender" type="button" value="Сформировать" />
                <input id="btnRestoreDefaults" type="button" value="Вернуть по умолчанию" />
                <input type="button" id="btnBack" value="Назад" />
            </div>
        </div>
    <%= Html.PageBoxBottom() %>
    <% } %>

    <div id="articleSelector"></div>
    <div id="waybillSelector"></div>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">
</asp:Content>
