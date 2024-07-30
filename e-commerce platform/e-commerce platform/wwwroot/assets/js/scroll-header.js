document.addEventListener('DOMContentLoaded', function () {
    // Get the header element
    var header = document.getElementById('header');

    // Function to handle scroll event
    function onScroll() {
        // Check the scroll position
        if (window.scrollY > 100) {
            // Add the class if scrolled past 100px
            header.classList.add('header-scrolled');
        } else {
            // Remove the class if above 100px
            header.classList.remove('header-scrolled');
        }
    }

    // Attach the scroll event handler to the window
    window.addEventListener('scroll', onScroll);


});

