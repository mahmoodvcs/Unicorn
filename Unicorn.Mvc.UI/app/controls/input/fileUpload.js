var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var FileUploadProps = /** @class */ (function () {
    function FileUploadProps() {
        this.multiple = false;
        this.readBinary = false;
        this.showDetails = false;
    }
    return FileUploadProps;
}());
var FileUploadFile = /** @class */ (function () {
    function FileUploadFile() {
    }
    return FileUploadFile;
}());
var FileUploadState = /** @class */ (function () {
    function FileUploadState() {
    }
    return FileUploadState;
}());
var FileUpload = /** @class */ (function (_super) {
    __extends(FileUpload, _super);
    function FileUpload(props) {
        var _this = _super.call(this, props) || this;
        _this.fileSelected = _this.fileSelected.bind(_this);
        return _this;
    }
    FileUpload.prototype.componentWillMount = function () {
        if (this.props.files)
            this.setState({ files: this.props.files });
    };
    FileUpload.prototype.componentWillReceiveProps = function (nextProps) {
        if (nextProps.files)
            this.setState({ files: nextProps.files });
    };
    FileUpload.prototype.fileSelected = function () {
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
                    var result = reader.result;
                    nf.file = result.substring(result.indexOf(',') + 1);
                    self.addFile(files, nf);
                });
                reader.readAsDataURL(f);
            }
        }
        ;
    };
    FileUpload.prototype.addFile = function (files, f) {
        if (this.props.multiple)
            files.push(f);
        else
            files = [f];
        if (files.length - this.state.files.length == this.file.files.length || !this.props.multiple) {
            this.setState({ files: files });
            this.props.onChange(files);
        }
    };
    FileUpload.prototype.isImage = function (name) {
        if (!name)
            return false;
        var i = name.lastIndexOf('.');
        var ext = name.substr(i + 1);
        return ["png", "gif", "jpg", "jpeg"].indexOf(ext.toLowerCase()) >= 0;
    };
    FileUpload.prototype.removeFile = function (i) {
        var files = this.state.files;
        files.splice(i, 1);
        this.props.onChange(files);
        this.setState({ files: files });
    };
    FileUpload.prototype.fileNameChanged = function (name, i) {
        var files = this.state.files;
        files[i].name = name;
        this.props.onChange(files);
        this.setState({ files: files });
    };
    FileUpload.prototype.renderConsice = function () {
        var _this = this;
        return (<div style="float: right;">
            {this.state.files.map(function (f, i) {
            return (<div>
                    <img width="50" src={"data:image/png;base64," + f.file}/>
                    <button className="btn btn-xs btn-danger" onClick={function () { return _this.removeFile(i); }}>
                        <i className="fa fa-trash"></i>
                        </button>
                </div>);
        })}
        </div>);
    };
    FileUpload.prototype.renderTable = function () {
        var _this = this;
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
                {this.state.files.map(function (f, i) {
            return (<tr key={i}>
                            <td>{i + 1}</td>
                            <td>
                                {_this.props.editableNames ?
                <input value={f.name} onChange={function (e) { return _this.fileNameChanged(e.target.value, i); }}/>
                : f.name}
                            </td>
                            <td>{f.size}</td>
                            <td>{((f.type && f.type.match('image.*')) || _this.isImage(f.ext)) && <img width="50" src={"data:image/png;base64," + f.file}/>}</td>
                            <td>
                                <button className="btn btn-danger" onClick={function () { return _this.removeFile(i); }}><i className="fa fa-trash"></i></button>
                            </td>
                        </tr>);
        })}
            </tbody>
        </table>);
    };
    FileUpload.prototype.render = function () {
        var _this = this;
        var self = this;
        return (<div>
            <input type='file' ref={function (input) { return _this.file = input; }} onChange={this.fileSelected} multiple={this.props.multiple}/>
            {this.renderTable()}
        </div>);
    };
    return FileUpload;
}(React.Component));
//# sourceMappingURL=fileUpload.js.map