### **User Story**

**Title**: User Account Management and Product CRUD Operations with Role-Based Authorization

**As a** user,  
**I want to** be able to **create an account**, **log in**, and **manage product records** via an API,  
**so that** I can securely interact with the product data and perform CRUD operations based on my role (Admin or User).

---

### **Acceptance Criteria:**

#### 1. **User Account Management**:
- **As a user**, I can **create an account** with a username, password, and role (e.g., "admin", "user").
- **As a user**, I can **log in** with my username and password, which will generate a **JWT token** that I can use to authenticate future requests.
- **As a user**, my **account** (username, password hash, roles) should be **securely stored** in the database.

#### 2. **Product CRUD Operations (Role-Based Authorization)**:
- **As a user with the "user" role**, I can only **view** and **list** products (via `GET /products` and `GET /products/{id}`).
- **As a user with the "admin" role**, I can **create**, **update**, **delete**, and **view** products (via `POST /products`, `PUT /products/{id}`, `DELETE /products/{id}`, `GET /products/{id}`).
- **As a user**, if I try to access product management operations that are restricted to the "admin" role (Create, Update, Delete), I should receive an **Unauthorized** or **Forbidden** response.

---

### **API Endpoints:**

1. **User Account API (Authentication and Authorization)**:
    - **POST /api/v1/account/register**: Create a new user.
    - **POST /api/v1/account/login**: Log in and return a JWT token for authentication.

2. **Product CRUD API (Requires Authorization)**:
    - **POST /api/v1/products**: Create a new product (requires authenticated **admin** role).
    - **GET /api/v1/products**: Get a list of all products (requires authenticated **admin** or **user** role).
    - **GET /api/v1/products/{id}**: Get a specific product by ID (requires authenticated **admin** or **user**).
    - **PUT /api/v1/products**: Update a product by ID (requires authenticated **admin** role).
    - **DELETE /api/v1/products/{id}**: Delete a product by ID (requires authenticated **admin** role).

---

### **Example Workflow:**

1. **User Registration**:
    - A new user can register by providing a **username**, **password**, and **role** (e.g., "user" or "admin").
    - The password will be hashed and stored securely.
    
    **POST /api/v1/account/register**
    ```json
    {
       "userName": "john_doe",
       "password": "SecurePassword123",
       "role": "user"
    }
    ```

2. **User Login**:
    - A user can log in by providing their **username** and **password**.
    - If the credentials are valid, the API will return a **JWT token** that the user can use for future requests.

    **POST /api/v1/account/login**
    ```json
    {
       "userName": "john_doe",
       "password": "SecurePassword123"
    }
    ```

    **Response (200 OK)**:
    ```json
    {
      "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SZ7MNwMPTmU5te1tr3B3Jlz-FtmNmKvDlYbMNQdPq9g"
    }
    ```

3. **Accessing Product Data**:
    - After successful login, the user can now use the **JWT token** to make authenticated requests to the Product API endpoints.
    - For example, to **view** all products, the user sends a `GET` request to **/products** with the JWT token in the Authorization header.

    **GET /api/v1/products**
    ```http
    Authorization: Bearer <JWT_TOKEN>
    ```

    **Response (200 OK)**:
    ```json
    [
        {
           "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
           "name": "New Product 1",
           "description": "Description of product 1"
		   "price": 10 
        },
        {
           "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
           "name": "New Product 2",
           "description": "Description of product 2"
		   "price": 1
        }
    ]
    ```

4. **Admin Access (Create, Update, Delete Products)**:
    - An **admin** user can perform CRUD operations on products.
    - The API checks if the user has the **admin role** by validating the token's payload.

    **POST /api/v1/products** (Admin only)
    ```json
    {
	   "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
	   "name": "New Product 1",
	   "description": "Description of new product",
	   "price": 1
	}
    ```

    **Response (201 Created)**:
    ```json
    {
	   "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
	   "name": "New Product 1",
	   "description": "Description of new product",
	   "price": 1
    }
    ```

    **PUT /api/v1/products** (Admin only)
    ```json
    {
	   "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
	   "name": "New Product",
	   "description": "New Description",
	   "price": 5
	}
    ```

    **DELETE /api/v1/products/{id}** (Admin only)

    **Response (200 OK)**:
    ```json
    {
       "message": "Product 3fa85f64-5717-4562-b3fc-2c963f66afa6 deleted successfully"
    }
    ```

---

### **Security Considerations:**
- **JWT Authentication**: Protect endpoints using **JWT tokens**. The token will be passed in the Authorization header of requests to authenticate the user.
- **Role-based Authorization**: Only users with the **admin role** can perform certain operations such as **create**, **update**, and **delete** products.

---
### **Deploy Considerations:**
1. **Deploy**

    To deploy this project acces the project folder 
    
    ```bash
      project-folder/
    ```
    Then run:
    
    ```bash
      docker-compose up -d
    ```

2. **Environment Variables**

     `ConnectionStrings__DefaultConnection`
     
     `Jwt__SecretKey`
     
     `Jwt__TokenValidityInMinutes`      

3. **Running locally**
    Use postman for tests, just export the collection
    
    ```bash
      project-folder/postman_collection/ExerciseV3.postman_collection.json
    ```
