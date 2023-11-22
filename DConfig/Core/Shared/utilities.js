String.prototype.contains = function (it) { return this.indexOf(it) != -1; };
String.prototype.startWith = function (it) { return this.indexOf(it) == 0; };
String.prototype.endsWith = function (suffix) { return this.indexOf(suffix, this.length - suffix.length) !== -1; };
function isString(o) {
    return typeof o == "string" || (typeof o == "object" && o.constructor === String);
}
Array.prototype.contains = function (obj, property) {
    var i = this.length;
    while (i--) {
        if (property != null) {
            if (this[i][property] === obj[property]) {
                return true;
            }
        } else {
            if (this[i] == obj) {
                return true;
            }
        }
    }
    return false;
}

Array.prototype.containsById = function (property, id) {
    var i = this.length;
    while (i--) {
        if (this[i][property] == id) {
            return true;
        }
    }
    return false;
}

Array.prototype.spliceItem = function (obj, property) {
    var i = this.length;
    while (i--) {
        if (property != null) {
            if (this[i][property] === obj[property]) {
                this.splice(i, 1);
            }
        } else {
            if (this[i] == obj) {
                this.splice(i, 1);
            }
        }
    }
}

Array.prototype.spliceById = function (property, id) {
    var i = this.length;
    while (i--) {
        if (this[i][property] == id) {
            this.splice(i, 1);
        }
    }
}

function getQueryStringParameterByName(name, url) {
    if (!url) url = window.location.href;
    name = name.replace(/[\[\]]/g, "\\$&");
    var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"),
        results = regex.exec(url);
    if (!results) return null;
    if (!results[2]) return '';
    return decodeURIComponent(results[2].replace(/\+/g, " "));
}