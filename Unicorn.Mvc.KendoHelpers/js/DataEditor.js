var Unicorn = {};
Unicorn.kendoErrorHandler = function(data) {
    myAlert = function (s) {
        if (swal)
            swal(s, null, "error");
        else
            alert(s);
    }

    if (data.xhr.responseText)
        myAlert(data.xhr.responseText);
    else if (data.errorThrown)
        myAlert(data.errorThrown);
};

function GridImageEditor(container, oprtions) {
    var div = $("<div/>").appendTo(container);
    var input = $("<input class='file' type='file'/>");
    var hid = $("<input class='hidden' type='hidden' name='" + options.field + "' />");
    $("<button class='btn btn-sm btn-danger'><i class='fa fa-remove /><button>")
        .click(function () {
            var d = $(this).parent;
            d.find("input.hidden").val('');
            d.find("img").remove();
            d.find("input.remove").val('true');
            d.find("button").hide();
        });
    div.append(input);
    div.append(hid);
    input.on("change", function () {
        var f = this.files[0];
        var reader = new FileReader();
        var self = this;
        reader.addEventListener("load", function () {
            var nf = {
                name: f.name,
                size: f.size,
                type: f.type,
                data: reader.result.substring(reader.result.indexOf(',') + 1)
            };
            var d = $(self).parent();
            d.find("input.hidden").value(nf.data);
            $("<img src='" + read.result + "' width='50' />").appendTo(d);
            d.find("button").show();
            d.find("input.remove").val('');
        });
        reader.readAsDataURL(f);

    });
}

function DataEditor_GridEdit(args) {
    var tr = args.container;
    var data = args.model;
}
