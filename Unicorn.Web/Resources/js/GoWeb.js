/*
 *  Copyright © Northwoods Software Corporation, 1998-2008. All Rights
 *  Reserved.
 *
 *  Restricted Rights: Use, duplication, or disclosure by the U.S.
 *  Government is subject to restrictions as set forth in subparagraph
 *  (c) (1) (ii) of DFARS 252.227-7013, or in FAR 52.227-19, or in FAR
 *  52.227-14 Alt. III, as applicable.
 *
 */
var goDX = 0; var goDY = 0;  // mouse down
var goCX = 0; var goCY = 0;  // mouse down client coords
var goUX = 0; var goUY = 0;  // mouse up
var goVX = 0; var goVY = 0;  // mouse up client coords
var goMenuID = '';  // current context menu
var goInfo = null;  // clicked part information
var goImg = null;  // current <IMG> element
var goImg2 = null;

var goSt = 0;
var goTm = null;
var goCp = null;
var goCMEH = null;
var goErr = null;

var goIE = (navigator.appVersion.indexOf('MSIE') >= 0);
var goSaf = (navigator.appVersion.indexOf('Safari') >= 0);
function goNSDocMove(e) {
  e.stopPropagation();
  return false;
}
function goNoOp(e) { return false; }

function goMouseDown(e, id) {
  goHideMenu(e);
  if (!goIE && !goSaf) {
    goCMEH = document.oncontextmenu;
    document.oncontextmenu = goNoOp;
  }
  goImg = goFindImg(id);
  goDX = goMouseX(e);
  goDY = goMouseY(e);
  goCX = e.clientX;
  goCY = e.clientY;
  goUX = goDX;
  goUY = goDY;
  goVX = goCX;
  goVY = goCY;
  if (goSt == 0) {
    goSt = 1;
    goTm = setTimeout('goTimeout("' + id + '", ' + goButton(e) + ', "' + goQuery(e) + '")', 500);
  } else if (goSt == 2) {
    goSt = 3;
  }
  if (e.srcElement && e.srcElement.setCapture) {
    goCp = e.srcElement;
    if (goCp.style.cursor == 'auto' || goCp.style.cursor == '') 
    {
      goCp.style.cursor = 'move';
    }
    goCp.setCapture();
  }
}
function goMouseUp(e, id) {
  if (goSt == 0) return;
  if (goCp && document.releaseCapture) {
    goCp.style.cursor = 'auto';
    document.releaseCapture();
    goCp = null;
  }
  goImg2 = goFindImg(id);
  goVX = e.clientX;
  goVY = e.clientY;
  var resetU = false;
  if (goIE && goImg.goADO && document.elementFromPoint(goVX, goVY) != goImg) {
    resetU = true;
  } else {
    goUX = goMouseX(e);
    goUY = goMouseY(e);
  }
  var moved = goMouseHasMoved();
  if (goSt == 1 || goSt == 4) {
    if (moved || goSt == 4) {
      goSt = 0;
      if (goTm) clearTimeout(goTm);
      if (moved) {
        if (!goNoClick(goButton(e), id)) return;
      } else {
        if (!goDoClick(goButton(e), 1, id)) return;
      }
      goPost(id, goQuery(e));
    } else {
      goSt = 2;
    }
  } else if (goSt == 3) {
    goSt = 0;
    if (goTm) clearTimeout(goTm);
    if (moved) {
      if (!goNoClick(goButton(e), id)) return;
    } else {
      if (!goDoClick(goButton(e), 2, id)) return;
    }
    goPost(id, goQuery(e) + (moved ? '' : '&dblclick=1'));
  }
  if (resetU) {
    goUX = goMouseX(e); goUY = goMouseY(e);
  }
}
function goDblClick(e, id) {
  if (goSt == 0) return;
  goSt = 0;
  if (goTm) clearTimeout(goTm);
  goImg = goFindImg(id);
  goUX = goMouseX(e);
  goUY = goMouseY(e);
  goVX = e.clientX;
  goVY = e.clientY;
  if (goMouseHasMoved()) {
    if (!goNoClick(0, id)) return;
  } else {
    if (!goDoClick(0, 2, id)) return;
  }
  goPost(id, goQuery(e) + '&dblclick=1');
}
function goTimeout(id, but, a) {
  goTm = null;
  if (goSt == 1) {
    goSt = 4;
  } else if (goSt == 2) {
    goSt = 0;
    if (goMouseHasMoved()) {
      if (!goNoClick(but, id)) return;
    } else {
      if (!goDoClick(but, 1, id)) return;
    }
    goPost(id, a);
  }
}

