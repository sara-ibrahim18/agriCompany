// General site behaviors: AOS init, toastr config, TempData -> toast bridge.
document.addEventListener('DOMContentLoaded', function () {
    if (window.AOS) {
        AOS.init({ duration: 700, once: true, offset: 80 });
    }

    if (window.toastr) {
        toastr.options = {
            closeButton: true,
            progressBar: true,
            positionClass: 'toast-top-left',
            timeOut: 4000,
            rtl: true
        };
    }
});

// Helper exposed globally so Razor views can trigger toasts from TempData messages.
function showToast(type, message) {
    if (!message || !window.toastr) return;
    switch (type) {
        case 'success': toastr.success(message); break;
        case 'error': toastr.error(message); break;
        case 'warning': toastr.warning(message); break;
        default: toastr.info(message);
    }
}

// SweetAlert2 confirm-delete helper used by admin delete forms.
// Prefer confirmDeleteEl(button) over this: it reads the item name and target form id from
// data-* attributes on the button (safely HTML-encoded by Razor) instead of taking them as
// raw string arguments, which avoids breaking (or being exploitable via) special characters
// like a single quote inside a product/category name.
function confirmDelete(formId, itemName) {
    if (!window.Swal) {
        return document.getElementById(formId).submit();
    }
    Swal.fire({
        title: 'هل أنت متأكد؟',
        text: itemName ? `سيتم حذف "${itemName}" ولا يمكن التراجع عن هذا الإجراء` : 'لا يمكن التراجع عن هذا الإجراء',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#d33',
        cancelButtonColor: '#6c757d',
        confirmButtonText: 'نعم، احذف',
        cancelButtonText: 'إلغاء'
    }).then((result) => {
        if (result.isConfirmed) {
            document.getElementById(formId).submit();
        }
    });
    return false;
}

// Safe variant: call as onclick="confirmDeleteEl(this)" on a button that carries
// data-form-id="..." and data-item-name="..." attributes.
function confirmDeleteEl(button) {
    var formId = button.dataset.formId;
    var itemName = button.dataset.itemName;
    return confirmDelete(formId, itemName);
}
