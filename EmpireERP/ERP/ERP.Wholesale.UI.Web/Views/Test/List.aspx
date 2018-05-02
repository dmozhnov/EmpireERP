<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">  
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
        
    <script type="text/javascript">        
        $("#btnDo").live("click", function () {
            StartButtonProgress($(this));            
        });

        $("#btnStop").live("click", function () {
            StopButtonProgress();
        });

        $("span.link").live("click", function () {
            StartLinkProgress($(this));
        });

        $("#stop").live("click", function () {
            StopLinkProgress();
        });
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="button_set">
        <%--<div class="button_loading"><img src="/Content/Img/button_loading.gif" alt="загрузка..." /></div>        --%>
        
        <%: Html.Button("btnDo", "Принять") %>
        <%: Html.SubmitButton("btnDo123", "Принять")%>
        <%: Html.Button("btnStop", "Стоп") %>
        
    </div>
    <br /><br />

    

    <span class="link">Выберите сделку</span>
    
    <br /><br />
    <span id="stop">STOP</span>

    
    

    <%--<img src="/Content/Img/button_loading.gif" alt="" />--%>


    <%--<div style="height: 25pt; width: 80pt; display: none; " id="testDiv">
        <div id="messageList"></div>               

    </div>

    <input type="button" value="Сохранить" onclick="ShowSuccessMessage('Сохранено', 'messageList')" />
    <input type="button" value="Отменить" onclick="ShowErrorMessage('Отменено', 'messageList')" />

    <b class="link" id ="testlink" > 123123123123</b>
    
    <script type="text/javascript">
        $("#testlink").click(function () {
            var t = $("#testDiv").height();
            alert(t);
        });
    </script>--%>
</asp:Content>



<asp:Content ID="Content4" ContentPlaceHolderID="FeatureMenuContent" runat="server">
</asp:Content>
