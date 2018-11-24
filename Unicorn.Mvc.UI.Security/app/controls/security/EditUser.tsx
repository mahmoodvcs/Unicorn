class User {
    UserName: string;
    Password: string;
    FirstName: string;
}
class EditUserState {
    onClose: () => void = () => { };
    show: boolean;
    title: string;
}
class EditUser extends React.Component<any, any>
    {
    propTypes: {
        //user: React.PropTypes.instanceOf(Object).isRequired,
    }
    getInitialState() {
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
    }
    componentWillReceiveProps(newprops) {
        var v = newprops.user;
        if (v && v.hasOwnProperty("id"))
            this.setState({ user: v });
        else
            this.setState(this.getInitialState());
    }
    componentWillMount() {
        var v = this.props.user;
        if (v && v.hasOwnProperty("id"))
            this.setState({ user: v });
    }
    onSubmit(e) {
        var self = this;
        e.preventDefault();
        $.ajax({
            url: getUrl("user"),
            method: this.state.user.id > 0 ? "POST" : "PUT",
            data: self.state.user,
        }).done(function (r) {
            self.props.userSaved();
            self.props.onClose();
        })
    }
    onCancel() {
        this.props.onClose();
    }
    resetPassword() {
        this.setState({ resettingPassword: true });
    }
    render() {
        var u = this.state.user;
        var lu = Link.state(this, "user");
        var links = lu.pick("username", "password", "name", "email", "type");
        return (<div>
            <Dialog onOK={this.onSubmit} onCancel={this.onCancel} show={this.props.show}
                title={this.props.title}>
                <FieldGroup label="User Name" readOnly={u.id > 0} valueLink={links.username}></FieldGroup>
                {u.id == 0 &&
                    <FieldGroup label="Password" type="password" valueLink={links.password}></FieldGroup>}
                <FieldGroup label="Name" valueLink={links.name}></FieldGroup>
                <FieldGroup label="Email Address" valueLink={links.email}></FieldGroup>
                {u.id == 0 &&
                    <FieldGroup label="User Type" type="custom">
                        <UserTypeSelector valueLink={links.type}></UserTypeSelector>
                    </FieldGroup>}
                {u.id > 0 &&
                    <Button type="button" bsStyle="warning" onClick={() => this.resetPassword()}>
                        <i className="fa fa-key"></i> Reset Password
                    </Button>}
            </Dialog>
            <ResetPassword onClose={() => this.setState({ resettingPassword: false })}
                show={this.state.resettingPassword} user={u}>
            </ResetPassword>

        </div>);
    }
}