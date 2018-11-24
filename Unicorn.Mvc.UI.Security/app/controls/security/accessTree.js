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
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : new P(function (resolve) { resolve(result.value); }).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
var __generator = (this && this.__generator) || function (thisArg, body) {
    var _ = { label: 0, sent: function() { if (t[0] & 1) throw t[1]; return t[1]; }, trys: [], ops: [] }, f, y, t, g;
    return g = { next: verb(0), "throw": verb(1), "return": verb(2) }, typeof Symbol === "function" && (g[Symbol.iterator] = function() { return this; }), g;
    function verb(n) { return function (v) { return step([n, v]); }; }
    function step(op) {
        if (f) throw new TypeError("Generator is already executing.");
        while (_) try {
            if (f = 1, y && (t = y[op[0] & 2 ? "return" : op[0] ? "throw" : "next"]) && !(t = t.call(y, op[1])).done) return t;
            if (y = 0, t) op = [0, t.value];
            switch (op[0]) {
                case 0: case 1: t = op; break;
                case 4: _.label++; return { value: op[1], done: false };
                case 5: _.label++; y = op[1]; op = [0]; continue;
                case 7: op = _.ops.pop(); _.trys.pop(); continue;
                default:
                    if (!(t = _.trys, t = t.length > 0 && t[t.length - 1]) && (op[0] === 6 || op[0] === 2)) { _ = 0; continue; }
                    if (op[0] === 3 && (!t || (op[1] > t[0] && op[1] < t[3]))) { _.label = op[1]; break; }
                    if (op[0] === 6 && _.label < t[1]) { _.label = t[1]; t = op; break; }
                    if (t && _.label < t[2]) { _.label = t[2]; _.ops.push(op); break; }
                    if (t[2]) _.ops.pop();
                    _.trys.pop(); continue;
            }
            op = body.call(thisArg, _);
        } catch (e) { op = [6, e]; y = 0; } finally { f = t = 0; }
        if (op[0] & 5) throw op[1]; return { value: op[0] ? op[1] : void 0, done: true };
    }
};
var NodeAccessType;
(function (NodeAccessType) {
    NodeAccessType[NodeAccessType["HasAccess"] = 0] = "HasAccess";
    NodeAccessType[NodeAccessType["NoAccess"] = 1] = "NoAccess";
    NodeAccessType[NodeAccessType["SubNodes"] = 2] = "SubNodes";
})(NodeAccessType || (NodeAccessType = {}));
var AccessNode = /** @class */ (function () {
    function AccessNode() {
        this.parent = null;
        this.subActions = [];
        this.subActionsLoaded = false;
        this.hasChildren = false;
    }
    return AccessNode;
}());
var AccessTreeState = /** @class */ (function () {
    function AccessTreeState() {
        this.actions = [];
        this.expanded = [];
    }
    return AccessTreeState;
}());
var nodeShape = /** @class */ (function () {
    function nodeShape() {
    }
    return nodeShape;
}());
;
var nodeSpecialValuePrefix = "$#@";
var AccessTree = /** @class */ (function (_super) {
    __extends(AccessTree, _super);
    function AccessTree(props) {
        var _this = _super.call(this, props) || this;
        _this.checked = [];
        _this.onExpand = _this.onExpand.bind(_this);
        _this.onCheck = _this.onCheck.bind(_this);
        _this.state = new AccessTreeState();
        return _this;
    }
    AccessTree.prototype.getUrl = function (u, o) {
        return getUrl("Unicorn_Security/" + u, o);
    };
    AccessTree.prototype.componentDidMount = function () {
        return __awaiter(this, void 0, void 0, function () {
            var _a, _b;
            return __generator(this, function (_c) {
                switch (_c.label) {
                    case 0:
                        _a = this.setState;
                        _b = {};
                        return [4 /*yield*/, this.loadSubActions("")];
                    case 1:
                        _a.apply(this, [(_b.actions = _c.sent(), _b)]);
                        return [2 /*return*/];
                }
            });
        });
    };
    AccessTree.prototype.createNodes = function (actions, parentAction) {
        if (actions === void 0) { actions = this.state.actions; }
        if (parentAction === void 0) { parentAction = null; }
        if (!actions) {
            return {
                checks: [],
                nodes: []
            };
        }
        var nodes = [];
        var checks = [];
        var expanded = [];
        for (var _i = 0, actions_1 = actions; _i < actions_1.length; _i++) {
            var ac = actions_1[_i];
            var action = parentAction == null ? ac.action : parentAction + "." + ac.action;
            var node = {
                value: action,
                label: React.createElement("span", null, ac.title)
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
                node.children = [this.getNode("در حال بارگذاری ...", action + ("." + nodeSpecialValuePrefix + "loading"))];
                if (ac.access == NodeAccessType.SubNodes) {
                    node.children.push({
                        label: React.createElement("span", null, "..."),
                        value: action + ("." + nodeSpecialValuePrefix + "check"),
                    });
                    checks.push(action + ("." + nodeSpecialValuePrefix + "check"));
                }
                else if (ac.access == NodeAccessType.HasAccess) {
                    checks.push(node.children[0].value);
                }
            }
        }
        return {
            nodes: nodes, checks: checks //, expanded
        };
    };
    AccessTree.prototype.getNode = function (label, value) {
        return {
            label: React.createElement("span", null, label),
            value: value
        };
    };
    AccessTree.prototype.getDiff = function (ar1, ar2) {
        var diff = [];
        for (var _i = 0, ar1_1 = ar1; _i < ar1_1.length; _i++) {
            var s = ar1_1[_i];
            if (ar2.indexOf(s) < 0)
                diff.push(s);
        }
        return diff;
    };
    AccessTree.prototype.findNode = function (actions, value, actionOnParents) {
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
        for (var _i = 0, actions_2 = actions; _i < actions_2.length; _i++) {
            var acn = actions_2[_i];
            if (acn.action == ac) {
                if (sub != null && sub.indexOf(nodeSpecialValuePrefix) != 0) {
                    if (!acn.subActions || acn.subActions.length == 0)
                        throw "Requested access action does not exist. parent: " + ac + " - value: " + sub;
                    if (actionOnParents)
                        actionOnParents(acn);
                    return this.findNode(acn.subActions, sub, actionOnParents);
                }
                else
                    return acn;
            }
        }
        throw "Access action does not exist: " + value;
    };
    AccessTree.prototype.onExpand = function (expanded) {
        return __awaiter(this, void 0, void 0, function () {
            var values, v, actions, node, _a, _i, _b, ch, _c, _d, ch;
            return __generator(this, function (_e) {
                switch (_e.label) {
                    case 0:
                        values = this.getDiff(expanded, this.state.expanded);
                        this.setState({ expanded: expanded });
                        if (!(values.length > 0)) return [3 /*break*/, 2];
                        if (values.length > 1) {
                            throw "More than one expanded node: " + JSON.stringify(values);
                        }
                        v = values[0];
                        actions = this.cloneActions();
                        node = this.findNode(actions, v);
                        if (!!node.subActionsLoaded) return [3 /*break*/, 2];
                        _a = node;
                        return [4 /*yield*/, this.loadSubActions(v)];
                    case 1:
                        _a.subActions = _e.sent();
                        node.subActionsLoaded = true;
                        if (node.access != NodeAccessType.SubNodes) {
                            for (_i = 0, _b = node.subActions; _i < _b.length; _i++) {
                                ch = _b[_i];
                                ch.access = node.access;
                            }
                        }
                        for (_c = 0, _d = node.subActions; _c < _d.length; _c++) {
                            ch = _d[_c];
                            ch.parent = node;
                        }
                        this.setState({ actions: actions });
                        _e.label = 2;
                    case 2: return [2 /*return*/];
                }
            });
        });
    };
    AccessTree.prototype.setAccessBasedOnChildren = function (node) {
        if (!node.subActions) {
            return;
        }
        for (var _i = 0, _a = node.subActions; _i < _a.length; _i++) {
            var ch = _a[_i];
            this.setAccessBasedOnChildren(ch);
        }
        if (node.subActions.every(function (n) { return n.access == NodeAccessType.HasAccess; }))
            node.access = NodeAccessType.HasAccess;
        else if (node.subActions.every(function (n) { return n.access == NodeAccessType.NoAccess; }))
            node.access = NodeAccessType.NoAccess;
        else
            node.access = NodeAccessType.SubNodes;
    };
    AccessTree.prototype.isEveryChildHasAccess = function (node, access) {
        if (!node.subActions)
            return true;
        return node.subActions.every(function (child) {
            //if (child.subActions !== null) {
            //    return this.isEveryChildHasAccess(child, access);
            //}
            return child.access == access;
        });
    };
    AccessTree.prototype.setAccesses = function (actions, checked) {
        for (var _i = 0, actions_3 = actions; _i < actions_3.length; _i++) {
            var n = actions_3[_i];
            var action = this.getFullAction(n);
            if (!n.subActionsLoaded && n.hasChildren) {
                if (checked.indexOf(action + ("." + nodeSpecialValuePrefix + "loading")) >= 0)
                    n.access = NodeAccessType.HasAccess;
                else if (checked.indexOf(action + ("." + nodeSpecialValuePrefix + "check")) < 0)
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
    };
    AccessTree.prototype.onCheck = function (checked) {
        var chState = true;
        //var newCheckes = this.getDiff(checked, this.checked);
        var actions = this.cloneActions();
        this.setAccesses(actions, checked);
        for (var _i = 0, actions_4 = actions; _i < actions_4.length; _i++) {
            var n = actions_4[_i];
            this.setAccessBasedOnChildren(n);
        }
        this.setState({ actions: actions });
        if (this.props.hiddenInputId) {
            document.getElementById(this.props.hiddenInputId).value
                = JSON.stringify(this.getAuthorized(this.cloneActions(actions)));
        }
    };
    AccessTree.prototype.loadSubActions = function (action) {
        return __awaiter(this, void 0, void 0, function () {
            var actions;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0: return [4 /*yield*/, $.getJSON(this.getUrl("GetActions", { parentAction: action, userOrRoleId: this.props.userOrRoleId }))];
                    case 1:
                        actions = _a.sent();
                        return [2 /*return*/, actions];
                }
            });
        });
    };
    AccessTree.prototype.cloneActions = function (actions) {
        if (actions === void 0) { actions = this.state.actions; }
        actions = JSON.parse(JSON.stringify(actions, function (k, v) {
            if (k == "parent")
                return undefined;
            return v;
        }));
        for (var _i = 0, actions_5 = actions; _i < actions_5.length; _i++) {
            var ch = actions_5[_i];
            this.setParent(ch);
        }
        return actions;
    };
    AccessTree.prototype.setParent = function (node) {
        if (node.subActions) {
            for (var _i = 0, _a = node.subActions; _i < _a.length; _i++) {
                var ch = _a[_i];
                ch.parent = node;
                this.setParent(ch);
            }
        }
    };
    AccessTree.prototype.getAuthorized = function (actions) {
        actions = actions || this.cloneActions(this.state.actions);
        return this.stripUnused(actions);
    };
    AccessTree.prototype.stripUnused = function (actions) {
        for (var _i = 0, actions_6 = actions; _i < actions_6.length; _i++) {
            var ac = actions_6[_i];
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
    };
    AccessTree.prototype.getFullAction = function (node) {
        if (node.parent)
            return this.getFullAction(node.parent) + "." + node.action;
        return node.action;
    };
    AccessTree.prototype.save = function () {
        $.ajax(this.getUrl("SaveAccesses"), {
            method: "POST",
            data: {
                parentAction: this.props.rootAction,
                userOrRoleId: this.props.userOrRoleId,
                accesses: JSON.stringify(this.getAuthorized())
            },
        });
    };
    AccessTree.prototype.render = function () {
        var _this = this;
        var _a = this.createNodes(), nodes = _a.nodes, checks = _a.checks;
        this.checked = checks;
        return (React.createElement("div", null,
            React.createElement(ReactCheckboxTree, { nodes: nodes, checked: checks, expanded: this.state.expanded, onExpand: this.onExpand, onCheck: this.onCheck, showNodeIcon: false }),
            this.props.hiddenInputId == null && React.createElement("button", { type: "button", className: "btn btn-sucess", onClick: function () { return _this.save(); } }, "Save")));
    };
    return AccessTree;
}(React.Component));
//# sourceMappingURL=accessTree.js.map