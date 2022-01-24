var scripts = [];
function load() {
    var path = 'js/pages/' + location.pathname.split('/').filter(function (a) { return a })[0] + '.js';
    if (!scripts.includes(path)) {
        scripts.push(path);
        $.getScript(path);
    }
}

var pushState = history.pushState;
history.pushState = function () {
    pushState.apply(history, arguments);
    load();
};

load();