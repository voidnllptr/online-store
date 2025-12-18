// Handle search form submission
document.addEventListener('DOMContentLoaded', function() {
    const searchForm = document.querySelector('.search-form');
    const searchInput = document.querySelector('.search-input');
    const searchButton = document.querySelector('.search-button');

    // Handle form submission
    if (searchForm) {
        searchForm.addEventListener('submit', function(e) {
            e.preventDefault();
            const searchQuery = searchInput.value.trim();
            if (searchQuery) {
                window.location.href = `/Catalog/Index?search=${encodeURIComponent(searchQuery)}`;
            }
        });
    }

    // Also handle button click for better mobile support
    if (searchButton) {
        searchButton.addEventListener('click', function() {
            const searchQuery = searchInput.value.trim();
            if (searchQuery) {
                window.location.href = `/Catalog/Index?search=${encodeURIComponent(searchQuery)}`;
            }
        });
    }
});
