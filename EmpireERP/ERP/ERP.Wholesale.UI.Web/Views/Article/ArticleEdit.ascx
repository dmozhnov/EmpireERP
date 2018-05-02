<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.Article.ArticleEditViewModel>" %>

<script type="text/javascript">
    Article_Edit.Init();

    function OnFailArticleSave(ajaxContext) {
        Article_Edit.OnFailArticleSave(ajaxContext);
    }

    function OnSuccessManufacturerSave(ajaxContext) {
        $("#articleEdit #ManufacturerName").html(ajaxContext.Name);
        $("#articleEdit #ManufacturerId").val(ajaxContext.Id);

        HideModal(function () {
            HideModal();
        });
    }

    function OnSuccessTrademarkSave(ajaxContext) {
        Article_Edit.OnSuccessTrademarkSave(ajaxContext);
    }

    function OnSuccessCountrySave(ajaxContext) {
        Article_Edit.OnSuccessCountrySave(ajaxContext);
    }

    function OnSuccessMeasureUnitSave(ajaxContext) {
        Article_Edit.OnSuccessMeasureUnitSave(ajaxContext);
    }

    function OnSuccessArticleCertificateSave(ajaxContext) {
        Article_Edit.OnSuccessArticleCertificateSave(ajaxContext);
    }

    function OnBeginArticleSave() {
        StartButtonProgress($("#btnSaveArticle"));
    }
</script>

<div style="width: 740px">

