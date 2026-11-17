// Loading skeletons: a shimmering placeholder is shown until each image has loaded,
// then the image fades in. Styles live in site.css ("Loading skeletons").
(function () {
    var IMAGE_SELECTOR = [
        '.product-card .product-img-wrap img',
        '.category-card img',
        '.product-main-image',
        '.product-gallery-thumb',
        '.cat-chip img',
        '.search-suggestions img'
    ].join(',');

    function finish(el) {
        el.classList.remove('skeleton');
        el.classList.add('skeleton-done');
    }

    function watchImage(img) {
        if (img.dataset.skeleton) return;
        img.dataset.skeleton = '1';

        // Already loaded (e.g. from cache) — nothing to show
        if (img.complete && img.naturalWidth > 0) return;

        img.classList.add('skeleton');
    }

    // One capturing listener instead of one per image: it also covers copies Swiper makes
    // of looped slides, which carry the .skeleton class but none of the original's listeners.
    // On error the views' onerror handlers swap in a placeholder; either way the skeleton must go.
    function onImageSettled(e) {
        var el = e.target;
        if (el && el.tagName === 'IMG' && el.classList.contains('skeleton')) finish(el);
    }
    document.addEventListener('load', onImageSettled, true);
    document.addEventListener('error', onImageSettled, true);

    // Hero slides use an inline background-image, so preload it to know when it's ready.
    function watchHeroSlide(slide) {
        var match = /url\(['"]?(.*?)['"]?\)/.exec(slide.style.backgroundImage || '');
        if (!match || !match[1]) return;

        var url = match[1];
        var probe = new Image();
        var done = function () {
            // Clear the slide and any Swiper loop copies of it
            document.querySelectorAll('.hero-slide.skeleton').forEach(function (el) {
                if ((el.style.backgroundImage || '').indexOf(url) !== -1) finish(el);
            });
        };
        probe.onload = done;
        probe.onerror = done;
        probe.src = url;
        if (probe.complete && probe.naturalWidth > 0) return;

        slide.classList.add('skeleton');
    }

    function init(root) {
        (root || document).querySelectorAll(IMAGE_SELECTOR).forEach(watchImage);
    }

    // The main product image changes src when a thumbnail is clicked
    function watchMainImageSwaps() {
        var main = document.getElementById('mainProductImage');
        if (!main || !window.MutationObserver) return;
        new MutationObserver(function () {
            if (main.complete && main.naturalWidth > 0) return;
            main.classList.remove('skeleton-done');
            main.classList.add('skeleton');
        }).observe(main, { attributes: true, attributeFilter: ['src'] });
    }

    // Placeholder rows for the live search dropdown (used by site.js while fetching)
    window.renderSuggestionSkeleton = function (box, rows) {
        var html = '';
        for (var i = 0; i < (rows || 3); i++) {
            html += '<div class="suggestion-skeleton" aria-hidden="true">' +
                '<div class="skeleton skeleton-thumb"></div>' +
                '<div class="flex-grow-1">' +
                '<div class="skeleton-line" style="width:' + (70 - i * 12) + '%"></div>' +
                '<div class="skeleton-line" style="width:35%"></div>' +
                '</div></div>';
        }
        box.innerHTML = html;
        box.classList.add('show');
    };

    window.initSkeletons = init;

    // Runs right after the markup is parsed (script is at the end of <body>), before most images arrive
    init(document);
    document.querySelectorAll('.hero-slide[style*="background-image"]').forEach(watchHeroSlide);
    watchMainImageSwaps();

    // Safety net: nothing should still shimmer once the whole page has loaded
    window.addEventListener('load', function () {
        document.querySelectorAll('img.skeleton').forEach(function (img) {
            if (img.complete) finish(img);
        });
    });
})();
