// Hamburger menuitem toggle
const hamburger = document.getElementById('hamburger');
const navMenuItem = document.getElementById('nav-menuitem');
if (hamburger && navMenuItem) {
  hamburger.addEventListener('click', () => {
    navMenuItem.classList.toggle('active');
  });
}

// Set current year in footer
const yearSpan = document.getElementById('year');
if (yearSpan) {
  yearSpan.textContent = new Date().getFullYear();
}
