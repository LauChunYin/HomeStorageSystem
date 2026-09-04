### Create
## Create sln 创建解决方案文件 (.sln)
dotnet new sln -n HomeStorage

## Create Class Library & Web API 建立四层架构的项目（Class Library 和 Web API）
# 1. 领域层 (Domain) - 核心实体与接口，类库
dotnet new classlib -o src/HomeStorage.Domain
# 2. 应用层 (Application) - 业务逻辑与 DTO，类库
dotnet new classlib -o src/HomeStorage.Application
# 3. 基础设施层 (Infrastructure) - 数据库与外部服务，类库
dotnet new classlib -o src/HomeStorage.Infrastructure
# 4. Web API 表现层 (Api) - 控制器与入口，Web API 项目
dotnet new webapi -o src/HomeStorage.Api

## Import project to sln 将 4 个项目加入到解决方案中
dotnet sln add src/HomeStorage.Domain/HomeStorage.Domain.csproj
dotnet sln add src/HomeStorage.Application/HomeStorage.Application.csproj
dotnet sln add src/HomeStorage.Infrastructure/HomeStorage.Infrastructure.csproj
dotnet sln add src/HomeStorage.Api/HomeStorage.Api.csproj

## 建立项目之间的单向依赖关系（核心规则：单向依赖，禁止循环引用）
# Application 依赖 Domain
dotnet add src/HomeStorage.Application/HomeStorage.Application.csproj reference src/HomeStorage.Domain/HomeStorage.Domain.csproj
# Infrastructure 依赖 Domain 和 Application
dotnet add src/HomeStorage.Infrastructure/HomeStorage.Infrastructure.csproj reference src/HomeStorage.Domain/HomeStorage.Domain.csproj
dotnet add src/HomeStorage.Infrastructure/HomeStorage.Infrastructure.csproj reference src/HomeStorage.Application/HomeStorage.Application.csproj
# Api 依赖 Infrastructure 和 Application
dotnet add src/HomeStorage.Api/HomeStorage.Api.csproj reference src/HomeStorage.Infrastructure/HomeStorage.Infrastructure.csproj
dotnet add src/HomeStorage.Api/HomeStorage.Api.csproj reference src/HomeStorage.Application/HomeStorage.Application.csproj

## 清理自动生成的垃圾文件（可选）
删除各个项目文件夹里默认生成的 Class1.cs 或 WeatherForecast.cs，保持目录干净。

## 架构依赖图解（单向依赖，高内聚低耦合）
[ HomeStorage.Api ] ──┐
        │             │
        ▼             ▼
[ HomeStorage.Infrastructure ] ──► [ HomeStorage.Application ]
                                           │
                                           ▼
                                 [ HomeStorage.Domain ] (最底层，不依赖任何人)