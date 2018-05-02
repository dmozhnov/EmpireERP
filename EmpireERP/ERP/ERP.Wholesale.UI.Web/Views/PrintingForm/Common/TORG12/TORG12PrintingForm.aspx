<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<ERP.Wholesale.UI.ViewModels.PrintingForm.Common.TORG12.TORG12PrintingFormViewModel>" %>

<!DOCTYPE html>
<html>
<head> 
    <title>Товарная накладная</title>
    <meta http-equiv="Content-Type" content="text/html; CHARSET=utf-8"/>    
    <link rel="shortcut icon" href="/Content/Img/favicon.ico" />
    <meta name="SKYPE_TOOLBAR" content="SKYPE_TOOLBAR_PARSER_COMPATIBLE" />
    
    <link href="/Content/Style/Torg12PrintingForm.css" rel="stylesheet" type="text/css" />

    <script src="/Scripts/jquery-1.5.2.min.js" type="text/javascript"></script>
    <script src="/Scripts/jquery.validate.min.js" type="text/javascript"></script>
    <script src="/Scripts/jquery.validate.unobtrusive.min.js" type="text/javascript"></script>
    <script src="/Scripts/modules.min.js" type="text/javascript"></script>
    <script src="/Scripts/Common.js" type="text/javascript"></script>
        
    <script type="text/javascript">
        PrintingForm_TORG12PrintingForm.Init();
    </script>

