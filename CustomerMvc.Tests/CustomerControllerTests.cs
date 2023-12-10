using CustomerMvc.Controllers;
using CustomerMvc.Domain.Interfaces;
using CustomerMvc.Domain.Models;
using CustomerMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Net;

namespace CustomerMvc.Tests
{
    public class CustomerControllerTests
    {
        private List<CustomerModel> GetCustomerModelList()
        {
            return new List<CustomerModel>()
            {
                new CustomerModel()
                {
                    Id = 1,
                    Name = "Harshani",
                    Email = "harshani@email.com",
                    Address = "aaa"
                },
                new CustomerModel()
                {
                    Id = 2,
                    Name = "Viraj",
                    Email = "viraj@email.com",
                    Address = "bbb"
                },
                new CustomerModel()
                {
                    Id = 3,
                    Name = "saman",
                    Email = "saman@email.com",
                    Address = "ddd"
                }
            };

        }

        #region Index

        [Fact]
        public async Task Index_AllCustomers_ReturnsViewWithCustomers()
        {
            // Arrange
            var customerApiServiceMock = new Mock<ICustomerHttpClientService>();

            var customers = GetCustomerModelList();

            _ = customerApiServiceMock.Setup(service => service.GetCustomersAsync())
                                  .ReturnsAsync(new KeyValuePair<HttpStatusCode, List<CustomerModel>?>(HttpStatusCode.OK, customers));

            var controller = new CustomerController(customerApiServiceMock.Object);

            // Act
            var result = await controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<List<CustomerModel>>(viewResult.Model);
            Assert.Equal(customers, model);
        }

        [Fact]
        public async Task Index_NoCustomers_ReturnsNotificationView()
        {
            // Arrange
            var customerApiServiceMock = new Mock<ICustomerHttpClientService>();
            _ = customerApiServiceMock.Setup(service => service.GetCustomersAsync())
                                  .ReturnsAsync(new KeyValuePair<HttpStatusCode, List<CustomerModel>?>(HttpStatusCode.NoContent, new List<CustomerModel>()));

            var controller = new CustomerController(customerApiServiceMock.Object);

            // Act
            var result = await controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Notification", viewResult.ViewName);

            var model = Assert.IsType<NotificationModel>(viewResult.Model);
            Assert.Equal(HttpStatusCode.NoContent.ToString(), model.StatusCode);
            Assert.Equal("There are no customers to display.", model.Message);
        }

        [Fact]
        public async Task Index_BadRequestStatus_ReturnsNotificationView()
        {
            // Arrange
            var customerApiServiceMock = new Mock<ICustomerHttpClientService>();
            customerApiServiceMock.Setup(service => service.GetCustomersAsync())
                                  .ReturnsAsync(new KeyValuePair<HttpStatusCode, List<CustomerModel>?>(HttpStatusCode.BadRequest, new List<CustomerModel>()));

            var controller = new CustomerController(customerApiServiceMock.Object);

            // Act
            var result = await controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Notification", viewResult.ViewName);
            var model = Assert.IsType<NotificationModel>(viewResult.Model);
            Assert.Equal(HttpStatusCode.BadRequest.ToString(), model.StatusCode);
            Assert.Equal("The request is invalid. Please check your data.", model.Message);
        }

        [Fact]
        public async Task Index_500Status_ReturnsErrorView()
        {
            // Arrange
            var customerApiServiceMock = new Mock<ICustomerHttpClientService>();
            customerApiServiceMock.Setup(service => service.GetCustomersAsync())
                                  .ReturnsAsync(new KeyValuePair<HttpStatusCode, List<CustomerModel>?>(HttpStatusCode.InternalServerError, new List<CustomerModel>()));

            var controller = new CustomerController(customerApiServiceMock.Object);

            // Act
            var result = await controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);

            var model = Assert.IsType<ErrorViewModel>(viewResult.Model);
            Assert.Equal(HttpStatusCode.InternalServerError.ToString(), model.StatusCode);
            Assert.Equal("Internal Server Error!", model.Message);
        }

        [Fact]
        public async Task Index_ThrowsException_ReturnsException()
        {
            // Arrange
            var customerApiServiceMock = new Mock<ICustomerHttpClientService>();
            customerApiServiceMock.Setup(service => service.GetCustomersAsync())
                                  .ThrowsAsync(new Exception("Test exception"));

            var controller = new CustomerController(customerApiServiceMock.Object);

            // Act
            // Assert
            await Assert.ThrowsAsync<Exception>(async () => await controller.Index());
        }

        #endregion

        #region Details

