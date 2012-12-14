function JcoDataToolbar_CheckMessageLabel(lbl) {
    var label = document.getElementById(lbl);

    if (label.innerHTML != '') {
        label.style.display = "block";
        alert(label.innerHTML);
        setTimeout("JcoDataToolbar_HideMessageLabel('" + lbl + "');", 6000);
    }
    else
        setTimeout("JcoDataToolbar_CheckMessageLabel('" + lbl + "');", 1000);
}
function JcoDataToolbar_HideMessageLabel(lbl) {
    var label = document.getElementById(lbl);
    label.style.display = 'none';
    label.innerHTML = '';
    setTimeout("JcoDataToolbar_CheckMessageLabel('" + lbl + "');", 1000);
}