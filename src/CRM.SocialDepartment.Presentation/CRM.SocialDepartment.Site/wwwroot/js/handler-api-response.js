async function handleApiResponse(response) {
    const result = await response.json();

    if (result.success) {
        // Успешный ответ
        Swal.fire({
            title: 'Успех!',
            text: 'Операция выполнена успешно',
            icon: result.messageType || 'success'
        });
        return result.data;
    } else {
        // Ошибка
        Swal.fire({
            title: 'Ошибка!',
            text: result.errorMessage,
            icon: result.messageType || 'error'
        });
        throw new Error(result.errorMessage);
    }
}

// Пример использования:
try {
    const response = await fetch('/api/patients', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
        },
        body: JSON.stringify(data)
    });

    const result = await handleApiResponse(response);
    // Обработка успешного результата
} catch (error) {
    // Ошибка уже обработана в handleApiResponse
    console.error(error);
}