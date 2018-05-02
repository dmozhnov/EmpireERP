var ProductionOrder_List = {
    Init: function () {
        $(document).ready(function () {
            
            $("#btnCreateProductionOrder").click(function () {
                window.location = "/ProductionOrder/Create?" + GetBackUrl(true);
            });
        });
    }
};