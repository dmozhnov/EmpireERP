var OutgoingWaybillRow_IncomingWaybillRow = {
    Init: function () {
        $(document).ready(function () {

            $("#filterIncomingWaybillRow #Batch.select_link").click(function () {
                var recipientStorageId = 0;
                var senderStorageId = 0;

                var storageField = $("#StorageId");

                if (storageField.length > 0 && storageField.val() != "") {
                    var storageId = storageField.val();
                    recipientStorageId = storageId;
                    senderStorageId = storageId;
                }
                else {
                    var recipientStorageField = $("#RecipientStorageId");
                    var senderStorageField = $("#SenderStorageId");

                    if (recipientStorageField.length > 0 && recipientStorageField.val() != "") recipientStorageId = recipientStorageField.val();
                    if (senderStorageField.length > 0 && senderStorageField.val() != "") senderStorageId = senderStorageField.val();
                }

                $.ajax({
                    type: "GET",
                    url: "/Article/SelectArticleBatch/",
                    data: { articleId: $("#ArticleId").val(), senderId: $("#SenderId").val(),
                        recipientStorageId: recipientStorageId, senderStorageId: senderStorageId
                    },
                    success: function (result) {
                        $('#batchFilterSelector').hide().html(result);
                        ShowModal("batchFilterSelector");
                        $('#batchFilterSelector .attention').hide();
                        $("#batchFilterSelector").css("top", "50px");
                    },
                    error: function (XMLHttpRequest, textStatus, thrownError) {
                        ShowErrorMessage(XMLHttpRequest.responseText, "messageOutgoingWaybillRow");
                    }
                });
            });

            $("#batchFilterSelector #gridSelectArticleBatch .articleBatch_select_link").live('click', function () {
                $("#filterIncomingWaybillRow #Batch").text($(this).parent("td").parent("tr").find(".batchName").text());

                var batchId = $(this).parent("td").parent("tr").find(".ReceiptWaybillRowId").text();
                $("#filterIncomingWaybillRow #Batch").attr("selected_id", batchId);

                HideModal();
            });
        });
    }
};