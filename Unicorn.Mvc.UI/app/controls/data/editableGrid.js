var __extends = (this && this.__extends) || (function () {
    var extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var EditableGridEditorType;
(function (EditableGridEditorType) {
    EditableGridEditorType[EditableGridEditorType["text"] = 0] = "text";
    EditableGridEditorType[EditableGridEditorType["number"] = 1] = "number";
    EditableGridEditorType[EditableGridEditorType["color"] = 2] = "color";
    EditableGridEditorType[EditableGridEditorType["image"] = 3] = "image";
})(EditableGridEditorType || (EditableGridEditorType = {}));
var EditableGridState = /** @class */ (function () {
    function EditableGridState() {
        this.editingIndex = null;
        this.editingData = null;
        this.editingInPopup = false;
        this.addingData = null;
    }
    return EditableGridState;
}());
var EditableGrid = /** @class */ (function (_super) {
    __extends(EditableGrid, _super);
    function EditableGrid(props) {
        var _this = _super.call(this, props) || this;
        _this.state = new EditableGridState();
        _this.editRow = _this.editRow.bind(_this);
        _this.deleteRow = _this.deleteRow.bind(_this);
        _this.deleteRowOK = _this.deleteRowOK.bind(_this);
        _this.editRowCancel = _this.editRowCancel.bind(_this);
        _this.editRowOK = _this.editRowOK.bind(_this);
        _this.add = _this.add.bind(_this);
        _this.valueChanged = _this.valueChanged.bind(_this);
        return _this;
    }
    EditableGrid.prototype.editRow = function (e, index) {
        e.preventDefault();
        var row = Object.assign({}, this.props.data[index]);
        var st = {
            editingIndex: index,
            editingData: row,
            editingInPopup: false,
            addingData: null
        };
        if (this.props.editorTemplate) {
            st.editingInPopup = true;
        }
        this.setState(st);
    };
    EditableGrid.prototype.deleteRow = function (e, index) {
        e.preventDefault();
        this.props.onRowRemoved(index, this.props.data[index]);
    };
    EditableGrid.prototype.editRowCancel = function (e) {
        e.preventDefault();
        this.setState({
            editingIndex: null,
            editingData: null,
            addingData: null
        });
    };
    EditableGrid.prototype.editRowOK = function (e) {
        if (e)
            e.preventDefault();
        var _a = this.state, editingIndex = _a.editingIndex, editingData = _a.editingData, addingData = _a.addingData;
        this.setState({ editingIndex: null, editingData: null, addingData: null });
        if (addingData)
            this.props.onRowAdded(addingData);
        else if (editingData)
            this.props.onRowEdited(editingIndex, editingData);
    };
    EditableGrid.prototype.deleteRowOK = function (e) {
        e.preventDefault();
    };
    EditableGrid.prototype.add = function (e) {
        e.preventDefault();
        var data = this.props.defaultRow || {};
        this.setState({ addingData: data, editingIndex: 0 });
    };
    EditableGrid.prototype.valueChanged = function (field, value) {
        if (this.state.addingData) {
            var data = this.state.addingData;
            data[field] = value;
            this.setState({ addingData: data });
        }
        else {
            var data = this.state.editingData;
            data[field] = value;
            this.setState({ editingData: data });
        }
    };
    EditableGrid.prototype.getDisplay = function (c, row) {
        if (c.displayField) {
            return row[c.displayField];
        }
        else if (c.display) {
            return c.display.call(this, row, row[c.field], c);
        }
        var editor = c.editor | EditableGridEditorType.text;
        if (editor == EditableGridEditorType.color)
            return React.createElement("span", { style: { backgroundColor: row[c.field], width: 40, height: 20, display: "inline-block" } }, "\u00A0");
        else if (editor == EditableGridEditorType.image) {
            if (row[c.field])
                return React.createElement("img", { width: "50", src: "data:image/png;base64," + row[c.field] });
            else
                return null;
        }
        return row[c.field];
    };
    EditableGrid.prototype.getEditor = function (c, row) {
        var _this = this;
        if (typeof (c.editor) == "number" || typeof (c.editor) == "undefined") {
            var editor = c.editor | EditableGridEditorType.text;
            if (editor == EditableGridEditorType.image) {
                return React.createElement(FileUpload, { files: [{ file: row[c.field] }], onChange: function (files) {
                        if (files.length == 0)
                            row[c.field] = null;
                        else
                            row[c.field] = files[0].file;
                    } });
            }
            return React.createElement("input", { type: EditableGridEditorType[editor], className: editor == EditableGridEditorType.color ? "" : "form-control", onChange: function (e) { return _this.valueChanged(c.field, e.target.value); }, value: row[c.field] });
        }
        else if (c.editor.prototype.isReactComponent) {
            return React.createElement(c.editor, { value: row[c.field], onChange: function (v) { return _this.valueChanged(c.field, v); } });
        }
        else if (c.editor.isReactComponent) {
            return c.editor;
        }
        else if (typeof editor == "function") {
            c.editor.call(this, { value: row[c.field], onChange: function (v) { return _this.valueChanged(c.field, v); } });
        }
        return null;
    };
    EditableGrid.prototype.renderRow = function (row, index) {
        var _this = this;
        var _a = this.state, editingIndex = _a.editingIndex, editingData = _a.editingData;
        if (this.state.editingInPopup) {
            editingData = null;
            editingIndex = null;
        }
        var self = this;
        var columns = this.props.columns.map(function (c, i) { return (React.createElement("td", { key: i }, editingIndex == index ? self.getEditor(c, row) : self.getDisplay(c, row))); });
        if (index == editingIndex) {
            columns.push(React.createElement("td", { key: "ok" },
                React.createElement("button", { className: "btn btn-sm btn-success", onClick: function (e) { return _this.editRowOK(e); } },
                    React.createElement("i", { className: "fa fa-check" }))));
            columns.push(React.createElement("td", { key: "cancel" },
                React.createElement("button", { className: "btn btn-sm btn-warning", onClick: function (e) { return _this.editRowCancel(e); } },
                    React.createElement("i", { className: "fa fa-close" }))));
        }
        else {
            columns.push(React.createElement("td", { key: "edit" },
                React.createElement("button", { className: "btn btn-sm btn-primary", onClick: function (e) { return _this.editRow(e, index); } },
                    React.createElement("i", { className: "fa fa-pencil" }))));
            columns.push(React.createElement("td", { key: "delete" },
                React.createElement("button", { className: "btn btn-sm btn-danger", onClick: function (e) { return _this.deleteRow(e, index); } },
                    React.createElement("i", { className: "fa fa-trash" }))));
        }
        return (React.createElement("tr", { key: index }, columns));
    };
    EditableGrid.prototype.getData = function () {
        var data = [];
        if (this.state.addingData)
            data.push(this.state.addingData);
        if (this.props.data)
            data = data.concat(this.props.data);
        if (this.state.editingIndex >= 0 && this.state.editingData)
            data[this.state.editingIndex] = this.state.editingData;
        return data;
    };
    EditableGrid.prototype.render = function () {
        var _this = this;
        var data = this.getData();
        var editor = this.props.editorTemplate;
        return (React.createElement("div", null,
            React.createElement("button", { type: "button", className: "btn btn-primary", onClick: function (e) { return _this.add(e); } },
                React.createElement("i", { className: "fa fa-plus" })),
            React.createElement("table", { className: "table table-bordered table-stripped" },
                React.createElement("thead", null,
                    React.createElement("tr", null, this.props.columns.map(function (c, i) { return React.createElement("td", { key: i }, c.title); }))),
                React.createElement("tbody", null, data.map(function (r, i) { return _this.renderRow(r, i); }))),
            this.state.editingData && this.state.editingInPopup &&
                React.createElement(Dialog, { show: true, onOK: this.editRowOK, title: "\u0648\u06CC\u0631\u0627\u06CC\u0634", onCancel: this.editRowCancel }, editor && React.createElement(editor, { row: this.state.editingData, onChange: function (r) { return _this.setState({ editingData: r }); } }))));
    };
    return EditableGrid;
}(React.Component));
//class SampleEditor extends React.Component<{
//    link: Link<any>;
//}, any>{
//    render() {
//        return (
//            <FeildGroup 
//            );
//    }
//}
//# sourceMappingURL=editableGrid.js.map