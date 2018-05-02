function Timeparse_getReadable(d) {
    return Timeparse_padAZero(d.getHours())
+ ':'
+ Timeparse_padAZero(d.getMinutes())
+ ':'
+ Timeparse_padAZero(d.getSeconds());
}
function Timeparse_padAZero(s) { s = s.toString(); if (s.length == 1) { return '0' + s; } else { return s; } }
var Timeparse_timeParsePatterns = [{ re: /^now/i, example: new Array('now'), handler: function () { return new Date(); } }, { re: /(\d{1,2}):(\d{1,2}):(\d{1,2})(?:p| p)/, example: new Array('9:55:00 pm', '12:55:00 p.m.', '9:55:00 p', '11:5:10pm', '9:5:1p'), handler: function (bits) {
    var d = new Date(); var h = parseInt(bits[1], 10); if (h < 12) { h += 12; }
    d.setHours(h); d.setMinutes(parseInt(bits[2], 10)); d.setSeconds(parseInt(bits[3], 10)); return d;
} 
}, { re: /(\d{1,2}):(\d{1,2})(?:p| p)/, example: new Array('9:55 pm', '12:55 p.m.', '9:55 p', '11:5pm', '9:5p'), handler: function (bits) {
    var d = new Date(); var h = parseInt(bits[1], 10); if (h < 12) { h += 12; }
    d.setHours(h); d.setMinutes(parseInt(bits[2], 10)); d.setSeconds(0); return d;
} 
}, { re: /(\d{1,2})(?:p| p)/, example: new Array('9 pm', '12 p.m.', '9 p', '11pm', '9p'), handler: function (bits) {
    var d = new Date(); var h = parseInt(bits[1], 10); if (h < 12) { h += 12; }
    d.setHours(h); d.setMinutes(0); d.setSeconds(0); return d;
} 
}, { re: /(\d{1,2}):(\d{1,2}):(\d{1,2})/, example: new Array('9:55:00', '19:55:00', '19:5:10', '9:5:1', '9:55:00 a.m.', '11:55:00a'), handler: function (bits) { var d = new Date(); d.setHours(parseInt(bits[1], 10)); d.setMinutes(parseInt(bits[2], 10)); d.setSeconds(parseInt(bits[3], 10)); return d; } }, { re: /(\d{1,2}):(\d{1,2})/, example: new Array('9:55', '19:55', '19:5', '9:55 a.m.', '11:55a'), handler: function (bits) { var d = new Date(); d.setHours(parseInt(bits[1], 10)); d.setMinutes(parseInt(bits[2], 10)); d.setSeconds(0); return d; } }, { re: /(\d{1,6})/, example: new Array('9', '9a', '9am', '19', '1950', '195510', '0955'), handler: function (bits) {
    var d = new Date(); var h = bits[1].substring(0, 2); var m = parseInt(bits[1].substring(2, 4), 10); var s = parseInt(bits[1].substring(4, 6), 10); if (isNaN(m)) { m = 0; }
    if (isNaN(s)) { s = 0; }
    d.setHours(parseInt(h, 10)); d.setMinutes(parseInt(m, 10)); d.setSeconds(parseInt(s, 10)); return d;
} 
}, ]; function Timeparse_parseTimeString(s) {
    for (var i = 0; i < Timeparse_timeParsePatterns.length; i++) { var re = Timeparse_timeParsePatterns[i].re; var handler = Timeparse_timeParsePatterns[i].handler; var bits = re.exec(s); if (bits) { return handler(bits); } }
    throw new Error("Неверный формат времени.");
}
function Timeparse_magicTime(input) {
    var messagespan = input.id + '-messages'; try { var d = Timeparse_parseTimeString(input.value); input.value = Timeparse_getReadable(d); }
    catch (e) { } 
}