window.initializeScrollHandler = function (dotnetHelper) {
    window.addEventListener('scroll', () => {
        const scrollTop = document.documentElement.scrollTop || document.body.scrollTop;
        const windowHeight = window.innerHeight;
        const scrollHeight = document.documentElement.scrollHeight;

        if (scrollTop + windowHeight >= scrollHeight - 200) {
            dotnetHelper.invokeMethodAsync('OnScrollReachedBottom');
        }
    });
}