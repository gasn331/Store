
# Store API

This project implements a Web API for managing orders and products in an e-commerce store, built with ASP.NET Core. It follows Domain-Driven Design (DDD) principles, and provides essential functionalities to create, manage, and query orders and products.

## Table of Contents
- [Objective](#objective)
- [Implemented Features](#implemented-features)
- [Routes](#routes)
- [Requirements](#requirements)
- [Testing](#testing)
- [Setup Instructions](#setup-instructions)
- [Technologies Used](#technologies-used)
- [License](#license)

## Objective

The goal of this project is to create a Web API in ASP.NET Core that allows users to manage orders in an e-commerce store. The API includes essential functionalities such as adding/removing products to/from an order, closing orders, and retrieving orders by their ID.

## Implemented Features

- **Create a new order**: Allows the creation of a new order.
- **Add products to an order**: Allows products to be added to an open order.
- **Remove products from an order**: Allows removal of products from an open order.
- **Close an order**: Closes an order after ensuring it has at least one product.
- **List orders**: Retrieves all orders with pagination and filters.
- **Get order by ID**: Retrieves a specific order by its ID, including all associated products.

### Conditions
- Products cannot be added or removed from a closed order.
- An order can only be closed if it contains at least one product.

## Routes

### POST /orders
- **Description**: Starts a new order.
- **Request**: None
- **Response**: The ID of the created order.

### POST /orders/{orderId}/products
- **Description**: Adds a product to an existing order.
- **Request**: Product details (ID, name, price).
- **Response**: None (Returns a status indicating success).

### DELETE /orders/{orderId}/products/{productId}
- **Description**: Removes a product from an existing order.
- **Request**: None
- **Response**: None (Returns a status indicating success).

### POST /orders/{orderId}/close
- **Description**: Closes an existing order.
- **Request**: None
- **Response**: None (Returns a status indicating success).

### GET /orders
- **Description**: Retrieves a list of orders, with pagination and filtering by status (open/closed).
- **Request**: Page number, page size, optional filters (status).
- **Response**: List of orders.

### GET /orders/{orderId}
- **Description**: Retrieves a specific order by its ID.
- **Request**: Order ID.
- **Response**: Order details (including products).

## Requirements

- **Entity Framework Core**: Used for persistence.
- **In-Memory Database**: The project uses an in-memory database for simplicity.
- **Swagger**: Integrated for API documentation.
- **DDD Principles**: The project follows Domain-Driven Design (DDD) principles, with clearly separated layers for domain, application, and infrastructure.

## Testing

To test the API, you can use Swagger UI. Here’s a summary of the steps to test each feature:

1. **Create an order**:
   - Send a `POST` request to `/orders`.
   - The response will return the `ID` of the created order.

2. **Add a product**:
   - Send a `POST` request to `/orders/{orderId}/products` with product details.
   - Check that the product has been added by retrieving the order details.

3. **Remove a product**:
   - Send a `DELETE` request to `/orders/{orderId}/products/{productId}`.
   - Verify the product is removed by checking the order details.

4. **Close the order**:
   - Send a `POST` request to `/orders/{orderId}/close`.
   - Verify that the order is closed by checking the order status.

5. **List orders**:
   - Send a `GET` request to `/orders`.
   - Verify that orders are returned with pagination.

6. **Get order by ID**:
   - Send a `GET` request to `/orders/{orderId}`.
   - Verify that the correct order and associated products are returned.

## Setup Instructions

1. Clone this repository to your local machine.
   ```bash
   git clone https://github.com/gasn331/Store.git
   ```
   
2. Navigate to the project folder.
   ```bash
   cd Store
   ```
   
3. Open the solution file (`Store.sln`) in Visual Studio or Visual Studio Code.

4. Restore the required NuGet packages:
   ```bash
   dotnet restore
   ```

5. Run the application:
   ```bash
   dotnet run
   ```

6. Access the API documentation via Swagger at `http://localhost:5000/swagger` (by default).

## Technologies Used

- **ASP.NET Core**: Web API framework.
- **Entity Framework Core**: ORM for database access.
- **In-Memory Database**: Simplified persistence for testing.
- **Swagger**: For API documentation.
- **DDD (Domain-Driven Design)**: Project structure based on DDD principles.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