function goMouseX(e) {
  if (goIE) return e.offsetX; else return (e.pageX-e.target.x);
}
function goMouseY(e) {
  if (goIE) return e.offsetY; else return (e.pageY-e.target.y);
}
function goButton(e) {
  var b = e.button;
  if (goIE || goSaf) {
    if ((b&1) != 0) return 0;
    if ((b&2) != 0) return 2;
    if ((b&4) != 0) return 1;
    return 0;
  } else {
    return b;
  }
}
function goQuery(e) {
  var b = goButton(e);
  var q = 'downx=' + goDX + '&downy=' + goDY + '&upx=' + goUX + '&upy=' + goUY;
  if (e.ctrlKey)  q += '&ctrl=1';
  if (e.shiftKey) q += '&shift=1';
  if (e.altKey)   q += '&alt=1';
  if (b == 0) q += '&left=1';
  if (b == 1) q += '&middle=1';
  if (b == 2) q += '&right=1';
  return q;
}
function goMouseHasMoved() {
  return (Math.abs(goDX-goUX) >= 3 || Math.abs(goDY-goUY) >= 3);
}

function goKeyDown(e, id) {
  var c = e.keyCode;
  if (c == 9 || c == 16 || c == 17 || c == 18) return;
  if (goIE) e.cancelBubble = true; else e.preventBubble();
  var q = '';
  if (e.ctrlKey)  q += '&ctrl=1';
  if (e.shiftKey) q += '&shift=1';
  if (e.altKey)   q += '&alt=1';
  goPost(id, 'key=' + c + q);
}

function goScroll(dist, dx, dy, id) { goPost(id, 'scroll=' + dist + '&dx=' + dx + '&dy=' + dy); }
function goZoom(s, id) { goPost(id, 'zoom=' + s); }
function goResize(dx, dy, id) { goPost(id, 'resize=1&dx=' + dx + '&dy=' + dy); }
function goSize(w, h, id) { goPost(id, 'size&width=' + w + '&height=' + h); }
function goPosition(x, y, id) { goPost(id, 'position&x=' + x + '&y=' + y); }
function goAction(act, id) { goPost(id, 'act=' + act); }
function goRaiseEvent(id, name) { goPost(id, 'event=' + name + '&x=' + goDX + '&y=' + goDY); }
function goRequest(id, args) { goPost(id, 'request&' + args); }

function goFindImg(id) {
  return document.getElementById(id);
}
function goFindView(id) {
  var img = goFindImg(id);
  if (!img) return null;
  return img.goView;
}
function goInit(id, uid, goid, np, pg, refr, v, ado, pn) {
  var img = goFindImg(id);
  if (!img.goNoPost) {
    img.goUID = uid;
    img.goID = goid;
    img.goNoPost = np;
    img.goPage = pg;
    img.goSE = null;
    img.goC = 0;
    img.goP = pn;
    if (refr != '') {
      goAddUpdate(id, refr);
      goAddUpdate(refr, id);
    }
    var view = v;
    if (!view) view = eval('goview_'+id+'=new Object()');
    view.img = img;
    img.goView = view;
    img.goADO = ado;
    if (goSaf) {
      img.style['-khtml-user-drag'] = 'none';
    }
    goReloadData(img);
    if (v)
      goAfterLoad(id, false);
  } else {
    goReloadData(img);
    if (img != goImg2) return;
    var a = goImg2.goUpdates;
    if (a) {
      for (var i = 0; i < a.length; i++) {
        var img2 = a[i];
        if (typeof img2 == 'string') img2 = goFindImg(img2);
        if (typeof img2 == 'object') {
          goReload(img2, '');
        }
      }
    }
  }
}
function goAddUpdate(id, id2) {
  if (!id || !id2) return;
  var img = id;
  if (typeof img == 'string') img = goFindImg(img);
  var img2 = id2;
  if (typeof img2 == 'string') img2 = goFindImg(img2);
  var a = img.goUpdates;
  if (a) {
    for (var i = 0; i < a.length; i++) {
      if (a[i] == img2 || a[i] == img2.id) return;
    }
    a[a.length] = id2;
  } else {
    a = new Array();
    img.goUpdates = a;
    a[0] = id2;
  }
}

