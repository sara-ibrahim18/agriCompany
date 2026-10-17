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

// ---------- Quick search: mobile toggle + live suggestions ----------
document.addEventListener('DOMContentLoaded', function () {
    // Toggle the collapsible mobile search bar under the navbar.
    var toggleBtn = document.getElementById('mobileSearchToggle');
    var mobileBar = document.getElementById('mobileSearchBar');
    if (toggleBtn && mobileBar) {
        toggleBtn.addEventListener('click', function () {
            var isOpen = mobileBar.classList.toggle('show');
            toggleBtn.setAttribute('aria-expanded', isOpen ? 'true' : 'false');
            if (isOpen) {
                var input = document.getElementById('mobileSearchInput');
                if (input) setTimeout(function () { input.focus(); }, 150);
            }
        });
    }

    setupSearchSuggestions('navSearchInput', 'navSearchSuggestions');
    setupSearchSuggestions('mobileSearchInput', 'mobileSearchSuggestions');

    setupCategoriesStrip();
});

// ---------- Categories strip: continuous auto-scroll using Swiper ----------
// Swiper is already loaded on every page for the hero slider, and its `loop`
// mode is a mature, well-tested implementation of exactly this "infinite belt"
// effect (it clones slides internally and handles the wrap-around itself), so
// we lean on it instead of a hand-rolled CSS/JS loop.
function setupCategoriesStrip() {
    var el = document.getElementById('categoriesSwiper');
    if (!el || !window.Swiper) return;

    var reduceMotion = window.matchMedia && window.matchMedia('(prefers-reduced-motion: reduce)').matches;

    new Swiper('#categoriesSwiper', {
        loop: true,
        slidesPerView: 'auto',
        spaceBetween: 10,
        speed: reduceMotion ? 0 : 4000,
        allowTouchMove: true,
        freeMode: {
            enabled: true,
            momentum: false
        },
        autoplay: reduceMotion ? false : {
            delay: 1,
            disableOnInteraction: false,
            pauseOnMouseEnter: true
        }
    });
}

function setupSearchSuggestions(inputId, boxId) {
    var input = document.getElementById(inputId);
    var box = document.getElementById(boxId);
    if (!input || !box) return;

    var debounceTimer = null;

    input.addEventListener('input', function () {
        var q = input.value.trim();
        clearTimeout(debounceTimer);

        if (q.length < 2) {
            box.classList.remove('show');
            box.innerHTML = '';
            return;
        }

        debounceTimer = setTimeout(function () {
            fetch('/Products/Suggest?q=' + encodeURIComponent(q))
                .then(function (res) { return res.ok ? res.json() : []; })
                .then(function (items) { renderSuggestions(box, items, q); })
                .catch(function () { box.classList.remove('show'); });
        }, 250);
    });

    // Close suggestions when clicking outside.
    document.addEventListener('click', function (e) {
        if (!box.contains(e.target) && e.target !== input) {
            box.classList.remove('show');
        }
    });

    input.addEventListener('focus', function () {
        if (box.innerHTML.trim() !== '') box.classList.add('show');
    });
}

function renderSuggestions(box, items, q) {
    if (!items || items.length === 0) {
        box.innerHTML = '<div class="suggestion-empty">لا توجد نتائج مطابقة لـ "' + escapeHtml(q) + '"</div>';
        box.classList.add('show');
        return;
    }

    var html = items.map(function (item) {
        var img = item.image || '/images/placeholders/category.jpg';
        var cat = item.category ? '<div class="s-cat">' + escapeHtml(item.category) + '</div>' : '';
        return '<a class="suggestion-item" href="/Products/Details?slug=' + encodeURIComponent(item.slug) + '">' +
            '<img src="' + img + '" alt="" onerror="this.src=\'/images/placeholders/category.jpg\'" />' +
            '<div><div class="s-name">' + escapeHtml(item.name) + '</div>' + cat + '</div>' +
            '</a>';
    }).join('');

    html += '<a class="suggestion-more" href="/Products?search=' + encodeURIComponent(q) + '">عرض كل النتائج <i class="fa-solid fa-arrow-left"></i></a>';

    box.innerHTML = html;
    box.classList.add('show');
}

function escapeHtml(str) {
    var div = document.createElement('div');
    div.textContent = str || '';
    return div.innerHTML;
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