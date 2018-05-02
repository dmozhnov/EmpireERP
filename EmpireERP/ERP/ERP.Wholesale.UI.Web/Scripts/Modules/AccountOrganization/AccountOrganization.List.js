var AccountOrganization_List = {
    Init: function () {
        $('#btnCreateAccountOrganization').live('click', function () {
            $.ajax({
                type: "GET",
                url: "/AccountOrganization/Create",
                success: function (result) {
                    $('#economicAgentEdit').hide().html(result);
                    $.validator.unobtrusive.parse($("#economicAgentEdit"));
                    ShowModal("economicAgentEdit");
                },
                error: function (XMLHttpRequest, textStatus, thrownError) {
                    ShowErrorMessage(XMLHttpRequest.responseText, "messageAccountOrganizationList");
                }
            });
        });

        $('#gridAccountOrganization .delete_link').live('click', function () {
            var accountId = $(this).parent('td').parent('tr').find('.accountOrganizationId').text();
                        
            if (confirm("Вы уверены?")) {
                StartGridProgress($(this).closest(".grid"));
                
                $.ajax({
                    type: "POST",
                    url: "/AccountOrganization/Delete",
                    data: { accountOrganizationId: accountId },
                    success: function (result) {
                        RefreshGrid("gridAccountOrganization", function () {
                            ShowSuccessMessage("Собственная организация удалена.", "messageAccountOrganizationList");
                        });
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageAccountOrganizationList");
                    }
                });
            }
        });
    },

     OnSuccessAccountOrganizationEdit:function(ajaxContext) {
            RefreshGrid("gridAccountOrganization", function () {
                HideModal();
                ShowSuccessMessage("Организация добавлена.", "messageAccountOrganizationList");
            });
        },

        OnSuccessEconomicAgentTypeSelect:function(ajaxContext) {
            HideModal(function () {
                $("#economicAgentEdit").html(ajaxContext);
                $.validator.unobtrusive.parse($("#economicAgentEdit"));
                ShowModal("economicAgentEdit");
            });
        },
  };