function goPost(id, args) {
  var img = goFindImg(id);
  if (!goIE && img == goImg2 && goImg != goImg2 && goImg.goADO) {
    if (goDoClick(0, 1, goImg.id)) goPost(goImg.id, args + '&upx=' + goDX + '&upy=' + goDY);
    args = 'drop&src=' + goImg.goID + '&' + args;
  }
  if (img && img.goNoPost) {
    goReload(img, args);
  } else if (img && img.goUID) {
    __doPostBack(img.goUID, args);
  } else {
    __doPostBack(id, args);
  }
}
function goReload(img, args) {
  if (!img) return;
  if (!args) args = '';
  img.goC++;
  var pg = img.goPage;
  var qidx = img.src.indexOf('?');
  if (qidx > 0) pg = img.src.substring(0, qidx);
  img.src = pg + '?GoView=' + img.goID + '&gp=' + img.goP + '&go=' + img.goC + ((args != '') ? ('&' + args) : '');
}
function goReloadData(img) {
  if (!img) return;
  if (goIE && document.readyState != 'complete') {
    setTimeout('goReloadData(goFindImg("' + img.id + '"))', 100);
    return;
  }
  if (img.goSE) document.body.removeChild(img.goSE);
  img.goSE = document.createElement('script');
  img.goSE.type = 'text/javascript';
  document.body.appendChild(img.goSE);
  var pg = img.goPage;
  var qidx = img.src.indexOf('?');
  if (qidx > 0) pg = img.src.substring(0, qidx);
  img.goSE.src = pg + '?GoViewData=' + img.goID + '&gp=' + img.goP + '&go=' + img.goC;
}

function goAfterLoad(id, reload) {
  if (typeof goOnLoad == 'function') goOnLoad(id, reload);
  goDoLoad(id, reload);
}
function goDoLoad(id, reload) {
  var img = goFindImg(id);
  if (img && img == goImg) {
    goErr = null;
    if (!goIE) return;
    var dst = document.elementFromPoint(goVX, goVY);
    if (dst && dst != img && dst.goID && img.goADO && goImg2 != dst) {
      goImg2 = dst;
      var q = '&downx=' + goDX + '&downy=' + goDY + '&upx=' + goUX + '&upy=' + goUY;
      goPost(dst.id, 'drop&src=' + img.goID + q);
      goNoClick(0, dst.id);
    }
  }
}
function goLoadError(id) {
  if (typeof goOnError == 'function') goOnError(id);
  goDoError(id);
}
function goDoError(id) {
  var img = goFindImg(id);
  if (img && !goErr) {
    goErr = id;
    setTimeout('location.reload()', 1000);
  }
}

function goMouseMove(e, id) {
  if (goCp) return;
  if (typeof goOnMouseOver == 'function') goOnMouseOver(e, id);
  goDoMouseOver(e, id);
}
function goDoMouseOver(e, id) {
  var v = goFindView(id);
  if (!v) return;
  var curarr = v.Cursors;
  var ttarr = v.ToolTips;
  var ttdef = v.ToolTipDefault;
  if (goIE) {
    var c = goSearchEvent(e, curarr, 'auto');
    if (c == 'pointer') c = 'hand';
    e.srcElement.style.cursor = c;
    e.srcElement.title = goSearchEvent(e, ttarr, ttdef);
  } else {
    var c = goSearchEvent(e, curarr, 'auto');
    e.target.style.cursor = c;
    e.target.title = goSearchEvent(e, ttarr, ttdef);
  }
}

