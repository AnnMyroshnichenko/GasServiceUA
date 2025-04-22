const openModalBtn = document.getElementById('openModalBtn');
const modal = document.getElementById('modal');
const closeModalBtn = document.getElementById('closeModalBtn');
const sendMeterReadingsBtn = document.getElementById('sendMeterReadingsBtn');
const startMeterReadingsInput = document.getElementById('startMeterReadings');
const endMeterReadingsInput = document.getElementById('endMeterReadings');
const downloadMeterReadingsReportBtn = document.getElementById('downloadMeterReadingsReport');
const downloadPaymentsHistoryBtn = document.getElementById('downloadPaymentsHistory');

if (openModalBtn != null) {
    function openModal() {
        modal.classList.remove('hidden');
    }

    function closeModal(e, clickedOutside) {
        if (clickedOutside) {
            if (e.target.classList.contains('account__modal-overlay'))
                modal.classList.add('hidden');
        } else modal.classList.add('hidden');
    }

    openModalBtn.addEventListener('click', function () {
        modal.classList.remove('hidden');
    });

    modal.addEventListener('click', function (e) {
        if (e.target.classList.contains('account__modal-overlay')) {
            modal.classList.add('hidden');
        }
    });

    closeModalBtn.addEventListener('click', function () {
        modal.classList.add('hidden');
    });


    function validateMeterReadingInputs() {
        const startData = startMeterReadingsInput.value;
        const endData = endMeterReadingsInput.value;
        let isValid = true;

        startMeterReadingError.style.visibility = 'hidden';
        startMeterReadingError.querySelector('p').textContent = '';
        endMeterReadingError.style.visibility = 'hidden';
        endMeterReadingError.querySelector('p').textContent = '';

        if (startData.length !== 10 || isNaN(startData)) {
            startMeterReadingError.style.visibility = 'visible';
            startMeterReadingError.querySelector('p').textContent = 'Введіть 10-цифровий показник лічильника станом на початок місяця';
            isValid = false;
        } else if (endData.length !== 10 || isNaN(endData)) {
            endMeterReadingError.style.visibility = 'visible';
            endMeterReadingError.querySelector('p').textContent = 'Введіть 10-цифровий показник лічильника станом на кінець місяця';
            isValid = false;
        } else if (parseInt(startData) > parseInt(endData)) {
            endMeterReadingError.style.visibility = 'visible';
            endMeterReadingError.querySelector('p').textContent = 'Перевірте введені дані. Кінцеві показники не можуть бути меншими за початкові';
            isValid = false;
        }

        if (!isValid) {
            sendMeterReadingsBtn.classList.add('disabled_button');
            sendMeterReadingsBtn.disabled = true;
            sendMeterReadingsBtn.classList.add('disabled_button');
        } else {
            sendMeterReadingsBtn.classList.remove('disabled_button');
            sendMeterReadingsBtn.disabled = false;
            sendMeterReadingsBtn.classList.remove('disabled_button');
            calcCost();
        }
    }

    startMeterReadingsInput.addEventListener('focusout', validateMeterReadingInputs);
    endMeterReadingsInput.addEventListener('focusout', validateMeterReadingInputs);

    sendMeterReadingsBtn.addEventListener('click', function () {
        const startData = document.getElementById('startMeterReadings').value;
        const endData = document.getElementById('endMeterReadings').value;

        validateMeterReadingInputs();

        if (sendMeterReadingsBtn.disabled) {
            preventDefault();
            return;
        }

        sendMeterReadingsBtn.disabled = true;

        $.ajax({
            url: 'UserAccount/UserAccount/SendMeterReadings',
            type: 'POST',
            data: {
                startMeterReading: startData,
                endMeterReading: endData
            },
            success: function () {
                sendMeterReadingsBtn.style.backgroundColor = '#67E389';
                sendMeterReadingsBtn.innerHTML = '<i class="fa fa-check-circle"></i>Надіслано';
                sendMeterReadingsBtn.querySelector('.fa-check-circle').style.color = '#0E9634';
                sendMeterReadingsBtn.querySelector('.fa-check-circle').style.textShadow = 'none';
                sendMeterReadingsBtn.querySelector('.fa-check-circle').style.fontSize = '25px';
                sendMeterReadingsBtn.style.cursor = 'default';
                startMeterReadingsInput.readOnly = true;
                endMeterReadingsInput.readOnly = true;
            }
        });
    });
}

function calcCost() {
    const startData = document.getElementById('startMeterReadings').value;
    const endData = document.getElementById('endMeterReadings').value;

    $.ajax({
        url: 'UserAccount/UserAccount/CalcCost',
        type: 'POST',
        data: {
            startMeterReading: startData,
            endMeterReading: endData
        },
        success: function (cost) {
            const costPreview = document.getElementById('costPreview');
            costPreview.textContent = parseFloat(cost).toFixed(2) + ' грн';
        }
    });
}

function validatePeriodInputs() {
    const fromDate = reportGenerationForm.querySelector("form[name='reportGenerationForm'] input[name='fromDate']");
    const toDate = reportGenerationForm.querySelector("form[name='reportGenerationForm'] input[name='toDate']");
    const errorMessage = document.getElementById("periodValidationError");

    if (fromDate.value === '' || toDate.value === '' || fromDate.value > toDate.value) {
        errorMessage.classList.remove('hidden');
        return false;
    } else {
        if (errorMessage.classList.contains('hidden')) {
            errorMessage.classList.add('hidden');
        }
        return true;
    }
}

downloadMeterReadingsReportBtn.addEventListener('click', function () {
    if (validatePeriodInputs()) {
        const startDateInput = reportGenerationForm.querySelector("input[name='fromDate']").value;
        const endDateInput = reportGenerationForm.querySelector("input[name='toDate']").value;
        var link = document.createElement('a');
        link.href = `UserAccount/Reports/GenerateMeterReadingsReport?fromDate=${startDateInput}&toDate=${endDateInput}`;
        link.download = 'file.pdf';
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
    }
});


downloadPaymentsHistoryBtn.addEventListener('click', function () {
    if (validatePeriodInputs()) {
        const startDateInput = reportGenerationForm.querySelector("input[name='fromDate']").value;
        const endDateInput = reportGenerationForm.querySelector("input[name='toDate']").value;

        var link = document.createElement('a');
        link.href = `UserAccount/Reports/GeneratePaymentHistoryReport?fromDate=${startDateInput}&toDate=${endDateInput}`;
        link.download = 'file.pdf';
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
    }
});