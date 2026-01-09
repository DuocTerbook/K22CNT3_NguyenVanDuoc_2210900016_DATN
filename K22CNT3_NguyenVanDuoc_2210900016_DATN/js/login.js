function login() {
    const username = document.getElementById("username").value.trim();
    const password = document.getElementById("password").value.trim();
    const error = document.getElementById("login-error");

    // LẤY TÀI KHOẢN TRONG LOCALSTORAGE
    const customer = JSON.parse(localStorage.getItem("customerAccount"));
    const admin = JSON.parse(localStorage.getItem("adminAccount"));

    // KIỂM TRA ADMIN TRƯỚC
    if (admin && username === admin.username && password === admin.password) {
        localStorage.setItem("adminLogin", "true");
        localStorage.setItem("adminName", admin.username);
        window.location.href = "admin/dashboard.html";
        return;
    }

    // KIỂM TRA KHÁCH HÀNG
    if (customer && username === customer.username && password === customer.password) {
        localStorage.setItem("customerLogin", "true");
        localStorage.setItem("customerName", customer.fullname);
        window.location.href = "index.html";
        return;
    }

    // KHÔNG KHỚP
    error.innerText = "Sai tài khoản hoặc mật khẩu!";
}
