
$(document).ready(validate);

function validate() {
	$('#password').on('input', function () {
		var password = $(this).val();
		validatePassword(password);
	});
}

function validatePassword(password) {
	var errors = [];
	var lengthRegex = /^.{8,}$/;
	var upperRegex = /(?=.*?[A-Z])/;
	var lowerRegex = /(?=.*?[a-z])/;
	var digitRegex = /(?=.*?[0-9])/;
	var specialCharRegex = /(?=.*?[#?!@$%^&*-])/;

	if (!lengthRegex.test(password)) {
		errors.push("Пароль должен содержать минимум 8 символов.");
	}
	if (!upperRegex.test(password)) {
		errors.push("Пароль должен содержать хотя бы одну заглавную букву.");
	}
	if (!lowerRegex.test(password)) {
		errors.push("Пароль должен содержать хотя бы одну строчную букву.");
	}
	if (!digitRegex.test(password)) {
		errors.push("Пароль должен содержать хотя бы одну цифру.");
	}
	if (!specialCharRegex.test(password)) {
		errors.push("Пароль должен содержать хотя бы один специальный символ.");
	}

	// Отображение ошибок
	if (errors.length > 0) {
		$('#password').addClass('is-invalid');
		$('span[data-valmsg-for="Password"]').text(errors.join(' '));
	} else {
		$('#password').removeClass('is-invalid');
		$('span[data-valmsg-for="Password"]').text('');
	}
}