<% using (Ajax.BeginForm("Save", "Article", new AjaxOptions() { OnBegin = "OnBeginArticleSave", OnSuccess = "Article_List.OnSuccessSaveArticle", OnFailure = "OnFailArticleSave" }))%>
<%{ %>
    <%: Html.HiddenFor(model => model.Id) %>
    <%: Html.HiddenFor(model => model.ArticleGroupId) %>
    <%: Html.HiddenFor(model => model.IsCurrentArticleGroupLevelCorrect)%>
    <%: Html.HiddenFor(model => model.SalaryPercentFromGroup)%>
    <%: Html.HiddenFor(model => model.MeasureUnitScale)%>

    <div class="modal_title"><%: Model.Title %><%: Html.Help("/Help/GetHelp_Article_Edit") %></div>
    <div class="h_delim"></div>

    <div style="padding: 10px 0px 5px 10px;">
        <div id="messageArticleEdit"></div>

        <table class='editor_table'>
            <tr>
                <td class='row_title'>
                    <%: Html.HelpLabelFor(model => model.Number, "/Help/GetHelp_Article_Edit_Number")%>:
                </td>
                <td style="text-align:left">
                    <%: Html.TextBoxFor(model => model.Number, new { id = "number", style = "width:120px", maxlength = 30 }, !Model.AllowToEdit)%>
                    <%: Html.ValidationMessageFor(model => model.Number)%>
                </td>
                <td class='row_title'>
                    <%: Html.HelpLabelFor(model => model.ManufacturerNumber, "/Help/GetHelp_Article_Edit_ManufacturerNumber")%>:
                </td>
                <td style="text-align:left; width: 143px;">
                    <%: Html.TextBoxFor(model => model.ManufacturerNumber, new { id = "manufacturerNumber", style = "width:120px", maxlength = 30 }, !Model.AllowToEdit)%>
                    <%: Html.ValidationMessageFor(model => model.ManufacturerNumber)%>
                </td>
            </tr>
            <tr>
                <td class='row_title'>
                    <%: Html.HelpLabelFor(model => model.FullArticleName, "/Help/GetHelp_Article_Edit_FullArticleName")%>:
                </td>
                <td colspan="3">
                    <%: Html.TextBoxFor(model => model.FullArticleName, new { id = "FullArticleName", style = "width:525px" }, !Model.AllowToEdit)%>
                    <%: Html.ValidationMessageFor(model => model.FullArticleName)%>
                </td>
            </tr>
            <tr>
                <td class="row_title">
                    <%: Html.HelpLabelFor(model => model.ShortName, "/Help/GetHelp_Article_Edit_ShortName")%>:
                </td>
                <td colspan="3">
                    <%: Html.TextBoxFor(model => model.ShortName, new { id = "ShortName", style = "width:525px" }, !Model.AllowToEdit)%>
                    <%: Html.ValidationMessageFor(model => model.ShortName)%>
                </td>
            </tr>
            <tr>
                <td class="row_title">
                    <%: Html.HelpLabelFor(model => model.ArticleGroupName, "/Help/GetHelp_Article_Edit_ArticleGroupName")%>:
                </td>
                <td colspan="3">
                    <% if (Model.AllowToEdit) { %>
                        <span class="select_link" id="ArticleGroupName"><%: Model.ArticleGroupName%></span>
                    <%} else { %>
                        <%: Model.ArticleGroupName%>
                    <%} %>

                    <%: Html.ValidationMessageFor(model => model.ArticleGroupId)%>
                    <%: Html.ValidationMessage("IsCurrentArticleGroupLevelCorrect")%>
                </td>
            </tr>
            <tr>
                <td style="width:110px" class="row_title">
                    <%: Html.HelpLabelFor(model => model.TrademarkName, "/Help/GetHelp_Article_Edit_TrademarkName")%>:
                </td>
                <td>
                    <span id="selectTrademark" <% if(Model.AllowToEdit){ %>class="select_link"<%} %>><%: Model.TrademarkName %></span>
                    <%: Html.HiddenFor(model => model.TrademarkId) %>
                </td>
                <td class="row_title">
                    <%: Html.HelpLabelFor(model => model.MarkupPercent, "/Help/GetHelp_Article_Edit_MarkupPercent")%>:
                </td>
                <td style="text-align:left;width:132px">
                    <%: Html.TextBoxFor(model => model.MarkupPercent, new { style = "width:60px" }, !Model.AllowToEdit)%>&nbsp;%
                    <%: Html.ValidationMessageFor(model => model.MarkupPercent)%>
                </td>
            </tr>
            <tr>
                <td class="row_title">
                    <%: Html.HelpLabelFor(model => model.ManufacturerId, "/Help/GetHelp_Article_Edit_Manufacturer")%>:
                </td>
                <td>
                    <% if(Model.AllowToEdit) { %>
                        <span class="select_link" id="ManufacturerName"><%: Model.ManufacturerName %></span>
                    <% } else { %>
                        <%: Model.ManufacturerName %>
                    <% } %>
                    <%: Html.HiddenFor(x => x.ManufacturerId) %>
                </td>
                <td class="row_title">
                    <%: Html.HelpLabelFor(model => model.PackSize, "/Help/GetHelp_Article_Edit_PackSize")%>:
                </td>
                <td style="text-align:left">
                    <%: Html.TextBoxFor(model => model.PackSize, new { style = "width:60px", maxlength = "13" }, !Model.AllowToEdit)%>&nbsp;<span id="MeasureUnitShortName"><%:Model.MeasureUnitShortName%></span>
                    <%: Html.ValidationMessageFor(model => model.PackSize)%>
                </td>
            </tr>
            <tr>
                <td class="row_title">
                    <%: Html.HelpLabelFor(model => model.ProductionCountryId, "/Help/GetHelp_Article_Edit_ProductionCountry")%>:
                </td>
                <td>
                    <%: Html.DropDownListFor(model => model.ProductionCountryId, Model.ProductionCountryList, new { style = "width:160px" }, !Model.AllowToEdit)%>
                    <%if (Model.AllowToEdit && Model.AllowToAddCountry)
                      { %>
                        <span class="edit_action" id="AddCountry">[ Добавить ]</span>
                    <%} %>
                </td>
                <td class="row_title">
                    <%: Html.HelpLabelFor(model => model.PackWeight, "/Help/GetHelp_Article_Edit_PackWeight")%>:
                </td>
                <td style="text-align:left">
                    <%: Html.TextBoxFor(model => model.PackWeight, new { style = "width:60px" }, !Model.AllowToEdit)%>&nbsp;кг
                    <%: Html.ValidationMessageFor(model => model.PackWeight)%>
                </td>
            </tr>
            <tr>
                <td class="row_title">
                    <%: Html.HelpLabelFor(model => model.MeasureUnitId, "/Help/GetHelp_Article_Edit_MeasureUnit")%>:
                </td>
                <td colspan = "3">
                    <span id="selectMeasureUnit" <% if(Model.AllowToEdit){ %>class="select_link"<%} %>><%: Model.MeasureUnitName%></span>
                    <%: Html.HiddenFor(model => model.MeasureUnitId)%>
                    <%: Html.ValidationMessageFor(model => model.MeasureUnitId) %>
                </td>
            </tr>
            <tr>
                <td class="row_title">
                    <%: Html.HelpLabelFor(x => x.PackLength, "/Help/GetHelp_Article_Edit_PackLength")%>:
                </td>
                <td>
                    <%: Html.TextBoxFor(x => x.PackHeight, new { style = "width:40px" }, !Model.AllowToEdit)%>
                    &nbsp;x&nbsp;
                    <%: Html.TextBoxFor(x => x.PackWidth, new { style = "width:40px" }, !Model.AllowToEdit)%>
                    &nbsp;x&nbsp;
                    <%: Html.TextBoxFor(x => x.PackLength, new { style = "width:40px" }, !Model.AllowToEdit)%>
                    <%: Html.ValidationMessageFor(x => x.PackHeight) %>
                    <%: Html.ValidationMessageFor(x => x.PackWidth) %>
                    <%: Html.ValidationMessageFor(x => x.PackLength) %>
                    <span style="margin-left:35px" class="greytext">или</span>
                </td>
                <td class="row_title">
                    <%:Html.HelpLabelFor(model => model.PackVolume, "/Help/GetHelp_Article_Edit_PackVolume")%>:
                </td>
                <td>
                    <%: Html.TextBoxFor(model => model.PackVolume, new { style = "width:60px" }, !Model.AllowToEdit)%>&nbsp;куб.&nbsp;м.
                </td>
            </tr>
            <tr>
                <td class="row_title">
                    <%: Html.HelpLabelFor(model => model.CertificateId, "/Help/GetHelp_Article_Edit_Certificate")%>:
                </td>
                <td colspan = "3">
                    <% string allowToClearCertificateDisplay = (Model.AllowToClearCertificate ? "inline" : "none"); %>
                    <span id="selectCertificate" <% if(Model.AllowToEdit){ %>class="select_link"<%} %>><%: Model.CertificateName%></span>
                     <% if (Model.AllowToEdit)
                        { %>
                        &nbsp;&nbsp;<span class="link" id="clearCertificate" style="display:<%= allowToClearCertificateDisplay %>">[ Сбросить ]</span>
                     <%} %>
                    <%: Html.HiddenFor(model => model.CertificateId)%>
                    <%: Html.ValidationMessageFor(model => model.CertificateId)%>
                </td>
            </tr>
        </table>
    </div>

    <div class='h_delim'></div>

    <div style="padding: 10px;">
        <table class='editor_table'>
            <tr>
                <td style = "width: 94px" class="row_title">
                    <%: Html.HelpLabelFor(model => model.SalaryPercent, "/Help/GetHelp_Article_Edit_SalaryPercent")%>:
                </td>
                <td id = "salaryPercentTextBox">
                    <%  object obj = null;
                        if (Model.IsSalaryPercentFromGroup == "1")
                            obj = new { disabled = "disabled", style = "width:60px" };
                        else
                            obj = new { style = "width:60px" };
                    %>
                    <%: Html.TextBoxFor(model => model.SalaryPercent, obj, !Model.AllowToEdit)%> %
                    <%: Html.HiddenFor(model => model.isSalaryPercentCorrect) %>
                    <%: Html.ValidationMessageFor(model => model.isSalaryPercentCorrect)%>
                </td>
                <td style="padding-left:2px">
                    <span id="spanIsSalaryPercentFromGroup">
                        <%: Html.LabelFor(model => model.IsSalaryPercentFromGroup)%>:
                        <%: Html.YesNoToggleFor(model => model.IsSalaryPercentFromGroup, Model.AllowToEdit)%>
                    </span>
                </td>
                <td style="padding-left:2px">
                    <span>
                        <%: Html.HelpLabelFor(model => model.IsObsolete, "/Help/GetHelp_Article_Edit_IsObsolete")%>:
                        <%: Html.YesNoToggleFor(model => model.IsObsolete, Model.AllowToEdit)%>
                    </span>
                </td>
            </tr>
            <tr>
                <td class="row_title">
                    <%: Html.HelpLabelFor(model => model.Comment, "/Help/GetHelp_Comment")%>:
                </td>
                <td colspan="3">
                        <%: Html.CommentFor(model => model.Comment, new { style = "width:592px" }, !Model.AllowToEdit, rowsCount: 4)%>
                        <%: Html.ValidationMessageFor(model => model.Comment)%>
                </td>
            </tr>
        </table>
    </div>

    <div class="button_set">
        <%: Html.SubmitButton("btnSaveArticle", "Сохранить", Model.AllowToEdit, Model.AllowToEdit) %>
        <input type="button" value="Закрыть" onclick="HideModal()" />
    </div>
<%} %>

</div>

<div id="articleGroupSelector"></div>
<div id="manufacturerAdd"></div>
<div id="countryAdd"></div>
<div id="trademarkSelector"></div>
<div id="measureUnitSelector"></div>
<div id="articleCertificateSelector"></div>
