// Builds a WhatsApp deep link with a pre-filled order message and opens it in a new tab.
// Used on product cards / product details pages. Orders are placed exclusively via WhatsApp;
// this site never stores or processes an "order" record.
//
// IMPORTANT: values are read from data-* attributes on the button element (safely HTML-encoded
// by Razor when rendered) rather than being interpolated directly into a JS string literal.
// Embedding raw text straight into an inline onclick="...('@Model.Name')" string is unsafe:
// a single quote inside the product name would break out of the string and either throw a JS
// error or allow injected script to run. Reading from the DOM avoids that entirely.
//function orderViaWhatsApp(button) {
//    var whatsappNumber = button.dataset.whatsapp;
//    var productNameAr = button.dataset.name;
//    var productCode = button.dataset.code;
//    var price = button.dataset.price;
//    var currency = button.dataset.currency;
//    var size = button.dataset.size || '';

//    var qty = 1;
//    var qtyInput = document.getElementById('qty-' + productCode);
//    if (qtyInput && parseInt(qtyInput.value, 10) > 0) {
//        qty = parseInt(qtyInput.value, 10);
//    }

//    var lines = [
//        'مرحبًا، أرغب في طلب المنتج التالي:',
//        'المنتج: ' + productNameAr,
//        'الكود: ' + productCode,
//        'السعر: ' + price + ' ' + currency
//    ];

//    if (size) {
//        lines.push('الحجم: ' + size);
//    }
//    lines.push('الكمية: ' + qty);

//    var message = encodeURIComponent(lines.join('\n'));
//    var url = 'https://wa.me/' + whatsappNumber + '?text=' + message;
//    window.open(url, '_blank');
//}
//function orderViaWhatsApp(btn) {
//    const whatsapp = btn.dataset.whatsapp;
//    const name = btn.dataset.name;
//    const code = btn.dataset.code;
//    const price = btn.dataset.price;
//    const currency = btn.dataset.currency;

//    // الحصول على الحجم المختار
//    const selectedSize = document.getElementById('selectedSize') ?
//        document.getElementById('selectedSize').value : '';

//    const qtyInput = document.getElementById('qty-' + code);
//    const qty = qtyInput ? qtyInput.value : 1;

//    let text = `مرحبا، عايز أطلب:\n\n` +
//        `المنتج: ${name}\n` +
//        `الكود: ${code}\n`;

//    if (selectedSize) {
//        text += `الحجم: ${selectedSize}\n`;
//    }

//    text += `الكمية: ${qty}\n` +
//        `السعر: ${price} ${currency}\n\n` +
//        `شكراً`;

//    const url = `https://wa.me/${whatsapp}?text=${encodeURIComponent(text)}`;
//    window.open(url, '_blank');
//}
//function orderViaWhatsApp(btn) {
//    const whatsapp = btn.dataset.whatsapp;
//    const name = btn.dataset.name;
//    const code = btn.dataset.code;
//    const currency = btn.dataset.currency;

//    // الحصول على الحجم والسعر المختارين
//    const selectedSize = document.getElementById('selectedSize') ?
//        document.getElementById('selectedSize').value : '';
//    const selectedPrice = document.getElementById('selectedPrice') ?
//        document.getElementById('selectedPrice').value : btn.dataset.price;

//    const qtyInput = document.getElementById('qty-' + code);
//    const qty = qtyInput ? qtyInput.value : 1;

//    let text = `مرحبا، عايز أطلب:\n\n` +
//        `المنتج: ${name}\n` +
//        `الكود: ${code}\n`;

//    if (selectedSize) {
//        text += `الحجم: ${selectedSize}\n`;
//    }

//    text += `الكمية: ${qty}\n` +
//        `السعر: ${selectedPrice} ${currency}\n\n` +
//        `شكراً`;

//    const url = `https://wa.me/${whatsapp}?text=${encodeURIComponent(text)}`;
//    window.open(url, '_blank');
//}
//function orderViaWhatsApp(btn) {
//    const whatsapp = btn.dataset.whatsapp;
//    const name = btn.dataset.name;
//    const code = btn.dataset.code;
//    const currency = btn.dataset.currency;

//    // الحصول على الحجم والسعر المختارين
//    const selectedSize = document.getElementById('selectedSize')?.value || '';
//    let selectedPrice = parseFloat(document.getElementById('selectedPrice')?.value) || parseFloat(btn.dataset.price) || 0;

//    const qtyInput = document.getElementById('qty-' + code);
//    const qty = parseInt(qtyInput ? qtyInput.value : 1);

//    const totalPrice = qty * selectedPrice;

//    let text = `مرحبا، عايز أطلب:\n\n` +
//        `المنتج: ${name}\n` +
//        `الكود: ${code}\n`;

//    if (selectedSize) {
//        text += `الحجم: ${selectedSize}\n`;
//    }

//    text += `الكمية: ${qty}\n` +
//        `سعر الوحدة: ${selectedPrice} ${currency}\n` +
//        `الإجمالي: ${totalPrice} ${currency}\n\n` +
//        `شكراً`;

//    const url = `https://wa.me/${whatsapp}?text=${encodeURIComponent(text)}`;
//    window.open(url, '_blank');
//}
//function orderViaWhatsApp(btn) {
//    const whatsapp = btn.dataset.whatsapp;
//    const name = btn.dataset.name;
//    const code = btn.dataset.code;
//    const currency = btn.dataset.currency;

//    // الحصول على الحجم والسعر المختارين
//    const selectedSizeElement = document.getElementById('selectedSize');
//    const selectedPriceElement = document.getElementById('selectedPrice');

//    const selectedSize = selectedSizeElement ? selectedSizeElement.value : '';
//    const selectedPrice = selectedPriceElement ? parseFloat(selectedPriceElement.value) : 0;

//    const qtyInput = document.getElementById('qty-' + code);
//    const qty = qtyInput ? parseInt(qtyInput.value) || 1 : 1;

//    const totalPrice = qty * selectedPrice;

//    let text = `مرحبا، عايز أطلب:\n\n` +
//        `المنتج: ${name}\n` +
//        `الكود: ${code}\n`;

//    if (selectedSize) {
//        text += `الحجم: ${selectedSize}\n`;
//    }

//    text += `الكمية: ${qty}\n` +
//        `سعر الوحدة: ${selectedPrice} ${currency}\n` +
//        `الإجمالي: ${totalPrice} ${currency}\n\n` +
//        `شكراً`;

//    const url = `https://wa.me/${whatsapp}?text=${encodeURIComponent(text)}`;
//    window.open(url, '_blank');
//}
function orderViaWhatsApp(btn) {
    const whatsapp = btn.dataset.whatsapp;
    const name = btn.dataset.name;
    const code = btn.dataset.code;
    const currency = btn.dataset.currency;

    const selectedSize = document.getElementById('selectedSize')?.value || '';
    const selectedPrice = parseFloat(document.getElementById('selectedPrice')?.value) || 0;

    const qtyInput = document.getElementById('qty-' + code);
    const qty = parseInt(qtyInput ? qtyInput.value : 1);

    const totalPrice = qty * selectedPrice;

    let text = `مرحبا، عايز أطلب:\n\n` +
        `المنتج: ${name}\n`;

    if (selectedSize) {
        text += `الحجم: ${selectedSize}\n`;
    }

    text += `الكمية: ${qty}\n` +
        `سعر الوحدة: ${selectedPrice} ${currency}\n` +
        `الإجمالي: ${totalPrice} ${currency}\n\n` +
        `شكراً`;

    const url = `https://wa.me/${whatsapp}?text=${encodeURIComponent(text)}`;
    window.open(url, '_blank');
}