<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.Export._1C.ExportTo1CSettingsViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Настройка экспорта в 1С
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        ExportTo1C_Settings.Init();
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%= Html.PageTitle("Report", "Экспорт в 1С", "")%>
    <%= Html.PageBoxTop("Настраиваемые параметры экспорта")%>

    <div style="background: #fff; padding: 10px 0;">
        <div class="button_set">            
            <input id="btnExport2" type="button" value="Выгрузить" />
            <input id="btnRestoreDefaults2" type="button" value="Вернуть по умолчанию" />
        </div>
        
        <div id="messageExportTo1CSettings"></div>
        
        <div class="group_title">
            Основные параметры</div>
        <div class="h_delim"></div>
        <br />

        <div style="min-width: 450px; max-width: 630px; padding-left: 10px;">
        <table>
            <tr>
                <td>
                <table class="editor_table">
                    <tr>
                        <td class='row_title'><%: Html.LabelFor(model=>model.StartDate) %>:</td>
                        <td><%= Html.DatePickerFor(model => model.StartDate)%> - <%= Html.DatePickerFor(model => model.EndDate)%></td>
                        <td></td>
                    </tr>
                    <tr>
                        <td class='row_title'><%: Html.LabelFor(model => model.OperationTypeId) %>:</td>
                        <td><%: Html.DropDownListFor(model => model.OperationTypeId, Model.OperationTypes, new { style = "min-width: 240px;" })%></td>
                        <td></td>
                    </tr>
                </table>
            </td>
            </tr>
        </table>
        </div>

        <br />

        <div id="AccountOrganizations">
        <div class="group_title">Выберите организацию, для которой выгружать данные:</div>
            <div class="h_delim"></div>
            <%= Html.MultipleSelector("multipleSelectorAccountOrganization", Model.AccountOrganizationList, "Список всех организаций", "Список выбранных организаций")%>
        </div>
        
        <div id="CommissionaireOrganizations" style="display:none; padding-left: 10px;">
            <%: Html.LabelFor(x => x.AddTransfersToCommission)%>: <%= Html.YesNoToggleFor(x => x.AddTransfersToCommission) %>  
            <br />
            <br />
            <div class = "OrganizationList"></div>
        </div>

        <div id="ReturnsFromCommissionaireOrganizations" style="display:none; padding-left: 10px;">
            <%: Html.LabelFor(x => x.AddReturnsFromCommissionaires)%>: <%= Html.YesNoToggleFor(x => x.AddReturnsFromCommissionaires)%> 
            <br />
            <br />
            <div class = "OrganizationList"></div>
        </div>

        <div id="ReturnsAcceptedByCommissionaireOrganizations" style="display:none; padding-left: 10px;">
            <%: Html.LabelFor(x => x.AddReturnsAcceptedByCommissionaires)%>: <%= Html.YesNoToggleFor(x => x.AddReturnsAcceptedByCommissionaires)%> 
            <br />
            <br />
            <div class = "OrganizationList"></div>
        </div>

        <div id="ConsignorOrganizations" style="display:none">
        </div>

        <div class="button_set">
            <div class="button_set">            
                <input id="btnExport" type="button" value="Выгрузить" />
                <input id="btnRestoreDefaults" type="button" value="Вернуть по умолчанию" />
            </div>    
        </div>
    </div>
    <%= Html.PageBoxBottom() %>
    
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">
</asp:Content>
