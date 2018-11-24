declare var ReactCheckboxTree: any;

enum NodeAccessType {
    HasAccess,
    NoAccess,
    SubNodes
}
class AccessNode {
    parent: AccessNode = null;
    action: string;
    title: string;
    subActions: AccessNode[] = [];
    subActionsLoaded: boolean = false;
    access: NodeAccessType;
    hasChildren: boolean = false;
}

class AccessTreeState {
    actions: AccessNode[] = [];
    expanded: string[] = [];
}

interface AccessTreeProps {
    rootAction: string;
    userOrRoleId: string;
    hiddenInputId?: string;
}

class nodeShape {
    label: JSX.Element;
    value: string;
    icon?: JSX.Element;
    children?: nodeShape[];
};

const nodeSpecialValuePrefix = "$#@";

class AccessTree extends React.Component<AccessTreeProps, AccessTreeState>{
    checked: string[] = [];
    getUrl(u, o?): string {
        return getUrl("Unicorn_Security/" + u, o);
    }

    constructor(props) {
        super(props);
        this.onExpand = this.onExpand.bind(this);
        this.onCheck = this.onCheck.bind(this);
        this.state = new AccessTreeState();
    }

    async componentDidMount() {
        this.setState({ actions: await this.loadSubActions("") });
    }
    createNodes(actions: AccessNode[] = this.state.actions, parentAction: string = null)
        : { nodes: nodeShape[], checks: string[]/*, expanded: string[]*/ } {
        if (!actions) {
            return {
                checks: [],
                nodes: []
            };
        }
        var nodes: nodeShape[] = [];
        var checks: string[] = [];
        var expanded: string[] = [];
        for (var ac of actions) {
            var action = parentAction == null ? ac.action : parentAction + "." + ac.action;
            var node: nodeShape = {
                value: action,
                label: <span>{ac.title}</span>
            };
            nodes.push(node);

            //if (actions[i].expanded)
            //    expanded.push(action);
            if (ac.access == NodeAccessType.HasAccess)
                checks.push(action);

            if (ac.subActionsLoaded) {
                var sub = this.createNodes(ac.subActions, action);
                checks = checks.concat(sub.checks);
                node.children = sub.nodes;
                //expanded = expanded.concat(sub.expanded);
            }
            else if (ac.hasChildren) {
                node.children = [this.getNode("در حال بارگذاری ...", action + `.${nodeSpecialValuePrefix}loading`)];
                if (ac.access == NodeAccessType.SubNodes) {
                    node.children.push({
                        label: <span>...</span>,
                        value: action + `.${nodeSpecialValuePrefix}check`,
                    });
                    checks.push(action + `.${nodeSpecialValuePrefix}check`);
                }
                else if (ac.access == NodeAccessType.HasAccess) {
                    checks.push(node.children[0].value);
                }
            }
        }

        return {
            nodes, checks//, expanded
        };
    }
    getNode(label: string, value: string): nodeShape {
        return {
            label: <span>{label}</span>,
            value: value
        };
    }
    getDiff(ar1: string[], ar2: string[]): string[] {
        var diff: string[] = []
        for (var s of ar1) {
            if (ar2.indexOf(s) < 0)
                diff.push(s);
        }
        return diff;
    }
    findNode(actions: AccessNode[], value: string, actionOnParents?: (p: AccessNode) => void): AccessNode {
        if (value.indexOf(nodeSpecialValuePrefix) == 0)
            return null;
        var i = value.indexOf(".");
        var ac = value;
        var sub = "";
        if (i > 0) {
            ac = value.substring(0, i);
            sub = value.substring(i + 1);
        }
        else
            sub = null;
        for (var acn of actions) {
            if (acn.action == ac) {
                if (sub != null && sub.indexOf(nodeSpecialValuePrefix) != 0) {
                    if (!acn.subActions || acn.subActions.length == 0)
                        throw `Requested access action does not exist. parent: ${ac} - value: ${sub}`;
                    if (actionOnParents)
                        actionOnParents(acn);
                    return this.findNode(acn.subActions, sub, actionOnParents);
                }
                else
                    return acn;
            }
        }
        throw `Access action does not exist: ${value}`;
    }
    async onExpand(expanded: string[]) {
        var values = this.getDiff(expanded, this.state.expanded);
        this.setState({ expanded });

        if (values.length > 0) {
            if (values.length > 1) {
                throw "More than one expanded node: " + JSON.stringify(values);
            }

            var v = values[0];
            var actions = this.cloneActions();
            var node = this.findNode(actions, v);

            if (!node.subActionsLoaded) {
                node.subActions = await this.loadSubActions(v);
                node.subActionsLoaded = true;

                if (node.access != NodeAccessType.SubNodes) {
                    for (var ch of node.subActions)
                        ch.access = node.access;
                }

                for (var ch of node.subActions) {
                    ch.parent = node;
                }

                this.setState({ actions });
            }
        }
    }
    setAccessBasedOnChildren(node: AccessNode) {
        if (!node.subActions) {
            return;
        }

        for (var ch of node.subActions)
            this.setAccessBasedOnChildren(ch);
        if (node.subActions.every((n) => n.access == NodeAccessType.HasAccess))
            node.access = NodeAccessType.HasAccess;
        else if (node.subActions.every((n) => n.access == NodeAccessType.NoAccess))
            node.access = NodeAccessType.NoAccess;
        else
            node.access = NodeAccessType.SubNodes;
    }
    isEveryChildHasAccess(node: AccessNode, access: NodeAccessType) {
        if (!node.subActions)
            return true;

        return node.subActions.every((child) => {
            //if (child.subActions !== null) {
            //    return this.isEveryChildHasAccess(child, access);
            //}

            return child.access == access;
        });
    }
    setAccesses(actions: AccessNode[], checked: string[]) {
        for (var n of actions) {
            var action = this.getFullAction(n);
            if (!n.subActionsLoaded && n.hasChildren) {
                if (checked.indexOf(action + `.${nodeSpecialValuePrefix}loading`) >= 0)
                    n.access = NodeAccessType.HasAccess;
                else if (checked.indexOf(action + `.${nodeSpecialValuePrefix}check`) < 0)
                    n.access = NodeAccessType.NoAccess;
                else
                    n.access = NodeAccessType.SubNodes;
            }
            else if (checked.indexOf(action) >= 0)
                n.access = NodeAccessType.HasAccess;
            else if (n.access == NodeAccessType.HasAccess)
                n.access = NodeAccessType.NoAccess;
            if (n.subActions)
                this.setAccesses(n.subActions, checked);
        }
    }
    onCheck(checked: string[]) {
        var chState = true;
        //var newCheckes = this.getDiff(checked, this.checked);

        var actions = this.cloneActions();
        this.setAccesses(actions, checked);

        for (var n of actions) {
            this.setAccessBasedOnChildren(n);
        }

        this.setState({ actions });
        if (this.props.hiddenInputId) {
            (document.getElementById(this.props.hiddenInputId) as HTMLInputElement).value
                = JSON.stringify(this.getAuthorized(this.cloneActions(actions)));
        }
    }
    async loadSubActions(action: string): Promise<AccessNode[]> {
        var actions = await $.getJSON(this.getUrl("GetActions", { parentAction: action, userOrRoleId: this.props.userOrRoleId }));
        return actions;
    }
    cloneActions(actions: AccessNode[] = this.state.actions): AccessNode[] {
        actions = JSON.parse(JSON.stringify(actions, (k, v) => {
            if (k == "parent")
                return undefined;
            return v;
        }));

        for (var ch of actions) {
            this.setParent(ch);
        }
        return actions;
    }
    setParent(node: AccessNode) {
        if (node.subActions) {
            for (var ch of node.subActions) {
                ch.parent = node;
                this.setParent(ch);
            }
        }
    }
    getAuthorized(actions?: AccessNode[]) {
        actions = actions || this.cloneActions(this.state.actions);
        return this.stripUnused(actions);
    }
    stripUnused(actions) {
        for (var ac of actions) {
            delete ac.subActionsLoaded;
            delete ac.title;
            delete ac.hasChildren;
            delete ac.parent;
            if (ac.access != NodeAccessType.SubNodes)
                delete ac.subActions;
            else if (ac.subActions)
                this.stripUnused(ac.subActions);
        }
        return actions;
    }
    getFullAction(node: AccessNode): string {
        if (node.parent)
            return this.getFullAction(node.parent) + "." + node.action;
        return node.action;
    }

    save() {
        $.ajax(this.getUrl("SaveAccesses"), {
            method: "POST",
            data: {
                parentAction: this.props.rootAction,
                userOrRoleId: this.props.userOrRoleId,
                accesses: JSON.stringify(this.getAuthorized())
            },
            //contentType:"application/json"
        });
    }
    render() {
        var { nodes, checks } = this.createNodes();
        this.checked = checks;
        return (<div><ReactCheckboxTree nodes={nodes} checked={checks} expanded={this.state.expanded}
            onExpand={this.onExpand} onCheck={this.onCheck} showNodeIcon={false}>

        </ReactCheckboxTree>
            {this.props.hiddenInputId == null && <button type="button" className="btn btn-sucess" onClick={() => this.save()}>Save</button>}
        </div>);
    }
}