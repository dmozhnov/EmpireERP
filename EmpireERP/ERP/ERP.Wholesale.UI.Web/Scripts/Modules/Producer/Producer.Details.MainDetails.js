var Producer_Details_MainDetails = {
    Init: function () {
        $(document).ready(function () {
            if (IsTrue($("#AllowToViewCuratorDetails").val())) {
                $("#CuratorName").attr("href", "/User/Details?id=" + $("#CuratorId").val() + GetBackUrl());
            }
            else {
                $("#CuratorName").addClass("disabled");
            }
        });
    }
};