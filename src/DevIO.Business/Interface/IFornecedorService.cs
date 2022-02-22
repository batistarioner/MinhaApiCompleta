using DevIO.Business.Models;
using System;
using System.Threading.Tasks;

namespace DevIO.Business.Interface
{
    public interface IFornecedorService : IDisposable
    {
        Task<bool> Adicionar(Fornecedor fornecedor);

        Task<bool> Atualizar(Fornecedor fornecedor);

        Task AtualizarEndereco(Endereco endereco);

        Task<bool> Remover(Guid id);
    }
}
