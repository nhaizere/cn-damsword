(function() {
    var _origNotify = $.notify;
    $.notify = function(type, message) {
        var isViewportXs = ResponsiveBootstrapToolkit.is('xs');
        _origNotify({ message: message },
        {
            type: type,
            placement: {
                from: 'bottom',
                align: isViewportXs ? 'center' : 'right'
            },
            delay: 2800,
            offset: 20
        });
    }
})();