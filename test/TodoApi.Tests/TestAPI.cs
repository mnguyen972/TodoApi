using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using TodoApi.AutoMapperConfiguration;
using TodoApi.Controllers;
using TodoApi.Models;
using Xunit;

namespace TodoApi.Tests
{
    public class TestAPI
    {
        private static async Task<TodoContext> GetContextWithData()
        {
            var options = new DbContextOptionsBuilder<TodoContext>()
                              .UseInMemoryDatabase(Guid.NewGuid().ToString())
                              .Options;

            var context = new TodoContext(options);

            context.TodoItems.Add(new TodoItem() { Id = 1, Name = "test1", IsComplete = true });
            context.TodoItems.Add(new TodoItem() { Id = 2, Name = "test2", IsComplete = true });
            context.TodoItems.Add(new TodoItem() { Id = 3, Name = "test3", IsComplete = true });

            await context.SaveChangesAsync();

            return context;
        }

        private static Mapper GetMapper()
        {
            var mappingProfile = new MappingProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(mappingProfile));

            return new Mapper(configuration);
        }

        [Fact]
        public async Task Test_GetTodoItems()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<TodoItemsController>>();
            var controller = new TodoItemsController(await GetContextWithData(), GetMapper(), mockLogger.Object);

            // Act
            var result = await controller.GetTodoItems();

            // Assert
            Assert.Equal(3, result?.Value?.Count());
        }

        [Fact]
        public async Task Test_GetTodoItem()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<TodoItemsController>>();
            var controller = new TodoItemsController(await GetContextWithData(), GetMapper(), mockLogger.Object);

            // Act
            var result = await controller.GetTodoItem(1);

            // Assert
            Assert.Equal(1, result?.Value?.Id);
        }

        [Fact]
        public async Task Test_PutTodoItem()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<TodoItemsController>>();
            var controller = new TodoItemsController(await GetContextWithData(), GetMapper(), mockLogger.Object);

            // Act
            var actionResult = await controller.PutTodoItem(1, new TodoItemDTO { Id = 1, Name = "test1x", IsComplete = true });

            // Assert
            var result = actionResult as StatusCodeResult;
            Assert.Equal(StatusCodes.Status204NoContent, result?.StatusCode);
        }

        [Fact]
        public async Task Test_PostTodoItem()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<TodoItemsController>>();
            var controller = new TodoItemsController(await GetContextWithData(), GetMapper(), mockLogger.Object);

            // Act
            var actionResult = await controller.PostTodoItem(new TodoItemDTO { Id = 4, Name = "test4", IsComplete = true });

            // Assert
            var result = actionResult.Result as CreatedAtActionResult;
            Assert.Equal(StatusCodes.Status201Created, result?.StatusCode);
        }

        [Fact]
        public async Task Test_DeleteTodoItem()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<TodoItemsController>>();
            var controller = new TodoItemsController(await GetContextWithData(), GetMapper(), mockLogger.Object);

            // Act
            var actionResult = await controller.DeleteTodoItem(1);

            // Assert
            var result = actionResult as StatusCodeResult;
            Assert.Equal(StatusCodes.Status204NoContent, result?.StatusCode);
        }
    }
}