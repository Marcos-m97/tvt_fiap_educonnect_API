using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EduConnect_API.Services
{
    /// <summary>
    /// Serviço responsável por gerar tokens JWT para autenticação no EduConnect.
    ///
    /// Após um login válido, o UsuarioController chama este serviço para gerar
    /// um token contendo as informações principais do usuário autenticado.
    ///
    /// Esse token é enviado ao frontend e usado nas próximas requisições
    /// para acessar endpoints protegidos com [Authorize].
    /// </summary>
    public class JwtService
    {
        private readonly IConfiguration _config;

        /// <summary>
        /// Recebe as configurações da aplicação por injeção de dependência.
        ///
        /// As informações do JWT, como chave secreta, issuer, audience
        /// e tempo de expiração, são lidas da seção "Jwt" do appsettings.
        /// </summary>
        public JwtService(IConfiguration config)
        {
            _config = config;
        }

        // ============================================================
        // 1. GERAR TOKEN JWT
        // ============================================================

        /// <summary>
        /// Gera um token JWT para o usuário autenticado.
        ///
        /// O token contém claims com:
        /// - id do usuário;
        /// - nome do usuário;
        /// - tipo do usuário;
        /// - role usada pelo atributo [Authorize(Roles = "...")].
        ///
        /// No EduConnect, o campo tipo define o perfil:
        /// 0 = SuperAdmin
        /// 1 = Admin
        /// 2 = Professor
        /// 3 = Aluno
        /// </summary>
        public string GenerateToken(int id, string nome, int tipo)
        {
            var jwtSettings = _config.GetSection("Jwt");

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings["Key"])
            );

            var creds = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256
            );

            var claims = new[]
            {
                // Claim customizada usada para recuperar o ID do usuário logado.
                new Claim("id", id.ToString()),

                // Claim customizada com o nome do usuário autenticado.
                new Claim("nome", nome),

                // Claim customizada com o tipo numérico do usuário.
                new Claim("tipo", tipo.ToString()),

                // Claim padrão de role usada pelo ASP.NET Core Authorization.
                // É essa claim que permite validar permissões com [Authorize(Roles = "...")].
                new Claim(ClaimTypes.Role, tipo.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    Convert.ToInt32(jwtSettings["ExpiresInMinutes"])
                ),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}