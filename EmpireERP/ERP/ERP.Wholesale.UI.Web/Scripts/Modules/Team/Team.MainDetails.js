var Team_MainDetails = {
    Init: function () {
        $(document).ready(function () {
            var currentUrl = $("#currentUrl").val();

            var userId = $("#team_main_details #CreatorId").val();
            if (IsTrue($("#AllowToViewCreatorDetails").val())) {
                $("#CreatedBy").attr("href", "/User/Details?id=" + userId + "&backURL=" + currentUrl);
            }
            else {
                $("#CreatedBy").addClass("disabled");
            }
        });
    },

    RefreshMainDetails: function (details) {
        $("#Name").text(details.Name);
        $("#UserCount").text(details.UserCount);
        $("#CreationDate").text(details.CreationDate);
        $("#StorageCount").text(details.StorageCount);
        $("#CreatedBy").text(details.CreatedBy);
        $("#DealCount").text(details.DealCount);
        $("#Comment").html(details.Comment);
    }
};