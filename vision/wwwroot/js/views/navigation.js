
window.addEventListener('DOMContentLoaded', function () {

    // Sidebar
    var sideBar = document.querySelector('#site-sidebar');
    var overlay = document.querySelector('#site-overlay');
    var scaleMQL = window.matchMedia('(max-width: 768px)');
    function handleScaleMQLChange() {

    }
    scaleMQL.addListener(handleScaleMQLChange);

    document.body.addEventListener('change', function (event) {
        if (event.target.id === 'switcher-scale') {
            switcher.scale = event.detail.value;
        }
        else if (event.target.id === 'switcher-theme') {
            switcher.theme = event.detail.value;
        }
        else if (event.target.id === 'switcher-direction') {
            switcher.direction = event.detail.value;
        }
    });

    var sidebarMQL = window.matchMedia('(max-width: 960px)');
    function handleSidebarMQLChange() {
        if (!sidebarMQL.matches) {
            // Get rid of the overlay if we resize while the sidebar is open
            hideSideBar();
        }
    }
    sidebarMQL.addListener(handleSidebarMQLChange);

    handleScaleMQLChange();
    handleSidebarMQLChange();

    function showSideBar() {
        if (sidebarMQL.matches) {
            overlay.addEventListener('click', hideSideBar);
            sideBar.classList.add('is-open');
            overlay.classList.add('is-open');
        }
    }

    function hideSideBar() {
        overlay.removeEventListener('click', hideSideBar);
        overlay.classList.remove('is-open');
        if (sideBar) {
            sideBar.classList.remove('is-open');
        }
        if (window.siteSearch) {
            window.siteSearch.hideResults();
        }
    }

    document.querySelector('#site-menu').addEventListener('click', function (event) {
        if (sideBar.classList.contains('is-open')) {
            hideSideBar();
        }
        else {
            showSideBar();
        }
    });

    // Search isn't supported on IE 11 and sideBar will not be exist in test mode
    if (typeof Search !== 'undefined' && document.querySelector('#site-search')) {
        window.siteSearch = new Search(document.querySelector('#site-search'))
    }

    window.addEventListener('SearchFocused', function () {
        showSideBar();

        // Immediately hide results, otherwise they show up in the wrong position since we're in the middle of animation
        siteSearch.hideResults();
    });
});
