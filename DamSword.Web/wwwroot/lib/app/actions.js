$(function() {
    var $actions = $('[data-ajax-action], [data-ajax-method]');
    $actions.each(function () {
        var $el = $(this);
        var url = $el.data('ajax-action') || $el.attr('href');
        var method = $el.data('ajax-method') || 'get';
        var data = $el.data('ajax-data') || null;
        var successMessage = $el.data('ajax-success') || undefined;
        var errorMessage = $el.data('ajax-error') || undefined;

        $el.click(_.partial(invokeAjaxAction, url, method, data, successMessage, errorMessage));
    });

    function invokeAjaxAction(url, method, data, successMessage, errorMessage, event) {
        event.preventDefault();

        var xhr = new XMLHttpRequest();
        xhr.open(method, url, true);
        xhr.setRequestHeader('Content-type', 'application/json', true);
        xhr.onload = function() {
            if (this.status >= 200 && this.status < 300) {

            } else {
                $.notify('danger', errorMessage || this.status + ' ' + xhr.statusText);
            }
        }

        xhr.onerror = function () {
            $.notify('danger', errorMessage || this.status + ' ' + xhr.statusText);
        };

        var json = data ? JSON.stringify(data) : null;
        xhr.send(json);
    }
});
