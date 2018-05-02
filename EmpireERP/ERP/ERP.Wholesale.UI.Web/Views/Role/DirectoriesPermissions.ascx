<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ERP.Wholesale.UI.ViewModels.Role.DirectoriesPermissionsViewModel>" %>

<script type="text/javascript">
    function OnSuccessDirectoriesPermissionsSave() {
        ShowSuccessMessage("Сохранено.", "messageDirectoriesPermissionsEdit");
        
        if ($(window).scrollTop() > $(window).height()) {
            scroll(0, $("#messageDirectoriesPermissionsEdit").offset().top - 10);
        }
    }

    function OnFailDirectoriesPermissionsSave(ajaxContext) {
        ShowErrorMessage(ajaxContext.responseText, "messageDirectoriesPermissionsEdit");

        if ($(window).scrollTop() > $(window).height()) {
            scroll(0, $("#messageDirectoriesPermissionsEdit").offset().top - 10);
        }
    }

    $(document).ready(function () {
        $('#form0 input[type="submit"]').click(function () {
            StartButtonProgress($(this));
            $('#form0').trigger('submit');
        });
    });
</script>

<% using (Ajax.BeginForm("SaveDirectoriesPermissions", "Role", new AjaxOptions()
   {
       OnSuccess = "OnSuccessDirectoriesPermissionsSave", OnFailure = "OnFailDirectoriesPermissionsSave" })) %>
<%{ %>
    <%= Html.HiddenFor(model => model.RoleId) %>
    
    <%if (Model.AllowToEdit)
      { %>

    <div class="button_set">
        <input type="submit" id="btnDirectoriesPermissionsSaveTop" value="Сохранить" />
    </div>

    <%} %>

    <div id="messageDirectoriesPermissionsEdit"></div>
    
    <div class="permission_group">
        <div class="title">Места хранения и собственные организации</div>
            <div style="line-height: 1.4; font-size: 11px; background: #E5ECEB; padding: 8px 10px;">
                Места хранения и собственные организации настраиваются в разделе «Товародвижение»                
            </div>
    </div>

    <div class="permission_group">
        <div class="title">Организации контрагентов</div>
            <div style="line-height: 1.4; font-size: 11px; background: #E5ECEB; padding: 8px 10px;">
                Организации поставщиков настраиваются в разделе «Товародвижение -> Поставщики»
                <br />
                Организации клиентов настраиваются в разделе «Продажи -> Клиенты»
            </div>
    </div>

    <div class="permission_group">
        <div class="title">Банки</div>
        <table>
            <%= Html.Permission(Model.Bank_Create_ViewModel)%>
            <%= Html.Permission(Model.Bank_Edit_ViewModel) %>
            <%= Html.Permission(Model.Bank_Delete_ViewModel) %>
        </table>
    </div>

    <div class="permission_group">
        <div class="title">Валюты</div>
        <table>
            <%= Html.Permission(Model.Currency_Create_ViewModel)%>
            <%= Html.Permission(Model.Currency_Edit_ViewModel) %>
            <%= Html.Permission(Model.Currency_AddRate_ViewModel) %>
            <%= Html.Permission(Model.Currency_Delete_ViewModel) %>
        </table>
    </div>

    <div class="permission_group">
        <div class="title">Товары</div>
        <table>
            <%= Html.Permission(Model.Article_List_Details_ViewModel, true)%>
            <%= Html.Permission(Model.Article_Create_ViewModel) %>
            <%= Html.Permission(Model.Article_Edit_ViewModel) %>
            <%= Html.Permission(Model.Article_Delete_ViewModel) %>
        </table>
    </div>

    <div class="permission_group">
        <div class="title">Группы товаров</div>
        <table>
            <%= Html.Permission(Model.ArticleGroup_Create_ViewModel) %>
            <%= Html.Permission(Model.ArticleGroup_Edit_ViewModel) %>
            <%= Html.Permission(Model.ArticleGroup_Delete_ViewModel) %>
        </table>
    </div>

    <div class="permission_group">
        <div class="title">Торговые марки</div>
        <table>
            <%= Html.Permission(Model.Trademark_Create_ViewModel) %>
            <%= Html.Permission(Model.Trademark_Edit_ViewModel) %>
            <%= Html.Permission(Model.Trademark_Delete_ViewModel) %>
        </table>
    </div>

    <div class="permission_group">
        <div class="title">Изготовители</div>
        <table>
            <%= Html.Permission(Model.Manufacturer_Create_ViewModel) %>
            <%= Html.Permission(Model.Manufacturer_Edit_ViewModel) %>
            <%= Html.Permission(Model.Manufacturer_Delete_ViewModel) %>
        </table>
    </div>

    <div class="permission_group">
        <div class="title">Страны</div>
        <table>
            <%= Html.Permission(Model.Country_Create_ViewModel) %>
            <%= Html.Permission(Model.Country_Edit_ViewModel) %>
            <%= Html.Permission(Model.Country_Delete_ViewModel) %>
        </table>
    </div>

    <div class="permission_group">
        <div class="title">Единицы измерения</div>
        <table>
            <%= Html.Permission(Model.MeasureUnit_Create_ViewModel) %>
            <%= Html.Permission(Model.MeasureUnit_Edit_ViewModel) %>
            <%= Html.Permission(Model.MeasureUnit_Delete_ViewModel) %>
        </table>
    </div>

    <div class="permission_group">
        <div class="title">Сертификаты товаров</div>
        <table>
            <%= Html.Permission(Model.ArticleCertificate_Create_ViewModel) %>
            <%= Html.Permission(Model.ArticleCertificate_Edit_ViewModel) %>
            <%= Html.Permission(Model.ArticleCertificate_Delete_ViewModel) %>
        </table>
    </div>

    <div class="permission_group">
        <div class="title">Ставки НДС</div>
        <table>
            <%= Html.Permission(Model.ValueAddedTax_Create_ViewModel) %>
            <%= Html.Permission(Model.ValueAddedTax_Edit_ViewModel)%>
            <%= Html.Permission(Model.ValueAddedTax_Delete_ViewModel)%>
        </table>
    </div>

    <div class="permission_group">
        <div class="title">Организационно-правовые формы</div>
        <table>
            <%= Html.Permission(Model.LegalForm_Create_ViewModel) %>
            <%= Html.Permission(Model.LegalForm_Edit_ViewModel)%>
            <%= Html.Permission(Model.LegalForm_Delete_ViewModel)%>
        </table>
    </div>

    <%if (Model.AllowToEdit)
      { %>

    <div class="button_set">
        <input type="submit"  id="btnDirectoriesPermissionsSaveBottom" value="Сохранить" />
    </div>

    <%} %>

<% } %>