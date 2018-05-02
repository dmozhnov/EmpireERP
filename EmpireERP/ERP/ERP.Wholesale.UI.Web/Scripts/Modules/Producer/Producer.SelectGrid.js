var Producer_SelectGrid = {
    Init: function () {
        $(document).ready(function () {
            // Действия после выбора производителя из грида (ссылка "Выбрать")
            $(".linkProducerSelect").click(function () {
                var producerId = $(this).parent("td").parent("tr").find(".Id").text();
                var producerName = $(this).parent("td").parent("tr").find(".ProducerName").text();
                OnProducerSelectLinkClick(producerId, producerName);
            });
        });
     }
};