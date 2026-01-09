function register() {
    const fullname = document.getElementById("fullname").value.trim();
    const username = document.getElementById("username").value.trim();
    const password = document.getElementById("password").value.trim();
    const repassword = document.getElementById("repassword").value.trim();
    const error = document.getElementById("register-error");

    if (!fullname || !username || !password || !repassword) {
        error.innerText = "Vui lòng nhập đầy đủ thông tin!";
        return;
    }

    if (password !== repassword) {
        error.innerText = "Mật khẩu nhập lại không khớp!";
        return;
    }

    // Lưu thông tin khách hàng (demo)
    const customer = {
        fullname: fullname,
        username: username,
        password: password
    };

    localStorage.setItem("customerAccount", JSON.stringify(customer));

    alert("Đăng ký thành công! Vui lòng đăng nhập.");
    window.location.href = "login.html";
}
