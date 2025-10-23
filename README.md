# Como Rodar o Projeto Localmente

Para rodar o projeto e a API localmente, siga os passos abaixo:

### 1. Inicializar o SQL Server com Docker

1.  Navegue até a raiz do projeto onde se encontra o arquivo `docker-compose.yml`.
2.  Execute o seguinte comando no seu terminal:

    ```bash
    docker-compose up
    ```
   __Isso fará com que o SQL Server seja criado.__
   
   Exemplo Container rodando:

   <img width="1083" height="222" alt="image" src="https://github.com/user-attachments/assets/2a963d85-cbf5-48dc-82ff-ba97b24bf78a" />


### 2. Executar a Solução do Projeto

1.  Após o SQL Server estar em execução, abra a solução do projeto no **Visual Studio**.
2.  Rode a solução (você pode executá-la através do **IIS Express**).

    __Isso fará com que a Api seja executada em localhost com o swagger, para tornar possível a utilização dos endpoints e também criará o banco de dados no SQLServer (banco de dados **DbCaixaBanco**).__

### 3. Credenciais de Conexão com o Banco de Dados

Para se conectar diretamente ao banco de dados através de SQL Server Management Studio (SSMS) ou outra ferramenta, utilize as seguintes credenciais:

| Parâmetro | Valor |
| :--- | :--- |
| **ServerName** | `localhost,1433` |
| **UserName** | `sa` |
| **Password** | `SenhaF0rte!` |

__A autenticação deve ser do tipo: **SQLServer authentication**__

__**Importante** setar o **Trust Server Certificate** como **true**__

Imagem exemplo abaixo:

<img width="478" height="584" alt="image" src="https://github.com/user-attachments/assets/a321ad45-17e2-49cd-969a-3bfd4aaab681" />
