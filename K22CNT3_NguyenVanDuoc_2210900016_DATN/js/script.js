document.addEventListener("DOMContentLoaded", function () {

    /* ================= SLIDER (AN TOÀN) ================= */
    const slides = document.querySelector(".slides");

    if (slides && slides.children.length > 0) {
        let slideIndex = 0;

        setInterval(() => {
            slideIndex++;
            if (slideIndex >= slides.children.length) {
                slideIndex = 0;
            }
            slides.style.transform = `translateX(-${slideIndex * 100}%)`;
        }, 3000);
    }

    /* ================= TÌM KIẾM SẢN PHẨM ================= */
    const searchInput = document.getElementById("search");
    const searchBtn = document.getElementById("search-btn");
    const resultText = document.getElementById("search-result");
    const products = document.querySelectorAll(".product-item");

    if (searchInput && searchBtn && products.length > 0) {
        searchBtn.addEventListener("click", function () {
            const keyword = searchInput.value.toLowerCase().trim();
            let found = false;

            products.forEach(product => {
                const nameEl = product.querySelector("h3");
                if (!nameEl) return;

                const name = nameEl.innerText.toLowerCase();
                if (name.includes(keyword)) {
                    product.style.display = "block";
                    found = true;
                } else {
                    product.style.display = "none";
                }
            });

            if (resultText) {
                resultText.style.display = found ? "none" : "block";
            }
        });
    }

    /* ================= HIỂN THỊ TÊN KHÁCH ================= */
    const customerNameEl = document.getElementById("customer-name");
    const customerName = localStorage.getItem("customerName");

    if (customerNameEl && customerName) {
        customerNameEl.innerText = "Xin chào, " + customerName;
    }

});

/* ================= ĐĂNG XUẤT ================= */
function logout() {
    localStorage.removeItem("customerLogin");
    localStorage.removeItem("customerName");
    window.location.href = "login.html";
}
