# Web API
Использовал Visual Studio 2022, .Net 8
Добавил DTO 
Хотел также добавить базовый контроллер (для общих методов)
и инкапсуляцию, но этого не требовалось т.к объем кода не такой большой. 
# MS SQL
Сделал экспорт БД
``` 
db.bacpac
```
Использовал EntityFramework
```
Scaffold-DbContext "Server=home-pc;Database=testTask_hospital;user id=root;password=1234; TrustServerCertificate=True; Trusted_Connection=true" Microsoft.EntityFrameworkCore.SqlServer -OutPutdir "Model"
```