function goSearchEvent(e, a, def) { return goSearchAt(goMouseX(e), goMouseY(e), a, def); }
function goSearchAt(x, y, a, def) {
  if (!a) return def;
  for (var i = 0; i < a.length; ) {
    var t = a[i]; i++;
    if (t == 0) {
      if (x >= a[i] && x < a[i]+a[i+2] && y >= a[i+1] && y < a[i+1]+a[i+3])
        return a[i+4];
      i += 5;
    } else if (t == 1) {
      var v = a[i]; i++;
      if (goInStroke(a, i, x, y))
        return v;
      i += 8+a[i+6]*2;
    } else {
      return def;
    }
  }
  return def;
}
function goInStroke(a, j, px, py) {
  var rx = a[j]; var ry = a[j+1]; var rw = a[j+2]; var rh = a[j+3];
  var fuzz = a[j+4];
  if (px < rx-fuzz || px > rx+rw+fuzz || py < ry-fuzz || py > ry+rh+fuzz)
    return false;
  var cubic = a[j+5];
  var numpts = a[j+6];
  if (numpts <= 1) return false;
  var lim = j+8 + numpts*2;
  if (cubic==1 && numpts >= 4) {
    fuzz = fuzz*Math.max(1, Math.max(rw, rh)/100);
    for (var i = j+8; i < lim; i += 6) {
      if (goInCubic(a[i], a[i+1], a[i+2], a[i+3], a[i+4], a[i+5], a[i+6], a[i+7], fuzz, px, py))
        return true;
    }
  } else {
    for (var i = j+8; i < lim; i += 2) {
      if (goInLine(a[i], a[i+1], a[i+2], a[i+3], fuzz, px, py))
        return true;
    }
  }
  return false;
}
function goInLine(ax, ay, bx, by, fuzz, px, py) {
  var maxx, minx, maxy, miny;
  if (ax < bx) {
    minx = ax; maxx = bx;
  } else {
    minx = bx; maxx = ax;
  }
  if (ay < by) {
    miny = ay; maxy = by;
  } else {
    miny = by; maxy = ay;
  }
  if (ax == bx) return (miny <= py && py <= maxy && ax-fuzz <= px && px <= ax+fuzz);
  if (ay == by) return (minx <= px && px <= maxx && ay-fuzz <= py && py <= ay+fuzz);
  var xrh = maxx+fuzz;
  var xrl = minx-fuzz;
  if ((xrl <= px) && (px <= xrh)) {
    var yrh = maxy+fuzz;
    var yrl = miny-fuzz;
    if ((yrl <= py) && (py <= yrh)) {
      if (xrh-xrl > yrh-yrl) {
        if (Math.abs(ax-bx) > fuzz) {
          var slope = (by-ay)/(bx-ax);
          var gy = (slope*(px-ax)+ay);
          if ((gy-fuzz <= py) && (py <= gy+fuzz))
            return true;
        } else {
          return true;
        }
      } else {
        if (Math.abs(ay-by) > fuzz) {
          var slope = (bx-ax)/(by-ay);
          var gx = (slope*(py-ay)+ax);
          if ((gx-fuzz <= px) && (px <= gx+fuzz))
            return true;
        } else {
          return true;
        }
      }
    }
  }
  return false;
}
function goInCubic(b0x, b0y, b1x, b1y, b2x, b2y, b3x, b3y, fuzz, px, py) {
  var c0x = b0x; var c0y = b0y;
  var c1x = (b0x+b1x)/2; var c1y = (b0y+b1y)/2;
  var c2x = (b1x+b2x)/2; var c2y = (b1y+b2y)/2;
  var c3x = (b2x+b3x)/2; var c3y = (b2y+b3y)/2;
  var c4x = b3x; var c4y = b3y;
  var d0x = c0x; var d0y = c0y;
  var d1x = (c0x+c1x)/2; var d1y = (c0y+c1y)/2;
  var d2x = (c1x+c2x)/2; var d2y = (c1y+c2y)/2;
  var d3x = (c2x+c3x)/2; var d3y = (c2y+c3y)/2;
  var d4x = (c3x+c4x)/2; var d4y = (c3y+c4y)/2;
  var d5x = c4x; var d5y = c4y;
  if (goInLine(d0x, d0y, d1x, d1y, fuzz, px, py)) return true;
  if (goInLine(d1x, d1y, d2x, d2y, fuzz, px, py)) return true;
  if (goInLine(d2x, d2y, d3x, d3y, fuzz, px, py)) return true;
  if (goInLine(d3x, d3y, d4x, d4y, fuzz, px, py)) return true;
  if (goInLine(d4x, d4y, d5x, d5y, fuzz, px, py)) return true;
  return false;
}

function goFindInfoEvent(e, id) { return goFindInfoAt(goMouseX(e), goMouseY(e), id); }
function goFindInfoAt(x, y, id) {
  var v = goFindView(id);
  if (!v) return null;
  var infoid = goSearchAt(x, y, v.Infos, null);
  if (infoid && v.InfoDefs) {
    return v.InfoDefs[infoid];
  }
  return null;
}
function goDoClick(but, cnt, id) {  // do only client-side behavior if returns false
  goInfo = goFindInfoAt(goDX, goDY, id);
  var i = goInfo;
  if (but == 0) {
    var v = goFindView(id);
    if (cnt >= 2) {
      if (i && i.DoubleClick)
        return goEval(i.DoubleClick);
      else if (v && v.DoubleClickDefault)
        return goEval(v.DoubleClickDefault);
    } else {
      if (i && i.SingleClick)
        return goEval(i.SingleClick);
      else if (v && v.SingleClickDefault)
        return goEval(v.SingleClickDefault);
    }
  } else if (but == 2) {
    var v = goFindView(id);
    if (i && i.ContextClick)
      return goEval(i.ContextClick);
    else if (v && v.ContextClickDefault)
      return goEval(v.ContextClickDefault);
    else if (goTryMenu(v))
      return true;
  }
  return goNoClick(but, id);
}
function goNoClick(but, id) {
  var v = goFindView(id);
  if (!v) return true;
  if (v.NoClick)
    return goEval(v.NoClick);
  return true;
}
function goEval(x) {
  var r = eval(x);
  if (r == null) return true;
  return r;
}

