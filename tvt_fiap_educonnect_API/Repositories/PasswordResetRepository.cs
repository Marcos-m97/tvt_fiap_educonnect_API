using EduConnect_API.Data;
using EduConnect_API.Models;
using Microsoft.EntityFrameworkCore;

public class PasswordResetRepository : IPasswordResetRepository
{
    private readonly AppDbContext _context;

    public PasswordResetRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task Salvar(PasswordResetCode code)
    {
        _context.PasswordResetCodes.Add(code);
        await _context.SaveChangesAsync();
    }

    public async Task<PasswordResetCode?> Obter(string email, string codigo)
    {
        return await _context.PasswordResetCodes
            .Where(x => x.Email == email && x.Codigo == codigo)
            .OrderByDescending(x => x.ExpiraEm)
            .FirstOrDefaultAsync();
    }

    public async Task Atualizar(PasswordResetCode code)
    {
        _context.PasswordResetCodes.Update(code);
        await _context.SaveChangesAsync();
    }
}
