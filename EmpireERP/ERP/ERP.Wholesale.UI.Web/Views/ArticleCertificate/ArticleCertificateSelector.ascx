<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.ArticleCertificate.ArticleCertificateSelectViewModel>" %>

<script src="/Scripts/DatePicker.min.js" type="text/javascript"></script>
<script type="text/javascript" src="/Scripts/DatePicker.js"></script>

<script type="text/javascript">
    ArticleCertificate_ArticleCertificateSelector.Init();
</script>

<div style="width: 800px; padding: 0 10px 0;">
    <div class="modal_title"><%: Model.Title %><%: Html.Help("/Help/GetHelp_ArticleCertificate_Select") %></div>    
    <br />

    <%= Html.GridFilterHelper("filterArticleCertificate", Model.Filter, new List<string> { "gridArticleCertificate" }, true) %>

    <div id="messageSelectArticleCertificate"></div>

    <% if(Model.AllowToCreateArticleCertificate){ %>
        <span id="createArticleCertificate" class="selector_link">Создать сертификат товара и выбрать его</span>
        <br />
        <br />
    <%} %>

    <div style="max-height: 420px; overflow: auto;">
        <% Html.RenderPartial("ArticleCertificateSelectGrid", Model.Grid); %>
    </div>

    <div class="button_set">            
         <input type="button" value="Закрыть" onclick="HideModal()" />
    </div>
</div>

<div id="articleCertificateEdit"></div>