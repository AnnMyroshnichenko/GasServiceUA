// animation on scroll
console.log("animation on scroll");

const observer = new IntersectionObserver((entries) => {
    entries.forEach((entry) => {
        if (entry.isIntersecting) {
            entry.target.classList.add("in-view")
        }
    })
}, {
    rootMargin: "0px",
    threshold: [0, 0.1, 1]
})

const animatedSections = document.querySelectorAll('.animated');
animatedSections.forEach((section) => {
    observer.observe(section)
})