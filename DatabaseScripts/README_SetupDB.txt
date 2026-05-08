После запуска программы в "Консоль диспетчера пакетов" выполняеете Update-Database
Если миграции не работают, то удаляйте их (всю папку миграций) и пишите:
Enable-Migrations
Add-Migration MigrationFirst
Update-Database

P.S.- вместо MigrationFirst можно написать другое название

sql-скрипты для заполнения локальной бд через запросы (вводите свои данные):

# Продукты
INSERT INTO Products (Name, SKU, Quantity) VALUES
('---', '---', --),

# Склад
INSERT INTO Warehouses (Name, Location) VALUES
('---', '----'),

# Пользователи (Роли: Manager, Client, WarehouseKeeper)
INSERT INTO Users (Login, PasswordHash, Role, RegistrationDate) VALUES
('----', '------------------------', '----', GETDATE()),

# Клиенты
INSERT INTO Clients (Name, Email, Phone, UserId) VALUES
('---', '---', '+7(000)000-00-00', ---)

# Распределение по складам
INSERT INTO ProductWarehouse (ProductId, WarehouseId, Quantity) VALUES
(-,-,-), (-,-,-),

# Заказы (Статусы: Completed, Shipped, New)
INSERT INTO Orders (ClientId, OrderDate, Status) VALUES
(-, '0000-00-00', '----'),

# Связь заказа с продуктами (У заказа может быть несколько продуктов)
INSERT INTO OrderProducts (OrderId, ProductId, Quantity) VALUES
(-,-,-), (-,-,-),

# Отгрузки
INSERT INTO Shipments (OrderId, WarehouseId, PlannedShipmentDate, ShipmentDate) VALUES
(-,-, DATEADD(day,2,'0000-00-00'), '0000-00-00'),
