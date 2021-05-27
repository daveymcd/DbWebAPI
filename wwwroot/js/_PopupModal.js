/// <summary>
///
///  DbWebApi.wwwroute.js._PopupModal.js - JavaScript for modal processing.
///
///  This JavaScript is called by DbWebAPI.Pages.Index.cshtml (via site.js) 
///  to handle the Modal processing.
///
///  Including the Ajax calls (page handlers are not invoked by form submits), 
///  the disposing and loading of partial views and and cancel verification.
///
/// </summary>



var createClicked;
var saveClicked;                                                    // Global Javascript Variables used by modal   
var deleteClicked;                                                  // Note: If initialised, each time the modal   
var copyClicked;                                                    // body is disposed and reloaded, their state  
var docChanged;                                                     // wil be destroyed. So declared as undefined  

/// <summary>
///
///     ConfirmAction() - Asks user to confirm action.
///
/// <summary>
/// <remarks>
///
///     When updating (Save, Delete, Create or Copy), Ask user to confirm action.
///
///     when leaving current record for a new one (Close, Copy, Delete, Prev or Next), 
///     if data has been amended but not saved, asked to confirm action.
///
///     when leaving (Close), if amendments have been made which affect
///     the parent document list (Save, Delete or Copy), a reload of the parent
///     view is forced, to ensure the document list is refreshed.
///
///
/// </remarks>
/// <param name="value">Name tag of element</param>
function ConfirmAction(value) {

    const selectedDoc = document.getElementById('selectedDoc').value;
    const selectedTS = document.getElementById('selectedTS').value;
    const documentMsg = 'Document ' + selectedDoc + ' - ' + selectedTS.substr(0, 10) + ' ' + selectedTS.substr(11, 5) + ' ';

    if ("Next Prev Close Copy Delete".match(value)) {               // If operation is Delete, Copy, Close, Next or Prev document 
        if (window.docChanged) {                                    // and unsaved amendments have been made, get confirmation 
            if (!confirm('\n\n    ' + documentMsg + ' has changed.\n    If you continue without saving, your changes will be lost.\n\nContinue ?')) {
                return false;
            }
        }
    }

    if ("Save Copy Delete Create".match(value)) {                   // If operation is Save Delete Create or Copy,         
        if (!confirm('\n\n' + value + ' ' + documentMsg)) {         // ask the user to confirm action                
            return false;
        }
    }

    if ('Close'.match(value)) {
        $('#modal-insert').find('.modal').modal('dispose');         // delete modal, force reload & reset flags     
        if (window.saveClicked || window.deleteClicked || window.copyClicked || window.createClicked) {
            window.saveClicked =  window.deleteClicked =  window.copyClicked =  window.createClicked = false;
            location.reload();
        }
    }

    window.docChanged = false;
    return true;
}

/// <summary>
///
///     DbWebAPI.wwwroot.js.PopupModal/PopupNew.GetNewModalContent()' - Change Modal Content.
///
/// </summary>
/// <remarks>
///
///     This JavaScript is wired to the modals buttons via onclick=GetNewModalContent().
///
///     This Javascript keeps the modal open while changing its content to the
///     partial view returned from the page handler.
///
///     'docChanged' is checked to see if the user has any unsaved
///     amendments. If so, the user is warned any unsaved amendments will
///     be lost and asked to confirm the action.
///
///     a GET is then performed (), The 'url' points to the event handler and
///     has 'href' appended to provide the query string.
///
///     The Get invokes a page handler which returns new content in the form
///     of a partial view (_PopupModal.cshtml or _PopupNew.cshtml).
///
///     This JavaScript will then dispose the old partial view and load the new one.
///
///     Note: All actions use GET as they result in the return of a new partialView
///           instead of returning to the parent Index (document list).
///
/// </remarks>
///
function GetNewModalContent(e) {

    const doc = e.getAttribute('data-doc');
    const selectedDoc = document.getElementById('selectedDoc').value;

    if (e.value == 'Save' && !window.docChanged) {
        PopoverMsg(null, 'No amendments to save');
        return false;
    }
    else if ('Save Delete Copy Create'.match(e.value) && selectedDoc == '')
    {
        PopoverMsg(e.value, 'Document Type is required.');
        return false;
    }
    else if (!ConfirmAction(e.value)) { return false; }

    $(e).html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> ');

    let url = e.getAttribute('data-url');                           // url points to handler                        
    let qry = e.getAttribute('href');                               // href contains query string to append to 'url'
    if (qry != null && qry.startsWith('&', 0)) { url = url + qry; };
    let formJson = null;
    let action = 'GET';
    if ("Save Create".match(e.value)) {                             // When 'Save' or 'Create', send the sCxItem     
        let form = $('.modal').find('form');                        // changes to OnGetSaveAsync or onGetCreateAsync.  
        //if (e.value == 'Create') { action = 'POST'; }
        formJson = form.serialize();
    }

    try {
        $.ajax({                                                    // Invoke event handler with a GET              
            type: action,                                           // a new partialView is returned for loading    
            url: url,
            data: formJson,
            contentType: false,
            processData: false,
            error:
                function (xhr, ajaxOptions, thrownError) {
                    AjaxError(xhr, ajaxOptions, thrownError, e.value);
                    $(e).html('');
                },
            success:
                function (data) {
                    $('#modal-insert').find('.modal').modal('dispose'); // delete old partial view   
                    $('#modal-insert').html(data);                      // load new partialView to existing 'modal-body'

                    if ('Save Delete Copy Create'.match(e.value)) {
                        let msg = "Undefined Error";
                        switch (e.value) {                              // Set flag to force document list reload on close 
                            case 'Save':
                                window.saveClicked = true;
                                window.docChanged = false;
                                msg = 'Document Saved'
                                break;
                            case 'Delete':
                                deleteClicked = true;
                                msg = 'Document Deleted'
                                break;
                            case 'Copy':
                                copyClicked = true;
                                msg = 'Document Copied'
                                break;
                            case 'Create':
                                createClicked = true;
                                msg = 'Document Created'
                                break;
                        }
                        $(e).html('');
                        PopoverMsg(null, msg);
                    }
                }
        });
    }
    catch (ex) {
        throw (ex);
    }
};

/// <summary>
///
///     'changable' Event Listner - records user has amended data.
///
/// </summary>
/// <remarks>
///
///     This anonymous JQuery input handler is wired to this modals
///     form fields via the class attribute class="changeable".
///
///     This Jquery marks data has been input using the 'docChanged' bool.
///     Later, when leaving the partial view, if data has been amended but
///     the save button has not been clicked, the user will be asked to
///     confirm they wish to leave the document without updating.
///
///
/// </remarks>
///
$('form').on('change', '*.changeable', function () {
    window.docChanged = true;
});

/// <summary>
///
///     'hidden.bs.modal' Event Listner - Checks if parent view requires reload on exit.
///
/// <summary>
/// <remarks>
///
///     This anonymous JQuery click handler is wired to the 'hide.bs.modal' event
///     and is invoked as the modal closes.
///
///     The handler checks to see if document has any unsaved amendedments. if so
///     the user is asked to confirm exit.
///
/// </remarks>
///*@
$('#popup-modal').on('hide.bs.modal', function (event) {
    if (!ConfirmAction('Close')) {
        event.preventDefault();
        event.stopImmediatePropagation();
        return false;
    }
    let rtn2 = $('form').off('change', '*.changeable');                    // remove anonymous change handler
    let rtn3 = $('#popup-modal').off('hide.bs.modal');                     // remove anonymous hide handler (this)
});
