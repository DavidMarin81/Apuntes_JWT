const API_URL = "https://localhost:7058/api/Auth"; // ajusta tu puerto

document.getElementById("login-btn").addEventListener("click", async () => {
    const email = document.getElementById("email").value;
    const password = document.getElementById("password").value;

    const response = await fetch(`https://localhost:7058/api/Auth/login`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ email, password }),
        credentials: "include" // Muy importante para enviar cookies
    });

    if (!response.ok) {
        alert("Credenciales incorrectas");
        return;
    }

    // No se necesita leer token, la cookie se guarda automáticamente
    showNavbar();
});

async function showNavbar() {
    // Ocultamos login
    document.getElementById("login-container").classList.add("hidden");

    // Mostramos navbar
    document.getElementById("navbar").classList.remove("hidden");

    // Pedimos los datos del usuario al backend
    const res = await fetch(`https://localhost:7058/api/Auth/me`, {
        method: "GET",
        credentials: "include"
    });

    if (!res.ok) {
        alert("No autorizado, por favor inicia sesión");
        location.reload();
        return;
    }

    const data = await res.json();
    const role = data.role;

    // Ocultamos todos los botones primero
    document.querySelectorAll("#navbar button").forEach(btn => {
        btn.style.display = "none";
    });

    // Botones comunes
    document.getElementById("ver-tabla").style.display = "inline-block";

    // Según rol:
    if (role === "normal") {
        document.getElementById("cambiar-pass").style.display = "inline-block";
    } else if (role === "master") {
        document.getElementById("ver-empresas").style.display = "inline-block";
    } else if (role === "intranet") {
        document.getElementById("ver-historicos").style.display = "inline-block";
    }

    // Logout
    const logoutBtn = document.getElementById("logout-btn");
    logoutBtn.style.display = "inline-block";
    logoutBtn.onclick = async () => {
        alert("Cerrar Sesion");
        await fetch("https://localhost:7058/api/Auth/logout", {
            method: "POST",
            credentials: "include"
        });
        location.reload();
    };

    // Acciones de botones
    document.getElementById("ver-tabla").onclick = () => alert("Mostrando tabla...");
    document.getElementById("cambiar-pass").onclick = () => alert("Cambiar contraseña...");
    document.getElementById("ver-historicos").onclick = () => alert("Mostrando históricos...");
    document.getElementById("ver-empresas").onclick = () => alert("Mostrando empresas...");
}