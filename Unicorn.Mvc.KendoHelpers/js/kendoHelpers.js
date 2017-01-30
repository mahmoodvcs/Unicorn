var Unicorn = {};
Unicorn.kendoErrorHandler = function(grid, args) {
    if (args.errors) {
        //var grid = $("#orderDetailsGrid").data("kendoGrid");
        //var validationTemplate = kendo.template($("#orderDetailsValidationMessageTemplate").html());
        var pl = $("<ul/>");
        grid.one("dataBinding", function(e) {
            e.preventDefault();

            $.each(args.errors, function(propertyName) {
                pl.append("<li/>").html( propertyName);
                for (var i = 0; i < length; i++) {

                }
                this.errors
                grid.editable.element.find(".errors").append(renderedTemplate);
            });
        });
    }
};

function categoryDropDownEditor(container, options) {
    $('<input type="file" name="' + options.field + '"/>')
        .appendTo(container)
        .kendoUpload({
            async: false,
            dataTextField: "CategoryName",
            dataValueField: "CategoryID",
            dataSource: {
                type: "odata",
                transport: {
                    read: "//demos.telerik.com/kendo-ui/service/Northwind.svc/Categories"
                }
            }
        });