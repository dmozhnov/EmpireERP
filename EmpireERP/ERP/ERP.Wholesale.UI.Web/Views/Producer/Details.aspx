<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.Producer.ProducerDetailsViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Детали производителя
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        Producer_Details.Init();

        function RefreshMainDetails(obj) {
            Producer_Details.RefreshMainDetails(obj)
        }

        function OnSuccessRussianBankAccountEdit(ajaxContext) {
           Producer_Details.OnSuccessRussianBankAccountEdit(ajaxContext)
        }

        function OnSuccessForeignBankAccountEdit(ajaxContext) {
            Producer_Details.OnSuccessForeignBankAccountEdit(ajaxContext)
        }

        function OnProductionOrderPaymentEditCurrencyRateSelectLinkClick(currencyId, currencyRateId, currencyRate, currencyRateForEdit, currencyRateStartDate) {
            Producer_Details.OnProductionOrderPaymentEditCurrencyRateSelectLinkClick(currencyId, currencyRateId, currencyRate, currencyRateForEdit, currencyRateStartDate);
        }

        $("#btnCreateNewTask").live("click", function () {
            Task_NewTaskGrid.CreateNewTaskByContractor($("#Id").val());
        });
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%: Html.HiddenFor(model => model.Id) %>
    <%: Html.HiddenFor(model => model.BackURL) %>
    
    <%= Html.PageTitle("Producer", "Детали производителя", Model.Name, "/Help/GetHelp_Producer_Details")%>

    <div class="button_set">
        <%= Html.Button("btnEdit", "Редактировать", Model.AllowToEdit, Model.AllowToEdit)%>
        <%= Html.Button("btnDelete", "Удалить", Model.AllowToDelete, Model.AllowToDelete)%>
        <input id="btnBackTo" type="button" value="Назад" />
    </div>

    <div id="messageProducerEdit"></div>
    <% Html.RenderPartial("ProducerMainDetails", Model.MainDetails); %>
    <br />

    <% Html.RenderPartial("~/Views/Task/NewTaskGrid.ascx", Model.TaskGrid);%>

    <%if(Model.AllowToViewProductionOrderList) {%>
    <div id="messageProductionOrdersList"></div>
    <% Html.RenderPartial("ProductionOrdersGrid", Model.ProductionOrdersGrid); %>
    <%} %>
    
    <%if (Model.AllowToViewPaymentList) {%>
    <div id="messagePaymentsList"></div>
    <% Html.RenderPartial("ProducerPaymentsGrid", Model.PaymentsGrid); %>
    <%} %>

    <div id='messageRussianBankAccountList'></div>
    <% Html.RenderPartial("~/Views/Organization/RussianBankAccountGrid.ascx", Model.BankAccountGrid); %>

    <div id='messageForeignBankAccountList'></div>
    <% Html.RenderPartial("~/Views/Organization/ForeignBankAccountGrid.ascx", Model.ForeignBankAccountGrid); %>

    <div id='messageManufactureSelector'></div>
    <% Html.RenderPartial("ProducerManufacturerGrid", Model.ManufacturerGrid); %>

    <div id="producerBankAccountDetailsForEdit"></div>
    <div id="producerForeignBankAccountDetailsForEdit"></div>
    <div id="producerSelector"></div>
    <div id="productionOrderPaymentEdit"></div>
    <div id="currencyRateSelector"></div>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">
</asp:Content>
