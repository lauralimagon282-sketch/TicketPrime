using Dapper;
using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
    policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

app.UseCors();
app.UseDefaultFiles();
app.UseStaticFiles();

string connectionString = "Server=localhost\\SQLEXPRESS;Database=TicketPrime;Trusted_Connection=True;TrustServerCertificate=True;";

// --- 1. USUÁRIOS & LOGIN ---
app.MapPost("/api/usuarios", async (Usuario usuario) =>
{
    if (string.IsNullOrWhiteSpace(usuario.Cpf) || string.IsNullOrWhiteSpace(usuario.Nome))
        return Results.BadRequest("Campos obrigatórios ausentes.");

    using var db = new SqlConnection(connectionString);
    var existe = await db.QueryFirstOrDefaultAsync<int>("SELECT 1 FROM Usuarios WHERE Cpf = @Cpf", new { usuario.Cpf });
    if (existe == 1) return Results.BadRequest("Este CPF já está cadastrado.");

    var sql = @"INSERT INTO Usuarios (Cpf, Nome, Email, Senha, NivelAcesso) 
                VALUES (@Cpf, @Nome, @Email, @Senha, @NivelAcesso)";
    await db.ExecuteAsync(sql, usuario);
    return Results.Created($"/api/usuarios/{usuario.Cpf}", usuario);
});

app.MapPost("/api/login", async (LoginRequest login) =>
{
    using var db = new SqlConnection(connectionString);
    var sql = "SELECT * FROM Usuarios WHERE LOWER(TRIM(Email)) = LOWER(TRIM(@Email)) AND Senha = @Senha";
    var user = await db.QueryFirstOrDefaultAsync<Usuario>(sql, login);
    if (user != null) return Results.Ok(user);
    return Results.BadRequest("E-mail ou senha incorretos.");
});

// --- 2. EVENTOS (CORRIGIDO PARA INCLUIR O LUGAR NO INSERT) ---

app.MapPost("/api/eventos", async (Evento ev) => {
    using var db = new SqlConnection(connectionString); 
    
    // CORREÇÃO AQUI: Adicionado 'Lugar' tanto na lista de colunas quanto nos @parâmetros
    var sql = @"INSERT INTO Eventos (Nome, Lugar, CapacidadeTotal, DataEvento, PrecoPadrao, ImagemURL) 
                VALUES (@Nome, @Lugar, @CapacidadeTotal, @DataEvento, @PrecoPadrao, @ImagemURL)";
    
    await db.ExecuteAsync(sql, ev);
    return Results.Ok("Show anunciado com sucesso!");
});

app.MapGet("/api/eventos", async () => {
    using var db = new SqlConnection(connectionString);
    var sql = "SELECT * FROM Eventos ORDER BY DataEvento ASC";
    var eventos = await db.QueryAsync<Evento>(sql);
    return Results.Ok(eventos);
});

app.MapDelete("/api/eventos/{id}", async (int id) => {
    using var db = new SqlConnection(connectionString);
    await db.ExecuteAsync("DELETE FROM Eventos WHERE Id = @id", new { id });
    return Results.Ok("Evento removido!");
});

// --- 3. CUPONS ---
app.MapPost("/api/cupons", async (Cupom cp) => {
    using var db = new SqlConnection(connectionString);
    var sql = @"INSERT INTO Cupons (codigo, PorcentagemDesconto, valorMinimoregra) 
                VALUES (@codigo, @PorcentagemDesconto, @valorMinimoregra)";
    await db.ExecuteAsync(sql, cp);
    return Results.Ok("Cupom criado!");
});

app.MapGet("/api/cupons", async () => {
    using var db = new SqlConnection(connectionString);
    return Results.Ok(await db.QueryAsync<Cupom>("SELECT * FROM Cupons"));
});
// NOVA ROTA: Necessária para o botão "Aplicar Cupom" do cliente funcionar
app.MapGet("/api/cupons/{codigo}", async (string codigo) => {
    using var db = new SqlConnection(connectionString);
    // Busca o cupom pelo código exato
    var sql = "SELECT * FROM Cupons WHERE UPPER(codigo) = UPPER(@codigo)";
    var cupom = await db.QueryFirstOrDefaultAsync<Cupom>(sql, new { codigo });
    
    if (cupom != null) return Results.Ok(cupom);
    return Results.NotFound("Cupom não encontrado.");
});

app.MapDelete("/api/cupons/{codigo}", async (string codigo) => {
    using var db = new SqlConnection(connectionString);
    await db.ExecuteAsync("DELETE FROM Cupons WHERE codigo = @codigo", new { codigo });
    return Results.Ok("Cupom removido!");
});

// --- 4. RESERVAS ---
app.MapPost("/api/reservas", async (Reserva res) => {
    using var db = new SqlConnection(connectionString);
    var sql = @"INSERT INTO Reservas (UsuarioCpf, EventoId, ValorFinalPago) 
                VALUES (@UsuarioCpf, @EventoId, @ValorFinalPago)";
    await db.ExecuteAsync(sql, res);
    return Results.Ok("Reserva realizada com sucesso!");
});

app.Run();

// --- MODELS ---
public record Usuario(string Cpf, string Nome, string Email, string Senha, int NivelAcesso);
public record LoginRequest(string Email, string Senha);

// O record Evento já estava certo, mas o SQL acima não usava o campo 'Lugar'
public record Evento(int? Id, string Nome, string Lugar, int CapacidadeTotal, DateTime DataEvento, decimal PrecoPadrao, string ImagemURL);

public record Cupom(string codigo, decimal PorcentagemDesconto, decimal valorMinimoregra);
public record Reserva(string UsuarioCpf, int EventoId, decimal ValorFinalPago);