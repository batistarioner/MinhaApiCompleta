using AutoMapper;
using DevIO.Api.ViewModels;
using DevIO.Business.Interface;
using DevIO.Business.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DevIO.Api.Controllers
{
    [Route("api/[controller]")]
    public class ProdutosController : MainController
    {
        private readonly IProdutoRepository _produtoRepository;
        private readonly IProdutoService _produtoService;
        private readonly IMapper _mapper;
        public ProdutosController(IProdutoRepository produtoRepository,
                                  IProdutoService produtoService,
                                  IMapper mapper,
                                  INotificador notificador) : base(notificador)
        {
            _produtoRepository = produtoRepository;
            _produtoService = produtoService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IEnumerable<ProdutoViewModel>> ObterTodos()
        {
            return _mapper.Map<IEnumerable<ProdutoViewModel>>(await _produtoRepository.ObterProdutosFornecedores());
        }

        [HttpGet("{id:Guid}")]
        public async Task<ActionResult<ProdutoViewModel>> ObterPorId(Guid id)
        {
            var ProdutoViewModel = await ObterProduto(id);

            if (ProdutoViewModel == null) { return NotFound(); }

            return ProdutoViewModel;
        }

        [HttpPost]
        public async Task<ActionResult<ProdutoViewModel>> Adicionar(ProdutoViewModel produtoViewModel)
        {
            if (!ModelState.IsValid) { return CustomResponse(ModelState); }

            var imagemNome = Guid.NewGuid() + "-" + produtoViewModel.Imagem;

            if(!UploadArquivo(produtoViewModel.ImagemUpload, imagemNome))
            {
                return CustomResponse(produtoViewModel);
            }

            produtoViewModel.Imagem = imagemNome;

            await _produtoService.Adicionar(_mapper.Map<Produto>(produtoViewModel));

            return CustomResponse(produtoViewModel);
        }

        [HttpPut("{id:Guid}")]
        public async Task<ActionResult<ProdutoViewModel>> Atualizar(Guid id, ProdutoViewModel produtoViewModel)
        {
            if (produtoViewModel.Id != id)
            {
                NotificarErro("O Id informado não é o mesmo que foi passado na query");
                return CustomResponse(produtoViewModel);
            }

            if (!ModelState.IsValid) { return CustomResponse(ModelState); }

            await _produtoService.Atualizar(_mapper.Map<Produto>(produtoViewModel));

            return CustomResponse(produtoViewModel);
        }

        [HttpDelete("{id:Guid}")]
        public async Task<ActionResult<ProdutoViewModel>> Excluir(Guid id)
        {
            var ProdutoViewModel = await ObterProduto(id);

            if (ProdutoViewModel == null) { return NotFound(); }

            await _produtoService.Remover(id);

            return CustomResponse();
        }

        private async Task<ProdutoViewModel> ObterProduto(Guid id)
        {
            return _mapper.Map<ProdutoViewModel>(await _produtoRepository.ObterProdutoFornecedor(id));
        }

        private bool UploadArquivo(string arquivo, string imgNome)
        {
            if (string.IsNullOrEmpty(arquivo))
            {
                NotificarErro("Forneça uma imagem para este produto");
                return false;
            }

            var imageDateByteArray = Convert.FromBase64String(arquivo);

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/imagens", imgNome);

            if (System.IO.File.Exists(filePath))
            {
                ModelState.AddModelError(string.Empty, "Já existe um arquivo com este nome");
                return false;
            }

            System.IO.File.WriteAllBytesAsync(filePath, imageDateByteArray);

            return true;
        }
    }
}
     