document.addEventListener('DOMContentLoaded', function () {
    const documentInputs = document.querySelectorAll('input[pattern]');

    documentInputs.forEach(input => {
        input.addEventListener('input', function () {
            validateDocumentInput(this);
        });

        input.addEventListener('blur', function () {
            validateDocumentInput(this);
        });
    });

    function validateDocumentInput(input) {
        const pattern = new RegExp(input.pattern);
        const value = input.value.trim();
        const errorMessage = input.dataset.errorMessage;
        const feedback = input.nextElementSibling;

        if (!pattern.test(value)) {
            input.classList.add('is-invalid');
            feedback.textContent = errorMessage;
        } else {
            input.classList.remove('is-invalid');
            feedback.textContent = '';
        }
    }
});