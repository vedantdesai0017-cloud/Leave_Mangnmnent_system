// Leave application form functionality
document.addEventListener('DOMContentLoaded', function() {
    const startDateInput = document.getElementById('StartDate');
    const endDateInput = document.getElementById('EndDate');
    const numberOfDaysInput = document.getElementById('NumberOfDays');
    const leaveTypeSelect = document.getElementById('LeaveTypeId');
    const balanceDisplay = document.getElementById('balanceDisplay');
    const balanceValue = document.getElementById('balanceValue');
    const dayCalculation = document.getElementById('dayCalculation');

    function calculateDays() {
        if (!startDateInput || !endDateInput || !numberOfDaysInput) return;
        
        const startDate = new Date(startDateInput.value);
        const endDate = new Date(endDateInput.value);
        
        if (startDate && endDate && !isNaN(startDate) && !isNaN(endDate)) {
            if (endDate < startDate) {
                if (numberOfDaysInput) numberOfDaysInput.value = 0;
                if (dayCalculation) {
                    dayCalculation.innerHTML = '<span class="text-danger"><i class="bi bi-exclamation-triangle"></i> End date must be after start date</span>';
                    dayCalculation.style.display = 'block';
                }
                return;
            }
            
            const diffTime = Math.abs(endDate - startDate);
            const diffDays = Math.floor(diffTime / (1000 * 60 * 60 * 24)) + 1;
            
            if (numberOfDaysInput) numberOfDaysInput.value = diffDays;
            if (dayCalculation) {
                dayCalculation.innerHTML = '<span class="text-success"><i class="bi bi-check-circle"></i> ' + diffDays + ' day(s) selected</span>';
                dayCalculation.style.display = 'block';
            }
        } else {
            if (numberOfDaysInput) numberOfDaysInput.value = 0;
            if (dayCalculation) dayCalculation.style.display = 'none';
        }
    }

    if (startDateInput) startDateInput.addEventListener('change', calculateDays);
    if (endDateInput) endDateInput.addEventListener('change', calculateDays);

    // Set min date to today for new applications
    if (startDateInput && !startDateInput.value) {
        const today = new Date().toISOString().split('T')[0];
        startDateInput.setAttribute('min', today);
        endDateInput.setAttribute('min', today);
    }

    // Update end date min when start date changes
    if (startDateInput) {
        startDateInput.addEventListener('change', function() {
            if (endDateInput) {
                endDateInput.setAttribute('min', this.value);
                if (endDateInput.value && endDateInput.value < this.value) {
                    endDateInput.value = this.value;
                }
            }
            calculateDays();
        });
    }
});
