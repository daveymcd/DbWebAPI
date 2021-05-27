/// DbWebAPI.wwwroot.js.site.js - Site wide JaveScript (automatically loaded).
///

/// <summary>
///
///     GetModal() - Display Popver Modal
///
/// </summary>
/// <remarks>
///
///     This function is wired to all clickables with an 'onclick=GetModal(this);' attribute.
///     By specifying class="search", class="new" or class="edit", the event url points to 
///     the HTML of the handler (_PopupSearch.cshtml, _PopupNew.cshtml, _PopupEdit.cshtml 
///     respectively) which the JQuery will reference to load the partial view into the 
///     'popup-modal' Div of the Html imediately below here, and toggle the popup between 
///     hidden and visible.
///
///     Note: When table row is clicked (in <tr>), the query string is tacked onto the end of the URL.
///
/// </remarks>
function GetModal(e) {
    var placeholderPopup = $('#modal-insert');
    let url = e.getAttribute('data-url');                   // url points to handler                        
    let qry = e.getAttribute('href');                       // href contains query string to append to 'url'
    if (qry != null && qry.startsWith('&', 0)) { url = url + qry; };

    //$(this).find('.modal-dialog').css({                   // for larger popup window override the model-dialog CSS
    //    width: 'auto',
    //    height: '250px',
    //    'max-height': '300px'
    //});

    $.get(url).done(function (data) {
        placeholderPopup.html(data);                        // load HTML into 'modal-body' section of this view
        placeholderPopup.find('.modal').modal('show');      // aria-hidden is toggled to show popup
    });
}

//function onBegin() {
//    debugger
//    $("#documentList").html('<image src="/Img/spinner.gif" alt="Loading, please wait" />');
//}

//function onComplete() {
//    $("#documentList").html("");
//}

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
    $("#infoPopover").fadeOut('fast');                      // display error msg - delays 4 sec after the previous msg 
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
        case 300: msg = msg + "Server redirect error. Multipul choice request"; break
        case 302: msg = msg + "Server redirect error. Resource moved permanently"; break;
        case 304: msg = msg + "Server redirect error. Requested page not modified"; break;
        case 400: msg = msg + "Client error. Server understood the request, but request content was invalid."; break;
        case 401: msg = msg + "Client error. Unauthorized access."; break;
        case 402: msg = msg + "Client error. Payment Required."; break;
        case 403: msd = msg + "Client error. Forbidden resource can't be accessed."; break;
        case 404: msg = msg + "Client error. The document was not found."; break;
        case 405: msg = msg + "Client error. Method not allowed."; break
        case 408: msg = msg + "Client error. Request timeout"; break;
        case 500: msg = msg + "Internal server error."; break
        case 501: msg = msg + "Internal server error. Service not implemented"; break
        case 502: msg = msg + "Internal server error. Bad gateway"; break;
        case 503: msg = msg + "Internal server error. Service unavailable."; break;
        case 504: msg = msg + "Internal server error. Gateway timeout."; break;
        case 503: msg = msg + "Internal server error. HTTP version not supported."; break;
        default:  msg = msg + "Unknown error reported by server."; break;
    }

    if (ajaxOptions == 'parsererror')  { msg = msg + " - Parsing JSON Request failed."; }
    else if (ajaxOptions == 'timeout') { msg = msg + " - Request Time out."; }
    else if (ajaxOptions == 'abort')   { msg = msg + " - Request was aborted by the server"; }

    msg = msg + ' Please reload the Page or try again later.'

    PopoverMsg(value + " Document Error ", msg);debugger
}

