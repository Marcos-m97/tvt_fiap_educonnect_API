using EduConnect_API.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EduConnect_API.Services.Interfaces
{
    public interface IAdminService
    {
        Task<AdminDTO> Criar(CriarAdminDTO dto);
        Task<AdminDTO?> ObterPorUsuario(Guid usuarioId);
        Task<IEnumerable<AdminDTO>> Listar();
        Task<AdminDTO?> Atualizar(Guid id, CriarAdminDTO dto);
    }
}
