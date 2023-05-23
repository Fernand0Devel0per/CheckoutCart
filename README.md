# 📰 TechShop API

Bem-vindo à TechShop API, uma solução de e-commerce voltada para produtos de tecnologia, desenvolvida em ASP.NET Core 7.0. Esta API foi projetada para oferecer uma maneira intuitiva e eficaz de gerenciar um carrinho de compras, mantendo as melhores práticas do Design Orientado a Domínio (DDD). Com o suporte direto ao SQL Server através da biblioteca System.Data.SqlClient, a TechShop API pode ser adaptada para lidar com qualquer tipo de produto.

# 🌟 Funcionalidades

1. Autenticação JWT.
2. CRUD completo para produtos, pedidos, usuários e categorias de produtos.
3. Recuperação e atualização de status do pedido.
4. Atualização de status e quantidade do produto em um pedido.
5. Documentação através do Swagger.

# 🚀 Começando

Para começar a usar a TechShop API, siga as etapas abaixo:

1. Clone este repositório em sua máquina local.
2. Configure a string de conexão do SQL Server no arquivo appsettings.json.
3. Execute o comando dotnet restore para restaurar os pacotes NuGet necessários.
4. Execute o comando dotnet run para iniciar a aplicação.

A API estará disponível no endereço [http://localhost:5000](http://localhost:5000/).

# 📚 Rotas

A TechShop API possui uma série de rotas para gerenciar usuários, produtos, pedidos, categorias de produtos e status de pedidos. Algumas das principais rotas incluem:

- Autenticação:
    - POST /api/auth/login: Realiza o login de um usuário.
- Categorias de produtos:
    - GET /api/category: Recupera todas as categorias de produtos.
- Status de pedidos:
    - GET /api/status: Recupera todos os status de pedidos.
- Usuários:
    - GET /api/users/username/{username}: Recupera um usuário pelo nome de usuário.
    - POST /api/users: Cria um novo usuário.
    - PUT /api/users: Atualiza um usuário existente.
- Produtos:
    - POST /api/products: Cria um novo produto.
    - PUT /api/products/{id}: Atualiza um produto existente.
    - PUT /api/products/{id}/status: Altera o status de um produto.
    - DELETE /api/products/{id}: Exclui um produto.
    - GET /api/products/{id}: Recupera um produto pelo ID.
    - GET /api/products: Recupera todos os produtos.
    - GET /api/products/category/{categoryCode}: Recupera todos os produtos de uma determinada categoria.
- Pedidos:
    - POST /api/order: Cria um novo pedido.
    - PUT /api/order/{id}: Atualiza o status de um pedido.
    - GET /api/order/{id}: Recupera um pedido pelo ID.
    - GET /api/order/{id}/products: Recupera um pedido pelo ID, incluindo seus produtos.
    - GET /api/order/user/{userId}: Recupera todos os pedidos de um usuário.
    - POST /api/order/{orderId}/products: Adiciona um produto a um pedido.
    - PUT /api/order/{orderId}/products/{productId}/quantity/{newQuantity}: Atualiza a quantidade de um produto em um pedido.
    - DELETE /api/order/{orderId}/products/{productId}: Remove um produto de um pedido.
    
    Mais detalhes podem ser encontrados na documentação do Swagger.
    

# 🛠️ Tecnologias utilizadas

- ASP.NET Core 7.0
- SQL Server
- System.Data.SqlClient
- AutoMapper
- Newtonsoft.Json
- Swagger

# 📖 Conclusão

A TechShop API é uma solução de carrinho de compras completa, seguindo as melhores práticas de desenvolvimento e arquitetura. Com esta API, você pode criar, atualizar, excluir e recuperar produtos e pedidos, além de gerenciar usuários e categorias de produtos.

Além disso, a API segue os princípios RESTful, facilitando a integração com outras aplicações e garantindo escalabilidade e manutenibilidade.

Esperamos que você aproveite este projeto e que ele atenda às suas necessidades de e-commerce. Se você tiver alguma dúvida ou sugestão, sinta-se à vontade para abrir uma issue ou enviar um pull request.

Boa sorte e feliz codificação! 🚀👩‍💻👨‍💻


## English Version


# 📰 TechShop API

Welcome to the TechShop API, an e-commerce solution targeted at technology products, developed in ASP.NET Core 7.0. This API is designed to offer an intuitive and effective way to manage a shopping cart while adhering to Domain-Driven Design (DDD) best practices. With direct support for SQL Server through the System.Data.SqlClient library, the TechShop API can be adapted to handle any type of product.

# 🌟 Features

    JWT Authentication.
    Full CRUD for products, orders, users, and product categories.
    Retrieval and update of order status.
    Update of product status and quantity within an order.
    Documentation through Swagger.


# 🚀 Getting Started

To get started with the TechShop API, follow the steps below:

    Clone this repository to your local machine.
    Set up the SQL Server connection string in the appsettings.json file.
    Run the dotnet restore command to restore the necessary NuGet packages.
    Run the dotnet run command to start the application.

The API will be available at http://localhost:5000.

# 📚 Routes

The TechShop API has a series of routes to manage users, products, orders, product categories, and order statuses. Some of the key routes include:

    Authentication:
        POST /api/auth/login: Logs a user in.
    Product Categories:
        GET /api/category: Retrieves all product categories.
    Order Statuses:
        GET /api/status: Retrieves all order statuses.
    Users:
        GET /api/users/username/{username}: Retrieves a user by username.
        POST /api/users: Creates a new user.
        PUT /api/users: Updates an existing user.
    Products:
        POST /api/products: Creates a new product.
        PUT /api/products/{id}: Updates an existing product.
        PUT /api/products/{id}/status: Changes a product's status.
        DELETE /api/products/{id}: Deletes a product.
        GET /api/products/{id}: Retrieves a product by ID.
        GET /api/products: Retrieves all products.
        GET /api/products/category/{categoryCode}: Retrieves all products from a particular category.
    Orders:
        POST /api/order: Creates a new order.
        PUT /api/order/{id}: Updates an order's status.
        GET /api/order/{id}: Retrieves an order by ID.
        GET /api/order/{id}/products: Retrieves an order by ID, including its products.
        GET /api/order/user/{userId}: Retrieves all orders for a user.
        POST /api/order/{orderId}/products: Adds a product to an order.
        PUT /api/order/{orderId}/products/{productId}/quantity/{newQuantity}: Updates the quantity of a product in an order.
        DELETE /api/order/{orderId}/products/{productId}: Removes a product from an order.
    More details can be found in the Swagger documentation.


# 🛠️ Technologies Used

    ASP.NET Core 7.0
    SQL Server
    System.Data.SqlClient
    AutoMapper
    Newtonsoft.Json
    Swagger


# 📖 Conclusion

The TechShop API is a complete shopping cart solution, following the best practices of development and architecture. With this API, you can create, update, delete, and retrieve products and orders, as well as manage users and product categories.

Moreover, the API adheres to RESTful principles, making it easy to integrate with other applications and ensuring scalability and maintainability.

We hope you enjoy this project and that it meets