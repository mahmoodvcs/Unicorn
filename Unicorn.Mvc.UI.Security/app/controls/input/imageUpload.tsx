class FileUploadProps {
    multiple?: boolean = false;
    readBinary?: boolean = false;
    maxSize?: number;
    onChange: (any) => void;
    files?: FileUploadFile[];
    showDetails?: boolean = false;
}
class FileUploadFile {
    name: string;
    size: number;
    type?: string;
}
class FileUploadState {
    files: FileUploadFile[]
}

class FileUpload extends React.Component<FileUploadProps, FileUploadState>
{
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

    isImage(name) {
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

    render() {
        var self = this;

        return (<div>
            <input type='file' ref={(input) => this.file = input} onChange={this.fileSelected} multiple={this.props.multiple} />
            <Table bordered hover striped condensed>
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
                    {this.state.files.map(function (f, i) {
                        return (
                            <tr key={i}>
                                <td>{i + 1}</td>
                                <td>
                                    {
                                        self.props.editableNames ?
                                            <input value={f.name} onChange={(e) => self.fileNameChanged(e.target.value, i)} />
                                            : f.name
                                    }
                                </td>
                                <td>{f.size}</td>
                                <td>{((f.type && f.type.match('image.*')) || self.isImage(f.ext)) && <img width="50" src={"data:image/png;base64," + f.file} />}</td>
                                <td>
                                    <Button bsStyle="danger" onClick={() => self.removeFile(i)}><i className="fa fa-trash"></i></Button>
                                </td>
                            </tr>);
                    })}
                </tbody>
            </Table>
        </div>);
    }
}