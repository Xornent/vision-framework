function nav_click(e) {
    nav_clearall();
    e.parentNode.setAttribute("class", "nav-menu-item has-submenu is-open");
    e.setAttribute("aria-expanded", "true");

    var doc = e.parentNode.children[1];
    var rect = doc.getBoundingClientRect();
    var display = window.innerWidth - rect.left;
    var delta = rect.width - display;
    if (delta + 18 > 0)
        doc.style.left = (-delta - 18)+"px";
}

function nav_clearall() {
    var start = document.getElementById("nav.explore");
    start.parentNode.setAttribute("class", "nav-menu-item has-submenu");
    start.setAttribute("aria-expanded", "false");
    
    start = document.getElementById("nav.page");
    if (start != undefined) {
        start.parentNode.setAttribute("class", "nav-menu-item has-submenu");
        start.setAttribute("aria-expanded", "false");
    }

    start = document.getElementById("nav.contrib");
    start.parentNode.setAttribute("class", "nav-menu-item has-submenu");
    start.setAttribute("aria-expanded", "false");

    start = document.getElementById("nav.user");
    start.parentNode.setAttribute("class", "nav-menu-item has-submenu");
    start.setAttribute("aria-expanded", "false");

    start = document.getElementById("nav.about");
    start.parentNode.setAttribute("class", "nav-menu-item has-submenu");
    start.setAttribute("aria-expanded", "false");

    var doc = document.getElementById("ul.explore");
    doc.style.left = 0;
    doc = document.getElementById("ul.page");
    if (doc != undefined)
        doc.style.left = 0;
    doc = document.getElementById("ul.contrib");
    doc.style.left = 0;
    doc = document.getElementById("ul.user");
    doc.style.left = 0;
    doc = document.getElementById("ul.about");
    doc.style.left = 0;
}