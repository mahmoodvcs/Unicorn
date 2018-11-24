
enum EditableGridEditorType {
    text,
    number,
    color,
    image
}
interface EditableGridColumnInfo {
    field: string;
    title: string;
    editor?: any;//EditableGridEditorType | React.ComponentClass<{}> | (o: any, i: number)=> JSX.Element;
    displayField?: string;
    display?: (row: any, value?: any, column?: EditableGridColumnInfo) => any;
}
interface IEditableGridProps<T> {
    columns: EditableGridColumnInfo[];
    data: T[];
    onRowAdded?: (row: T) => void;
    onRowEdited?: (index: number, row: T) => void;
    onRowRemoved?: (index: number, row: T) => void;
    defaultRow?: T;
    editorTemplate?: React.ComponentClass<{
        row: T;
        onChange: (row: T) => void;
    }>;
}


class EditableGridState<T> {
    editingIndex?: number = null;
    editingData?: T = null;
    editingInPopup?: boolean = false;
    addingData?: T = null;
}


class EditableGrid<T> extends React.Component<IEditableGridProps<T>, EditableGridState<T>> {
    constructor(props) {
        super(props);
        this.state = new EditableGridState<T>();

        this.editRow = this.editRow.bind(this);
        this.deleteRow = this.deleteRow.bind(this);
        this.deleteRowOK = this.deleteRowOK.bind(this);
        this.editRowCancel = this.editRowCancel.bind(this);
        this.editRowOK = this.editRowOK.bind(this);
        this.add = this.add.bind(this);
        this.valueChanged = this.valueChanged.bind(this);
    }
    editRow(e, index) {
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
    }
    deleteRow(e, index) {
        e.preventDefault();
        this.props.onRowRemoved(index, this.props.data[index]);
    }
    editRowCancel(e) {
        e.preventDefault();
        this.setState({
            editingIndex: null,
            editingData: null,
            addingData: null
        });
    }
    editRowOK(e?: any) {
        if (e)
            e.preventDefault();
        var { editingIndex, editingData, addingData } = this.state;
        this.setState({ editingIndex: null, editingData: null, addingData: null });
        if (addingData)
            this.props.onRowAdded(addingData);
        else if (editingData)
            this.props.onRowEdited(editingIndex, editingData);
    }
    deleteRowOK(e) {
        e.preventDefault();
    }
    add(e) {
        e.preventDefault();
        var data = this.props.defaultRow || {} as T;
        this.setState({ addingData: data, editingIndex: 0 });
    }
    valueChanged(field, value) {
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
    }
    getDisplay(c: EditableGridColumnInfo, row: any) {
        if (c.displayField) {
            return row[c.displayField];
        }
        else if (c.display) {
            return c.display.call(this, row, row[c.field], c);
        }
        var editor = c.editor | EditableGridEditorType.text;
        if (editor == EditableGridEditorType.color)
            return <span style={{ backgroundColor: row[c.field], width: 40, height: 20, display: "inline-block" }} >&nbsp;</span>;
        else if (editor == EditableGridEditorType.image) {
            if (row[c.field])
                return <img width="50" src={"data:image/png;base64," + row[c.field]} />;
            else
                return null;
        }
        return row[c.field];
    }
    getEditor(c: EditableGridColumnInfo, row: any) {
        if (typeof (c.editor) == "number" || typeof (c.editor) == "undefined") {
            var editor = c.editor | EditableGridEditorType.text;
            if (editor == EditableGridEditorType.image) {
                return <FileUpload files={[{ file: row[c.field] }]} onChange={(files) => {
                    if (files.length == 0)
                        row[c.field] = null;
                    else
                        row[c.field] = files[0].file;
                }} />;
            }
            return <input type={EditableGridEditorType[editor]}
                className={editor == EditableGridEditorType.color ? "" : "form-control"}
                onChange={(e) => this.valueChanged(c.field, e.target.value)}
                value={row[c.field]} />;
        }
        else if (c.editor.prototype.isReactComponent) {
            return React.createElement(c.editor, { value: row[c.field], onChange: (v) => this.valueChanged(c.field, v) });
        }
        else if (c.editor.isReactComponent) {
            return c.editor;
        }
        else if (typeof editor == "function") {
            c.editor.call(this, { value: row[c.field], onChange: (v) => this.valueChanged(c.field, v) });
        }
        return null;
    }
    renderRow(row: T, index) {
        var { editingIndex, editingData } = this.state;
        if (this.state.editingInPopup) {
            editingData = null;
            editingIndex = null;
        }

        var self = this;
        var columns = this.props.columns.map((c, i) => (<td key={i}>
            {editingIndex == index ? self.getEditor(c, row) : self.getDisplay(c, row)}
        </td>));

        if (index == editingIndex) {
            columns.push(<td key="ok">
                <button className="btn btn-sm btn-success" onClick={(e) => this.editRowOK(e)}>
                    <i className="fa fa-check"></i>
                </button>
            </td>);
            columns.push(<td key="cancel">
                <button className="btn btn-sm btn-warning" onClick={(e) => this.editRowCancel(e)}>
                    <i className="fa fa-close"></i>
                </button>
            </td>);
        }
        else {
            columns.push(<td key="edit">
                <button className="btn btn-sm btn-primary" onClick={(e) => this.editRow(e, index)}>
                    <i className="fa fa-pencil"></i>
                </button>
            </td>);
            columns.push(<td key="delete">
                <button className="btn btn-sm btn-danger" onClick={(e) => this.deleteRow(e, index)}>
                    <i className="fa fa-trash"></i>
                </button>
            </td>);
        }
        return (<tr key={index}>
            {columns}
        </tr>);
    }
    getData() {
        var data = [];
        if (this.state.addingData)
            data.push(this.state.addingData);
        if (this.props.data)
            data = data.concat(this.props.data);
        if (this.state.editingIndex >= 0 && this.state.editingData)
            data[this.state.editingIndex] = this.state.editingData;
        return data;
    }
    render() {
        var data = this.getData();
        var editor = this.props.editorTemplate;
        return (
            <div>
                <button type="button" className="btn btn-primary" onClick={(e) => this.add(e)}>
                    <i className="fa fa-plus"></i></button>
                <table className="table table-bordered table-stripped">
                    <thead>
                        <tr>
                            {this.props.columns.map((c, i) => <td key={i}>
                                {c.title}
                            </td>)}
                        </tr>
                    </thead>
                    <tbody>
                        {data.map((r, i) => this.renderRow(r, i))}
                    </tbody>
                </table>
                {this.state.editingData && this.state.editingInPopup &&
                    <Dialog show={true}
                        onOK={this.editRowOK} title="ویرایش"
                        onCancel={this.editRowCancel}>
                        {editor && React.createElement(editor, { row: this.state.editingData, onChange: (r) => this.setState({ editingData: r }) })}
                    </Dialog>}
            </div>);
    }
}


//class SampleEditor extends React.Component<{
//    link: Link<any>;
//}, any>{
//    render() {
//        return (
//            <FeildGroup 
//            );
//    }
//}