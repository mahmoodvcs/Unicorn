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
var User = /** @class */ (function () {
    function User() {
    }
    return User;
}());
var EditUserState = /** @class */ (function () {
    function EditUserState() {
        this.onClose = function () { };
    }
    return EditUserState;
}());
var EditUser = /** @class */ (function (_super) {
    __extends(EditUser, _super);
    function EditUser() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    EditUser.prototype.getInitialState = function () {
        return {
            user: {
                id: 0,
                username: '',
                password: '',
                name: '',
                email: '',
                type: 2
            }
        };
    };
    EditUser.prototype.componentWillReceiveProps = function (newprops) {
        var v = newprops.user;
        if (v && v.hasOwnProperty("id"))
            this.setState({ user: v });
        else
            this.setState(this.getInitialState());
    };
    EditUser.prototype.componentWillMount = function () {
        var v = this.props.user;
        if (v && v.hasOwnProperty("id"))
            this.setState({ user: v });
    };
    EditUser.prototype.onSubmit = function (e) {
        var self = this;
        e.preventDefault();
        $.ajax({
            url: getUrl("user"),
            method: this.state.user.id > 0 ? "POST" : "PUT",
            data: self.state.user,
        }).done(function (r) {
            self.props.userSaved();
            self.props.onClose();
        });
    };
    EditUser.prototype.onCancel = function () {
        this.props.onClose();
    };
    EditUser.prototype.resetPassword = function () {
        this.setState({ resettingPassword: true });
    };
    EditUser.prototype.render = function () {
        var _this = this;
        var u = this.state.user;
        var lu = Link.state(this, "user");
        var links = lu.pick("username", "password", "name", "email", "type");
        return (React.createElement("div", null,
            React.createElement(Dialog, { onOK: this.onSubmit, onCancel: this.onCancel, show: this.props.show, title: this.props.title },
                React.createElement(FieldGroup, { label: "User Name", readOnly: u.id > 0, valueLink: links.username }),
                u.id == 0 &&
                    React.createElement(FieldGroup, { label: "Password", type: "password", valueLink: links.password }),
                React.createElement(FieldGroup, { label: "Name", valueLink: links.name }),
                React.createElement(FieldGroup, { label: "Email Address", valueLink: links.email }),
                u.id == 0 &&
                    React.createElement(FieldGroup, { label: "User Type", type: "custom" },
                        React.createElement(UserTypeSelector, { valueLink: links.type })),
                u.id > 0 &&
                    React.createElement(Button, { type: "button", bsStyle: "warning", onClick: function () { return _this.resetPassword(); } },
                        React.createElement("i", { className: "fa fa-key" }),
                        " Reset Password")),
            React.createElement(ResetPassword, { onClose: function () { return _this.setState({ resettingPassword: false }); }, show: this.state.resettingPassword, user: u })));
    };
    return EditUser;
}(React.Component));
//# sourceMappingURL=EditUser.js.map