        [Fact]
        public async Task Details_ValidId_ReturnsViewWithCustomer()
        {
            // Arrange
            int? customerId = 1;

            var customer = new CustomerModel { Id = 1, Name = "Harshani", Email = "harshani@email.com", Address = "aaa" };

            var customerApiServiceMock = new Mock<ICustomerHttpClientService>();
            _ = customerApiServiceMock.Setup(service => service.GetCustomer(customerId))
                                  .ReturnsAsync(new KeyValuePair<HttpStatusCode, CustomerModel?>(HttpStatusCode.OK, customer));

            var controller = new CustomerController(customerApiServiceMock.Object);

            // Act
            var result = await controller.Details(customerId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<CustomerModel>(viewResult.Model);
            Assert.Equal(customer, model);
        }

        [Theory]
        [InlineData(100)]
        [InlineData(0)] 
        public async Task GeDetails_InvalidId_ReturnsNotificationView(int? customerId)
        {
            // Arrange
            var customerApiServiceMock = new Mock<ICustomerHttpClientService>();
            customerApiServiceMock.Setup(service => service.GetCustomer(customerId))
                                  .ReturnsAsync(new KeyValuePair<HttpStatusCode, CustomerModel?>(HttpStatusCode.NotFound, null));

            var controller = new CustomerController(customerApiServiceMock.Object);

            // Act
            var result = await controller.Details(customerId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Notification", viewResult.ViewName);

            var model = Assert.IsType<NotificationModel>(viewResult.Model);
            Assert.Equal(HttpStatusCode.NotFound.ToString(), model.StatusCode);
            Assert.Equal("No customers found.", model.Message);
        }

        [Fact]
        public async Task Details_NotFoundStatus_ReturnsNotificationView()
        {
            // Arrange
            var customerId = 1;
            var customerApiServiceMock = new Mock<ICustomerHttpClientService>();
            customerApiServiceMock.Setup(service => service.GetCustomer(customerId))
                                  .ReturnsAsync(new KeyValuePair<HttpStatusCode, CustomerModel?>(HttpStatusCode.NotFound, null));

            var controller = new CustomerController(customerApiServiceMock.Object);

            // Act
            var result = await controller.Details(customerId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Notification", viewResult.ViewName);

            var model = Assert.IsType<NotificationModel>(viewResult.Model);
            Assert.Equal(HttpStatusCode.NotFound.ToString(), model.StatusCode);
            Assert.Equal("No customers found.", model.Message);
        }

        [Fact]
        public async Task Details_BadRequestStatus_ReturnsNotificationView()
        {
            // Arrange
            var customerId = 1;
            var customerApiServiceMock = new Mock<ICustomerHttpClientService>();
            customerApiServiceMock.Setup(service => service.GetCustomer(customerId))
                                  .ReturnsAsync(new KeyValuePair<HttpStatusCode, CustomerModel?>(HttpStatusCode.BadRequest, null));

            var controller = new CustomerController(customerApiServiceMock.Object);

            // Act
            var result = await controller.Details(customerId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Notification", viewResult.ViewName);

            var model = Assert.IsType<NotificationModel>(viewResult.Model);
            Assert.Equal(HttpStatusCode.BadRequest.ToString(), model.StatusCode);
            Assert.Equal("The request is invalid. Please check your data.", model.Message);
        }

        [Fact]
        public async Task Details_500Status_ReturnsErrorView()
        {
            // Arrange
            var customerId = 1;
            var customerApiServiceMock = new Mock<ICustomerHttpClientService>();
            customerApiServiceMock.Setup(service => service.GetCustomer(customerId))
                                  .ReturnsAsync(new KeyValuePair<HttpStatusCode, CustomerModel?>(HttpStatusCode.InternalServerError, null));

            var controller = new CustomerController(customerApiServiceMock.Object);

            // Act
            var result = await controller.Details(customerId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);

            var model = Assert.IsType<ErrorViewModel>(viewResult.Model);
            Assert.Equal(HttpStatusCode.InternalServerError.ToString(), model.StatusCode);
            Assert.Equal("Internal Server Error!", model.Message);
        }

        #endregion

        #region Create

        [Fact]
        public async Task Create_ValidModelState_RedirectsToIndex()
        {
            // Arrange
            var customerModel = new CustomerModel { Id = 1, Name = "Harshani", Email = "harshani@email.com", Address = "aaa" };

            var customerApiServiceMock = new Mock<ICustomerHttpClientService>();
            customerApiServiceMock.Setup(service => service.SaveCustomer(customerModel))
                                  .ReturnsAsync(new KeyValuePair<HttpStatusCode, CustomerModel?>(HttpStatusCode.Created, customerModel));

            var controller = new CustomerController(customerApiServiceMock.Object);

            // Act
            var result = await controller.Create(customerModel);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task Create_InvalidModelState_ReturnsViewWithModel()
        {
            // Arrange
            var customerModel = new CustomerModel { Id = 1, Name = "Harshani", Email = "harshani@email.com", Address = "aaa" };

            var customerApiServiceMock = new Mock<ICustomerHttpClientService>();

            var controller = new CustomerController(customerApiServiceMock.Object);
            controller.ModelState.AddModelError("Phone", "Phone number must be 11 digits.");

            // Act
            var result = await controller.Create(customerModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.NotNull(result);
            Assert.Equal(customerModel, viewResult.Model);
        }

        [Theory]
        [InlineData(HttpStatusCode.NoContent, "Notification")]
        [InlineData(HttpStatusCode.BadRequest, "Notification")]
        [InlineData(HttpStatusCode.InternalServerError, "Error")]
        public async Task Create_MutipleStatusCodes_RedirectsToCorrectViews(HttpStatusCode statusCode, string expectedViewName)
        {
            // Arrange
            var customerApiServiceMock = new Mock<ICustomerHttpClientService>();
            var customerModel = new CustomerModel { Id = 1, Name = "Harshani", Email = "harshani@email.com", Address = "aaa" };
            customerApiServiceMock.Setup(service => service.SaveCustomer(customerModel))
                                  .ReturnsAsync(new KeyValuePair<HttpStatusCode, CustomerModel?>(statusCode, null));

            var controller = new CustomerController(customerApiServiceMock.Object);

            // Act
            var result = await controller.Create(customerModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(expectedViewName, viewResult.ViewName);
        }

        #endregion
    }
}