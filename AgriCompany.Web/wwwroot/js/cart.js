// سلة المشتريات — محفوظة في localStorage على جهاز الزائر.
// الإضافة بتتم من صفحة المنتج أو كارت المنتج، وإتمام الطلب بيتم من صفحة السلة (/Cart)
// بإرسال كل المنتجات في رسالة واتساب واحدة.
//
// كل القيم بتتقري من data-* attributes (Razor بيعمل لها encode) ومش بتتحط جوه نص JS مباشرة،
// وعند العرض بنستخدم textContent بدل innerHTML لأي نص جاي من المستخدم أو من المنتج.
(function () {
    var STORAGE_KEY = 'albaraka_cart_v1';

    function read() {
        try {
            var data = JSON.parse(localStorage.getItem(STORAGE_KEY) || '[]');
            return Array.isArray(data) ? data : [];
        } catch (e) {
            return [];
        }
    }

    function write(items) {
        try {
            localStorage.setItem(STORAGE_KEY, JSON.stringify(items));
        } catch (e) { /* التخزين مقفول (وضع التصفح الخاص مثلاً) */ }
        updateBadges();
        document.dispatchEvent(new CustomEvent('cart:changed'));
    }

    function itemKey(code, size) {
        return String(code) + '|' + String(size || '');
    }

    function count() {
        return read().reduce(function (sum, i) { return sum + (parseInt(i.qty, 10) || 0); }, 0);
    }

    function total() {
        return read().reduce(function (sum, i) { return sum + (Number(i.price) || 0) * (parseInt(i.qty, 10) || 0); }, 0);
    }

    function add(item) {
        var qty = Math.max(1, parseInt(item.qty, 10) || 1);
        var items = read();
        var key = itemKey(item.code, item.size);
        var existing = items.find(function (i) { return i.key === key; });
        if (existing) {
            existing.qty = (parseInt(existing.qty, 10) || 0) + qty;
            existing.price = Number(item.price) || 0;
        } else {
            items.push({
                key: key,
                code: String(item.code || ''),
                name: String(item.name || ''),
                size: String(item.size || ''),
                price: Number(item.price) || 0,
                qty: qty,
                image: String(item.image || ''),
                url: String(item.url || ''),
                currency: String(item.currency || '')
            });
        }
        write(items);
    }

    function setQty(key, qty) {
        qty = parseInt(qty, 10);
        var items = read();
        if (!qty || qty < 1) {
            items = items.filter(function (i) { return i.key !== key; });
        } else {
            items.forEach(function (i) { if (i.key === key) i.qty = qty; });
        }
        write(items);
    }

    function remove(key) {
        write(read().filter(function (i) { return i.key !== key; }));
    }

    function clear() {
        write([]);
    }

    function formatNumber(n) {
        return Number(n || 0).toLocaleString('ar-EG');
    }

    function updateBadges() {
        var c = count();
        document.querySelectorAll('[data-cart-count]').forEach(function (el) {
            el.textContent = c > 99 ? '99+' : String(c);
            el.classList.toggle('d-none', c === 0);
        });
    }

    // ---------- زر "أضف إلى السلة" ----------
    // يقرأ بيانات المنتج من data-* على الزر. لو الزر جوه صفحة المنتج، بياخد الحجم والسعر المختارين والكمية.
    window.addToCart = function (btn) {
        var d = btn.dataset;
        var size = d.size || '';
        var price = Number(d.price) || 0;

        if (d.useSelection === 'true') {
            var sizeEl = document.getElementById('selectedSize');
            var priceEl = document.getElementById('selectedPrice');
            if (document.querySelector('.size-btn') && !(sizeEl && sizeEl.value)) {
                if (window.toastr) toastr.warning('من فضلك اختر الحجم أولاً');
                return;
            }
            if (sizeEl && sizeEl.value) size = sizeEl.value;
            if (priceEl && priceEl.value) price = Number(priceEl.value) || price;
        }

        var qty = 1;
        var qtyInput = document.getElementById('qty-' + d.code);
        if (qtyInput) qty = Math.max(1, parseInt(qtyInput.value, 10) || 1);

        add({
            code: d.code,
            name: d.name,
            size: size,
            price: price,
            qty: qty,
            image: d.image,
            url: d.url,
            currency: d.currency
        });

        if (window.toastr) {
            toastr.success(
                '<a href="/Cart" class="text-white fw-bold text-decoration-underline">عرض السلة وإتمام الطلب</a>',
                'تمت إضافة المنتج إلى السلة',
                { escapeHtml: false }
            );
        }
    };

    // ---------- صفحة السلة ----------
    function renderCartPage(root) {
        var items = read();
        var list = root.querySelector('[data-cart-list]');
        var emptyBox = root.querySelector('[data-cart-empty]');
        var content = root.querySelector('[data-cart-content]');
        var currency = root.dataset.currency || '';

        emptyBox.classList.toggle('d-none', items.length > 0);
        content.classList.toggle('d-none', items.length === 0);
        list.innerHTML = '';

        var tpl = root.querySelector('#cartItemTemplate');
        items.forEach(function (item) {
            var node = tpl.content.firstElementChild.cloneNode(true);
            var img = node.querySelector('[data-f="image"]');
            img.src = item.image || '/images/placeholders/product.jpg';
            img.alt = item.name;
            node.querySelectorAll('[data-f="link"]').forEach(function (a) {
                if (item.url) a.href = item.url; else a.removeAttribute('href');
            });
            node.querySelector('[data-f="name"]').textContent = item.name;
            var sizeEl = node.querySelector('[data-f="size"]');
            if (item.size) sizeEl.textContent = 'الحجم: ' + item.size; else sizeEl.remove();
            node.querySelector('[data-f="price"]').textContent = formatNumber(item.price) + ' ' + currency;
            node.querySelector('[data-f="subtotal"]').textContent = formatNumber(item.price * item.qty) + ' ' + currency;

            var qtyInput = node.querySelector('[data-f="qty"]');
            qtyInput.value = item.qty;
            qtyInput.addEventListener('change', function () { setQty(item.key, qtyInput.value); });
            node.querySelector('[data-act="inc"]').addEventListener('click', function () { setQty(item.key, item.qty + 1); });
            node.querySelector('[data-act="dec"]').addEventListener('click', function () {
                if (item.qty > 1) setQty(item.key, item.qty - 1);
            });
            node.querySelector('[data-act="remove"]').addEventListener('click', function () { remove(item.key); });

            list.appendChild(node);
        });

        root.querySelectorAll('[data-cart-total]').forEach(function (el) {
            el.textContent = formatNumber(total()) + ' ' + currency;
        });
        root.querySelectorAll('[data-cart-items-count]').forEach(function (el) {
            el.textContent = formatNumber(count());
        });
    }

    function buildWhatsAppMessage(currency, customer) {
        var items = read();
        var lines = ['مرحبا، عايز أطلب المنتجات التالية:', ''];
        items.forEach(function (item, idx) {
            lines.push((idx + 1) + ') ' + item.name);
            if (item.size) lines.push('   الحجم: ' + item.size);
            lines.push('   الكمية: ' + item.qty);
            lines.push('   سعر الوحدة: ' + item.price + ' ' + currency);
            lines.push('   الإجمالي: ' + (item.price * item.qty) + ' ' + currency);
            lines.push('');
        });
        lines.push('الإجمالي الكلي: ' + total() + ' ' + currency);

        if (customer.name || customer.phone || customer.address || customer.notes) {
            lines.push('');
            lines.push('بيانات العميل:');
            if (customer.name) lines.push('الاسم: ' + customer.name);
            if (customer.phone) lines.push('الموبايل: ' + customer.phone);
            if (customer.address) lines.push('العنوان: ' + customer.address);
            if (customer.notes) lines.push('ملاحظات: ' + customer.notes);
        }
        lines.push('');
        lines.push('شكراً');
        return lines.join('\n');
    }

    function initCartPage() {
        var root = document.getElementById('cartPage');
        if (!root) return;

        renderCartPage(root);
        document.addEventListener('cart:changed', function () { renderCartPage(root); });
        window.addEventListener('storage', function (e) {
            if (e.key === STORAGE_KEY) { updateBadges(); renderCartPage(root); }
        });

        var clearBtn = root.querySelector('[data-act="clear"]');
        if (clearBtn) {
            clearBtn.addEventListener('click', function () {
                if (window.Swal) {
                    Swal.fire({
                        title: 'تفريغ السلة؟',
                        text: 'هيتم حذف كل المنتجات من السلة.',
                        icon: 'warning',
                        showCancelButton: true,
                        confirmButtonText: 'نعم، فرّغ السلة',
                        cancelButtonText: 'إلغاء',
                        confirmButtonColor: '#dc3545'
                    }).then(function (r) { if (r.isConfirmed) clear(); });
                } else if (confirm('هيتم حذف كل المنتجات من السلة. متأكد؟')) {
                    clear();
                }
            });
        }

        var form = root.querySelector('#checkoutForm');
        form.addEventListener('submit', function (e) {
            e.preventDefault();
            if (read().length === 0) return;

            if (!form.checkValidity()) {
                form.classList.add('was-validated');
                return;
            }

            var whatsapp = (root.dataset.whatsapp || '').replace(/[^\d]/g, '');
            var customer = {
                name: form.elements['customerName'].value.trim(),
                phone: form.elements['customerPhone'].value.trim(),
                address: form.elements['customerAddress'].value.trim(),
                notes: form.elements['customerNotes'].value.trim()
            };
            var text = buildWhatsAppMessage(root.dataset.currency || '', customer);
            window.open('https://wa.me/' + whatsapp + '?text=' + encodeURIComponent(text), '_blank');

            if (window.Swal) {
                Swal.fire({
                    title: 'تم تجهيز طلبك على واتساب',
                    text: 'بعد ما تبعت الرسالة على واتساب، تحب نفرّغ السلة؟',
                    icon: 'success',
                    showCancelButton: true,
                    confirmButtonText: 'نعم، فرّغ السلة',
                    cancelButtonText: 'لا، خليها',
                    confirmButtonColor: '#198754'
                }).then(function (r) { if (r.isConfirmed) clear(); });
            }
        });
    }

    window.Cart = { read: read, add: add, setQty: setQty, remove: remove, clear: clear, count: count, total: total };

    document.addEventListener('DOMContentLoaded', function () {
        updateBadges();
        initCartPage();
    });
    window.addEventListener('storage', function (e) {
        if (e.key === STORAGE_KEY) updateBadges();
    });
})();
