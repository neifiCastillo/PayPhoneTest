
# 📘 PayphoneTest API

API REST para la gestión de billeteras, transferencias de saldo y visualización de historial de movimientos.  
Esta API está construida con **ASP.NET Core 8**, usa **Entity Framework Core** como ORM y **SQL Server** como base de datos.

---
## 🌐 Base URL / 
🔧 En entorno local: http://localhost:{puerto}/api/
---

## 🔐 Autenticación

La API utiliza **autenticación basada en JWT**.  
La mayoría de los endpoints requieren el header: Authorization: Bearer {token}

### ❗ Acceso público
El único endpoint accesible sin autenticación es: GET /api/HistoryMovements/{walletId}
-----------------------

#### ✅ `POST /api/Wallet/CreateWallet`
Crea una nueva billetera.
##### 🔸 Request Body

"json
{
"documentId": "4566222335",
"name": "Juan Perez",
balance: 100.0
}

📝Validation rules:
documentId: exactly 10 characters
Name: required, cannot be empty
Balance: Mayor o igual a 0

🔸 Answer
{
"Success": true,
"WalletId": 1
}

---------------

#### ✅ GET /api/Wallet/GetAllWallet
A list of all registered Billeteras.
🔸 Answer
{
"value": [
{
"id": 1,
"documentId": "4566222335",
"name": "Juan Perez",
"balance": 100.0,
"createdAt": "2024-04-20T10:00:00Z",
"updatedAt": "2024-04-20T10:00:00Z"
}
]
}
---------------------------------
#### ✅ Get /api/wallet/{id}
View the invoice to get your ID.
🔸 Answer (200 OK)
{
"id": 1,
"documentId": "1234567890",
"name": "Juan Perez",
"balance": 100.0,
"createdAt": "2024-04-20T10:00:00Z",
"updatedAt": "2024-04-20T10:00:00Z"
}
🔸 Response (404 Not Found)
{
"success": error,
"message": "No bookings found"
}

--------------------------------------
#### ✅ PUT /api/Wallet/UpdateWallet/{id}
Actualiza el nombre de una billetera existente.
🔸 Request Body
{
  "documentId": "1234567890", // debe seguir siendo válido
  "name": "Nuevo Nombre",
  "balance": 100.0
}
--------------------------------
#### ✅ DELETE /api/Wallet/DeleteWallet/{id}
Elimina una billetera por su ID.
🔸 Response (200 OK)
{
  "success": true
}
🔸 Response (404 Not Found)
{
  "success": false,
  "message": "Billetera no encontrada"
}
----------------------------------

💸 Transferencias

####✅ POST /api/Wallet/Transfer
Transfiere saldo de una billetera a otra.
🔸 Request Body
{
  "fromWalletId": 1,
  "toWalletId": 2,
  "amount": 50.0,
  "toWalletName": "María" // debe coincidir exactamente con el nombre del destinatario
}

📝 Validaciones:
fromWalletId y toWalletId deben ser distintos
amount > 0
toWalletName es obligatorio y debe coincidir

🔸 Response (200 OK)
{
  "success": true,
  "message": "Transferencia completada exitosamente."
}
🔸 Response (400 Bad Request)
{
  "success": false,
  "message": "Saldo insuficiente."
}
-------------------------------
📜 Historial de Movimientos

####✅ GET /api/HistoryMovements/{walletId}
Consulta los movimientos (débito/crédito) de una billetera específica.
🔓 Acceso público (no requiere autenticación).
🔸 Response (200 OK)
[
  {
    "amount": 50.0,
    "type": "Débito",
    "createdAt": "2024-04-20T10:00:00Z"
  },
  {
    "amount": 50.0,
    "type": "Crédito",
    "createdAt": "2024-04-20T10:01:00Z"
  }
]
🔸 Response (404 Not Found)
{
  "success": false,
  "message": "No hay movimientos registrados"
}
🔸 Response (400 Bad Request)
{
  "success": false,
  "message": "El ID de la billetera debe ser válido."
}
---------------------------------------

⚙️ Tecnología utilizada

ASP.NET Core 8
Entity Framework Core
SQL Server
Swagger
JWT Authentication

----------------------------------------

Pruebas

✅ Pruebas unitarias con xUnit
✅ Pruebas de integración con TestServer
✅ Validación de negocio en servicios
✅ Manejo de errores HTTP (400, 404, 500)

--------------------------------
📊 Documentación visual (Swagger)
Disponible en: 
https://localhost:{puerto}/swagger
Muestra todos los endpoints y permite probarlos desde la web.

🤝 Desarollador
Neifi Joel Calzado Castillo
Contacto: neifi03128@gmail.com
