window.navbarToggle = (navbarId, isShow) => {
    if (isShow) {
        document.getElementById(navbarId).style.display = "flex";
    }
    else {
        document.getElementById(navbarId).style.display = "none";
    }
}

window.navbarClose = (navbarId) => {
    document.getElementById(navbarId).style.display = "none";
}