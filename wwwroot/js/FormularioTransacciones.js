function incializarFormularioTransacciones(urlObtenerCategorias) {

    $("#TipoOperacionId").change(async function () {
        const valorselect = $(this).val();


        const response = await fetch(urlObtenerCategorias, {
            method: 'POST',
            body: valorselect,
            headers: {
                'Content-Type': 'application/json'
            }
        });
        const json = await response.json();
        //recibir respuesta json para utilizar el  modelo.Categorias
        const opciones = json.map(categoria =>
            `<option value=${categoria.value}> ${categoria.text}</option>`);
        $("#CategoriaId").html(opciones);


    });
}