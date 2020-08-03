function searchIn(x) {
    var page = document.getElementById("search-box-main").value;
    var url = "index.html?page=" + page.replace(" ","%20").replace("+","%20");//此处拼接内容
    window.location.href = url;
    return true;
}

function searchInEdit(x) {
    var page = document.getElementById("search-box-main").value;
    var url = "edit.html?page=" + page.replace(" ", "%20").replace("+","%20");//此处拼接内容
    window.location.href = url;
    return true;
}