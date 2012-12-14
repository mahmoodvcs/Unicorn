
var cart_grid_text_first = 'اولين';
var cart_grid_text_last = 'آخرين';

function getCaretPosition(ctrl) {
    var CaretPos = 0; // IE Support
    if (document.selection) {
        ctrl.focus();
        var Sel = document.selection.createRange();
        Sel.moveStart('character', -ctrl.value.length);
        CaretPos = Sel.text.length;
    }
    // Firefox support
    else if (ctrl.selectionStart || ctrl.selectionStart == '0')
        CaretPos = ctrl.selectionStart;
    return (CaretPos);
}
function setCaretPosition(ctrl, pos) {
    if (ctrl.setSelectionRange) {
        ctrl.focus();
        ctrl.setSelectionRange(pos, pos);
    }
    else if (ctrl.createTextRange) {
        var range = ctrl.createTextRange();
        range.collapse(true);
        range.moveEnd('character', pos);
        range.moveStart('character', pos);
        range.select();
    }
}

if (typeof Jco == "undefined" || !Jco)
    var Jco = {};
if (typeof Jco.GS == "undefined" || !Jco.GS)
    Jco.GS = {};
if (typeof Jco.GS.d == "undefined")
    Jco.GS.d = {};

Jco.GS.clearTimer = function() {
    if (Jco.GS.timer) {
        try {
            clearTimeout(Jco.GS.timer);
        }
        catch (ex) { }
    }
}

Jco.GS.search = function(txt, gridID, f) {
    if (typeof Jco.GS.d[gridID] == "undefined")
        Jco.GS.d[gridID] = {};
    if (typeof Jco.GS.d[gridID].columnFilters == "undefined")
        Jco.GS.d[gridID].columnFilters = [];
    Jco.GS.clearTimer();
    Jco.GS.grid = gridID;
    Jco.GS.txt = txt;
    Jco.GS.fieldName = f;
    Jco.GS.timer = setTimeout(Jco.GS.filter, 500);
}
Jco.GS.filter = function () {
    var gid = Jco.GS.grid;
    var txt = Jco.GS.txt;
    var v = txt.value;
    var f = Jco.GS.fieldName;
    var columnFilters = Jco.GS.d[gid].columnFilters;
    if (columnFilters[f] == v)
        return;
    if (v == '') {
        delete columnFilters[f];
    }
    else {
        columnFilters[f] = v;
    }
    Jco.GS.value = v;

    var b = true;
    v = '';
    var chYMain = 'ي', chY1 = 'ى', chY2 = 'ي';
    var chKMain = 'ك', chK1 = 'ک';

    for (var key in columnFilters) {
        if (b)
            b = false;
        else
            v += ' && ';
        var t = columnFilters[key];
        var text = [];
        //ي
        text[0] = t.replace(chY1, chYMain).replace(chY2, chYMain).replace(chK1, chKMain);
        text[1] = text[0].replace(chYMain, chY1);
        text[2] = text[0].replace(chYMain, chY2);
        //ک
        text[3] = text[0].replace(chKMain, chK1);
        text[4] = text[1].replace(chKMain, chK1);
        text[5] = text[2].replace(chKMain, chK1);
        var s = '(' + text.join(')|(') + ')';
        v += "/" + s + "/gi.test(DataItem.getMember('" + key + "').get_value().toString())";
    }
    var g = eval(gid);
    //g.filter("/" + v + "/gi.test(DataItem.getMember('" + f + "').get_value().toString())");
    Jco.GS.caret = getCaretPosition(txt);
    g.filter(v);
    g.render();
    setTimeout(Jco.GS.setTextFocus, 100);
    Jco.GS.txt = txt.id;
}

Jco.GS.getColumnFilter = function(gid, field) {
    if (typeof Jco.GS.d[gid] == "undefined")
        return "";
    if (Jco.GS.d[gid].columnFilters[field]) {
        return Jco.GS.d[gid].columnFilters[field];
    }
    else {
        return "";
    }
}
Jco.GS.setTextFocus = function() {
    var txt = document.getElementById(Jco.GS.txt);
    txt.value = Jco.GS.value;
    txt.focus();
    setCaretPosition(txt, Jco.GS.caret);
}

Jco.GS.intrenalSelect = false;

Jco.GS.gridItemSelected = function (sender, e) {
    if (Jco.GS.intrenalSelect === true)
        return;
    Jco.GS.intrenalSelect = false;
    var t = e.get_item();
    for (var i = 0; i < t.Cells.length; i++)
        if (t.Cells[i].Template === "JcoSelectCheckBoxTemp") {
            document.getElementById(t.Table.Grid.Id + "_cell_" + t.Index + "_" + i).firstChild.firstChild.checked = true;
        }
}
Jco.GS.gridItemUnSelected = function (sender, e) {
    if (Jco.GS.intrenalSelect === true)
        return;
    Jco.GS.intrenalSelect = false;
    var t = e.get_item();
    for (var i = 0; i < t.Cells.length; i++)
        if (t.Cells[i].Template === "JcoSelectCheckBoxTemp") {
            document.getElementById(t.Table.Grid.Id + "_cell_" + t.Index + "_" + i).firstChild.firstChild.checked = false;
        }
}