</head>
<body style="background: #ffffff; margin: 0; font-family: Arial; font-size: 8pt; font-style: normal;">
    <div style="display: none;">
        <%:Html.HiddenFor(model => model.WaybillId) %>
        <%:Html.HiddenFor(model => model.RowsContentURL) %>
        <%:Html.HiddenFor(model => model.PriceTypeId)%>
        <%:Html.HiddenFor(model => model.ConsiderReturns)%>

        <div id="PageHeader">
            <table style="width: 0px; height: 0px;" cellspacing="0">
                <col width="7">
                <col width="111">
                <col width="264">
                <col width="98">
                <col width="98">
                <col width="265">
                <col width="98">
                <col width="18">
                <col width="49">
                <col width="78">
                <tr class="R0">
                    <td class="R0C9" colspan="10"><div style="white-space: nowrap">Унифицированная&nbsp;форма&nbsp;№&nbsp;ТОРГ-12<br /> Утверждена&nbsp;постановлением&nbsp;Госкомстата&nbsp;России&nbsp;от&nbsp;25.12.98&nbsp;№&nbsp;132</div>
                    </td>
                    <td></td>
                </tr>
                <tr class="R1">
                    <td class="R1C0"></td>
                    <td class="R1C0"></td>
                    <td class="R1C0"></td>
                    <td class="R1C0"></td>
                    <td class="R1C0"></td>
                    <td class="R1C0"></td>
                    <td></td>
                    <td class="R1C7"></td>
                    <td class="R1C7"></td>
                    <td class="R1C9"><div style="white-space: nowrap">Коды</div></td>
                    <td></td>
                </tr>
                <tr class="R2">
                    <td class="R2C0"></td>
                    <td class="R2C1" colspan="5" rowspan="2">
                        <%: Model.OrganizationName%></td>
                    <td class="R2C7" colspan="3"><div style="white-space: nowrap">Форма&nbsp;по&nbsp;ОКУД&nbsp;</div></td>
                    <td class="R2C9"><div style="white-space: nowrap">0330212</div></td>
                    <td></td>
                </tr>
                <tr class="R2">
                    <td class="R3C0"></td>
                    <td class="R3C6"></td>
                    <td class="R3C7" colspan="2"><div style="white-space: nowrap">по&nbsp;ОКПО</div></td>
                    <td class="R3C9"><div style="white-space: nowrap">
                        <%: Model.OrganizationOKPO %></div></td>
                    <td></td>
                </tr>
                <tr class="R4">
                    <td class="R4C0" colspan="2"></td>
                    <td class="R4C2" colspan="4"><div style="white-space: nowrap">организация-грузоотправитель,&nbsp;адрес,&nbsp;телефон,&nbsp;факс,&nbsp;банковские&nbsp;реквизиты</div>
                    </td>
                    <td></td>
                    <td class="R4C7"></td>
                    <td class="R4C7"></td>
                    <td class="R4C9" rowspan="2"></td>
                    <td></td>
                </tr>
                <tr class="R2">
                    <td class="R3C0"></td>
                    <td class="R5C1" colspan="6"></td>
                    <td class="R5C7"></td>
                    <td class="R5C7"></td>
                    <td>&nbsp;</td>
                </tr>
                <tr class="R6">
                    <td class="R6C0" colspan="2" style="border-left: #ffffff 0px none;"></td>
                    <td class="R6C2" colspan="4"><div style="white-space: nowrap">структурное&nbsp;подразделение</div></td>
                    <td class="R6C7" colspan="3"><div style="white-space: nowrap">Вид&nbsp;деятельности&nbsp;по&nbsp;ОКДП</div></td>
                    <td class="R6C9"></td>
                    <td></td>
                </tr>
                <tr class="R7">
                    <td class="R7C0"></td>
                    <td class="R7C1">Грузополучатель</td>
                    <td class="R7C2" colspan="5">
                        <%: Model.Recepient%></td>
                    <td class="R7C7" colspan="2"><div style="white-space: nowrap">по&nbsp;ОКПО</div></td>
                    <td class="R7C9"><div>
                        <%: Model.RecepientOKPO %></div></td>
                    <td></td>
                </tr>
                <tr class="R4">
                    <td class="R8C0"></td>
                    <td class="R8C1"></td>
                    <td class="R8C2" colspan="4"><div style="white-space: nowrap">организация,&nbsp;адрес,&nbsp;телефон,&nbsp;факс,&nbsp;банковские&nbsp;реквизиты</div></td>
                    <td></td>
                    <td class="R8C7"></td>
                    <td class="R8C7"></td>
                    <td class="R8C9" rowspan="2"><div style="white-space: nowrap">
                        <%: Model.SenderOKPO  %></div></td>
                    <td></td>
                </tr>
                <tr class="R7">
                    <td class="R7C7" colspan="2"><div style="white-space: nowrap">Поставщик</div></td>
                    <td class="R7C2" colspan="5">
                        <%: Model.Sender %></td>
                    <td class="R7C7" colspan="2"><div style="white-space: nowrap">по&nbsp;ОКПО</div></td>
                    <td></td>
                </tr>
                <tr class="R4">
                    <td class="R8C0"></td>
                    <td class="R8C1"></td>
                    <td class="R8C2" colspan="4"><div style="white-space: nowrap">организация,&nbsp;адрес,&nbsp;телефон,&nbsp;факс,&nbsp;банковские&nbsp;реквизиты</div></td>
                    <td></td>
                    <td class="R8C7"></td>
                    <td class="R8C7"></td>
                    <td class="R8C9" rowspan="2"><div>
                        <%: Model.PayerOKPO %></div></td>
                    <td></td>
                </tr>
                <tr class="R7">
                    <td class="R7C7" colspan="2"><div style="white-space: nowrap">Плательщик</div></td>
                    <td class="R7C2" colspan="5">
                        <%: Model.Payer %></td>
                    <td class="R7C7" colspan="2"><div style="white-space: nowrap">по&nbsp;ОКПО</div></td>
                    <td></td>
                </tr>
                <tr class="R4">
                    <td class="R8C0"></td>
                    <td class="R8C1"></td>
                    <td class="R8C2" colspan="4"><div style="white-space: nowrap">организация,&nbsp;адрес,&nbsp;телефон,&nbsp;факс,&nbsp;банковские&nbsp;реквизиты</div></td>
                    <td></td>
                    <td></td>
                    <td class="R12C8" rowspan="2"><div style="white-space: nowrap">номер</div></td>
                    <td class="R8C9" rowspan="2"></td>
                    <td></td>
                </tr>
                <tr class="R2">
                    <td class="R3C7" colspan="2"><div style="white-space: nowrap">Основание</div></td>
                    <td class="R13C2" colspan="5"><div style="white-space: nowrap"><%: Model.Reason %></div></td>
                    <td></td>
                    <td></td>
                </tr>
                <tr class="R2">
                    <td></td>
                    <td class="R14C1"></td>
                    <td class="R14C2" colspan="4"><div style="white-space: nowrap">договор,&nbsp;заказ-наряд</div></td>
                    <td></td>
                    <td></td>
                    <td class="R14C8"><div style="white-space: nowrap">дата</div></td>
                    <td class="R3C9"></td>
                    <td></td>
                </tr>
                <tr class="R2">
                    <td></td>
                    <td></td>
                    <td></td>
                    <td class="R15C3"><div style="white-space: nowrap">Номер&nbsp;документа</div></td>
                    <td class="R15C3"><div style="white-space: nowrap">Дата&nbsp;составления</div></td>
                    <td class="R15C6" colspan="2"><div style="white-space: nowrap">Транспортная&nbsp;накладная</div></td>
                    <td></td>
                    <td class="R14C8"><div style="white-space: nowrap">номер</div></td>
                    <td class="R3C9"></td>
                    <td></td>
                </tr>
                <tr class="R2">
                    <td class="R16C2" colspan="3"><div style="white-space: nowrap">ТОВАРНАЯ&nbsp;НАКЛАДНАЯ&nbsp;&nbsp;</div></td>
                    <td class="R16C3"><div style="white-space: nowrap">
                        <%: Model.Number %></div></td>
                    <td class="R16C4"><div style="white-space: nowrap">
                        <%: Model.Date %></div></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td class="R14C8"><div style="white-space: nowrap">дата</div></td>
                    <td class="R3C9"></td>
                    <td></td>
                </tr>
                <tr class="R2">
                    <td class="R17C7" colspan="9"><div style="white-space: nowrap">Вид&nbsp;операции</div></td>
                    <td class="R17C9"></td>
                    <td></td>
                </tr>
            </table>
        </div>
        <div id="MainTable">
            <table style="width: 0px; height: 0px;" cellspacing="0">
                <colgroup>
                    <col width="7">
                    <col width="45">
                    <col width="214">
                    <col width="50">
                    <col width="56">
                    <col width="45">
                    <col width="42">
                    <col width="42">
                    <col width="46">
                    <col width="72">
                    <col width="63">
                    <col width="81">
                    <col width="91">
                    <col width="58">
                    <col width="81">
                    <col width="91">
                </colgroup>
                <tr class="R18">
                    <td class="R18C15" colspan="16"><div style="white-space: nowrap" class="PageNumber">Страница&nbsp;1</div></td>
                    <td></td>
                </tr>
                <tr class="R19">
                    <td class="R19C0"></td>
                    <td class="R19C1" rowspan="2"><div>Номер по порядку</div></td>
                    <td class="R19C2" colspan="2"><div style="white-space: nowrap">Товар</div></td>
                    <td class="R19C2" colspan="2"><div style="white-space: nowrap">Единица&nbsp;измерения</div></td>
                    <td class="R19C1" rowspan="2"><div>Вид упаков-<br />ки</div></td>
                    <td class="R19C2" colspan="2"><div style="white-space: nowrap">Количество</div></td>
                    <td class="R19C1" rowspan="2"><div>Масса брутто</div></td>
                    <td class="R19C2" rowspan="2"><div style="white-space: nowrap">Коли-<br>
                        чество&nbsp;<br>
                        (масса&nbsp;<br>
                        нетто)</div></td>
                    <td class="R19C2" rowspan="2"><div style="white-space: nowrap">Цена,<br>
                        руб.&nbsp;коп.</div></td>
                    <td class="R19C2" rowspan="2"><div style="white-space: nowrap">Сумма&nbsp;без<br>
                        учета&nbsp;НДС,<br>
                        руб.&nbsp;коп.</div></td>
                    <td class="R19C2" colspan="2"><div style="white-space: nowrap">НДС</div></td>
                    <td class="R19C2" rowspan="2"><div style="white-space: nowrap">Сумма&nbsp;с<br>
                        учетом НДС,&nbsp;<br>
                        руб.&nbsp;коп.</div></td>
                    <td></td>
                </tr>
                <tr class="R20" style="height: 25pt;">
                    <td class="R20C0"></td>
                    <td class="R20C2">наименование, характеристика, сорт, артикул товара</td>
                    <td class="R20C3"><div style="white-space: nowrap">код</div></td>
                    <td class="R20C2">наиме- нование</td>
                    <td class="R20C2">код по ОКЕИ</td>
                    <td class="R20C2">в одном месте</td>
                    <td class="R20C2">мест,<br>
                        штук</td>
                    <td class="R20C2">ставка, %</td>
                    <td class="R20C2">сумма,
                        <br>
                        руб. коп.</td>
                    <td></td>
                </tr>
                <tr class="R18">
                    <td class="R21C0"></td>
                    <td class="R21C1"><div style="white-space: nowrap">1</div></td>
                    <td class="R21C1"><div style="white-space: nowrap">2</div></td>
                    <td class="R21C3"><div style="white-space: nowrap">3</div></td>
                    <td class="R21C1"><div style="white-space: nowrap">4</div></td>
                    <td class="R21C3"><div style="white-space: nowrap">5</div></td>
                    <td class="R21C3"><div style="white-space: nowrap">6</div></td>
                    <td class="R21C3"><div style="white-space: nowrap">7</div></td>
                    <td class="R21C3"><div style="white-space: nowrap">8</div></td>
                    <td class="R21C3"><div style="white-space: nowrap">9</div></td>
                    <td class="R21C3"><div style="white-space: nowrap">10</div></td>
                    <td class="R21C3"><div style="white-space: nowrap">11</div></td>
                    <td class="R21C3"><div style="white-space: nowrap">12</div></td>
                    <td class="R21C1"><div style="white-space: nowrap">13</div></td>
                    <td class="R21C3"><div style="white-space: nowrap">14</div></td>
                    <td class="R21C3"><div style="white-space: nowrap">15</div></td>
                    <td></td>
                </tr>
            </table>
        </div>
        <div id="PageFooter">
            <table style="width: 0px; height: 0px;" cellspacing="0">
                <col width="7">
                <col width="125">
                <col width="91">
                <col width="30">
                <col width="103">
                <col width="13">
                <col width="138">
                <col width="13">
                <col width="27">
                <col width="56">
                <col width="42">
                <col width="86">
                <col width="11">
                <col width="28">
                <col width="21">
                <col width="51">
                <col width="12">
                <col width="234">
                <tr class="R2">
                    <td></td>
                    <td></td>
                    <td colspan="3"><div style="white-space: nowrap">Товарная&nbsp;накладная&nbsp;имеет&nbsp;приложение&nbsp;на</div></td>
                    <td class="R34C5"></td>
                    <td class="R34C6"></td>
                    <td class="R3C6"></td>
                    <td class="R3C6"></td>
                    <td class="R3C6"></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td colspan="3"></td>
                    <td></td>
                    <td></td>
                    <td></td>
                </tr>
                <tr class="R19">
                    <td></td>
                    <td></td>
                    <td><div style="white-space: nowrap">и&nbsp;содержит</div></td>
                    <td class="R35C3" colspan="4">
                        <div id="RowsCountString" style="white-space: nowrap">&nbsp;</div></td>
                    <td class="R35C7"></td>
                    <td class="R35C8"></td>
                    <td class="R35C8"></td>
                    <td class="R35C8"></td>
                    <td colspan="8"><div style="white-space: nowrap">порядковых&nbsp;номеров&nbsp;записей</div></td>
                </tr>
                <tr class="R4">
                    <td></td>
                    <td></td>
                    <td></td>
                    <td class="R36C3" colspan="8"><div style="white-space: nowrap">прописью</div></td>
                    <td></td>
                    <td></td>
                    <td colspan="3"></td>
                    <td></td>
                    <td class="R36C17" rowspan="2"></td>
                    <td></td>
                </tr>
                <tr class="R18">
                    <td></td>
                    <td></td>
                    <td></td>
                    <td class="R37C3" colspan="2" rowspan="3"></td>
                    <td></td>
                    <td colspan="2"><div style="white-space: nowrap">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Масса&nbsp;груза&nbsp;(нетто)</div></td>
                    <td class="R37C8"></td>
                    <td class="R37C8"></td>
                    <td class="R37C8"></td>
                    <td class="R37C8"></td>
                    <td class="R37C8"></td>
                    <td class="R37C8" colspan="3"></td>
                    <td></td>
                    <td></td>
                </tr>
                <tr class="R4">
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td class="R36C3" colspan="8"><div style="white-space: nowrap">прописью</div></td>
                    <td></td>
                    <td class="R36C17" rowspan="2"></td>
                    <td></td>
                </tr>
                <tr class="R18">
                    <td></td>
                    <td></td>
                    <td><div style="white-space: nowrap">Всего&nbsp;мест</div></td>
                    <td></td>
                    <td colspan="2"><div style="white-space: nowrap">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Масса&nbsp;груза&nbsp;(брутто)</div></td>
                    <td class="R37C3" colspan="6">
                        <div id="WeightBruttoString" style="white-space: nowrap">&nbsp;</div></td>
                    </td>
                    <td class="R39C9" colspan="8"></td>
                    <td></td>
                </tr>
                <tr class="R4">
                    <td></td>
                    <td></td>
                    <td></td>
                    <td class="R36C3" colspan="2"><div style="white-space: nowrap">прописью</div></td>
                    <td class="R36C3"></td>
                    <td></td>
                    <td></td>
                    <td class="R36C3" colspan="8"><div style="white-space: nowrap">прописью</div></td>
                    <td class="R36C3"></td>
                    <td></td>
                    <td></td>
                </tr>
                <tr class="R41">
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td colspan="3"></td>
                    <td></td>
                    <td></td>
                    <td>&nbsp;</td>
                </tr>
                <tr class="R42">
                    <td></td>
                    <td colspan="3"><div style="white-space: nowrap">Приложение&nbsp;(паспорта,&nbsp;сертификаты&nbsp;и&nbsp;т.п.)&nbsp;на&nbsp;</div></td>
                    <td class="R42C4"></td>
                    <td></td>
                    <td colspan="2" style="border-right: #000000 1px solid;"><div style="white-space: nowrap">листах</div></td>
                    <td class="R42C10" colspan="3"><div style="white-space: nowrap">По&nbsp;доверенности&nbsp;№</div></td>
                    <td class="R42C4" colspan="3"></td>
                    <td class="R42C14"><div style="white-space: nowrap">от</div></td>
                    <td class="R42C4" colspan="3"></td>
                    <td></td>
                </tr>
                <tr class="R43">
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td class="R43C4" colspan="2"><div style="white-space: nowrap">прописью</div></td>
                    <td></td>
                    <td class="R43C7"></td>
                    <td></td>
                    <td colspan="3"></td>
                    <td></td>
                    <td colspan="3"></td>
                    <td></td>
                    <td></td>
                    <td></td>
                </tr>
                <tr class="R42">
                    <td class="R44C0"></td>
                    <td class="R44C1" colspan="2"><div style="white-space: nowrap">Всего&nbsp;отпущено&nbsp;&nbsp;на&nbsp;сумму</div></td>
                    <td colspan="5" style="border-right: #000000 1px solid;"></td>
                    <td></td>
                    <td colspan="2"><div style="white-space: nowrap">выданной</div></td>
                    <td class="R44C11" colspan="7"></td>
                    <td></td>
                </tr>
                <tr class="R18">
                    <td class="R45C0"></td>
                    <td class="R45C1" colspan="7" style="border-right: #000000 1px solid;">
                        <span id="TotalSalePriceString"></span>
                    </td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td class="R39C16" colspan="7"><div style="white-space: nowrap">кем,&nbsp;кому&nbsp;(организация,&nbsp;должность,&nbsp;фамилия,&nbsp;и.&nbsp;о.)</div></td>
                    <td></td>
                </tr>
                <tr class="R4">
                    <td></td>
                    <td class="R46C1" colspan="6"><div style="white-space: nowrap">прописью</div></td>
                    <td class="R46C7"></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td class="R46C11"></td>
                    <td class="R46C11"></td>
                    <td class="R46C11" colspan="3"></td>
                    <td class="R46C11"></td>
                    <td class="R46C11"></td>
                    <td></td>
                </tr>
                <tr class="R47">
                    <td></td>
                    <td><div style="white-space: nowrap">Отпуск&nbsp;груза&nbsp;разрешил</div></td>
                    <td class="R47C2" colspan="2">&nbsp;</td>
                    <td class="R47C4"></td>
                    <td></td>
                    <td class="R47C6"><div style="white-space: nowrap"></div></td>
                    <td class="R47C7"></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td class="R47C4" colspan="7"></td>
                    <td></td>
                </tr>
                <tr class="R48">
                    <td></td>
                    <td></td>
                    <td class="R48C2"><div style="white-space: nowrap">должность</div></td>
                    <td></td>
                    <td class="R48C2"><div style="white-space: nowrap">подпись</div></td>
                    <td></td>
                    <td class="R48C2"><div style="white-space: nowrap">расшифровка&nbsp;подписи</div></td>
                    <td class="R48C7"></td>
                    <td></td>
                    <td></td>
                    <td class="R48C10"></td>
                    <td class="R48C11"></td>
                    <td class="R48C11"></td>
                    <td class="R48C11" colspan="3"></td>
                    <td class="R48C11"></td>
                    <td class="R48C11"></td>
                    <td></td>
                </tr>
                <tr class="R18">
                    <td class="R49C0"></td>
                    <td class="R49C0" colspan="3"><div style="white-space: nowrap">Главный&nbsp;(старший)&nbsp;бухгалтер</div></td>
                    <td class="R49C4"></td>
                    <td></td>
                    <td class="R49C6"><div style="white-space: nowrap"></div></td>
                    <td class="R49C7"></td>
                    <td></td>
                    <td colspan="2"><div style="white-space: nowrap">Груз&nbsp;принял</div></td>
                    <td class="R49C4"></td>
                    <td></td>
                    <td class="R49C4" colspan="3"></td>
                    <td></td>
                    <td class="R49C4"></td>
                    <td></td>
                </tr>
                <tr class="R48">
                    <td></td>
                    <td></td>
                    <td class="R50C2" colspan="2" rowspan="2"></td>
                    <td class="R48C2"><div style="white-space: nowrap">подпись</div></td>
                    <td></td>
                    <td class="R48C2"><div style="white-space: nowrap">расшифровка&nbsp;подписи</div></td>
                    <td class="R48C7"></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td class="R48C2"><div style="white-space: nowrap">должность</div></td>
                    <td></td>
                    <td class="R48C2" colspan="3"><div style="white-space: nowrap">подпись</div></td>
                    <td></td>
                    <td class="R48C2"><div style="white-space: nowrap">расшифровка&nbsp;подписи</div></td>
                    <td></td>
                </tr>
                <tr class="R18">
                    <td></td>
                    <td><div style="white-space: nowrap">Отпуск&nbsp;груза&nbsp;произвел</div></td>
                    <td class="R49C4"></td>
                    <td></td>
                    <td class="R49C6"></td>
                    <td class="R49C7"></td>
                    <td></td>
                    <td colspan="2"><div style="white-space: nowrap">Груз&nbsp;получил&nbsp;</div></td>
                    <td class="R49C4"></td>
                    <td></td>
                    <td class="R49C4" colspan="3"></td>
                    <td></td>
                    <td class="R49C4"></td>
                    <td></td>
                </tr>
                <tr class="R18">
                    <td></td>
                    <td></td>
                    <td class="R39C16"><div style="white-space: nowrap">должность</div></td>
                    <td></td>
                    <td class="R39C16"><div style="white-space: nowrap">подпись</div></td>
                    <td></td>
                    <td class="R39C16"><div style="white-space: nowrap">расшифровка&nbsp;подписи</div></td>
                    <td class="R49C7"></td>
                    <td></td>
                    <td class="R18C0" colspan="2"><div style="white-space: nowrap">грузополучатель</div></td>
                    <td class="R39C16"><div style="white-space: nowrap">должность</div></td>
                    <td></td>
                    <td class="R39C16" colspan="3"><div style="white-space: nowrap">подпись</div></td>
                    <td></td>
                    <td class="R39C16"><div style="white-space: nowrap">расшифровка&nbsp;подписи</div></td>
                    <td></td>
                </tr>
                <tr class="R41">
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td class="R53C7"></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td></td>
                    <td colspan="3"></td>
                    <td></td>
                    <td></td>
                    <td>&nbsp;</td>
                </tr>
                <tr class="R18">
                    <td class="R33C7" colspan="2"><div style="white-space: nowrap">М.П.</div></td>
                    <td class="R33C7" colspan="2"><div style="white-space: nowrap">"&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"</div></td>
                    <td class="R49C4"><div style="white-space: nowrap">&nbsp;</div></td>
                    <td colspan="3" style="border-right: #000000 1px solid;"><div style="white-space: nowrap">20__&nbsp;года</div></td>
                    <td class="R54C10" colspan="4"><div style="white-space: nowrap">М.П.</div></td>
                    <td colspan="7"><div style="white-space: nowrap">"&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"&nbsp;_____________&nbsp;20&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;года</div></td>
                </tr>
            </table>
        </div>
    </div>
    <div id="mainContentPrintingForm"></div>
</body>
</html>
