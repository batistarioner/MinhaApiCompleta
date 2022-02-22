using DevIO.Business.Interface;
using DevIO.Business.Models;
using DevIO.Data.Context;
using DevIO.Data.Reposity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevIO.Data.Repository
{
    public class EnderecoRepository : Repository<Endereco>, IEnderecoRepository
    {
        public EnderecoRepository(MeuDbContext context) : base(context) { }

        public async Task<Endereco> ObterEnderecoPorFornecedor(Guid fornecedorId)
        {
            return await Db.Enderecos.AsNoTracking()
                .Include(f => f.Fornecedor)
                .FirstOrDefaultAsync(e => e.FornecedorId == fornecedorId);
        }
    }
}
