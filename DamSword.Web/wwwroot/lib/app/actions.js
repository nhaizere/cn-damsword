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

        method = method.toUpperCase();

        var xhr = new XMLHttpRequest();
        xhr.open(method, url, true);
        xhr.setRequestHeader('Content-type', 'application/json', true);
        xhr.setRequestHeader('X-Requested-With', 'XMLHttpRequest', true);

        xhr.onload = function (event) {
            var response = event.target.response;
            var result = response ? JSON.parse(response) : null;

            if (this.status >= 200 && this.status < 300) {
                var actionName;
                switch (method) {
                    case 'PUT':
                    case 'PATCH':
                        actionName = 'Updated';
                        break;
                    case 'DELETE':
                        actionName = 'Deleted';
                        break;
                    default:
                    case 'POST':
                        actionName = 'Saved';
                        break;
                }
                
                $.notify('success', successMessage || actionName + ' successfully.');
            } else {
                if (result && result.message)
                    errorMessage = result.message;

                $.notify('danger', this.status + ' ' + xhr.statusText + (errorMessage ? ': ' + errorMessage : ''));
            }
        }

        xhr.onerror = function () {
            $.notify('danger', this.status + ' ' + xhr.statusText + (errorMessage ? ' ' + errorMessage : ''));
        };

        var json = data ? JSON.stringify(data) : null;
        xhr.send(json);
    }
});
