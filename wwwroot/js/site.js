/// DbWebAPI.wwwroot.js.site.js - Site wide JaveScript (automatically loaded).


/// <summary>
///
///     PopoverMsg() - Display Popver Message on Modal
///
///     Uses an empty <span id="infoPopover" data-toggle="popover" />
///
/// <summary>
/// <param name="heading">popover heading</param>
/// <param name="content">popover content</param>
///
function PopoverMsg(heading = null, content = null) {
    $('#infoPopover').popover('hide')
    $('#infoPopover').attr("data-original-title", heading);
    $('#infoPopover').attr("data-content", content);
    $('#infoPopover').popover('show');
    $("#infoPopover").fadeOut('fast');        // display error msg - delays 4 sec after the previous msg 
    $("#infoPopover").fadeIn('slow');
    setTimeout(function () { $('#infoPopover').popover('hide') }, 3000);
}

/// <summary>
///
///     AjaxError() - Output Ajax Error Message to Modal Popover
///
/// <summary>
/// <param name="xhr">xht from Ajax call</param>
/// <param name="ajaxOtions">From Ajax call</param>
/// <param name="thrownError">rtn form Ajax Call</param>
/// <param name="value">Client Request (Save, Delete etc)</param>
function AjaxError(xhr, ajaxOptions, thrownError, value) {

    let msg = thrownError + " " + xhr.statusText + ':' + xhr.responseText + '-' + ' [' + xhr.status + '] ';

    switch (xhr.status) {
        case 000: msg = msg + "Not connected.\n Please verify your network connection."; break;
        case 400: msg = msg + "Server understood the request, but request content was invalid."; break;
        case 401: msg = msg + "Unauthorized access."; break;
        case 403: msd = msg + "Forbidden resource can't be accessed."; break;
        case 404: msg = msg + "The document was not found."; break;
        case 500: msg = msg + "Internal server error."; break;
        case 503: msg = msg + "Service unavailable."; break;
        default:  msg = msg + "Unknown error reported by server."; break;
    }

    if (ajaxOptions == 'parsererror')  { msg = msg + " - Parsing JSON Request failed."; }
    else if (ajaxOptions == 'timeout') { msg = msg + " - Request Time out."; }
    else if (ajaxOptions == 'abort')   { msg = msg + " - Request was aborted by the server"; }

    msg = msg + ' Please reload the Page or try again later.'

    PopoverMsg(value + " Document Error ", msg);
}

