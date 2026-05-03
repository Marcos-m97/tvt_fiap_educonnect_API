using System;

namespace EduConnect_API.Models
{
    /// <summary>
    /// Entidade que representa uma aula cadastrada dentro de uma turma/disciplina.
    ///
    /// No EduConnect, a aula é um conteúdo acadêmico disponibilizado pelo professor
    /// dentro de uma disciplina ofertada em uma turma específica.
    ///
    /// A aula pode conter título, descrição, vídeo complementar, vídeo principal,
    /// material de apoio em PDF e observações adicionais.
    ///
    /// O vínculo com TurmaDisciplina permite saber em qual turma, disciplina
    /// e professor essa aula está inserida.
    /// </summary>
    public class Aula
    {
        /// <summary>
        /// Identificador único da aula no banco de dados.
        /// </summary>
        public int Id { get; set; }

        // ============================================================
        // 1. RELACIONAMENTO COM TURMA/DISCIPLINA
        // ============================================================

        /// <summary>
        /// Chave estrangeira que vincula a aula a uma associação entre turma,
        /// disciplina e professor.
        ///
        /// Esse vínculo indica em qual contexto acadêmico a aula será exibida.
        /// </summary>
        public int TurmaDisciplinaId { get; set; }

        /// <summary>
        /// Propriedade de navegação para acessar a associação entre turma,
        /// disciplina e professor.
        ///
        /// O null! é utilizado porque o Entity Framework preenche essa propriedade
        /// quando a relação é carregada com Include. Na criação da aula,
        /// normalmente informamos apenas o TurmaDisciplinaId.
        /// </summary>
        public TurmaDisciplina TurmaDisciplina { get; set; } = null!;

        // ============================================================
        // 2. DADOS DA AULA
        // ============================================================

        /// <summary>
        /// Título da aula exibido para professores e alunos.
        /// </summary>
        public string Titulo { get; set; } = string.Empty;

        /// <summary>
        /// Descrição da aula.
        ///
        /// Pode conter resumo do conteúdo, objetivos da aula ou instruções
        /// para os alunos.
        /// </summary>
        public string Descricao { get; set; } = string.Empty;

        /// <summary>
        /// URL de vídeo complementar da aula.
        ///
        /// Pode apontar para plataformas externas como YouTube, Microsoft Stream,
        /// Google Drive ou outro serviço de vídeo.
        /// </summary>
        public string UrlVideo { get; set; } = string.Empty;

        /// <summary>
        /// Caminho do vídeo principal da aula salvo no servidor.
        ///
        /// Esse campo é opcional, pois nem toda aula precisa ter um arquivo
        /// de vídeo enviado diretamente para a plataforma.
        /// </summary>
        public string? VideoAula { get; set; }

        /// <summary>
        /// Caminho do material de apoio da aula.
        ///
        /// Normalmente utilizado para armazenar PDFs ou outros materiais
        /// complementares disponibilizados pelo professor.
        /// </summary>
        public string? MaterialApoio { get; set; }

        /// <summary>
        /// Campo opcional para observações adicionais da aula.
        ///
        /// Pode ser usado para instruções extras, avisos ou comentários
        /// do professor.
        /// </summary>
        public string? Observacoes { get; set; }

        // ============================================================
        // 3. CONTROLE E AUDITORIA
        // ============================================================

        /// <summary>
        /// Data de criação da aula.
        ///
        /// O valor padrão é definido em UTC para manter consistência no backend.
        /// </summary>
        public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Identificação simples de quem criou a aula.
        ///
        /// Esse campo pode armazenar o nome, e-mail ou identificador do professor/admin
        /// responsável pela criação do conteúdo.
        /// </summary>
        public string CriadoPor { get; set; } = string.Empty;
    }
}