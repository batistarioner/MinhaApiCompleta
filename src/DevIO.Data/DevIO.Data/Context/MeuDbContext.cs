using DevIO.Business.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DevIO.Data.Context
{
    public class MeuDbContext : DbContext
    {
        public MeuDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Fornecedor> Fornecedores { get; set; }
        public DbSet<Endereco> Enderecos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Defini como default para tamanho varchar(100) para campos do tipo string que não foi configurado tamanho
            foreach (var property in modelBuilder.Model.GetEntityTypes()
                       .SelectMany(e => e.GetProperties()
                       .Where(p => p.ClrType == typeof(string))))
                property.Relational().ColumnType = "varchar(100)";
                
            // Rodar todos DbSets em uma unica vez
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(MeuDbContext).Assembly);

            // Para retirar a rotina que quando excluir um registro pai o sistema exclui os filhos.
            foreach (var relatioship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys())) relatioship.DeleteBehavior = DeleteBehavior.ClientSetNull;

            base.OnModelCreating(modelBuilder);
        }
    }
}
