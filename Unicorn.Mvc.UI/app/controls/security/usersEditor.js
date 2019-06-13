/// <reference path="../typings/react.d.ts" />
/// <reference path="../../../edmmi/edmmi/app/components/editablegrid.tsx" />
/// <reference path="../lib/react-datagrid/react-datagrid.js" />
/// <reference path="../localization/fa-ir.js" />
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
if (typeof Unicorn == "undefined")
    Unicorn = {};
(function () {
    var UsersEditor = /** @class */ (function (_super) {
        __extends(UsersEditor, _super);
        function UsersEditor() {
            return _super !== null && _super.apply(this, arguments) || this;
        }
        UsersEditor.prototype.addUser = function () {
            this.setState({ editingUser: {} });
        };
        UsersEditor.prototype.editUser = function (user) {
            this.setState({ editingUser: user });
        };
        UsersEditor.prototype.deleteUser = function (user) {
            var self = this;
            swal({
                title: "Delete User",
                text: "Are you sure you want to delete the user '" + user.name + "'",
                showConfirmButton: true,
                showCancelButton: true,
                type: "warning",
            }, function (r) {
                if (!r)
                    return;
                $.ajax({
                    url: getUrl("user?id=" + user.id),
                    method: "DELETE"
                }).done(function (r) {
                    noty2("User deleted", "success");
                    self.refresh();
                });
            });
        };
        UsersEditor.prototype.userSaved = function () {
            this.refresh();
        };
        UsersEditor.prototype.closeDialog = function () {
            this.setState({ editingUser: null });
        };
        UsersEditor.prototype.dataSource = function (query) {
            var page = query.skip / query.pageSize + 1;
            var search = "";
            if (this.state.search) {
                search = "searchBy=all&searchTerm=" + this.state.search + "&";
            }
            return $.ajax(getUrl("user/query?" + search + "pageSize=" + query.pageSize + '&page=' + page));
        };
        UsersEditor.prototype.search = function (e) {
            this.setState({ search: e.target.value });
            if (this.searchTimeout)
                window.clearTimeout(this.searchTimeout);
            var self = this;
            this.searchTimeout = window.setTimeout(function () {
                self.grid.gotoPage(1);
                self.refresh();
            }, 500);
        };
        UsersEditor.prototype.refresh = function () {
            this.grid.reload();
        };
        UsersEditor.prototype.render = function () {
            var _this = this;
            var self = this;
            //var users = this.state.users.map(function (u, i) {
            //    return <UserRow user={u} index={i } key={i} disableEdit={self.props.disableEdit}
            //                    editUser={self.editUser} deleteUser={self.deleteUser}
            //                    onSelect={self.props.onSelect}></UserRow>;
            //});
            var columns = [
                { name: 'username', title: Unicorn.localization.UserName },
                { name: 'name', title: Unicorn.localization.Name },
                { name: 'email', title: Unicorn.localization.Email },
                { name: 'roles', width: 120, title: Unicorn.localization.Roles },
                {
                    title: "", width: 90, render: function (id, u, col) {
                        return (<span>
                            {!self.props.disableEdit && <span>
                                <button type="button" className="btn btn-sm btn-primary" onClick={function () { return self.editUser(u); }}>
                                    <i className="fa fa-edit"></i>
                                </button>
                                <button type="button" className="btn btn-sm btn-danger" onClick={function () { return self.deleteUser(u); }}>
                                    <i className="fa fa-trash"></i>
                                </button>
                            </span>}
                            {self.props.onSelect &&
                            <button type="button" className="btn btn-sm btm-default" onClick={function () { return self.props.onSelect(u); }}>
                                    Select
                    </button>}
                        </span>);
                    },
                }
            ];
            return (<div>
                    <div className="row">
                        <div className="col col-md-3 col-md-6">
                            <Button bsStyle="primary" onClick={this.addUser}><i className="fa fa-plus"></i> Add New User</Button>
                        </div>
                        <div className="col col-md-3 col-md-6">
                            <div className="form-group">
                                <label>Search </label><input type="text" className="form-control" value={this.state.search} onChange={this.search}/>
                            </div>
                        </div>
                    </div>

                    <DataGrid dataSource={this.dataSource} defaultPageSize={10} idProperty='id' ref={function (g) { return _this.grid = g; }} columns={columns} style={{ height: 500 }}/>

                    <EditUser show={this.state.editingUser != null} title={(this.state.editingUser != null && this.state.editingUser.id) ? "Edit User" : "New User"} user={this.state.editingUser} onClose={this.closeDialog} userSaved={this.userSaved}>
                    </EditUser>
                </div>);
        };
        return UsersEditor;
    }(React.Component));
    Unicorn.UsersEditor = UsersEditor;
})();
//# sourceMappingURL=usersEditor.js.map