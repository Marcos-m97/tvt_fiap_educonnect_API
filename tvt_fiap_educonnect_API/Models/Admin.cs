using System;

namespace EduConnect_API.Models
{
    /// <summary>
    /// Entidade que representa o perfil administrativo de um usuário no EduConnect.
    ///
    /// No sistema, o Admin é uma especialização da entidade Usuario.
    /// Enquanto Usuario concentra dados comuns de autenticação e autorização,
    /// Admin armazena informações específicas do contexto administrativo,
    /// como departamento e cargo.
    ///
    /// Esse perfil é utilizado para operações de gestão acadêmica, como criação
    /// de cursos, turmas, disciplinas, usuários e acompanhamento de matrículas.
    /// </summary>
    public class Admin
    {
        /// <summary>
        /// Identificador único do perfil administrativo no banco de dados.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Chave estrangeira que vincula o administrador a um usuário do sistema.
        ///
        /// Todo administrador também é um usuário, pois precisa autenticar
        /// na plataforma e possuir permissões específicas.
        /// </summary>
        public int UsuarioId { get; set; }

        /// <summary>
        /// Propriedade de navegação para acessar os dados do usuário vinculado.
        ///
        /// O null! é utilizado porque o Entity Framework preenche essa propriedade
        /// quando a relação é carregada com Include ou pelo tracking do contexto.
        /// Na criação do Admin, normalmente informamos apenas o UsuarioId.
        /// </summary>
        public Usuario Usuario { get; set; } = null!;

        /// <summary>
        /// Departamento ao qual o administrador pertence.
        ///
        /// Exemplo: Secretaria Acadêmica, Coordenação, Gestão EAD ou RH.
        /// </summary>
        public string? Departamento { get; set; }

        /// <summary>
        /// Cargo ou função administrativa exercida pelo usuário.
        ///
        /// Exemplo: Coordenador, Analista Acadêmico, Gestor EAD ou Administrador.
        /// </summary>
        public string? Cargo { get; set; }
    }
}