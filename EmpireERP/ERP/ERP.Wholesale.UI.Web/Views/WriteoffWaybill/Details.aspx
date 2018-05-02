<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.WriteoffWaybill.WriteoffWaybillDetailsViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Детали накладной списания
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        Waybill_Details.Init();
        WriteoffWaybill_Details.Init();
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%: Html.HiddenFor(model=>model.Id) %>
    <%: Html.HiddenFor(model => model.BackURL) %>
    
    <%= Html.PageTitle("WriteoffWaybill", "Детали накладной списания", Model.Name)%>
    
    <div class="button_set">
        <%: Html.Button("btnPrepareToAccept", "Подготовить к проводке", Model.AllowToPrepareToAccept, Model.IsPossibilityToPrepareToAccept)%>
        <%: Html.Button("btnCancelReadinessToAccept", "Отменить готовность к проводке", Model.AllowToCancelReadinessToAccept, Model.AllowToCancelReadinessToAccept)%>
        <%: Html.Button("btnAccept", "Провести", Model.AllowToAccept, Model.IsPossibilityToAccept)%>
        <%: Html.Button("btnCancelAcceptance", "Отменить проводку", Model.AllowToCancelAcceptance, Model.AllowToCancelAcceptance)%>
        <%: Html.Button("btnAddRowsByList", "Добавить позиции списком", Model.AllowToEdit, Model.AllowToEdit)%>
        <%: Html.Button("btnWriteoff", "Списать", Model.AllowToWriteoff, Model.IsPossibilityToWriteoff)%>
        <%: Html.Button("btnCancelWriteoff", "Отменить списание", Model.AllowToCancelWriteoff, Model.AllowToCancelWriteoff) %>
        <%: Html.Button("btnEditWriteoffWaybill", "Редактировать", Model.AllowToEdit, Model.AllowToEdit) %>
        <%: Html.Button("btnDeleteWriteoffWaybill", "Удалить", Model.AllowToDelete, Model.AllowToDelete)%>
        <input id="btnBackTo" type="button" value="Назад" />
    </div>

    <div id="messageWriteoffWaybillEdit"></div>
    <% Html.RenderPartial("WriteoffWaybillMainDetails", Model.MainDetails); %>
    <br />

    <div id="messageWriteoffWaybillRowList"></div>
    <%: Html.WaybillTabPanelWithGrids("WriteoffWaybillRowGrid", Model.RowGrid, "WriteoffWaybillArticleGroupGrid", Model.ArticleGroupGridState)%>

    <% Html.RenderPartial("WriteoffWaybillDocumentGrid", Model.DocumentGrid); %>
    
    <div id="writeoffWaybillRowEdit"></div>
    <div id="writeoffWaybillPrintingForm"></div>
    <div id="writeoffWaybillSourceLink"></div>
    <div id="curatorSelector"></div>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">
    <% var style = Model.AllowToPrintForms ? "'display: block'" : "'display: none'"; %>
    <div class="feature_menu_box" id="feature_menu_box" style=<%= style %> >
        <div class="feature_menu_box_title">Печатные формы</div>
        <div class="link" id="lnkWriteoffWaybillPrintingForm">Накладная списания</div>
    </div>

</asp:Content>
