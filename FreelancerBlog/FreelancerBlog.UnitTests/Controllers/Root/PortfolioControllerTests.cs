﻿using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using FreelancerBlog.Areas.Admin.ViewModels.Portfolio;
using FreelancerBlog.Controllers;
using FreelancerBlog.Core.Domain;
using FreelancerBlog.Core.Repository;
using FreelancerBlog.Mapper;
using GenFu;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace FreelancerBlog.UnitTests.Controllers.Root
{
    public class PortfolioControllerTests
    {
        private Mock<IUnitOfWork> _uw;
        private Mock<IFreelancerBlogMapper> _freelancerBlogMapper;
        private Mock<IPortfolioRepository> _portfolioRepository;

        public PortfolioControllerTests()
        {
            _uw = new Mock<IUnitOfWork>();
            _freelancerBlogMapper = new Mock<IFreelancerBlogMapper>();
            _portfolioRepository = new Mock<IPortfolioRepository>();

            _uw.SetupGet<IPortfolioRepository>(u => u.PortfolioRepository).Returns(_portfolioRepository.Object);

            _freelancerBlogMapper.Setup(w => w.PortfolioToPorfolioViewModel(It.IsAny<Portfolio>())).Returns(A.New<PortfolioViewModel>());

            _freelancerBlogMapper.Setup(w => w.PortfolioCollectionToPortfolioViewModelCollection(It.IsAny<List<Portfolio>>()))
                .Returns(A.ListOf<PortfolioViewModel>(10));
        }

        [Fact]
        public async Task Detail_ShoudReturnBadRequest_IfIdIsNotSupplied()
        {
            //Arrange
            var sut = new PortfolioController(_uw.Object, _freelancerBlogMapper.Object);

            //Act
            var result = (BadRequestResult)await sut.Detail(default(int));

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestResult>();
            result.StatusCode.Should().Be(400);
        }

        [Fact(Skip ="Temp")]
        public async Task Detail_ShoudReturnNotFound_IfPorfolioDetailNotFound()
        {
            //Arrange
            var sut = new PortfolioController(_uw.Object, _freelancerBlogMapper.Object);

            //_portfolioRepository.Setup(p => p.FindByIdAsync(It.IsAny<int>())).ReturnsAsync(null);

            //Act
            var result = (NotFoundResult)await sut.Detail(1);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundResult>();
            result.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task Detail_ShoudReturnRequestedDetailView_IfPorfolioDetailExist()
        {
            //Arrange
            var sut = new PortfolioController(_uw.Object, _freelancerBlogMapper.Object);

            _portfolioRepository.Setup(p => p.FindByIdAsync(It.IsAny<int>())).ReturnsAsync(A.New<Portfolio>());

            //Act
            var result = (ViewResult)await sut.Detail(1);


            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ViewResult>();
            result.ViewName.Should().BeNull();
        }

        [Fact]
        public async Task Detail_ShoudReturnPortfolioDetailViewModel_IfPorfolioDetailExist()
        {
            //Arrange
            var sut = new PortfolioController(_uw.Object, _freelancerBlogMapper.Object);

            _portfolioRepository.Setup(p => p.FindByIdAsync(It.IsAny<int>())).ReturnsAsync(A.New<Portfolio>());

            //Act
            var result = (ViewResult)await sut.Detail(1);


            //Assert

            result.Should().NotBeNull();
            result.Should().BeOfType<ViewResult>();
            result.Model.Should().NotBeNull();
            result.Model.Should().BeOfType<PortfolioViewModel>();
        }

        [Fact]
        public async Task Index_ShoudReturnIndexView_Always()
        {
            //Arrange
            var sut = new PortfolioController(_uw.Object, _freelancerBlogMapper.Object);

            _portfolioRepository.Setup(p => p.GetAllAsync()).ReturnsAsync(A.ListOf<Portfolio>(10));

            //Act
            var result = (ViewResult)await sut.Index();

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ViewResult>();
            result.ViewName.Should().BeNull();
        }

        [Fact]
        public async Task Index_ShoudReturnIndexWithPortfolioViewModel_Always()
        {
            //Arrange
            var sut = new PortfolioController(_uw.Object, _freelancerBlogMapper.Object);

            _portfolioRepository.Setup(p => p.GetAllAsync()).ReturnsAsync(A.ListOf<Portfolio>(10));

            //Act
            var result = (ViewResult)await sut.Index();

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ViewResult>();
            result.Model.Should().NotBeNull();
            result.Model.Should().BeOfType<List<PortfolioViewModel>>();
        }
    }
}