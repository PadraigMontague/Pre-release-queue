# Pre Release Queue webservice
During the covid-19 pandemic I noticed that people queued for computer hardware and there was not an easy way to manage queues outside shops. I decided to develop this webservice to help solve this problem.

## Technologies used
    Core technologies:
        - C# .net  


## How to run the project?
To run the project download the project using git or as a zip file. Once the download is completed open the cmd and cd into the folder.
    Then run the following commands:
         - dotnet run

## Pre Release Queue webservice API endpoints:
    User endpoints:
    - /api/NormalUser/CreateUser                    |   POST HTTP Body { firstname, lastname, password, username }
    - /api/NormalUser/GetAllUsers                   |   GET HTTP Body { token }
    - /api/NormalUser/ChangePassword                |   POST HTTP Body { username, password, new_password }

    Login endpoints:
    - /api/Authentication/Login                     |   POST HTTP Body { username, type, password }

    Queue item endpoints:
    - /api/Item/CreateItem                          |   POST HTTP Body { firstname, lastname, product, storeName, userID, storeID, queueID, token }

    Product endpoints:
    - /api/Product/GetAllProducts                   |   GET  HTTP Body { token }
    - /api/Product/CreateProduct                    |   POST HTTP Body { product, description, storename, token }
    - /api/Product/DeleteProduct                    |   POST HTTP Body { productID, token }
    - /api/Product/UpdateProduct                    |   POST HTTP Body { product, productID, productDescription, storename, token }

    Queued products endpoints:
    - /api/QueuedProducts/GetAllQueuedProducts      |   GET HTTP Body { token }
    - /api/QueuedProducts/AddProductToQueue         |   POST HTTP Body { product, queueID, storeID, productID, storeName, token }
    - /api/QueuedProducts/deleteQueuedProduct       |   POST HTTP Body { queuedProductsID, token }

    Queue generator endpoints:
    - /api/QueueGenerator/GetAllQueues              |   GET HTTP Body { token }
    - /api/QueueGenerator/CreateQueue               |   POST HTTP Body { storeName, storeID, address, product, token }
    - /api/QueueGenerator/deleteQueue               |   POST HTTP Body { queueID, StoreName, token}

    Queue users endpoints:
    - /api/QueueUsers/GetQueue                      |   GET HTTP Body { queueID, token }
    - /api/QueueUsers/AddUser                       |   POST HTTP Body { queueID, storeID, productID, productName, username, storeName, token } 
    - /api/QueueUsers/RemoveUserFromQueue           |   POST HTTP Body { queueID, username, token }

    Store user endpoints:
    - /api/StoreUser/CreateStore                    |   POST HTTP Body { storeName, address, password }
    - /api/StoreUser/ChangePassword                 |   POST HTTP Body { storename, password, new_password }
    - /api/StoreUser/GetAllStores                   |   GET HTTP Body { token }
