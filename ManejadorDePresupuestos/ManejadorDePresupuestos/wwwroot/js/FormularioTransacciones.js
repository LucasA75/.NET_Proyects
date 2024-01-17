function inicializarFormTransacciones(urlObtenerCategorias) {
    $("#TipoOperacion").change(async function () {
        const valorSeleccionado = $(this).val();

        const res = await fetch(url, {
            method: "POST",
            body: valorSeleccionado,
            headers: { 'Content-Type': 'application/json' },
        })

        const json = await res.json();

        const categoria = json.map(x => `<option value=${x.id}>${x.nombre}</option>`)
        console.log(categoria)
        console.log(json)
        $("#CategoriaID").html(categoria)
    })
}

