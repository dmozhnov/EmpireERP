var EmployeePost_List = {
    OnSuccessEmployeePostSave: function () {
        HideModal();
        RefreshGrid("gridEmployeePost", function () {
            ShowSuccessMessage("Сохранено.", "messageEmployeePostList");
        });
    }
};
