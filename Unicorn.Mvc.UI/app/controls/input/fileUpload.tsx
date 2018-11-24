
class FileUploadProps {
    multiple?: boolean = false;
    readBinary?: boolean = false;
    maxSize?: number;
    files?: FileUploadFile[];
    showDetails?: boolean = false;

    onChange: (any) => void;
}
class FileUploadFile {
    name: string;
    size: number;
    type?: string;
    file?: File | string;
    ext?: string;
}

class FileUploadState {
    files: FileUploadFile[]
}

class FileUpload extends React.Component<FileUploadProps, FileUploadState>
{
    constructor(props) {
        super(props);
        this.fileSelected = this.fileSelected.bind(this);
    }
    file: HTMLInputElement;

    componentWillMount() {
        if (this.props.files)
            this.setState({ files: this.props.files });
    }

    componentWillReceiveProps(nextProps) {
        if (nextProps.files)
            this.setState({ files: nextProps.files });
    }
    fileSelected() {
        var files = this.state.files.concat([]);
        var self = this;
        var count = files.length;
        for (var i = 0; i < this.file.files.length; i++) {
            var f = self.file.files[i];
            var nf = {
                name: f.name,
                size: f.size,
                type: f.type,
                ext: f.name.substring(f.name.lastIndexOf('.')),
                file: null
            };
            if (this.props.readBinary) {
                nf.file = f;
                self.addFile(files, nf);
            }
            else {
                var reader = new FileReader();
                reader.addEventListener("load", function () {
                    var result = (reader.result as string);
                    nf.file = result.substring(result.indexOf(',') + 1);
                    self.addFile(files, nf);
                });
                reader.readAsDataURL(f);
            }
        };
    }

    addFile(files, f) {
        if (this.props.multiple)
            files.push(f);
        else
            files = [f];
        if (files.length - this.state.files.length == this.file.files.length || !this.props.multiple) {
            this.setState({ files: files });
            this.props.onChange(files);
        }
    }

    isImage(name): boolean {
        if (!name)
            return false;
        var i = name.lastIndexOf('.');
        var ext = name.substr(i + 1);
        return ["png", "gif", "jpg", "jpeg"].indexOf(ext.toLowerCase()) >= 0;
    }

    removeFile(i) {
        var files = this.state.files;
        files.splice(i, 1);
        this.props.onChange(files);
        this.setState({ files: files });
    }

    fileNameChanged(name, i) {
        var files = this.state.files;
        files[i].name = name;
        this.props.onChange(files);
        this.setState({ files: files });
    }
    renderConsice() {
        return (<div style="float: right;">
            {this.state.files.map((f, i) =>
                (<div>
                    <img width="50" src={"data:image/png;base64," + f.file} />
                    <button className="btn btn-xs btn-danger" onClick={()=> this.removeFile(i)}>
                        <i className="fa fa-trash"></i>
                        </button>
                </div>)
            )}
        </div>);
    }
    renderTable() {
        return (<table className="table-bordered table-hover table-striped table-condensed">
            <thead>
                <tr>
                    <th>#</th>
                    <th>نام فایل</th>
                    <th>اندازه</th>
                    <th>پیش نمایش</th>
                    <th></th>
                </tr>
            </thead>
            <tbody>
                {this.state.files.map((f, i) => {
                    return (
                        <tr key={i}>
                            <td>{i + 1}</td>
                            <td>
                                {
                                    this.props.editableNames ?
                                        <input value={f.name} onChange={(e) => this.fileNameChanged(e.target.value, i)} />
                                        : f.name
                                }
                            </td>
                            <td>{f.size}</td>
                            <td>{((f.type && f.type.match('image.*')) || this.isImage(f.ext)) && <img width="50" src={"data:image/png;base64," + f.file} />}</td>
                            <td>
                                <button className="btn btn-danger" onClick={() => this.removeFile(i)}><i className="fa fa-trash"></i></button>
                            </td>
                        </tr>);
                })}
            </tbody>
        </table>);
    }
    render() {
        var self = this;

        return (<div>
            <input type='file' ref={(input) => this.file = input} onChange={this.fileSelected} multiple={this.props.multiple} />
            {this.renderTable()}
        </div>);
    }
}