function goTryMenu(v) {  // show any context menu
  if (!v || !v.img.goNoPost) return false;
  var m = goSearchAt(goDX, goDY, v.Menus, '');
  if (m == '') m = v.MenuDefault;
  var id = goFindMenu(v, m);
  if (goShowMenu(id)) return false;
  return true;
}
function goFindMenu(v, m) {
  if (document.getElementById(m)) return m;
  if (!v || !v.MenuDefs) return null;
  var def = v.MenuDefs[m];
  if (!def) return null;
  if (typeof def == 'string') {
    return def;
  } else {
    return goBuildMenu(def, v);
  }
}
function goBuildMenu(a, v) {
  var m = document.createElement('div');
  mid = 'go_ContextMenu';
  m.id = mid;
  m.className = 'goContextMenu';
  for (var i = 1; i < a.length; i++) {
    var b = a[i];
    if ((b[1]&1) != 0) { // Visible
      var mi = document.createElement('div');
      if (b[0] == '-') {
        mi.className = 'goMenuItemSeparator';
      } else {
        if ((b[1]&2) != 0) { // Enabled
          mi.className = 'goMenuItem';
          mi.innerHTML = b[0];
          var eh;
          if ((b[1]&8) != 0) { // ServerEvent
            eh = new Function('goRaiseEvent("' + v.img.id + '", "' + b[2] + '")');
          } else {
            eh = new Function(b[2]);
          }
          if (goIE)
            mi.attachEvent('onclick', eh);
          else
            mi.addEventListener('click', eh, false);
        } else {
          mi.className = 'goMenuItemDisabled';
          mi.innerHTML = b[0];
        }
      }
      m.appendChild(mi);
    }
  }
  var old = document.getElementById(mid);
  if (old) old.parentNode.removeChild(old);
  document.body.appendChild(m);
  return mid;
}
function goShowMenu(menuid) {
  var x = goCX + document.body.scrollLeft;
  var y = goCY + document.body.scrollTop;
  var m = document.getElementById(menuid);
  if (!m) {
    if (menuid && menuid != '') {
      alert('Menu not found: ' + menuid);
    }
    return false;
  }
  if (!m.className || m.className == '') {
    alert('Menu does not have CSS class: ' + menuid);
    return false;
  }
  goMenuID = menuid;
  m.style.left = x.toString()+"px";
  m.style.top = y.toString()+"px";
  m.style.visibility = 'visible';
  return true;
}
function goHideMenu(e) {
  if (goMenuID == '') return;
  var m = document.getElementById(goMenuID);
  if (!m) return;
  m.style.visibility = 'hidden';
  if (!goIE && !goSaf) {
    document.oncontextmenu = goCMEH;
  }
}
function goDisplay(yes) { // varargs after boolean
  var a = goDisplay.arguments;
  var s = 'inline';
  if (!yes) s = 'none';
  for (var i = 1; i < a.length; i++) {
    var pid = a[i];
    if (pid) {
      var panel = document.getElementById(pid);
      if (panel) {
        panel.style.display = s;
      }
    }
  }
}
function goShow(id) {
  var x = document.getElementById(id);
  if (x) {
    x.style.display = 'inline';
  }
}
function goHide(id) {
  var x = document.getElementById(id);
  if (x) {
    x.style.display = 'none';
  }
}
function goShowPanel(pid, tbid, text) {
  goShow(pid);
  var tc = document.getElementById(tbid);
  if (tc) {
    tc.value = text;
  }
}

if (goIE) {
  document.attachEvent('onclick', goHideMenu);
} else {
  document.addEventListener('click', goHideMenu, false);
  document.addEventListener('draggesture', goNSDocMove, false);
}
