//  DbWebApi.wwwroute.js._PopupEdit.js - JavaScript for Edit modal of same name.


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
///     When updating (Save, Delete or Copy), Ask user to confirm action.
///
///     when leaving (Close Prev or Next), if data has been amended but not
///     saved, asked to confirm action.
///
///     when leaving (Close), if amendments have been made which affect
///     the parent document list (Save, Delete or Copy), a reload of the parent
///     view is forced, to ensure the document list is refreshed.
///
///
/// </remarks>
/// <param name="value">Name tag of element</param>
function ConfirmAction(value, docMsg) {

    if ("Next Prev Close Copy Delete".match(value)) {               // If operation is Delete, Copy, Close, Next or Prev document 
        if (window.docChanged) {                                    // and amendments are unsaved, get confirmation 
            if (!confirm('\n\n    ' + docMsg + ' has changed.\n    If you continue without saving, your changes will be lost.\n\nContinue ?')) {
                return false;
            }
        }
    }

    if ("Save Copy Delete".match(value)) {                          // If operation is Save Delete or Copy,         
        if (!confirm('\n\n' + value + ' ' + docMsg)) {              // ask the user to confirm action                
            return false;
        }
    }

    if (value == 'Close') {
        $('#modal-insert').find('.modal').modal('dispose');         // delete modal, force reload & reset flags     
        if (window.saveClicked || window.deleteClicked || window.copyClicked) {
            window.saveClicked  = window.deleteClicked  = window.copyClicked = false;
            location.reload();
        }
    }

    window.docChanged = false;
    return true;
}

/// <summary>
///
///     'GetNewModalContent' Event Listner - change content of modal.
///
/// </summary>
/// <remarks>
///
///     This anonymous JQuery click handler is wired to the modals
///     buttons via the attribute <div id="GetNewModalContent">.
///
///     This JQuery keeps the modal open while changing its content to the
///     partial view returned from the page handler.
///
///     'docChanged' is checked to see if the user has any unsaved
///     amendments. If so, the user is warned any unsaved amendments will
///     be lost and asked to confirm the action.
///
///     a GET is then performed (Note: GET is used because all actions result in 
///     the return of a new partialView), The 'url' points to the event handler and
///     has 'href' appended to provide the query string.
///
///     The Get invokes a page handler which returns new content in the form
///     of a partial view (_PopupEdit.cshtml).
///
///     This Jquery will then dispose the old partial view and load the new one.
///
/// </remarks>
///
$('button[id="GetNewModalContent"]').click(function (event) {

    const value = this.getAttribute('value');
    const docMsg = this.getAttribute('data-docMsg');
    if (value == "Save" && !window.docChanged) {
        PopoverMsg(null, "No amendments to save");
        return false;
    }
    if (!ConfirmAction(value, docMsg)) { return false; }

    let url = $(this).data('url');                                  // url points to handler                        
    let qry = this.getAttribute('href');                            // href contains query string to append to 'url'
    if (qry != null && qry.startsWith("&", 0)) { url = url + qry; };

    let formJson = null;
    if (value == "Save") {                                          // When 'Save', send the sCxItem changes to     
        let form = $(this).parents('.modal').find('form');          // OnGetSaveAsync                               
        formJson = form.serialize();
    }

    $.ajax({                                                        // Invoke event handler with a GET              
        type: "GET",                                                // a new partialView is returned for loading    
        url: url,
        data: formJson,
        error:
            function (xhr, ajaxOptions, thrownError) {
                AjaxError(xhr, ajaxOptions, thrownError, value);
            },
        success:
            function (data) {
                $('#modal-insert').find('.modal').modal('dispose'); // delete old partial view                      
                $('#modal-insert').html(data);                      // load new partialView to existing 'modal-body'

                if ("Save Delete Copy".match(value)) {
                    let msg = "Undefined Error";
                    switch (value) {                                // Set flag to force document list reload on close 
                        case 'Save':
                            window.saveClicked = true;
                            window.docChanged = false;
                            msg = "Document Saved"
                            break;
                        case 'Delete':
                            deleteClicked = true;
                            msg = "Document Deleted"
                            break;
                        case 'Copy':
                            copyClicked = true;
                            msg = "Document Copied"
                            break;
                    }
                    PopoverMsg(null, msg);
                }
            }
    });
});

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
    ele = document.getElementById('Close');
    const docMsg = ele.getAttribute('data-docMsg');
    if (!ConfirmAction('Close', docMsg)) {
        event.preventDefault();
        event.stopImmediatePropagation();
        return false;
    }
});
