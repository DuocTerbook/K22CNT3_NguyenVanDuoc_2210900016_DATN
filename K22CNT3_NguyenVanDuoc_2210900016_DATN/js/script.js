/* ===== SLIDER TỰ ĐỘNG ===== */
let slideIndex = 0;
const slides = document.querySelector(".slides");

function autoSlide() {
    slideIndex++;
    if (slideIndex >= slides.children.length) {
        slideIndex = 0;
    }
    slides.style.transform = `translateX(-${slideIndex * 100}%)`;
}

setInterval(autoSlide, 3000);


/* ===== TÌM KIẾM SẢN PHẨM ===== */
document.getElementById("search-btn").addEventListener("click", function () {
    const keyword = document.getElementById("search").value.toLowerCase();
    const products = document.querySelectorAll(".product-item");
    let found = false;

    products.forEach(product => {
        const name = product.querySelector("h3").innerText.toLowerCase();
        if (name.includes(keyword)) {
            product.style.display = "block";
            found = true;
        } else {
            product.style.display = "none";
        }
    });

    document.getElementById("search-result").style.display = found ? "none" : "block";
});
const name = localStorage.getItem("customerName");
if (name) {
    document.getElementById("customer-name").innerText = "Xin chào, " + name;
}

function logout() {
    localStorage.removeItem("customerLogin");
    localStorage.removeItem("customerName");
    window.location.href = "login.html";
}

