<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.ProductionOrder.ProductionOrderBatchDetailsViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%:Model.Title%>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">

    <script src="../../Scripts/ExecutionGraph.js" type="text/javascript"></script>
    <link href="../../Content/Style/ExecutionGraph.css" rel="stylesheet" type="text/css" />

 <script type="text/javascript">

     ProductionOrder_ProductionOrderBatchDetails.Init();

     function OnSuccessProductionOrderBatchRowEdit(ajaxContext) {
         ProductionOrder_ProductionOrderBatchDetails.OnSuccessProductionOrderBatchRowEdit(ajaxContext);
     }
</script>

</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%:Html.HiddenFor(model => model.Id)%>
    <%:Html.HiddenFor(model => model.BackUrl)%>

    <%=Html.PageTitle("ProductionOrder", Model.Title, Model.Name)%>

    <div class="button_set">
        <%= Html.Button("btnAccept", "Провести партию", Model.MainDetails.AllowToAccept, Model.MainDetails.AllowToAccept)%>
        <%= Html.Button("btnEditStages", "Опер. план", Model.MainDetails.AllowToEditStages, Model.MainDetails.AllowToEditStages)%>
        <%= Html.Button("btnApproveByLineManager", "Утвердить: руководитель", Model.MainDetails.AllowToApproveByLineManager, Model.MainDetails.AllowToApproveByLineManager)%>
        <%= Html.Button("btnCancelApprovementByLineManager", "Отменить: руководитель", Model.MainDetails.AllowToCancelApprovementByLineManager, Model.MainDetails.AllowToCancelApprovementByLineManager)%>
        <%= Html.Button("btnApproveByFinancialDepartment", "Утвердить: фин. отдел", Model.MainDetails.AllowToApproveByFinancialDepartment, Model.MainDetails.AllowToApproveByFinancialDepartment)%>
        <%= Html.Button("btnCancelApprovementByFinancialDepartment", "Отменить: фин. отдел", Model.MainDetails.AllowToCancelApprovementByFinancialDepartment, Model.MainDetails.AllowToCancelApprovementByFinancialDepartment)%>
        <%= Html.Button("btnApproveBySalesDepartment", "Утвердить: отдел продаж", Model.MainDetails.AllowToApproveBySalesDepartment, Model.MainDetails.AllowToApproveBySalesDepartment)%>
        <%= Html.Button("btnCancelApprovementBySalesDepartment", "Отменить: отдел продаж", Model.MainDetails.AllowToCancelApprovementBySalesDepartment, Model.MainDetails.AllowToCancelApprovementBySalesDepartment)%>
        <%= Html.Button("btnApproveByAnalyticalDepartment", "Утвердить: аналит. отдел", Model.MainDetails.AllowToApproveByAnalyticalDepartment, Model.MainDetails.AllowToApproveByAnalyticalDepartment)%>
        <%= Html.Button("btnCancelApprovementByAnalyticalDepartment", "Отменить: аналит. отдел", Model.MainDetails.AllowToCancelApprovementByAnalyticalDepartment, Model.MainDetails.AllowToCancelApprovementByAnalyticalDepartment)%>
        <%= Html.Button("btnApproveByProjectManager", "Утвердить: РП", Model.MainDetails.AllowToApproveByProjectManager, Model.MainDetails.AllowToApproveByProjectManager)%>
        <%= Html.Button("btnCancelApprovementByProjectManager", "Отменить: РП", Model.MainDetails.AllowToCancelApprovementByProjectManager, Model.MainDetails.AllowToCancelApprovementByProjectManager)%>

        <%= Html.Button("btnCancelAcceptance", "Отменить проводку", Model.MainDetails.AllowToCancelAcceptance, Model.MainDetails.AllowToCancelAcceptance)%>
        <%= Html.Button("btnApprove", "Готово", Model.MainDetails.AllowToApprove, Model.MainDetails.AllowToApprove)%>
        <%= Html.Button("btnCancelApprovement", "Отменить готовность", Model.MainDetails.AllowToCancelApprovement, Model.MainDetails.AllowToCancelApprovement)%>
        
        <%= Html.Button("btnRenameBatch", "Переименовать", Model.MainDetails.AllowToRename, Model.MainDetails.AllowToRename)%>
        <%= Html.Button("btnSplitBatch", "Разделить партию", Model.MainDetails.AllowToSplitBatch, Model.MainDetails.AllowToSplitBatch)%>
        <%= Html.Button("btnDeleteBatch", "Удалить", Model.MainDetails.AllowToDeleteBatch, Model.MainDetails.AllowToDeleteBatch) %>
        <%= Html.Button("btnBack", "Назад")%>
    </div>

    <div id="messageProductionOrderBatchEdit"></div>
    <% Html.RenderPartial("ProductionOrderBatchMainDetails", Model.MainDetails);%>
    <br />

    <div id="messageExecutionGraph"></div>

    <%if (Model.AllowToViewStageList) {%> 
    <div id="executionGraph" style="display:block">
        <div class="top_line"></div>
        <div class="grid">
            <table>
                <tr>
                    <td>
                        <div class="title">График исполнения партии <%: Html.Help("/Help/GetHelp_ProductionOrder_Details_ProductionOrderExecutionGraphs") %></div>
                    </td>
                    <td>
                        <div>
                        </div>
                    </td>
                </tr>
            </table>
            <div id="graphData" class="graphData" style="display:none"><%:Model.ExecutionGraphData%></div>
            <div id="graph" class="scheduleGraph graph" style="background-color:White;height:107px;padding:15px 0px 0px 0px;"></div>            
        </div>
    </div>
    <% } %>

    <div id="messageProductionOrderBatchRowList"></div>
    <% Html.RenderPartial("ProductionOrderBatchRowGrid", Model.ProductionOrderBatchRowGrid);%>

    <div id="productionOrderBatchRowEdit"></div>
    <div id="productionOrderBatchChangeStage"></div>
    <div id="productionOrderBatchEditStages"></div>
    <div id="productionOrderRenameBatch"></div>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">
</asp:Content>
