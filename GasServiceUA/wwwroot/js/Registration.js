const registerBtnContainer = document.getElementById("registerBtnContainer");
const registerNextBtn = document.getElementById('registerNext');
const registerBackBtn = document.getElementById('registerBack');
const registerSubmitBtn = document.getElementById('registerSubmit');

registerNextBtn.addEventListener('click', function (event) {
    event.preventDefault();

    const registerFirstForm = document.getElementById('registrerFirstForm');
    const registerSecondForm = document.getElementById('registrerSecondForm');

    if ($("#registerForm").valid()) {
        registerFirstForm.classList.add('hidden');
        registerSecondForm.classList.remove('hidden');
        registerNextBtn.classList.add('hidden');
        registerBackBtn.classList.remove('hidden');
        registerSubmitBtn.classList.remove('hidden');
    }
});

registerBackBtn.addEventListener('click', function (event) {
    event.preventDefault();

    const registerFirstForm = document.getElementById('registrerFirstForm');
    const registerSecondForm = document.getElementById('registrerSecondForm');

    registerFirstForm.classList.remove('hidden');
    registerSecondForm.classList.add('hidden');
    registerNextBtn.classList.remove('hidden');
    registerBackBtn.classList.add('hidden');
    registerSubmitBtn.classList.add('hidden');
});

function clearValidationErrors(form) { 
    const validationFields = form.querySelectorAll('.text-danger');
    validationFields.forEach(field => {
        field.innerHTML = '';
    });
}